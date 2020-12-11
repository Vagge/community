using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Community.Data;
using Community.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Community.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the CommunityUser class
    public class CommunityUser : IdentityUser
    {
        public int messagesRead { get; set; }
        public int messagesDeleted { get; set; }
        public DateTime lastLoggedIn { get; set; }
        public static List<string> getEmails(CommunityContext context)
        {
            List<string> users = context.Users //move to model
                                .Select(u => u.Email)
                                .ToList();
            return users;
        }

        public static string getEmail(string Id, CommunityContext context)
        {
            return context.Users
                    .Single(u => u.Id == Id).Email;
        }

        public static DateTime getLastLoggedIn(string Id, CommunityContext context)
        {
            return context.Users
                    .Single(u => u.Id == Id).lastLoggedIn;
        }

        public static async Task<bool> setLastLoggedInAsync(string Id, CommunityContext context)
        {
            CommunityUser user = context.Users
                    .Single(u => u.Id == Id);
            user.lastLoggedIn = DateTime.Now;
            context.Update(user);
            await context.SaveChangesAsync();
            return true;
        }

        public static int getNrOfDeletedMessages(string Id, CommunityContext context)
        {
            return context.Users
                    .Single(u => u.Id == Id).messagesDeleted;
        }

        public static async Task<bool> setDeletedMessagesAsync(string Id, CommunityContext context)
        {
            CommunityUser user = context.Users
                    .Single(u => u.Id == Id);
            user.messagesDeleted = user.messagesDeleted + 1;
            context.Update(user);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
