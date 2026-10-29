using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Domain.Entities
{
    public class ArchivedInventoryLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "WarehouseItemId Has InValid Value ")]
        public int WarehouseItemId { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "WarehouseItemId Has InValid Value ")]
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "QuantityChanged Field is Required")]
        [Range(1, 10000, ErrorMessage = "QuantityChanged Must Be Larger than 0")]

        public int QuantityChanged { get; set; }
        [Required(ErrorMessage = "OperationType Field is Required")]
        public Operation OperationType { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public WarehouseItem WarehouseItem { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;

    }
}
