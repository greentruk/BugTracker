using BugTracker.HELPER;
using BugTracker.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class AdminUserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        //GET: /AssignUserRoles
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserRoles(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelperClass helper = new UserRolesHelperClass(db);
            var currentRoles = helper.ListUserRoles(id);
            var absentRoles = helper.ListAbsentUserRoles(id);
            AdminModel.Roles = new MultiSelectList(currentRoles);
            AdminModel.AbsentRoles = new MultiSelectList(absentRoles);
            AdminModel.User = user;

            return View(AdminModel);
        }


        // GET: UserDashboard

        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        public ActionResult UserDashboard()
        {
            return View(db.Users.ToList());
        }




        // POST: Add User Role
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddRole(string AddId, List<string> SelectedAbsentRoles)
        {

            if (ModelState.IsValid)
            {
                UserRolesHelperClass helper = new UserRolesHelperClass(db);
                var user = db.Users.Find(AddId);
                if (SelectedAbsentRoles != null)
                {
                    foreach (var role in SelectedAbsentRoles)
                    {

                        helper.AddUserToRole(AddId, role);
                        
                    }

                }

                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("UserDashboard");
            }
            return View(AddId);
        }

        // POST: Remove User Role
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult RemoveRole(string RemoveId, List<string> SelectedCurrentRoles)
        {
            if (ModelState.IsValid)
            {
                UserRolesHelperClass helper = new UserRolesHelperClass(db);
                var user = db.Users.Find(RemoveId);
                if (SelectedCurrentRoles != null)
                {
                    foreach (var role in SelectedCurrentRoles)
                    {

                        helper.RemoveUserFromRole(RemoveId, role);
                        
                    }
                }
                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("UserDashboard");
            }
            return View(RemoveId);
        }
    }
}




