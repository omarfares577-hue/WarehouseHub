using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core.Specification.SpecParams;
using WarehouseHub.Domain.Entities;

namespace WarehouseHub.Core.Specification
{
        public class InventoryLogArchiveSpec : BaseSpecification<ArchivedInventoryLog>
        {
            public InventoryLogArchiveSpec(InventorylogSpecparam specParam)
                : base(x =>
                    (string.IsNullOrEmpty(specParam.Search) ||
        (x.WarehouseItem != null && x.WarehouseItem.Name.ToLower().Contains(specParam.Search.ToLower())) ||
        (x.Supplier != null && x.Supplier.Name.ToLower().Contains(specParam.Search.ToLower())))
                )
            {
                AddInclude(x => x.WarehouseItem);
                AddInclude(x => x.Supplier);

                AddOrderByDesc(x => x.Timestamp);

                ApplyPagging(specParam.PageSize * (specParam.PageIndex - 1), specParam.PageSize);
            }
        }
    
}
