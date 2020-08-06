using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class ChatUsers
    {
        [Key]
        public int ID { get; set; }

        public string UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        public int ChatID { get; set; }
        [ForeignKey(nameof(ChatID))]
        public Chat Chat { get; set; }
    }
}
