using Operational_Risk_Management.Services.Interfaces;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Models.View_Models.Incident;
namespace Operational_Risk_Management.Models.Interfaces.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateIncidentReportPdfAsync(IncidentDetailDTO incident);
        Task<byte[]> GenerateDashboardReportPdfAsync(IncidentDashboardDTO dashboardData);
    }
}
