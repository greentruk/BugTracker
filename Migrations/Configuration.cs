namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            var userManager = new UserManager<ApplicationUser>(
           new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "pete@peteharvey.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "pete@peteharvey.com",
                    Email = "pete@peteharvey.com",
                    FirstName = "Pete",
                    LastName = "Harvey",
                    DisplayName = "Pete Harvey"
                }, "Coder@207");
            }
            if (!context.Users.Any(u => u.Email == "PM@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "PM@coderfoundry.com",
                    Email = "PM@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "J.Twich"
                }, "Abc&123!");
            }
            var userId = userManager.FindByEmail("pete@peteharvey.com").Id;
            userManager.AddToRole(userId, "Admin");
            userManager.AddToRole(userId, "Submitter");

            var user_Id = userManager.FindByEmail("PM@coderfoundry.com").Id;
            userManager.AddToRole(user_Id, "ProjectManager");


        }

    }
}

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        
    

