namespace Events.DataV2.Migrations
{
    using Events.Webv2.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class DbMigrationConfig : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public DbMigrationConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Events.Web.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any())
            {
                var adminemail = "katleho12@gmail.com";
                var adminusername = adminemail;
                var adminfullname = "system administrator";
                var adminpassword = adminemail;
                string adminrole = "administrator";

                CreateAdminUser(context, adminemail, adminusername, adminfullname, adminpassword, adminrole);
                context.SaveChanges();
                CreateSeveralEvents(context);
                context.SaveChanges();
                //}
                //  this method will be called after migrating to the latest version.

                //  you can use the dbset<t>.addorupdate() helper extension method 
                //  to avoid creating duplicate seed data. e.g.
                //
                //    context.people.addorupdate(
                //      p => p.fullname,
                //      new person { fullname = "andrew peters" },
                //      new person { fullname = "brice lambson" },
                //      new person { fullname = "rowan miller" }
                //    );
                //
            }


        }

        private void CreateAdminUser(ApplicationDbContext context, string adminEmail, string adminUserName, string adminFullName, string adminPassword, string adminRole)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                FullNme = adminFullName,
                Email = adminEmail
            };

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            var userCreatedResult = userManager.Create(adminUser, adminPassword);
            if (!userCreatedResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreatedResult.Errors));
            }

            var roleManger = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreatedResult = roleManger.Create(new IdentityRole(adminRole));
            if (!roleCreatedResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreatedResult.Errors));
            }

            var addAdminRoleResult = userManager.AddToRole(adminUser.Id, adminRole);
            if (!addAdminRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addAdminRoleResult.Errors));
            }

        }

        private void CreateSeveralEvents(ApplicationDbContext context)
        {
            context.Events.Add(new Event()
            {
                Title = "Party @ SoftUni",
                startDateTime = DateTime.Now.Date.AddDays(5).AddHours(21).AddMinutes(30),
                Author = context.Users.First()
            });

            context.Events.Add(new Event()
            {
                Title = "Passed Event <Anonymous>",
                startDateTime = DateTime.Now.Date.AddDays(-2).AddHours(10).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5),
                Comments = new HashSet<Comment>()
                {
                    new Comment(){Text = "<Anonymous> comment"},
                    new Comment(){Text = "User comment", Author= context.Users.First()}
                }
            });

            context.Events.Add(new Event()
            {
                Title = "Cut Feshers @ cutFields",
                startDateTime = DateTime.Now.Date.AddDays(2).AddHours(10).AddMinutes(30),
                Duration = TimeSpan.FromHours(8.5),
                Comments = new HashSet<Comment>()
                {
                    new Comment(){Text = "<Anonymous> comment"},
                    new Comment(){Text = "User comment", Author= context.Users.First()}
                }
            });
            context.Events.Add(new Event()
            {
                Title = "Passed Event <We Back Home>",
                startDateTime = DateTime.Now.Date.AddDays(-20).AddHours(10).AddMinutes(30),
                Duration = TimeSpan.FromHours(10),
                Comments = new HashSet<Comment>()
                {
                    new Comment(){Text = "<Anonymous> comment"},
                    new Comment(){Text = "User comment", Author= context.Users.First()}
                }
            });
            context.Events.Add(new Event()
            {
                Title = "Passed Event <Anonymous>",
                startDateTime = DateTime.Now.Date.AddDays(-2).AddHours(10).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5),
                Comments = new HashSet<Comment>()
                {
                    new Comment(){Text = "<Anonymous> comment"},
                    new Comment(){Text = "User comment", Author= context.Users.First()}
                }
            });

            context.Events.Add(new Event()
            {
                Title = "Passed Event <Anonymous>",
                startDateTime = DateTime.Now.Date.AddDays(-2).AddHours(10).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5),
                Author = context.Users.First()
            });
        }
    }
}
