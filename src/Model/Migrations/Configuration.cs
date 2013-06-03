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
    using Framework.Models;

    internal sealed partial class Configuration : DbMigrationsConfiguration<Model.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Model.AppDbContext appDbContext)
        {
            SeedSecurity();
            SeedSQLProcedures(appDbContext);
            SeedBuisness(appDbContext);
        }

        private static void SeedBuisness(Model.AppDbContext appDbContext)
        {
            var attributeAirConditioner = new Model.Attribute { Name = "Aire Acondicionado", Created = DateTime.Now, CreatedBy = "cvazquez"};
            var attributeParking = new Model.Attribute { Name = "Estacionamiento", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var attributeCashOnly = new Model.Attribute { Name = "No Acepta Tarjetas", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var attributeTakesReservation = new Model.Attribute { Name = "Toma Reservas", Created = DateTime.Now, CreatedBy = "cvazquez" };

            var category1 = new Category { Name = "Categoria1", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var category11 = new Category { Parent = category1, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria11" };

            var category2 = new Category { Name = "Categoria2", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var category21 = new Category { Parent = category2, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria21" };
            var category22 = new Category { Parent = category2, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria22" };
            var category23 = new Category { Parent = category2, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria23" };

            var category3 = new Category { Name = "Categoria3", Created = DateTime.Now, CreatedBy = "cvazquez" };
            var category31 = new Category { Parent = category3, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria31" };
            var category311 = new Category { Parent = category31, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria311" };
            var category312 = new Category { Parent = category31, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria312" };
            var category313 = new Category { Parent = category31, Created = DateTime.Now, CreatedBy = "cvazquez", Name = "Categoria313" };

            var country = new Country { Created = DateTime.Now, Name = "USA", CreatedBy = "CVazquez" };
            var state = new State { Created = DateTime.Now, Name = "New York", CreatedBy = "CVazquez", Country = country};

            var buffalo = new City { Created = DateTime.Now, Name = "Buffalo", CreatedBy = "CVazquez", State = state };
            var newYork = new City { Created = DateTime.Now, Name = "New York", CreatedBy = "CVazquez", State = state };
            var brooklyn = new City { Created = DateTime.Now, Name = "Brooklyn", CreatedBy = "CVazquez", State = state };
            var hoboken = new City { Created = DateTime.Now, Name = "Hoboken", CreatedBy = "CVazquez", State = state };
            var bronx = new City { Created = DateTime.Now, Name = "Bronx", CreatedBy = "CVazquez", State = state };

            var business1 = new Business
                             {
                                 Comments = "Comment 1",
                                 Name = "Buffalo Business",
                                 Created = DateTime.Now,
                                 CreatedBy = "CVazquez",
                                 HeadLocation = new Location {Description = "Main Address", Latitude = 45.1761F, Longitude = -93.8734F, City = buffalo, Created = DateTime.Now, CreatedBy = "cvazquez"},
                                 CompleteAddress = "435 West 5th Street Buffalo NY - 10101",
                                 Categories = new List<Category> { category11 },
                                 Attributes = new List<Model.Attribute> { attributeAirConditioner, attributeTakesReservation },
                                 Rating = Rating.FiveStars
                             };

            appDbContext.Businesses.AddOrUpdate(c => c.Name, business1);

            // HOBOKEN
            var business4 = new Business
            {
                Comments = "Comment 4",
                Name = "Hoboken Business",
                Created = DateTime.Now,
                CreatedBy = "CVazquez",
                HeadLocation = new Location { Description = "Main Address", Latitude = 40.7439F, Longitude = -74.0328F, City = hoboken, Created = DateTime.Now, CreatedBy = "cvazquez" },
                CompleteAddress = "21 West 31st Street Hoboken NJ - 10101",
                Categories = new List<Category> { category311 },
                Attributes = new List<Model.Attribute> { attributeParking, attributeCashOnly },
                Rating = Rating.FourStars
            };

            appDbContext.Businesses.AddOrUpdate(c => c.Name, business4);

            // NEW YORK
            for (int i = 1; i <= 3; i++)
            {
                var categories = new List<Category>();

                switch (i % 3)
                {
                    case 0:
                        categories = new List<Category> { category21 };
                        break;
                    case 1:
                        categories = new List<Category> { category22 };
                        break;
                    case 2:
                        categories = new List<Category> { category23 };
                        break;
                    default:
                        break;
                }

                var business = new Business
                {
                    Comments = "Comment 2",
                    Name = "New York Business " + i,
                    Created = DateTime.Now,
                    CreatedBy = "CVazquez",
                    HeadLocation = new Location { Description = "Main Address", Latitude = 40.7143F, Longitude = -74.006F, City = newYork, Created = DateTime.Now, CreatedBy = "cvazquez" },
                    CompleteAddress = "1" + i.ToString() + " West 31st Street New York NY - 10001",
                    Categories = categories,
                    Attributes = new List<Model.Attribute> { attributeAirConditioner, attributeCashOnly },
                    Rating = Rating.OneStar
                };

                appDbContext.Businesses.AddOrUpdate(c => c.Name, business);
            }

            // BROOKLYN
            for (int i = 1; i <= 19; i++)
            {

                var business = new Business
                {
                    Comments = "Comment 4",
                    Name = "Brooklyn Business " + i,
                    Created = DateTime.Now,
                    CreatedBy = "CVazquez",
                    HeadLocation = new Location { Description = "Main Address", Latitude = 40.6500F, Longitude = -73.9500F, City = brooklyn, Created = DateTime.Now, CreatedBy = "cvazquez" },
                    CompleteAddress = "Autocompleted",
                    Categories = new List<Category> { category312 },
                    Attributes = new List<Model.Attribute> { attributeParking, attributeCashOnly },
                    Rating = Rating.FourStars
                };

                appDbContext.Businesses.AddOrUpdate(c => c.Name, business);
            }

            // BRONX
            for (int i = 1; i <= 30; i++)
            {

                var business = new Business
                {
                    Comments = "Comment 4",
                    Name = "Bronx Business " + i,
                    Created = DateTime.Now,
                    CreatedBy = "CVazquez",
                    HeadLocation = new Location { Description = "Main Address", Latitude = 40.8500F, Longitude = -73.8667F, City = bronx, Created = DateTime.Now, CreatedBy = "cvazquez" },
                    CompleteAddress = "Autocompleted",
                    Categories = new List<Category> { category313 },
                    Attributes = new List<Model.Attribute> { attributeParking, attributeCashOnly },
                    Rating = Rating.FourStars
                };

                appDbContext.Businesses.AddOrUpdate(c => c.Name, business);
            }

            appDbContext.SaveChanges();
        }

    }

       
}
