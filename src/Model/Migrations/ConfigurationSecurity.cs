using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WebMatrix.WebData;

namespace Model.Migrations
{
    internal sealed partial class Configuration
    {
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
