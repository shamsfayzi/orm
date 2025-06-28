using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Extensions;

namespace Operational_Risk_Management.Models.Services
{


    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        internal ApplicationDBContext _context;
        internal DbSet<T> _dbSet;

        public GenericRepository(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public virtual async Task<T> AddAsync(T model)
        {
            var newEntity = await _dbSet.AddAsync(model);
            return newEntity.Entity;
        }
        public virtual async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            }
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task DeleteAsync(Guid id)
        {
            var targetModel = await _dbSet.FindAsync(id);
            _dbSet.Remove(targetModel);
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public virtual Task<bool> IsExistedByIdAsync(Guid id, out T? targetModel)
        {
            targetModel = _dbSet.Find(id);
            return Task.FromResult(targetModel is not null);
        }
        public virtual Task<bool> IsExistedByIdAsync(Guid id)
        {

            return Task.FromResult(_dbSet.Any(a => a.Id == id));
        }

        public virtual async Task<List<T>> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        public virtual Task UpdateAsync(T model)
        {
            _context.Entry(model).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task SoftDeleteAsync(Guid id, string deletedBy)
        {
            var entity = await GetByIdAsync(id);
            if (entity is ISoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                softDeleteEntity.DeletedDate = DateTime.UtcNow;
                softDeleteEntity.DeletedBy = deletedBy;
                await UpdateAsync(entity); // Use UpdateAsync to mark as modified
                await SaveChangesAsync();
            }
            else
            {
                // Optionally handle cases where the entity doesn't implement ISoftDelete
                // For now, we can just do nothing or throw an exception
                // throw new InvalidOperationException($"Entity {typeof(T).Name} does not support soft delete.");
                // Or just delete it hard?
                 DeleteAsync(id); // Fallback to hard delete if ISoftDelete is not implemented
            }
        }

        public virtual async Task<PaginatedModel<T>> GetPaginatedAsync(PaginatedModel<T> model, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToPaginatedAsync(model);
        }
    }
}
