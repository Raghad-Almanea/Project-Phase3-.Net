using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterDelivery.Models.TableDb;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WaterDelivery.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string FullName { get; set; }
        public string Img { get; set; }
        public int Type { get; set; } //Admin = 1  //Site = 2 // Provider = 3
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int FkCity { get; set; }
        

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
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<AddressUser> AddressUser { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<Device_Id> Device_Id { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderInfo> OrderInfo { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Provider> Provider { get; set; }
        public DbSet<Setting> Setting { get; set; }

        public DbSet<Slider> Slider { get; set; }

        public DbSet<Cart> Cart { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Notify> Notify { get; set; }
        public DbSet<Rate> Rate { get; set; }
        public DbSet<Copon> Copons { get; set; }
        public DbSet<CoponUsed> CoponUseds { get; set; }
        public DbSet<Favorite> Favorites { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }



    }
}