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
    class RiotConnectionService
    {
        public RiotApi Riot;

        public RiotConnectionService()
        {

        }

        public async void RequestAuth()
        {
            var client = new HttpClient();

            HttpResponseMessage response = await client.PostAsync("https://127.0.0.1:2999/swagger/v3/openapi.json");
        }
    }
}
