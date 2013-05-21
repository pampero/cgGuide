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

            var country = new Country { Created = DateTime.Now, Name = "USA", CreatedBy = "CVazquez" };

            var state = new State { Created = DateTime.Now, Name = "New York", CreatedBy = "CVazquez", Country = country};

            var city1 = new City { Created = DateTime.Now, Name = "Buffalo", CreatedBy = "CVazquez", State = state };

            var city2 = new City { Created = DateTime.Now, Name = "New York", CreatedBy = "CVazquez", State = state };

            var location1 = new Location {Description = "Main Address", Latitude = 45.1761F, Longitude = -93.8734F, City = city1, Created = DateTime.Now, CreatedBy = "cvazquez"};

            var location2 = new Location { Description = "Main Address", Latitude = 40.7143F, Longitude = -74.006F, City = city2, Created = DateTime.Now, CreatedBy = "cvazquez" };
          
            var seller1 = new Seller
                             {
                                 Comments = "Comment 1",
                                 Name = "Seller 1",
                                 Created = DateTime.Now,
                                 CreatedBy = "CVazquez",
                                 HeadLocation = location1,
                                 CompleteAddress = "435 West 5th Street Buffalo NY - 10101"
                             };

            var seller2 = new Seller
            {
                Comments = "Comment 2",
                Name = "Seller 2",
                Created = DateTime.Now,
                CreatedBy = "CVazquez",
                HeadLocation = location2,
                CompleteAddress = "19 West 31st Street New York NY - 10001"
            };

            appDbContext.Sellers.AddOrUpdate(c => c.Name, seller1);
            appDbContext.Sellers.AddOrUpdate(c => c.Name, seller2);

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
