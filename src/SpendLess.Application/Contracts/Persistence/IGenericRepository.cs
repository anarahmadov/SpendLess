using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Application.Contracts.Persistence
{
    public interface IGenericRepository<TEntity, TContext>
        where TEntity : class
        where TContext : class
    {
        Task<TEntity> Get(int id);
        Task<IReadOnlyList<TEntity>> GetAll();
        Task<TEntity> Add(TEntity entity);
        Task<bool> Exists(int id);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
    }
}
