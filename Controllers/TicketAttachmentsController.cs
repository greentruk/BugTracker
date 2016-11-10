using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace BugTracker.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketAttachments
        public ActionResult Index()
        {
            var ticketAttachments = db.Attachments.Include(t => t.Ticket).Include(t => t.UserId );
            return View(ticketAttachments.ToList());
        }

        // GET: TicketAttachments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachment ticketAttachment = db.Attachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        [Authorize(Roles = "Admin,ProjectManager,Developer,Submitter")]
        public ActionResult Create(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.UserId = user.Id;
            ViewBag.TicketId = id;
            ViewBag.AuthorID = User.Identity.GetUserId();
            return View();
        }

        // POST: TicketAttachment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,Description")] Attachment Attachment, HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                //check the file name to make sure its an image or word doc
                var ext = Path.GetExtension(fileUpload.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".doc" && ext != ".docx" && ext != ".txt" && ext != ".pdf")
                    ModelState.AddModelError("fileUpload", "Invalid Format.");
            }
            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    //relative server path
                    var filePath = "/Uploads/";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    Attachment.FilePath = filePath + fileUpload.FileName;
                    //to save image
                    fileUpload.SaveAs(Path.Combine(absPath, fileUpload.FileName));
                }
                Attachment.UserId = User.Identity.GetUserId();
                Attachment.Created = DateTimeOffset.Now;
                Attachment.AuthorID = db.Users.Find(User.Identity.GetUserId()).DisplayName;
                db.Attachments.Add(Attachment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = Attachment.TicketId });
            }
            return RedirectToAction("Create", "TicketAttachments", new { id = Attachment.TicketId });
        }

        // GET: TicketAttachment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachment ticketAttachment = db.Attachments.Find(id);
           
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = ticketAttachment.TicketId;


            return View(ticketAttachment);

        }

        // POST: TicketAttachment/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserId,AuthorID")] Attachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(ticketAttachment.TicketId);
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
   
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
        }

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachment ticketAttachment = db.Attachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attachment ticketAttachment = db.Attachments.Find(id);
            db.Attachments.Remove(ticketAttachment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
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
