using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.View_Models.Departments;
using Operational_Risk_Management.Models.View_Models.Indicator;
using Operational_Risk_Management.Models.View_Models.KRIIndicator;
using Operational_Risk_Management.Models.View_Models.Uploader.Indicator;

namespace Operational_Risk_Management.Models.Interfaces.Repositories
{
    public interface IIndicatorRepository : IGenericRepository<Indicator>
    {

        //public bool IsKRIIndicatorExists(string departmentName, Guid? id);
        public Task<PaginatedModel<Indicator>> GetTableAsync(PaginatedModel<Indicator> model);
        public Task<PaginatedModel<Indicator>> GetByTemplateId(Guid Tid, PaginatedModel<Indicator> model);
        public Task<PaginatedModel<Submission>> GetSubmissionsByIID(Guid id, PaginatedModel<Submission> model);
        public Task<bool> AddAsync(VM_KRIIndicatorCreate model);
        public Task UpdateAsync(VM_IndicatorEdit model);
        //public Task<PaginatedModel<VM_Indicator>> GetPaginatedWithDueSoon(PaginatedModel<VM_Indicator> model,string focalPoint);

    }
}
