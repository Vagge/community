using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Community.Models
{
    public class CommunityUserViewModel
    {
        [Display(Name = "Messags read:")]
        public int messagesRead { get; set; }
        [Display(Name = "New messages:")]
        public int messagesUnread { get; set; }
        [Display(Name = "Messages deleted:")]
        public int messagesDeleted { get; set; }

        [Display(Name = "Last Logged in:")]
        public DateTime lastLoggedIn { get; set; }
    }
}
