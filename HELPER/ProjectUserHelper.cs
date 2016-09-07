using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectUserHelper
    {
        //we need 1) a database context, 2) a UserManager object, and 3) a RoleManager
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public ProjectUserHelper(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(
              new UserStore<ApplicationUser>(context));
            this.roleManager = new RoleManager<IdentityRole>(
              new RoleStore<IdentityRole>(context));
            this.db = context;
        }

        //Assigns user to specific project
        public void AssignUser(string userId, int projectId)

        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Add(user);
            //db.Entry(user).State = EntityState.Modified;
        }

        //Removes user from specific project
        public void RemoveUser(string userId, int projectId)

        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Remove(user);
            db.Entry(user).State = EntityState.Modified;
        }



        //return list of users belonging to specific user
        public List<ApplicationUser> ListUsers(int? projectId)
        {
            var project = db.Projects.Find(projectId);
            var users = project.Users.ToList();

            return users;

        }
        // Get a list of projects assigned to a given user
        public List<Project> AssignedProjects(string userId)
        {
            var user = db.Users.Find(userId);
            var userProjects = user.Projects.ToList();
            return userProjects;
        }

        

        //return list of all users NOT in specific project
        public List<ApplicationUser> AbsentUsers(int? projectId)
        {
            var users = db.Users.ToList();
            var project = db.Projects.Find(projectId);
            var AbsentUsers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                if (!project.Users.Contains(user))
                {
                    AbsentUsers.Add(user);
                }
            }
            return AbsentUsers;
        }
    }
}