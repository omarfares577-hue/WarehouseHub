using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WarehouseHub.Infrastructure.Data;
using System.Data.Common;
using System.Linq;

namespace WarehouseHub.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private DbConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // إزالة الـ DbContext القديم الخاص بـ SQL Server
                var descriptorsToRemove = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<WarehouseContext>) ||
                    d.ServiceType == typeof(WarehouseContext)).ToList();

                foreach (var descriptor in descriptorsToRemove)
                {
                    services.Remove(descriptor);
                }

                // إنشاء اتصال SQLite In-Memory في الذاكرة
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                // تسجيل الـ DbContext الجديد باستخدام SQLite
                services.AddDbContext<WarehouseContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // إنشاء قاعدة البيانات والجداول فوراً للاختبارات
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}