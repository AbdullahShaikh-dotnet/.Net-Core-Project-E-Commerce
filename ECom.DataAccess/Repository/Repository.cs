using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ECom.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbset = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbset.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includePropertiesList = null, bool tracked = false)
        {
            IQueryable<T> query = tracked ? dbset : dbset.AsNoTracking();
            query = query.Where(filter);
            if (string.IsNullOrEmpty(includePropertiesList))
                return query.FirstOrDefault();

            foreach (var property in includePropertiesList.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(property);
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includePropertiesList = null)
        {
            IQueryable<T> query = dbset;
            if (filter is not null)
            {
                query = query.Where(filter);
            }
            if (string.IsNullOrEmpty(includePropertiesList))
                return query.ToList();

            foreach (var property in includePropertiesList.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(property);
            }

            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbset.RemoveRange(entity);
        }
    }
}
