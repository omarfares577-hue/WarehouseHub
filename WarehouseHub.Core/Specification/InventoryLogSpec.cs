using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core.Specification.SpecParams;
using WarehouseHub.Domain.Entities;

namespace WarehouseHub.Core.Specification
{
    public class InventoryLogSpec : BaseSpecification<InventoryLog>
    {
        public InventoryLogSpec() { }
        public InventoryLogSpec(int id) : base(p=>p.Id == id) { AddInclude(p => p.WarehouseItem); AddInclude(p => p.Supplier); }
        public InventoryLogSpec(InventorylogSpecparam specparam):
            base(p=>((string.IsNullOrEmpty(specparam.Search) ||
        (p.WarehouseItem != null && p.WarehouseItem.Name.ToLower().Contains(specparam.Search!.ToLower())) ||
        (p.Supplier != null && p.Supplier.Name.ToLower().Contains(specparam.Search.ToLower())))) &&
        (!specparam.WareHouseId.HasValue || p.WarehouseItemId == specparam.WareHouseId) &&
           (!specparam.SupplierId.HasValue || p.SupplierId == specparam.SupplierId))
        {
            AddInclude(p => p.WarehouseItem);
            AddInclude(p => p.Supplier);
            AddOrderBy(p => p.Id);
            if(!string.IsNullOrEmpty(specparam.Sort))
                switch(specparam.Sort.ToLower())
                {
                    case "dateasc":
                        AddOrderBy(p => p.Timestamp);
                        break;
                    case "datedesc":
                        AddOrderByDesc(p => p.Timestamp);
                        break;
                    default:
                        AddOrderBy(p => p.Id);
                        break;
                }
            ApplyPagging(specparam.PageSize * (specparam.PageIndex - 1), specparam.PageSize);

        }
    }
}
