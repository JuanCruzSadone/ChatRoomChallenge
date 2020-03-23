using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChatRoomChallenge.ApiClients
{
    public class StooqClient
    {
        private const string BaseUrl = "https://stooq.com/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv";
        private readonly HttpClient _client;

        public StooqClient()
        {
            _client = new HttpClient();
        }

        public async Task<(string, bool)> GetStockFor(string companyName)
        {
            var httpResponse = await _client.GetAsync(BaseUrl.Replace("{0}", companyName));

            if (!httpResponse.IsSuccessStatusCode)
            {
                return ("Cannot retrieve stock information", false);
            }

            var content = await httpResponse.Content.ReadAsStringAsync();

            return (content, true);
        }
    }
}