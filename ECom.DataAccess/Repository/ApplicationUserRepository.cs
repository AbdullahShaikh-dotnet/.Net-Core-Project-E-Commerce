﻿using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility.Interface;

namespace ECom.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db, ICacheService cacheService) : base(db, cacheService)
        {
            _db = db;
        }

        public void Update(ApplicationUser ApplicationUserObject)
        {
            _db.ApplicationUsers.Update(ApplicationUserObject);
        }
    }
}
