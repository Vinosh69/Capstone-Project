using FoodDeliveryApp.Models;
using FoodDeliveryApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryApplication.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Food> Foods { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<ProductsSold> ProductsSold { get; set; }
    }
}