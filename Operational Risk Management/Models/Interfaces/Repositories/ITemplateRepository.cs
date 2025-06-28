using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.View_Models.Indicator;
using Operational_Risk_Management.Models.View_Models.KRIIndicator;
using Operational_Risk_Management.Models.View_Models.Templates;

namespace Operational_Risk_Management.Models.Interfaces.Repositories
{
    public interface ITemplateRepository : IGenericRepository<Template>
    {
        public Task<PaginatedModel<Template>> GetTableAsync(PaginatedModel<Template> model);
        public Task<bool> AddAsync(VM_KRITemplateCreate model);
        public Task UpdateAsync(VM_KRITemplateUpdate model);
        public Task<bool> DeleteAsync(Guid id);
        public Task<PaginatedModel<Template>> TemplateDetails(PaginatedModel<Template> model, Guid id);
        public Task<bool> AddTemplateIndicatorAsync(VM_KRIIndicatorCreate vM_KRIIndicatorCreate);
        public Task UpdateTemplateIndicatorAsync(VM_IndicatorEdit vM_KRIIndicatorUpdate);
        public Task DeleteTemplateIndicatorAsync(Guid id);
        public Task<Template> GetTemplateByFocalPoint(string FocalPoint);
    }


}