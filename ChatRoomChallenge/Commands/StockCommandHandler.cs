using ChatRoomChallenge.ApiClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Commands
{
    public class StockCommandHandler
    {
        public const string Command = "stock";
        private readonly StooqClient _stooqClient;

        public StockCommandHandler()
        {
            _stooqClient = new StooqClient();
        }

        public async Task<(string, bool)> HandleAsync(string stockName)
        {
            try
            {
                //"Symbol,Date,Time,Open,High,Low,Close,Volume\r\nAAPL.US,2020-03-23,17:49:23,228.08,228.4997,212.61,222.96,35443220\r\n"
                var (resp, succ) = await _stooqClient.GetStockFor(stockName);

                //validate if resp ir CSV
                //validate the result is not ...,N/D,N/D,N/D,N/D,N/D,N/D,N/D

                var lines = resp.Split("\r\n");

                var stockinfo = lines[1].Split(",");
                if (decimal.TryParse(stockinfo[6], out _))
                {
                    string response = $"{stockinfo[0].ToUpper()} quote is ${stockinfo[6].ToUpper()} per share";
                    return (response, true);
                }
                return ("Cannot retrieve stock information", false);
            }
            catch (Exception)
            {
                return ("Cannot retrieve stock information", false);
            }
        }
    }
}