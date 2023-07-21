using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PantryManagementBudgetApp2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
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

        // Add "Period" entity to system
        // All other tables (balances, cashflows) will reference the periods table
        public DbSet<Period> Periods { get; set; }

        // Add "Balance" entity to system
        public DbSet<Balance> Balances { get; set; }

        // Add "Cashflow" entity to system
        public DbSet<Cashflow> Cashflows { get; set; }

        public DbSet<PantryItem> PantryItems { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Purchase> Purchases { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}