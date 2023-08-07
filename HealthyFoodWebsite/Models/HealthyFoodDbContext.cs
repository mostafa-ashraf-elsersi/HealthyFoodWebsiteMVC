using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
namespace HealthyFoodWebsite.Models
{
    public class HealthyFoodDbContext : DbContext
    {
        // Context Object Contructor
        public HealthyFoodDbContext(DbContextOptions options) : base(options) { }


        // Context Object Database Sets
        public DbSet<Logger> Logger { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Testimonial> Testimonial { get; set; }
        public DbSet<BlogPost> Blog { get; set; }
        public DbSet<BlogSubscriber> BlogSubscriber { get; set; }
        public DbSet<CustomerMessage> CustomerMessage { get; set; }
        public DbSet<ShoppingBagItem> ShoppingBag { get; set; }
        public DbSet<Order> Order { get; set; }

        
        // Inherited DbContext Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(options =>
            {
                options.HasIndex(e => e.Name).IsUnique();
                //options.HasQueryFilter(e => e.IsDisplayed);
            });

            modelBuilder.Entity<Logger>(options =>
            {
                options.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<BlogPost>(options =>
            {
                options.HasIndex(e => e.Title).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
