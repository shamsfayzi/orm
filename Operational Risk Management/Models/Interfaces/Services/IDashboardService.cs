using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.View_Models.Incident;

namespace Operational_Risk_Management.Services.Interfaces
{
    public interface IIncidentDashboardService
    {

            Task<IncidentDashboardDTO> GetDashboardDataAsync(DateTime? startDate, DateTime? endDate);
            Task<Dictionary<string, int>> GetIncidentsTrendAsync(DateTime? startDate, DateTime? endDate, string interval = "month");
            Task<PaginatedModel<IncidentListDTO>> GetFilteredIncidentsAsync(IncidentFilterDTO filter);
            Task<List<string>> GetDepartmentsAsync();
            Task<PaginatedModel<IncidentListDTO>> GetPendingReviewIncidentsAsync(int pageNumber = 1, int pageSize = 10);
            Task<PaginatedModel<IncidentListDTO>> GetRequiringRevisionIncidentsAsync(int pageNumber = 1, int pageSize = 10);
        
    }

        public class IncidentDashboardDTO
        {
            public int TotalIncidents { get; set; }
            public int OpenIncidents { get; set; }
            public int ClosedIncidents { get; set; }
            public int PendingReviewIncidents { get; set; }
            public int RequiringRevisionIncidents { get; set; }
            public Dictionary<string, int> IncidentsByDepartment { get; set; }
            public Dictionary<string, int> IncidentsByCategory { get; set; }
            public Dictionary<string, int> IncidentsByRiskLevel { get; set; }
            public Dictionary<string, int> IncidentsTrend { get; set; }
        }
}
