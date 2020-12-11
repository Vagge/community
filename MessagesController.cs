using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Community.Data;
using Community.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Community.Areas.Identity.Data;

namespace Community.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly CommunityMessageContext _context;
        private readonly CommunityContext _context2;
        private readonly UserManager<CommunityUser> _userManager;


        public MessagesController(CommunityMessageContext context, UserManager<CommunityUser> userManager, CommunityContext context2)
        {
            _context = context;
            _userManager = userManager;
            _context2 = context2; //maybe have user email here
        }

        // GET: Messages, gets last logged in and un read messages and also updates last logged in
        public async Task<IActionResult> Index() 
        {
            CommunityUserViewModel vm = new CommunityUserViewModel();
            vm.lastLoggedIn = CommunityUser.getLastLoggedIn(_userManager.GetUserId(User), _context2);
            vm.messagesUnread = await Message.getNrOfUnreadMessagesAsync(_userManager.GetUserId(User), _context);
            return await Index(vm);
        }

        // POST: Messages
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CommunityUserViewModel vm)
        {
            await CommunityUser.setLastLoggedInAsync(_userManager.GetUserId(User), _context2);
            return View(vm);
        }

        // GET: Messages, gets all messages from a specific sender 
        public async Task<IActionResult> MessagesFrom(String? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recieverId = _userManager.GetUserId(User);
            string senderId = _context2.Users.Single(u => u.Email == id).Id;
            List<MessageViewModel> messagesVM = new List<MessageViewModel>();
            List<Message> messages = await Message.getMessagesAsync(recieverId, senderId, _context);
            foreach (var message in messages) //converts messages to messagesVM
            {
                MessageViewModel vm = new MessageViewModel();
                vm.id = message.id;
                vm.Sender = id;
                vm.Title = message.Title;
                vm.text = message.text;
                vm.Read = message.Read;
                vm.TimeSent = message.TimeSent;
                messagesVM.Add(vm);
            }
            return View(messagesVM);
        }

        // GET: Messages, gets all senders of messages to receiver
        public async Task<IActionResult> Read()
        {
            var userId = _userManager.GetUserId(User);

            List<string> messages = Message.getIdOfSendersToReciever(userId, _context);
            List<string> emailsOfMessages = new List<string>();
            foreach (var item in messages) // all emails of senders
            {
                emailsOfMessages.Add(CommunityUser.getEmail(item, _context2));
            }
            ViewData["senders"] = emailsOfMessages;
            ViewData["totalNrOfRead"] = await Message.getNrOfReadMessagesAsync(userId, _context); ;
            ViewData["totalNrOfMessages"] = await Message.getNrOfMessagesAsync(userId, _context);
            ViewData["totalNrOfDeleted"] = CommunityUser.getNrOfDeletedMessages(userId, _context2);
            return View();
        }

        // GET: Messages/Details/5 read a message, makes message read
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.id == id);
            if (message == null)
            {
                return NotFound();
            }
            MessageViewModel vm = new MessageViewModel(); //convert message to messageVM
            vm.id = message.id;
            vm.Reciever = CommunityUser.getEmail(message.Reciever, _context2);
            vm.Sender = CommunityUser.getEmail(message.Sender, _context2);
            vm.Title = message.Title;
            vm.text = message.text;
            vm.Read = message.Read;
            vm.TimeSent = message.TimeSent;      
  
            return await Details(id, vm);
        }

        // POST: Messages/Details/5
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int? id, MessageViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }
            var message = await _context.Messages.FindAsync(id);
            if (message.Reciever.Equals(_userManager.GetUserId(User)))
            {
                     if (message == null)
                    {
                        return NotFound();
                    }
                    message.Read = true;
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                    return View(vm);
            }
            else
            {
                return NotFound();
            }
               
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            List<string> users = CommunityUser.getEmails(_context2);
            ViewData["users"] = users; 
            ViewData["Confirmation"] = "";
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Reciever,Title,text")] Message message)
        {
 
                List<string> users = CommunityUser.getEmails(_context2);
                ViewData["users"] = users;
                try//creates new message, gets receivers id and send back confirmation
                {
                    message.Reciever =_context2.Users.Single(u => u.Email == message.Reciever).Id;
                    var userId = _userManager.GetUserId(User);
                    message.Sender = userId;
                    message.TimeSent = DateTime.Now;
                    message.Read = false;
                    _context.Add(message);
                    await _context.SaveChangesAsync();
                    ViewData["Confirmation"] = "Message sent to " + CommunityUser.getEmail(message.Reciever, _context2) + ", " + DateTime.Now;
                    ViewData["users"] = users; 
                    return View(message);

                }   
                catch(InvalidOperationException e)
                {
                    ViewData["users"] = users;
                    ViewData["Confirmation"] = "Failed to deliver";
                    return View(message);
                }  
            
            return View(message);
        }


        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.id == id);
            MessageViewModel vm = new MessageViewModel(); 
            vm.id = message.id;
            vm.Reciever = CommunityUser.getEmail(message.Reciever, _context2);
            vm.Sender = CommunityUser.getEmail(message.Sender, _context2);
            vm.Title = message.Title;
            vm.text = message.text;
            vm.Read = message.Read;
            vm.TimeSent = message.TimeSent;
            if (message == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if(message.Reciever.Equals(_userManager.GetUserId(User)))
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                await CommunityUser.setDeletedMessagesAsync(_userManager.GetUserId(User), _context2);
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return NotFound();
            }
            
        }

        private bool MessageExists(int id)
        { 
            return _context.Messages.Any(e => e.id == id);
        }
    }
}
