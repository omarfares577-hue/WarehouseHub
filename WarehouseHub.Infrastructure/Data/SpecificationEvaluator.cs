using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core.Specification;

namespace WarehouseHub.Infrastructure.Data
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputquery, ISpecification<T> spec)
        {
            var query = inputquery;
            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);
            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending is not null)
                query = query.OrderByDescending(spec.OrderByDescending);
            if (spec.IsPaggingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);
            if (spec is not null && spec.Includes is not null)
                query = spec.Includes!.Aggregate(query, (current, include) => current.Include(include));
            return query;

        }
    }
}
