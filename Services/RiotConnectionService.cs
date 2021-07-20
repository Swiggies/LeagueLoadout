using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RiotSharp;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Authentication;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Net;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeagueLoadout
{
    public class RiotConnectionService
    {
        public EventHandler<LeagueEvent> MessageReceived { get; set; }

        private readonly Dictionary<string, List<EventHandler<LeagueEvent>>> _subscribers;
        private HttpClient _client;
        private ClientWebSocket _socket;


        public RiotConnectionService()
        {
            _subscribers = new Dictionary<string, List<EventHandler<LeagueEvent>>>();
        }

        public async Task RequestAuth()
        {
            _client = new HttpClient(new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            });

            var status = LeagueUtils.GetLeagueStatus();
            if (status == null) return;

            var byteArray = Encoding.ASCII.GetBytes("riot:" + status.Item2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            _socket = new ClientWebSocket();
            _socket.Options.Credentials = new NetworkCredential("riot", status.Item2);
            _socket.Options.RemoteCertificateValidationCallback = (_,_,_,_) => true;
            var msg = new ArraySegment<byte>(Encoding.ASCII.GetBytes("[5,\"OnJsonApiEvent\"]"));

            try
            {
                await _socket.ConnectAsync(new Uri($"wss://127.0.0.1:{status.Item3}/"), CancellationToken.None);
                await _socket.SendAsync(msg, WebSocketMessageType.Text, true, CancellationToken.None);

                var buffer = WebSocket.CreateClientBuffer(4096, 4096);
                WebSocketReceiveResult response = await _socket.ReceiveAsync(buffer, CancellationToken.None);

                while (response.MessageType != WebSocketMessageType.Close)
                {
                    string rawResponse = Encoding.ASCII.GetString(buffer);
                    var eventArray = JArray.Parse(rawResponse);
                    LeagueEvent leagueEvent = eventArray[2].ToObject<LeagueEvent>();

                    MessageReceived?.Invoke(this, leagueEvent);
                    Debug.WriteLine(leagueEvent.uri);
                    if(!_subscribers.TryGetValue(leagueEvent.uri, out var eventHandlers))
                    {
                        return;
                    }

                    eventHandlers.ForEach(eventHandler => eventHandler.Invoke(this, leagueEvent));

                    // Reset buffer
                    buffer = WebSocket.CreateClientBuffer(4096, 4096);
                    response = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                }
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        public void Subscribe(string uri, EventHandler<LeagueEvent> eventHandler)
        {
            if (_subscribers.TryGetValue(uri, out var eventHandlers))
                eventHandlers.Add(eventHandler);
            else
                _subscribers.Add(uri, new List<EventHandler<LeagueEvent>> { eventHandler });

            Debug.WriteLine($"New Subscriber: {uri}");
        }

        public async Task SendRequest()
        {
            try
            {
                var status = LeagueUtils.GetLeagueStatus();
                if (status == null) return;

                Debug.WriteLine(status.Item2);

                FlurlHttp.ConfigureClient($"https://127.0.0.1:{status.Item3}", cli => cli.Settings.HttpClientFactory = new CertFactory());
                var responseString = await $"https://127.0.0.1:{status.Item3}/lol-champ-select/v1/session/my-selection"
                    .WithBasicAuth("riot", status.Item2)
                    .PatchJsonAsync(new { spell1Id = 0,  spell2Id = 1 })
                    .ReceiveJson<LeagueEvent>();

                Debug.WriteLine(responseString);
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }

    public class CertFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
        }
    }
}
