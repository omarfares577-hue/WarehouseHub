using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Dtos
{
    public class InventoryLogDto
    {
        public int Id { get; set; }
        public string WarehouseItemName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;

        public int QuantityChanged { get; set; } 
        public string OperationType { get; set; } = string.Empty; 
        public string Timestamp { get; set; } = string.Empty;
    }
}
