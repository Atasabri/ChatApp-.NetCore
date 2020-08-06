using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class Message
    {
        [Key]
        public int ID { get; set; }
        public string Text { get; set; }
        public string SenderName { get; set; }
        public DateTime DateTime { get; set; }

        public int ChatID { get; set; }
        [ForeignKey(nameof(ChatID))]
        public Chat Chat { get; set; }
    }
}
