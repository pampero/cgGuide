using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using Model.Mappings;

namespace Model
{
    public class AppDbContext: DbContext
    {
        public AppDbContext()
            : base("Name=AppDbContext")
        {
        }

        public DbSet<Route> Routes { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<UserProfile> UserProfiles{ get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new RouteMap());
        }
    }
}
