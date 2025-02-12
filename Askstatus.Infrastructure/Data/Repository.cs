using System.Linq.Expressions;
using Askstatus.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Askstatus.Infrastructure.Data;
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<Repository<TEntity>> _logger;

    public Repository(ApplicationDbContext context, ILogger<Repository<TEntity>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        TEntity? result = null;
        try
        {
            result = await _context.Set<TEntity>().FindAsync(id)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync error");
        }
        return result!;
    }

    public async Task<List<TEntity>> ListAllAsync()
    {
        List<TEntity> result = new();
        try
        {
            result = await _context.Set<TEntity>().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ListAllAsync error");
        }
        return result;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        TEntity? result = null;
        try
        {
            var tmp = await _context.Set<TEntity>().AddAsync(entity);
            result = tmp.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddAsync error");
        }
        return result!;
    }

    public Task<bool> UpdateAsync(TEntity entity)
    {
        bool result = false;
        try
        {
            var tmp = _context.Set<TEntity>().Update(entity);
            if (tmp != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync error");
        }
        return Task.FromResult(result);
    }

    public Task<bool> DeleteAsync(TEntity entity)
    {
        bool result = false;
        try
        {
            var tmp = _context.Set<TEntity>().Remove(entity);
            if (tmp != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAsync error");
        }
        return Task.FromResult(result);
    }

    public async Task<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate)
    {
        TEntity? result = null;
        try
        {
            result = await _context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBy error");
        }
        return result!;
    }

    public async Task<IPagedList<TEntity>> GetListBy(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>>? keySelector, int page, int pageSize, bool desc = false)
    {
        IPagedList<TEntity> result = null!;
        try
        {
            var query = _context.Set<TEntity>().AsQueryable();
            if (predicate is not null)
            {
                query = query.Where(predicate);
            }
            if (keySelector is not null)
            {
                if (desc)
                {
                    query = query.OrderByDescending(keySelector);
                }
                else
                {
                    query = query.OrderBy(keySelector);
                }
            }
            result = await PagedList<TEntity>.CreateAsync(query, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetListBy error");
        }
        return result!;
    }
}
