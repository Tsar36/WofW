using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace WorldOfWords.Infrastructure.Data.EF.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        public EfRepository(DbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentException("dbContext");
            }

            DbContext = dbContext;
            DbSet = dbContext.Set<T>();
            dbContext.SaveChanges();
        }
        protected DbContext DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

       

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual Task<T> GetByIdAsync(int id)
        {
            return DbSet.FindAsync(id);
        }

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        
        public virtual void Add(IEnumerable<T> entities)
        {
            foreach(var entity in entities)
            {
                Add(entity);
            }
        }

        
        public void Update(T entity)
        {
            DbSet.AddOrUpdate(entity);
            
            //DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            //if (dbEntityEntry.State == EntityState.Detached)
            //{
            //    DbSet.Attach(entity);
            //}
            //dbEntityEntry.State = EntityState.Modified;
        }
        
        public void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach(var entity in entities.ToList())
            {
                Delete(entity);
            }
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null)
            {
                return;
            }
            Delete(entity);
        }

        protected bool Exists(int id)
        {
            return GetById(id) != null;
        }

        protected async Task<Boolean> ExistsAsync(int id)
        {
            return await GetByIdAsync(id) != null;
        }
    }
}
