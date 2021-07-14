using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using System.Diagnostics;
using System.Net.Http;

namespace LeagueLoadout
{
    public class RiotConnectionService
    {
        public RiotApi Riot;

        public RiotConnectionService()
        {

        }

        public async void RequestAuth()
        {
            try
            {
                var client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync("https://127.0.0.1:2999/swagger/v3/openapi.json");
                Debug.Print($"{response.StatusCode} // {response.Content}");
            }
            catch(HttpRequestException e)
            {
                Debug.Print(e.Message);
            }
        }
    }
}
