using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core.Contracts;
using WarehouseHub.Domain.Entities;
using WarehouseHub.Infrastructure.Data;

namespace WarehouseHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
       public IGenericrepository<WarehouseItem> WarehouseItems { get; }
        public IGenericrepository<InventoryLog> InventoryLogs { get; }
        public IGenericrepository<Category> Categories { get; }
        public IGenericrepository<Supplier> Suppliers { get; }
        public IGenericrepository<ArchivedInventoryLog> ArchivedInventoryLogs { get; }
        private IDbContextTransaction? _transaction;
        private readonly WarehouseContext _context;
        public UnitOfWork(WarehouseContext context)
        {
            _context = context;
            WarehouseItems = new GenericRepository<WarehouseItem>(_context);
            InventoryLogs = new GenericRepository<InventoryLog>(_context);
            Suppliers = new GenericRepository<Supplier>(_context);
            Categories = new GenericRepository<Category>(_context);
            ArchivedInventoryLogs = new GenericRepository<ArchivedInventoryLog>(_context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }
    }
}
