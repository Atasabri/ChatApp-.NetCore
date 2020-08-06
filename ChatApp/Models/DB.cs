using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class DB : IdentityDbContext<User>
    {
        public DB(DbContextOptions<DB> options)
      : base(options)
        { }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUsers> ChatUsers { get; set; }
    }
}
