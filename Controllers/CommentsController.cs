using System;
using BugTracker.HELPER;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(t => t.Ticket);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        [Authorize(Roles = "Admin,ProjectManager,Developer,Submitter")]
        public ActionResult Create(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            UserRolesHelperClass rolesHelper = new UserRolesHelperClass(db);
            var userRoles = rolesHelper.ListUserRoles(user.Id);
            ViewBag.UserId = user.Id;
            ViewBag.TicketId = id;
            ViewBag.AuthorID = User.Identity.GetUserId();

            return View();
        }

        // POST: Comments/Create
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        [HttpPost]
        //[ValidateAntiForgeryToken] 
        public ActionResult Create([Bind(Include = "Id,Body,Created,TicketId,UserId,AuthorID")] Comment comment)
        {     
            if (ModelState.IsValid)
            {
                comment.Created = DateTimeOffset.Now;
                comment.AuthorID = db.Users.Find(User.Identity.GetUserId()).DisplayName;
               
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = comment.TicketId});
            }
            
            return View(comment);
        }

    
        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", comment.TicketId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,Created,TicketId,UserId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(comment.TicketId);
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", comment.TicketId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            var ticket = db.Tickets.Find(comment.TicketId);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });

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

