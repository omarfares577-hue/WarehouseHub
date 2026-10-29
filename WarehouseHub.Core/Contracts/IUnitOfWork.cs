using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;

namespace WarehouseHub.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericrepository<WarehouseItem> WarehouseItems { get; }
        IGenericrepository<InventoryLog> InventoryLogs { get; }
        IGenericrepository<ArchivedInventoryLog> ArchivedInventoryLogs { get; }
        IGenericrepository<Category> Categories { get; }
        IGenericrepository<Supplier> Suppliers { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> CompleteAsync();
    }
}
