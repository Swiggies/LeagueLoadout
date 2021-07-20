using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.Net.WebSockets;
using System.Net;
using Websocket.Client;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace LeagueLoadout
{
    public class RiotConnectionService
    {
        public EventHandler<LeagueEvent> MessageReceived { get; set; }

        public Dictionary<string, List<EventHandler<LeagueEvent>>> _subscribers;
        private WebsocketClient _socket;

        public RiotConnectionService()
        {
            _subscribers = new Dictionary<string, List<EventHandler<LeagueEvent>>>();
            RequestAuth();
        }

        public async Task Connect()
        {
            await _socket.Start();
            await _socket.SendInstant("[5, \"OnJsonApiEvent\"]");
        }

        public void RequestAuth()
        {
            try
            {
                var status = LeagueUtils.GetLeagueStatus();
                if (status == null) return;

                _socket = new WebsocketClient(new Uri($"wss://127.0.0.1:{status.Item3}/"), () =>
                {
                    var socket = new ClientWebSocket
                    {
                        Options =
                        {
                            Credentials = new NetworkCredential("riot", status.Item2),
                            RemoteCertificateValidationCallback = (_,_,_,_) => true
                        }
                    };

                    socket.Options.AddSubProtocol("wamp");
                    return socket;
                });

                _socket.MessageReceived
                    .Where(msg => msg.Text != null)
                    .Where(msg => msg.Text.StartsWith('['))
                    .Subscribe(msg =>
                    {
                        var eventArray = JArray.Parse(msg.Text);
                        var eventNumer = eventArray[0].ToObject<int>();

                        if (eventNumer != 8) return;

                        var leagueEvent = eventArray[2].ToObject<LeagueEvent>();
                        MessageReceived?.Invoke(this, leagueEvent);

                        if (!_subscribers.TryGetValue(leagueEvent.Uri, out var eventHandlers))
                        {
                            return;
                        }

                        Debug.WriteLine($"Right event: {leagueEvent.Uri}");
                        eventHandlers.ForEach(eventHandler => eventHandler?.Invoke(this, leagueEvent));
                    });
                Debug.WriteLine("Successfully connected to websocket");
                
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

        public async Task<JToken> RetrieveDragonData()
        {
            try
            {
                var champions = await "https://ddragon.leagueoflegends.com/cdn/11.14.1/data/en_US/champion.json".GetStringAsync();
                return JToken.Parse(champions)["data"];
            } 
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
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
