using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoomChallenge.BackgroundServices
{
    public class ExecuteStockCommand : BackgroundService
    {
        private readonly ILogger<ExecuteStockCommand> _logger;

        public ExecuteStockCommand(
           ILogger<ExecuteStockCommand> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string command = string.Empty;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //execute commands from rabbitmq
                    //send response impersonating a chat user
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error executing command:{command}");
                }
                finally
                {
                    await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                }
            }
        }
    }
}