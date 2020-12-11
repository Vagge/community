using Community.Areas.Identity.Data;
using Community.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Community.Models
{
    public class Message
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Sender { get; set; }
        [Required]
        public string Reciever { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }
        [StringLength(300, MinimumLength = 3)]
        [Required]
        public string text { get; set; }
        public DateTime TimeSent { get; set;}
        public Boolean Read { get; set; }

        public static async Task<List<Message>> getMessagesAsync(string recieverId, string senderId, CommunityMessageContext context)
        {
            return await context.Messages
                            .Where(u => u.Sender == senderId && u.Reciever == recieverId).ToListAsync();
        }

        public static List<string> getIdOfSendersToReciever(string recieverId, CommunityMessageContext context)
        {
            List<string> messages = context.Messages
                .Where(u => u.Reciever == recieverId)
                .Select(u => u.Sender)
                .ToList()
                .Distinct()
                .ToList();
            return messages;
        }

        public static async Task<int> getNrOfUnreadMessagesAsync(string id, CommunityMessageContext context)
        {
             List<Message> messages = await context.Messages
                            .Where(u => u.Reciever == id && u.Read == false).ToListAsync();
            return messages.Count();
        }

        public static async Task<int> getNrOfReadMessagesAsync(string id, CommunityMessageContext context)
        {
            List<Message> messages = await context.Messages
                           .Where(u => u.Reciever == id && u.Read == true).ToListAsync();
            return messages.Count();
        }

        public static async Task<int> getNrOfMessagesAsync(string id, CommunityMessageContext context)
        {
            List<Message> messages = await context.Messages
                           .Where(u => u.Reciever == id).ToListAsync();
            return messages.Count();
        }
    }
}
