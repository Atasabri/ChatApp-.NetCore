using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApp.ViewComponents
{
    public class ChatsViewComponent : ViewComponent
    {
        private readonly DB _db;
        private readonly UserManager<User> usermanager;

        public ChatsViewComponent(DB db,UserManager<User> usermanager)
        {
            _db = db;
            this.usermanager = usermanager;
        }

        public async  Task<IViewComponentResult> InvokeAsync()
        {            
            string userId = usermanager.GetUserId(HttpContext.User);
            var chats = await _db.Chats.Include("ChatUsers.User").Where(x => x.ChatUsers.Any(a => a.UserID == userId)).ToListAsync();
            return View(chats);
        }
    }
}
