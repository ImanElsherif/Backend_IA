using System.Linq.Expressions;

namespace WebApplication1.Data
{
    public interface IDataRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByCustomCriteria(Expression<Func<T, bool>> criteria);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> Save();
    }
}
