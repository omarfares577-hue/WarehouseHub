using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Domain.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "NameField is Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name Must be between 2:50")]

        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Discription Field is Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name Must be between 2:100")]

        public string Discription { get; set; } = string.Empty;
        public ICollection<WarehouseItem> WarehouseItems = new List<WarehouseItem>();
    }
}
