using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BugTracker.HELPER
{
    public class UserRolesHelperClass
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public UserRolesHelperClass(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            this.roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            this.db = context;
        }

        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }

        public IList<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public IList<string> ListAbsentUserRoles(string userId)
        {
            var roles = roleManager.Roles.Where(r => r.Name != null).Select(r => r.Name).ToList();
            var AbsentUserRoles = new List<string>();
            foreach (var role in roles)
            {
                if (!IsUserInRole(userId, role))
                { 
                    AbsentUserRoles.Add(role);
                    }
            }
        
         return AbsentUserRoles;
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var userIds = roleManager.FindByName(roleName).Users.Select(role => role.UserId);
            return userManager.Users.Where(u => userIds.Contains(u.Id)).ToList();
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var userIds = Roles.GetUsersInRole(roleName);
            return userManager.Users.Where(u => !userIds.Contains(u.Id)).ToList();
        }
        public string GetRoleName(string roleId)
        {
            var role = db.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
            return role;
        }
    }
}

