using ChatRoomChallenge.Data;
using ChatRoomChallenge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Services
{
    public class ChatMessagesService
    {
        private ILogger<ChatMessagesService> _logger;
        private ApplicationDbContext _applicationDbContext;

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
                    .OrderByDescending(x => x.TimeStamp).Take(count).ToList();
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
                string[] commands = { "/stock=" };

                foreach (string cmd in commands)
                {
                    if (message.Contains(cmd)) return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error parsing message:{message}");
                return false;
            }
        }

        public string ExecuteCommand(string command)
        {
            try
            {
                return "null";
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error executing command:{command}");
                return $"Error executing command:{command}. " + e.Message;
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