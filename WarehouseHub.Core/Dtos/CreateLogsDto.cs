using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;

namespace WarehouseHub.Core.Dtos
{
    public class CreateLogsDto
    {
        [Required]
        [Range(1,int.MaxValue,ErrorMessage = "Enter a Positive Number for WarehouseItemId")]
        public int WarehouseItemId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a Positive Number for SupplierId")]
        public int SupplierId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter a Positive Number for QuantityChanged")]
        public int QuantityChanged { get; set; }
        [Required]
        public string OperationType { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;


    }
}
