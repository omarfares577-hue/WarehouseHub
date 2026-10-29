using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Domain.Entities
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "NameField is Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name Must be between 2:50")]
        public string Name { get; set; } = string.Empty;
        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();


    }
}
