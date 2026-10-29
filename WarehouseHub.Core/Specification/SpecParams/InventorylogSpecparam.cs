using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Specification.SpecParams
{
    public class InventorylogSpecparam : BaseSpecParam
    {
        public int? WareHouseId { get; set; }
        public int? SupplierId { get; set; }

    }
}
