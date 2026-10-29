using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Dtos
{
    public class CreateSupplierDto
    {
        [Required(ErrorMessage = "NameField is Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name Must be between 2:50")]
        public string Name { get; set; } = string.Empty;
    }
}
