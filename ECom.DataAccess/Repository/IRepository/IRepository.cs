using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includePropertiesList = null, bool applyCaching = false);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includePropertiesList = null, bool applyCaching = false);
        T Get(Expression<Func<T, bool>> filter, string? includePropertiesList = null, bool tracked = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        Task AddAsync(T entity);
        void BulkAdd(IEnumerable<T> entities);
        Task BulkAddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }
}
