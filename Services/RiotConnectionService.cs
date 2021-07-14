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

namespace LeagueLoadout
{
    public class RiotConnectionService
    {
        public static Action<string> OnMessageReceive;

        public RiotApi Riot;

        private HttpClient _client;
        private ClientWebSocket _socket;

        public RiotConnectionService()
        {
            OnMessageReceive += HandleMessage;
        }

        private void HandleMessage(string s)
        {
            Debug.Print(s);
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
                    OnMessageReceive?.Invoke(Encoding.ASCII.GetString(buffer));
                    buffer = WebSocket.CreateClientBuffer(4096, 4096);

                    response = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                }
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
            }
        }
    }
}
