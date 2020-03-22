using ChatRoomChallenge.Common.Constants;
using ChatRoomChallenge.Data;
using ChatRoomChallenge.Models;
using ChatRoomChallenge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.SignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private ILogger<ChatHub> _logger;
        private UserManager<IdentityUser> _userManager;
        private ChatMessagesService _chatMessagesService;
        //private ApplicationDbContext _applicationDbContext;

        public ChatHub(ILogger<ChatHub> logger, UserManager<IdentityUser> userManager, ChatMessagesService chatMessagesService)
        {
            _logger = logger;
            _userManager = userManager;
            _chatMessagesService = chatMessagesService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            /*await Clients.Client(Context.ConnectionId)*/
            var msgs = from msg in _chatMessagesService.FetchLastMessages()
                       select new { user = msg.User.Email, message = msg.Message };
            await Clients.Caller.SendAsync(SignalRMethods.ReceiveMessages, msgs);
        }

        public async Task SendMessage(string message)
        {
            try
            {
                bool isAuthenticated = Context?.User?.Identity?.IsAuthenticated ?? false;

                if (isAuthenticated)
                {
                    //asynchronous to provide maximum scalability
                    var user = await _userManager.GetUserAsync(Context.User);
                    var chatMessage = new ChatMessage()
                    {
                        IsCommand = _chatMessagesService.IsCommand(message),
                        Message = message,
                        TimeStamp = DateTime.Now,
                        User = user
                    };

                    _logger.LogInformation($"Message recived: user:{user.Email}, message:{message}");

                    //Validate if this message is not a command and save it in db
                    if (!chatMessage.IsCommand)
                    {
                        _chatMessagesService.SaveChatMessage(chatMessage);

                        //asynchronous to provide maximum scalability
                        await Clients.All.SendAsync(SignalRMethods.ReceiveMessage, user.Email, message);
                    }
                    else
                    {
                        await Clients.All.SendAsync(SignalRMethods.ReceiveMessage, user.Email, message);

                        //With the message broker, there is a method that sends the response to the chat
                        string commandResponse = _chatMessagesService.ExecuteCommand(message);
                        await Clients.All.SendAsync(SignalRMethods.ReceiveMessage, "StockBot", commandResponse);
                    }
                }
                else
                {
                    _logger.LogInformation($"Unauthorize message recived: message:{message}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error sending message:{message}");
            }
        }
    }
}