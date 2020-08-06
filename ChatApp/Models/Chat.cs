using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static ChatApp.Models.Helper;

namespace ChatApp.Models
{
    public class Chat
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        [Required]
        public ChatType Type { get; set; }

        public ICollection<ChatUsers> ChatUsers { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
