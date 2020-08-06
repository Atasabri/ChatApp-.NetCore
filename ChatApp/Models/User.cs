using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class User : IdentityUser
    {
        public ICollection<ChatUsers> ChatUsers { get; set; }
    }
}
