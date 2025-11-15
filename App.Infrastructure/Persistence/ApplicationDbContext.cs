using App.Domain.Entities;
using App.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurations uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Kategoriler
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Elektronik",
                    Description = "Elektronik ürünler ve aksesuarlar",
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = 2,
                    Name = "Giyim",
                    Description = "Giyim ve moda ürünleri",
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = 3,
                    Name = "Ev & Yaşam",
                    Description = "Ev dekorasyonu ve yaşam ürünleri",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Admin kullanıcı (şifre: Admin123!)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@ecommerce.com",
                    // BCrypt hash - gerçek uygulamada PasswordHasher kullanın
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FirstName = "Admin",
                    LastName = "User",
                    Role = Domain.Enums.UserRole.Admin,
                    IsEmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Örnek ürünler
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15 Pro",
                    Description = "Apple iPhone 15 Pro 256GB Titanyum",
                    Price = 45999.99m,
                    StockQuantity = 50,
                    SKU = "APPL-IPH15P-256-TIT",
                    CategoryId = 1,
                    IsActive = true,
                    ImageUrl = "https://via.placeholder.com/400x400?text=iPhone+15+Pro",
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S24",
                    Description = "Samsung Galaxy S24 Ultra 512GB",
                    Price = 38999.99m,
                    StockQuantity = 35,
                    SKU = "SAMS-S24U-512-BLK",
                    CategoryId = 1,
                    IsActive = true,
                    ImageUrl = "https://via.placeholder.com/400x400?text=Galaxy+S24",
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "Nike Air Max",
                    Description = "Nike Air Max 270 Erkek Spor Ayakkabı",
                    Price = 3299.99m,
                    StockQuantity = 100,
                    SKU = "NIKE-AM270-WHT-42",
                    CategoryId = 2,
                    IsActive = true,
                    ImageUrl = "https://via.placeholder.com/400x400?text=Nike+Air+Max",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
