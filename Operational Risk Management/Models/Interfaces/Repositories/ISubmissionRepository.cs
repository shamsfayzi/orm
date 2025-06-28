using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;

namespace Operational_Risk_Management.Models.Interfaces.Repositories
{
    public interface ISubmissionRepository : IGenericRepository<Submission>
    {
        Task<PaginatedModel<Submission>> GetTableAsync(PaginatedModel<Submission> model);
        //Task<bool> AddAsync(VM_SubmissionCreate model);
        //Task UpdateAsync(VM_SubmissionUpdate model);
        //Task<bool> DeleteAsync(Guid id);
        Task<Submission> SubmissionDetails( Guid id);
    }
}
