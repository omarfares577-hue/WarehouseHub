using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WarehouseHub.Domain.Entities
{
    public class WarehouseItem
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name Field is Required")]
        [StringLength(50, MinimumLength =2,ErrorMessage = "Name Must be between 2:50")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Sku Field is Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Sku Must be between 2:50")]
        public string Sku { get; set; } = string.Empty;
        [Required(ErrorMessage = "Price Field is Required")]
        [Range(0.01, int.MaxValue, ErrorMessage = "Price Must Be Larger than 0")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "StockQuantity Field is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "StockQuantity Must Be Larger than 0")]

        public int StockQuantity { get; set; }
        [Range(0,1000,ErrorMessage ="MinStockLevel Has InValid Value ")]
        public int MinStockLevel = 5;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     
        public DateTime? UpdatedAt { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "CategoryId Has InValid Value ")]

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
    }
}
