using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;
namespace WarehouseHub.Infrastructure.Data
{
    public class IdentityContext : IdentityDbContext<AppUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<AppUser> AppUsers { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Address>().HasOne(a => a.AppUser).WithOne(d => d.Address).HasForeignKey<Address>(a => a.AppUserId);
        }
    }
}


