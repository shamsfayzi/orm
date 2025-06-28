using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.View_Models.Users;

namespace Operational_Risk_Management.Models.Interfaces.Repositories
{
    public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
    {

        public bool IsUserNameExists(string userName, Guid? id);
        public Task<PaginatedModel<ApplicationUser>> GetTableAsync(PaginatedModel<ApplicationUser> model);
        public Task UpdateAsync(VM_AppUserUpdate model);
        public Task<bool> AddAsync(VM_AppUserCreate model);

    }
}
