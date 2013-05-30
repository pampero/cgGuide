using Model.Enums;

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
            var attributeAirConditioner = new Model.Attribute { Name = "Aire Acondicionado", Created = DateTime.Now, CreatedBy = "cvazquez"};
            var attributeParking = new Model.Attribute { Name = "Estacionamiento", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var attributeCashOnly = new Model.Attribute { Name = "No Acepta Tarjetas", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var attributeTakesReservation = new Model.Attribute { Name = "Toma Reservas", Created = DateTime.Now, CreatedBy = "cvazquez" };

            var category1 = new Category { Name = "Categoria1", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var category2 = new Category { Name = "Categoria2", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var category3 = new Category { Name = "Categoria3", Created = DateTime.Now, CreatedBy = "cvazquez" };

            var category11 = new Category
                                   {
                                       Parent = category1,
                                       Created = DateTime.Now,
                                       CreatedBy = "cvazquez",
                                       Name = "Categoria11"
                                   };

            var category21 = new Category
            {
                Parent = category2,
                Created = DateTime.Now,
                CreatedBy = "cvazquez",
                Name = "Categoria21"
            };

            var category31 = new Category
            {
                Parent = category3,
                Created = DateTime.Now,
                CreatedBy = "cvazquez",
                Name = "Categoria31"
            };
            
            var category311 = new Category
            {
                Parent = category31,
                Created = DateTime.Now,
                CreatedBy = "cvazquez",
                Name= "Categoria311"
            };

            
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
                                 CompleteAddress = "435 West 5th Street Buffalo NY - 10101",
                                 Categories = new List<Category> { category11 },
                                 Attributes = new List<Model.Attribute> { attributeAirConditioner, attributeTakesReservation },
                                 Rating = Rating.FiveStars
                             };

            var seller2 = new Seller
            {
                Comments = "Comment 2",
                Name = "Seller 2",
                Created = DateTime.Now,
                CreatedBy = "CVazquez",
                HeadLocation = location2,
                CompleteAddress = "19 West 31st Street New York NY - 10001",
                Categories = new List<Category> { category21 },
                Attributes = new List<Model.Attribute> { attributeAirConditioner, attributeCashOnly },
                Rating = Rating.OneStar
            };

            var seller3 = new Seller
            {
                Comments = "Comment 3",
                Name = "Seller 3",
                Created = DateTime.Now,
                CreatedBy = "CVazquez",
                HeadLocation = location2,
                CompleteAddress = "20 West 31st Street New York NY - 10001",
                Categories = new List<Category> { category31 },
                Attributes = new List<Model.Attribute> { attributeAirConditioner, attributeTakesReservation },
                Rating = Rating.FiveStars
            };

            var seller4 = new Seller
            {
                Comments = "Comment 4",
                Name = "Seller 4",
                Created = DateTime.Now,
                CreatedBy = "CVazquez",
                HeadLocation = location2,
                CompleteAddress = "21 West 31st Street New York NY - 10001",
                Categories = new List<Category> {  category311 },
                Attributes = new List<Model.Attribute> { attributeParking, attributeCashOnly },
                Rating = Rating.FourStars
            };

            appDbContext.Sellers.AddOrUpdate(c => c.Name, seller1);
            appDbContext.Sellers.AddOrUpdate(c => c.Name, seller2);
            appDbContext.Sellers.AddOrUpdate(c => c.Name, seller3);
            appDbContext.Sellers.AddOrUpdate(c => c.Name, seller4);

            appDbContext.Database.ExecuteSqlCommand(@"
CREATE PROCEDURE sp_GetCategories

	@seller_id as integer

AS

	SET NOCOUNT ON

SELECT Name as Categories
FROM Category 
WHERE Id IN (SELECT Category_ID FROM CategorySeller WHERE Seller_ID = @seller_id)");


            appDbContext.Database.ExecuteSqlCommand(@"
CREATE PROCEDURE sp_GetPaths

	@seller_id as integer

AS

	SET NOCOUNT ON

declare @T table(ID int, Name varchar(500), Parent_Id int);

;WITH Cat
AS
(
    SELECT Id AS StartingId, Id, Parent_Id, Name
    FROM Category

    UNION ALL

    SELECT Cat.StartingId, C.Id, C.Parent_Id, C.Name
    FROM Category C INNER JOIN Cat ON C.Id = Cat.Parent_Id
)
INSERT INTO @T (ID, Name, Parent_Id)
SELECT Id, Name, Parent_Id
FROM Cat 
WHERE Cat.StartingId IN (SELECT Category_ID FROM CategorySeller WHERE Seller_ID = @seller_id)

;with C as
(
  select ID,
         Name,
         Parent_Id,
         cast('' as varchar(max)) as ParentNames,
         0 As Generation
  from @T
  where Parent_Id is null
  union all
  select T.ID,
         T.Name,
         T.Parent_Id,
         C.ParentNames + '/' + C.Name,
         Generation + 1 As Generation
  from @T as T         
    inner join C
      on C.ID = T.Parent_Id
)      
select ID,
       Name as Categories,
       CASE WHEN ParentNames = '' THEN
			'0/' + Name
       ELSE
			Convert(varchar, Generation) + '/' + stuff(ParentNames, 1, 1, '') + '/' + Name 
       END as Paths
from C;");

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
