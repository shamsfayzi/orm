using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.View_Models.KRIIndicator;
using Operational_Risk_Management.Models.View_Models.KRITemplates;
using Operational_Risk_Management.Models.View_Models.Uploader;

namespace Operational_Risk_Management.Services.Interfaces
{
    public interface IKRIDashboardSercice
    {
        Task<KRIDashboardDataDTO> GetRiskManagementDashboardDataAsync(DateTime? startDate, DateTime? endDate);
        Task<List<VM_Submission>> GetSubmissionsWithFiltersAsync(FilterModelForSubmissions model, string submissionType = "all");
        Task<List<string>> GetAllDepartmentsAsync();
        Task<List<Indicator>> GetAllIndicatorsAsync();
    }


}
