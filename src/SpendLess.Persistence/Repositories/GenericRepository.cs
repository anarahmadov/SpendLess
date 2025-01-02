using Microsoft.EntityFrameworkCore;
using SpendLess.Application.Contracts.Persistence;
using SpendLess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Persistence.Repositories
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity, TContext>
        where TEntity : class
        where TContext : class
    {
        private readonly DbContext _dbContext;
        public GenericRepository(TContext dbContext)
        {
            switch (dbContext)
            {
                case SpendLessDbContext:
                    _dbContext = dbContext as SpendLessDbContext;
                        break;
                case SpendLessIdentityDbContext:
                    _dbContext = dbContext as SpendLessIdentityDbContext;
                    break;
            }
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
            return entity;
        }

        public async Task Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<bool> Exists(int id)
        {
            var entity = await Get(id);
            return entity != null;
        }

        public async Task<TEntity> Get(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<IReadOnlyList<TEntity>> GetAll()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
