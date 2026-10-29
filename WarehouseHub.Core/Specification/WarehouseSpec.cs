using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core.Specification.SpecParams;
using WarehouseHub.Domain.Entities;

namespace WarehouseHub.Core.Specification
{
    public class WarehouseSpec : BaseSpecification<WarehouseItem>
    {
        public WarehouseSpec() { }
        public WarehouseSpec(int id) : base(p=>p.Id == id) { AddInclude(p => p.Category); }
        public WarehouseSpec(WarehouseitemSpecparam specparam) : 
            base(p=> (String.IsNullOrEmpty(specparam.Search) || p.Name.ToLower().Contains(specparam.Search.ToLower()))
            && (!specparam.CategoryId.HasValue || p.CategoryId == specparam.CategoryId))
        {
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Id);
            if(!string.IsNullOrEmpty(specparam.Sort))
                switch (specparam.Sort.ToLower())
                {
                    case "priceasc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "pricedesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Id);
                        break;
                }
            ApplyPagging(specparam.PageSize * (specparam.PageIndex - 1), specparam.PageSize);
        }
    }
}
