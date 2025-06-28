using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.View_Models.Departments;

namespace Operational_Risk_Management.Models.Interfaces.Repositories
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {

        public bool IsDepartmentExists(string departmentName, Guid? id);
        public Task<PaginatedModel<Department>> GetTableAsync(PaginatedModel<Department> model);
        public Task<bool> AddAsync(VM_DepartmentCreate model);
        public Task UpdateAsync(VM_DepartmentUpdate model);

    }
}
