using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;

namespace WarehouseHub.Infrastructure.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext(DbContextOptions<WarehouseContext> options) : base(options)
        {
        }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ArchivedInventoryLog> ArchivedInventoryLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WarehouseItem>(entity =>
            {
                entity.Property(i => i.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(i => i.Sku)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.HasIndex(i => i.Sku)
                      .IsUnique();

                entity.Property(i => i.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
            });

            modelBuilder.Entity<InventoryLog>(entity =>
            {
                entity.Property(l => l.OperationType)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasConversion<string>();

                entity.HasOne(log => log.WarehouseItem)
                      .WithMany(item => item.InventoryLogs)
                      .HasForeignKey(log => log.WarehouseItemId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(log => log.Supplier)
                      .WithMany(item => item.InventoryLogs) 
                      .HasForeignKey(log => log.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
