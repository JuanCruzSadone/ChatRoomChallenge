using ChatRoomChallenge.Commands;
using ChatRoomChallenge.Common.Constants;
using ChatRoomChallenge.Data;
using ChatRoomChallenge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Services
{
    public class ChatMessagesService
    {
        private ILogger<ChatMessagesService> _logger;
        private ApplicationDbContext _applicationDbContext;
        private readonly Regex commandRegex = new Regex(@"\/((\S)+)=((\S)*)");

        public ChatMessagesService(ILogger<ChatMessagesService> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public void SaveChatMessage(ChatMessage msg)
        {
            try
            {
                if (!msg.IsCommand)
                {
                    _applicationDbContext.ChatMessages.Add(msg);
                    _applicationDbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error saveing message:{msg.User.UserName}, {msg.Message}");
            }
        }

        public List<ChatMessage> FetchLastMessages(int count = 50)
        {
            try
            {
                return _applicationDbContext.ChatMessages
                    .Include(msg => msg.User)
                    .OrderByDescending(msg => msg.TimeStamp).Take(count).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error fetching messages");
                return new List<ChatMessage>();
            }
        }

        public bool IsCommand(string message)
        {
            try
            {
                Match match = commandRegex.Match(message);
                if (match.Success)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error parsing message:{message}");
                return false;
            }
        }

        public async Task<string> ExecuteCommand(string fullCommand)
        {
            try
            {
                Match match = commandRegex.Match(fullCommand);

                if (!match.Success || match.Groups.Count < 2) return $"Sorry I couldn't understand the command { fullCommand}";

                string command = match.Groups[1].Value;
                string data = match.Groups[3].Value;

                switch (command)
                {
                    case StockCommandHandler.Command:
                        await new StockCommandHandler().HandleAsync(data);
                        return Status.ExecutingCommand;

                    default:
                        _logger.LogInformation($"Command {fullCommand} not found");
                        return $"Sorry I couldn't understand the command {fullCommand}";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error executing command:{fullCommand}");
                return $"Sorry I couldn't understand the command {fullCommand}";
            }
        }
    }

    public static partial class Helper
    {
        public static IServiceCollection AddChatMessagesService(this IServiceCollection services)
        {
            services.AddTransient(typeof(ChatMessagesService));

            return services;
        }
    }
}