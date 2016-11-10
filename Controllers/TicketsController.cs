using BugTracker.HELPER;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace BugTracker.Models
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Assignee).Include(t => t.Author).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Type).Include(t => t.Status);
            var user = db.Users.Find(User.Identity.GetUserId());
            UserRolesHelperClass rolesHelper = new UserRolesHelperClass(db);
            var userRoles = rolesHelper.ListUserRoles(user.Id);

            //if user is ADMIN, show all tickets
            if (userRoles.Contains("Admin"))
            {
                return View(tickets.ToList());
             
            }
            //if user is PM, show ticket for all PM
            if (userRoles.Contains("ProjectManager"))
            {
                return View(user.Projects.SelectMany(t => t.Tickets).ToList());

            }
            //if user is DEVELOPER, show all tickets assinged to Developer
            if (userRoles.Contains("Developer") && userRoles.Contains("Submitter"))
            {
                return View(db.Tickets.Where(t => t.AssigneeId == user.Id || t.AuthorId == user.Id).ToList());
            }

            if (userRoles.Contains("Developer"))
            {
                return View(db.Tickets.Where(t => t.AssigneeId == user.Id).ToList());
            }
            //if user is SUBMITTER, show all tickets he has submitted
            if (userRoles.Contains("Submitter"))
            {
                return View(db.Tickets.Where(t => t.AuthorId == user.Id).ToList());
            }
            return View(tickets);
            //return RedirectToAction("Login", "Account");
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRolesHelperClass rolesHelper = new UserRolesHelperClass(db);
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRoles = rolesHelper.ListUserRoles(user.Id);
            Ticket ticket = db.Tickets.Find(id);

            if (ticket == null)
            {
                return HttpNotFound();
            }

            //Prevents URL HiJacking
            if (userRoles.Contains("Admin"))
            {
                return View(ticket);
            }
            if (userRoles.Contains("ProjectManager"))
            {
                if (ticket.Project.Users.Contains(user))
                {
                    return View(ticket);
                }
            }
            if (userRoles.Contains("Developer") && userRoles.Contains("Submitter"))
            {
                if (ticket.AssigneeId == user.Id)
                {
                    return View(ticket);
                }
                if (ticket.AuthorId == user.Id)
                {
                    return View(ticket);
                }
            }
            if (userRoles.Contains("Developer"))
            {
                if (ticket.AssigneeId == user.Id)
                {
                    return View(ticket);
                }
            }
            if (userRoles.Contains("Submitter"))
            {
                if (ticket.AuthorId == user.Id)
                {
                    return View(ticket);
                }
            }

            return RedirectToAction("Login", "Account");
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            ViewBag.AuthorName = user.FirstName + ' ' + user.LastName;
            ViewBag.AuthorId = user.Id;

            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name");
            ViewBag.ProjectId = new SelectList(user.Projects, "Id", "Title");
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name");


            return View();
        }
        // POST: Tickets/Create
        [Authorize(Roles = "Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "Id,AuthorId,AssigneeId,Created,Updated,PriorityId,ProjectId,StatusId,TypeId,Description")] Ticket ticket)

        {
            History history = new History();
            {
                if (ModelState.IsValid)
                ticket.Created = DateTimeOffset.Now;
                ticket.Updated = ticket.Created;
                history.TicketId = ticket.Id;
                db.History.Add(history);
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager, Developer, Admin, Submitter")]
        public ActionResult Edit(int? id)
        {
            
            UserRolesHelperClass rolesHelper = new UserRolesHelperClass(db);
            ProjectUserHelper helper = new ProjectUserHelper(db);
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRoles = rolesHelper.ListUserRoles(user.Id);
            var tickets = db.Tickets.Include(t => t.Assignee ).Include(t => t.Project).Include(t => t.Priority).Include(t => t.Status).Include(t => t.Type);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

           

            ViewBag.AssigneeId = new SelectList(rolesHelper.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssigneeId);
            ViewBag.ProjectId = new SelectList(helper.AssignedProjects(user.Id), "Id", "Title", ticket.ProjectId);
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name", ticket.StatusId);

            if (ticket == null)
            {
                return HttpNotFound();
            }
            if (userRoles.Contains("ProjectManager"))
            {
                return View(ticket);
            }
            if (userRoles.Contains("Admin"))
            {
                return View(ticket);
            }
            if (userRoles.Contains("Developer"))
            {
                return View(ticket);
            }
            if (userRoles.Contains("Submitter"))
            {
                return View(ticket);
            }
            
            return RedirectToAction("Login", "Account");
        }

         

        // POST: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager, Developer, Submitter, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AuthorId,AssigneeId,Created,Updated,PriorityId,ProjectId,StatusId,TypeId,Description")] Ticket ticket)
       
        {
            StringBuilder sb = new StringBuilder();
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);


            if (ModelState.IsValid)
            {
                ticket.Updated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                var newTicket = db.Tickets.Find(ticket.Id);

                if (oldTicket != ticket)
                {
                    sb.AppendLine("Changed on " + DateTimeOffset.Now);
                    sb.Append("<br />");

                    if (oldTicket.Description != newTicket.Description)
                    {
                        sb.AppendLine("This ticket description changed from " + (oldTicket.Description) + " to " + (newTicket.Description));
                        sb.Append("<br />");
                    }
                    
                    if (oldTicket.StatusId != newTicket.StatusId)
                    {
                        var newStatus = db.Status.Where(s => s.Id == newTicket.StatusId).Select(n => n.Name).FirstOrDefault();
                        sb.AppendLine("This ticket status changed from " + (oldTicket.Status.Name) + " to " + (newStatus));
                        sb.Append("<br />");
                    }
                
                    if (oldTicket.TypeId != ticket.TypeId)
                    {
                        var newType = db.Types.Where(s => s.Id == newTicket.TypeId).Select(n => n.Name).FirstOrDefault();
                        sb.AppendLine("This ticket type changed from " + (oldTicket.Type.Name) + " to " + (newType));
                        sb.Append("<br />");
                    }
                    if (oldTicket.PriorityId != ticket.PriorityId)
                    {
                        var newPriority= db.Priorities.Where(s => s.Id == newTicket.PriorityId).Select(n => n.Name).FirstOrDefault();
                        sb.AppendLine("This ticket priority changed from " + (oldTicket.Priority.Name) + " to " + (newPriority));
                        sb.Append("<br />");

                    }
                    if (oldTicket.Assignee != ticket.Assignee)
                    {
                        var newAssignee = db.Users.Where(s => s.Id == newTicket.AssigneeId).Select(n => n.DisplayName).FirstOrDefault();
                        sb.AppendLine("This ticket assignee changed from " + (oldTicket.Assignee.DisplayName) + " to " + (newAssignee));
                        sb.Append("<br />");
                    }
                    
                }
                var History = new History();
                History.TicketId = ticket.Id;
                History.Body = sb.ToString();
                History.Updated = DateTimeOffset.Now;
                db.Entry(History).State = EntityState.Modified;
                db.History.Add(History);
                db.SaveChanges();
                await UserManager.SendEmailAsync(ticket.AssigneeId , "You have a ticket needing attention!", "Please log into dashboard to see if you have a new ticket or if the ticket you are working on has been updated!");

                return RedirectToAction("Index");
                }
                return View(ticket); 
            }
        private ApplicationUserManager _userManager;

        public TicketsController()
        {
        }

        public TicketsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
           
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }

        }


        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
       
        
        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}