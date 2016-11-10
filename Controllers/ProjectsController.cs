using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.HELPER;
using Microsoft.AspNet.Identity;
using System;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            UserRolesHelperClass rolesHelper = new UserRolesHelperClass(db);
            var userRoles = rolesHelper.ListUserRoles(user.Id);

            if (userRoles.Contains("Admin"))
            {
                return View(db.Projects.ToList());
            }
            if (userRoles.Contains("ProjectManager") || (userRoles.Contains("Developer")) || (userRoles.Contains("Submitter")))
            {
                return View(user.Projects.ToList());
             
            }
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin ,ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin ,ProjectManager")]
        public ActionResult Create([Bind(Include = "Id,Title,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        //GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            //var user = db.Users.Find(id);
            ProjectViewModel ProjectModel = new ProjectViewModel();
            ProjectUserHelper helper = new ProjectUserHelper(db);
            ProjectModel.Project = project;
            var currentUsers = helper.ListUsers(id);
            ProjectModel.Users = new MultiSelectList(currentUsers, "Id", "FirstName");

            var absentUsers = helper.AbsentUsers(id);
            ProjectModel.AbsentUsers = new MultiSelectList(absentUsers, "Id", "FirstName");


            return View(ProjectModel);

            //return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
         
                db.Entry(project).State = EntityState.Modified;
                //db.Projects.Attach(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        //POST: Projects/Edit **Add User TO Project
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        public ActionResult AddUser(int projectId, List<string> SelectedAbsentUsers)
        {

            if (ModelState.IsValid)
            {
                ProjectUserHelper helper = new ProjectUserHelper(db);
                var project = db.Projects.Find(projectId);



                if (SelectedAbsentUsers != null)
                {

                    foreach (var user in SelectedAbsentUsers)
                    {
                        helper.AssignUser(user, projectId);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit");
        }

        //POST: Projects/Edit **Remove User FROM Project
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUser(int projectId, List<string> SelectedCurrentUsers)
        {
            if (ModelState.IsValid)
            {
                ProjectUserHelper helper = new ProjectUserHelper(db);
                var project = db.Projects.Find(projectId);



                if (SelectedCurrentUsers != null)
                {

                    foreach (var user in SelectedCurrentUsers)
                    {
                        helper.RemoveUser(user, projectId);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index");
        }


        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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

