using System.Linq.Expressions;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includePropertiesList = null);
        T Get(Expression<Func<T, bool>> filter, string? includePropertiesList = null, bool tracked = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
