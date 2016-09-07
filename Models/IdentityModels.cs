﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Collections;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public ApplicationUser()
        {
            this.Projects = new HashSet<Project>();
        }
        public virtual ICollection<Project> Projects { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BugTracker.Models.Ticket> Tickets { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.Priority> Priorities { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.Type> Types { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.History> History { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.Status> Status { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.Attachment> Attachments { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.Comment> Comments { get; set; }
        public IEnumerable ApplicationUsers { get; internal set; }
    }
}