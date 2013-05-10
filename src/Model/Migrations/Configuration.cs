namespace Model.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Threading;
    using System.Web.Security;
    using Framework.Models;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<Model.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Model.AppDbContext appDbContext)
        {
            SeedSecurity();
            SeedBuisness(appDbContext);
        }

        private static void SeedBuisness(Model.AppDbContext appDbContext)
        {
            appDbContext.Routes.AddOrUpdate(c => c.Name, new Route { Name = "Name1", Created = DateTime.Now, CreatedBy = "cvazquez", Distance = 12 });
            appDbContext.Routes.AddOrUpdate(c => c.Name, new Route { Name = "Name2", Created = DateTime.Now, CreatedBy = "cvazquez", Distance = 13 });
            appDbContext.Routes.AddOrUpdate(c => c.Name, new Route { Name = "Name3", Created = DateTime.Now, CreatedBy = "cvazquez", Distance = 14 });

            appDbContext.SaveChanges();
        }

        // No se usa EF porque al crear el Profile también crea el Membership con su clave encriptada automáticamente.
        private static void SeedSecurity()
        {
            WebSecurity.InitializeDatabaseConnection("AppDbContext", "UserProfile", "UserId ", "UserName", autoCreateTables: true);

            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }
            if (!roles.RoleExists("Guest"))
            {
                roles.CreateRole("Guest");
            }
            if (!roles.RoleExists("Premium"))
            {
                roles.CreateRole("Premium");
            }
            if (membership.GetUser("admin", false) == null)
            {
                WebSecurity.CreateUserAndAccount("admin", "Passw0rd", new { FirstName = "Carlos", LastName = "Daniel", Email = "carlos.vazquez@outlook.com" }); 
                   
                Roles.AddUserToRole("admin", "Admin");
            }
        }
    }

       
}
