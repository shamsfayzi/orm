using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using System.Linq.Expressions;

namespace Operational_Risk_Management.Models.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> AddAsync(T model);
        Task UpdateAsync(T model);
        Task DeleteAsync(Guid id); // Hard delete
        Task SoftDeleteAsync(Guid id, string deletedBy);
        Task<IEnumerable<T>> GetAllAsync();
        Task<List<T>> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = false); // Added asNoTracking
        Task<bool> IsExistedByIdAsync(Guid id, out T targetModel);
        Task<bool> IsExistedByIdAsync(Guid id);
        Task SaveChangesAsync();
        Task<PaginatedModel<T>> GetPaginatedAsync(PaginatedModel<T> model, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
    }
}