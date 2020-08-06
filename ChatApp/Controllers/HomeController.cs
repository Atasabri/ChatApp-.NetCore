using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ChatApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DB _db;
        private IHubContext<ChatHub> _hubContext;

        public HomeController(DB db , IHubContext<ChatHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateGroup()
        {
            var users = _db.Users.Where(x=>x.Id != HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(string Name , List<string> UsersIDs)
        {
            var chat = new Chat
            {
                Name = Name,
                Type = Helper.ChatType.Room
            };
            
            await _db.Chats.AddAsync(chat);
            await _db.SaveChangesAsync();
            foreach (string UserID in UsersIDs)
            {
                _db.ChatUsers.Add(new ChatUsers { UserID = UserID,ChatID = chat.ID });
            }
            _db.ChatUsers.Add(new ChatUsers { UserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, ChatID = chat.ID });
            await _db.SaveChangesAsync();
            await _hubContext.Clients.Users(UsersIDs).SendAsync("ReceiveMessage", Name, this.User.Identity.Name + " Added You To Group "+Name, "AddGroup",chat.ID);
            return RedirectToAction(nameof(Chat), new { ChatID = chat.ID });
        }

        [HttpGet]
        public ActionResult CreatePrivate()
        {
            var users = _db.Users.Where(x => x.Id != HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePrivate(string UserID)
        {
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Chat chat = null;
            chat = _db.Chats.Include("ChatUsers.User").FirstOrDefault(x => x.Type == Helper.ChatType.Private &&
            x.ChatUsers.Any(a => a.UserID == UserID) && x.ChatUsers.Any(a => a.UserID == id));
            if(chat == null)
            {
                 chat = new Chat
                {
                    Type = Helper.ChatType.Private
                };
                await _db.Chats.AddAsync(chat);
                await _db.SaveChangesAsync();
                await _db.ChatUsers.AddAsync(new ChatUsers { UserID =  id, ChatID = chat.ID });
                await _db.ChatUsers.AddAsync(new ChatUsers { UserID = UserID, ChatID = chat.ID });
                await _db.SaveChangesAsync();
                await _hubContext.Clients.User(UserID).SendAsync("ReceiveMessage", this.User.Identity.Name, this.User.Identity.Name + " Added You To Private Chat", "AddPrivate", chat.ID);
            }

            return RedirectToAction(nameof(Chat), new { ChatID = chat.ID });
        }

        public IActionResult Chat(int ChatID)
        {
            var chat = _db.Chats.Include(x => x.Messages).Include(x=>x.ChatUsers).FirstOrDefault(x => x.ID == ChatID);
            if(chat.ChatUsers.Any(x=>x.UserID == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return View(chat);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string Text,int ChatID)
        {
            var message = new Message
            {
                SenderName = User.Identity.Name,
                DateTime = DateTime.Now,
                Text = Text,
                ChatID = ChatID,
            };
            if(_db.ChatUsers.Any(x=>x.ChatID == ChatID && x.UserID == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                await _hubContext.Clients.Group(ChatID.ToString()).SendAsync("ReceiveMessage", message.SenderName, message.Text,"Message",null);
                await _db.Messages.AddAsync(message);
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
