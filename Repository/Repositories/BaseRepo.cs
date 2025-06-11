using Core.Interfaces;
using Core.Specification;
using Microsoft.EntityFrameworkCore;
using Repository.Specification;

namespace Repository.Repositories
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        private readonly DbContext context;

        public BaseRepo(DbContext _context)
        {
            context = _context;
        }
        public T? GetByID(int id) =>
                                     context.Set<T>().Find(id);
        public IReadOnlyList<T> GetAll() =>
                                    context.Set<T>().AsNoTracking().ToList();

        public bool AddEntity(T entity)
        {
            var feedback = context.Set<T>().Add(entity);
            return feedback.State == EntityState.Added;
        }
        public async Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || !entities.Any())
                return false;

            await context.Set<T>().AddRangeAsync(entities, cancellationToken);
            return true;
        }

        public bool DeleteEntity(int id)
        {
            T? entity = this.GetByID(id);
            if (entity is null) return false;
            var feedback = context.Set<T>().Remove(entity);
            return feedback.State == EntityState.Deleted;
        }

        public T? Find(ISpecification<T> specification)
        {
            IQueryable<T> query = context.Set<T>().AsNoTracking();
            var values = SpecificationHandler<T>.ApplySpecification(query, specification);
            return values.AsNoTracking().FirstOrDefault();
        }

        public IReadOnlyList<T> FindAll(ISpecification<T> specification)
        {
            IQueryable<T> query = context.Set<T>().AsNoTracking();
            var values = SpecificationHandler<T>.ApplySpecification(query, specification);
            return values.AsNoTracking().ToList();
        }
        public bool UpdateEntity(T entity)
        {
            var state = context.Set<T>().Update(entity);
            return state.State == EntityState.Modified;
        }
    }

}
