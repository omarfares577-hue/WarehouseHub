using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core.Specification;

namespace WarehouseHub.Core.Contracts
{
    public interface IGenericrepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<T?> GetById(int id);
        Task<T?> GetWithSpecById(ISpecification<T> spec);
        Task<int> GetCountAsync(ISpecification<T> spec);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
