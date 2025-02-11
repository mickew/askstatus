using System.Linq.Expressions;

namespace Askstatus.Application.Interfaces;
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<List<TEntity>> ListAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);

    Task<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate);
    Task<IPagedList<TEntity>> GetListBy(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>>? keySelector, int page, int pageSize, bool desc = false);
}
