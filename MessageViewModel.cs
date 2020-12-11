using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Community.Models
{
    public class MessageViewModel
    {
        public int id { get; set; }
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public string Title { get; set; }
        public string text { get; set; }
        [Display(Name = "Time sent")]
        public DateTime TimeSent { get; set; }
        public Boolean Read { get; set; }
    }
}
