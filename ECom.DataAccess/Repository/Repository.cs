using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Utility;
using ECom.Utility.Interface;
using ECom.Utility.Services;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ECom.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;
        private readonly ICacheService _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public Repository(ApplicationDbContext db, ICacheService cacheService)
        {
            _db = db;
            dbset = _db.Set<T>();
            _cacheService = cacheService;
            _httpContextAccessor = new HttpContextAccessor();
            _userService = new UserService(_httpContextAccessor);
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

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includePropertiesList = null, bool applyCaching = false)
        {
            var entityName = typeof(T).Name;

            // Rate Limiter for Particular Entity 5 Request Per Second for a Particular User 
            string key = $"rate_limit:{_userService.GetUserId()}{entityName}";
            if (await _cacheService.IsRateLimitedAsync(key, SD.RATE_LIMITING_COUNT, TimeSpan.FromSeconds(1)))
                throw new Exception("Too many requests. Please try again later.");


            IQueryable<T> query;
            var cachedData = await _cacheService.GetAsync<IEnumerable<T>>(entityName);

            if (cachedData is not null && applyCaching)
            {
                query = cachedData.AsQueryable();
            }
            else
            {
                query = dbset;
                if (filter is not null)
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrEmpty(includePropertiesList))
                {
                    foreach (var property in includePropertiesList.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(property);
                    }
                }
                var resultList = query.ToList();

                if (applyCaching)
                    _ = await _cacheService.SetAsync(entityName, resultList, TimeSpan.FromSeconds(SD.CACHE_TIMESPAN_SECONDS));

                return resultList;
            }
            return query.ToList();
        }


        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includePropertiesList = null, bool applyCaching = false)
        {
            var entityName = typeof(T).Name;

            // Rate Limiter for Particular Entity 5 Request Per Second for a Particular User 
            string key = $"rate_limit:{_userService.GetUserId()}{entityName}";
            if (_cacheService.IsRateLimitedAsync(key, SD.RATE_LIMITING_COUNT, TimeSpan.FromSeconds(1)).GetAwaiter().GetResult())
                throw new Exception("Too many requests. Please try again later.");


            IQueryable<T> query;
            var cachedData = _cacheService.GetAsync<IEnumerable<T>>(entityName).GetAwaiter().GetResult();

            if (cachedData is not null && applyCaching)
            {
                query = cachedData.AsQueryable();
            }
            else
            {
                query = dbset;
                if (filter is not null)
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrEmpty(includePropertiesList))
                {
                    foreach (var property in includePropertiesList.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(property);
                    }
                }
                var resultList = query.ToList();

                if (applyCaching)
                    _cacheService.SetAsync(entityName, resultList, TimeSpan.FromSeconds(SD.CACHE_TIMESPAN_SECONDS))?.GetAwaiter().GetResult();

                return resultList;
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
