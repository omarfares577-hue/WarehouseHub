using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHub.Core.Specification
{
    public class BaseSpecification<T> : ISpecification<T> 
    {
        public BaseSpecification() { }
        public BaseSpecification(Expression<Func<T, bool>>  criteria) { Criteria = criteria;}
        public Expression<Func<T, bool>>? Criteria { get; private set; }
        public List<Expression<Func<T, object>>>? Includes { get; private set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPaggingEnabled { get; private set; }
        public void AddInclude(Expression<Func<T, object>> include)
        { Includes!.Add(include); }
        public void AddOrderBy(Expression<Func<T, object>>? orderbyexpression)
        { OrderBy = orderbyexpression; OrderByDescending = null; }
        public void AddOrderByDesc(Expression<Func<T, object>>? orderbydescexpression)
        { OrderByDescending = orderbydescexpression; OrderBy = null; }
        public void ApplyPagging(int skip,int take)
        {
            Take = take;
            Skip = skip;
            IsPaggingEnabled = true;
        }
    }
}
