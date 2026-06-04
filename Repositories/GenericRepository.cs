using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;

namespace Chap10.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly ILogger<GenericRepository<T>> _logger;

    public GenericRepository(DbContext context, ILogger<GenericRepository<T>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("AddAsync called for entity type: {EntityType} | OperationId: {OperationId}",
                typeof(T).Name, operationId);

            try
            {
                await _context.Set<T>().AddAsync(entity);
                _logger.LogDebug("Entity added to context for type: {EntityType} | OperationId: {OperationId}",
                    typeof(T).Name, operationId);
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error when adding entity of type: {EntityType} | OperationId: {OperationId}",
                    typeof(T).Name, operationId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when adding entity of type: {EntityType} | Error: {ErrorMessage} | OperationId: {OperationId}",
                    typeof(T).Name, ex.Message, operationId);
                throw;
            }
        }
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("SaveChangesAsync called | OperationId: {OperationId}", operationId);

            try
            {
                int changedCount = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync completed successfully. Changes saved: {ChangeCount} | OperationId: {OperationId}",
                    changedCount, operationId);
                return changedCount;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                _logger.LogError(concEx, "Concurrency error when saving changes | OperationId: {OperationId}", operationId);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error when saving changes | OperationId: {OperationId}", operationId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when saving changes | Error: {ErrorMessage} | OperationId: {OperationId}",
                    ex.Message, operationId);
                throw;
            }
        }
    }

    public virtual async Task DeleteAsync(int id)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("DeleteAsync called for entity type: {EntityType} with id: {Id} | OperationId: {OperationId}",
                typeof(T).Name, id, operationId);

            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity != null)
                {
                    _context.Set<T>().Remove(entity);
                    _logger.LogInformation("Entity marked for deletion: {EntityType} with id: {Id} | OperationId: {OperationId}",
                        typeof(T).Name, id, operationId);
                }
                else
                {
                    _logger.LogWarning("Entity not found for deletion: {EntityType} with id: {Id} | OperationId: {OperationId}",
                        typeof(T).Name, id, operationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity of type: {EntityType} with id: {Id} | Error: {ErrorMessage} | OperationId: {OperationId}",
                    typeof(T).Name, id, ex.Message, operationId);
                throw;
            }
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("GetAllAsync called for entity type: {EntityType} | OperationId: {OperationId}",
                typeof(T).Name, operationId);

            try
            {
                var entities = await _context.Set<T>().ToListAsync();
                _logger.LogInformation("GetAllAsync completed for {EntityType}. Records returned: {RecordCount} | OperationId: {OperationId}",
                    typeof(T).Name, entities.Count, operationId);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all entities of type: {EntityType} | Error: {ErrorMessage} | OperationId: {OperationId}",
                    typeof(T).Name, ex.Message, operationId);
                throw;
            }
        }
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("GetByIdAsync called for entity type: {EntityType} with id: {Id} | OperationId: {OperationId}",
                typeof(T).Name, id, operationId);

            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity != null)
                {
                    _logger.LogDebug("Entity found: {EntityType} with id: {Id} | OperationId: {OperationId}",
                        typeof(T).Name, id, operationId);
                }
                else
                {
                    _logger.LogWarning("Entity not found: {EntityType} with id: {Id} | OperationId: {OperationId}",
                        typeof(T).Name, id, operationId);
                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity of type: {EntityType} with id: {Id} | Error: {ErrorMessage} | OperationId: {OperationId}",
                    typeof(T).Name, id, ex.Message, operationId);
                throw;
            }
        }
    }

    public virtual async Task UpdateAsync(T entity)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("UpdateAsync called for entity type: {EntityType} | OperationId: {OperationId}",
                typeof(T).Name, operationId);

            try
            {
                _context.Set<T>().Update(entity);
                _logger.LogDebug("Entity marked for update: {EntityType} | OperationId: {OperationId}",
                    typeof(T).Name, operationId);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity of type: {EntityType} | Error: {ErrorMessage} | OperationId: {OperationId}",
                    typeof(T).Name, ex.Message, operationId);
                throw;
            }
        }
    }
}
