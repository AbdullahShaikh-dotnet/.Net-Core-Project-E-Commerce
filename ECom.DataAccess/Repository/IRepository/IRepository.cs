using System.Linq.Expressions;

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
    }
}
