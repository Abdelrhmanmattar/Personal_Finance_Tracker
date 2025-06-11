using Core.Specification;

namespace Core.Interfaces
{
    public interface IBaseRepo<T> where T : class
    {
        T? GetByID(int id);
        bool AddEntity(T entity);
        Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        bool DeleteEntity(int id);
        bool UpdateEntity(T entity);
        IReadOnlyList<T> GetAll();
        T? Find(ISpecification<T> specification);
        IReadOnlyList<T> FindAll(ISpecification<T> specification);
    }
}
