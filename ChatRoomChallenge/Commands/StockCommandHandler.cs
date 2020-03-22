using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Commands
{
    public class StockCommandHandler
    {
        public const string Command = "stock";

        public async Task HandleAsync(string command)
        {
            //send it to rabbitmq
        }
    }
}