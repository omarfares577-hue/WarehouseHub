using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core.Contracts;
using WarehouseHub.Core.Specification;
using WarehouseHub.Infrastructure.Data;

namespace WarehouseHub.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericrepository<T> where T : class 
    {
        private readonly WarehouseContext _context;
        public GenericRepository(WarehouseContext context) { _context = context; }
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return  SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }
        public async Task<T?> GetWithSpecById(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }
        public async Task<T?> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }
        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        public async Task AddAsync(T entity)
        {
           await _context.AddAsync( entity);
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
