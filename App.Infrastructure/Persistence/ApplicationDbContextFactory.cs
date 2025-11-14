using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory
         : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // 🔥 Geliştirme ortamında kullanacağın connection string
            // SQL Server için örnek:
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-187A7MN\\MSSQLEKREM;Database=App;Trusted_Connection=True;TrustServerCertificate=True");

            // Eğer PostgreSQL kullanıyorsan bunun yerine:
            // optionsBuilder.UseNpgsql(
            //     "Host=localhost;Port=5432;Database=ECommerceDb;Username=postgres;Password=123456");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
