using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }

        [NotMapped]
        public bool IsCommand { get; set; } = false;
    }
}