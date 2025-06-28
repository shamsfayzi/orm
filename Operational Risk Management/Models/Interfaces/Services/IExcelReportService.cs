using Operational_Risk_Management.Models.View_Models.Incident;

namespace Operational_Risk_Management.Models.Interfaces.Services
{
    public interface IExcelService
    {
        Task<byte[]> GenerateIncidentReportExcelAsync(List<IncidentListDTO> incidents);
        Task<byte[]> GenerateIncidentDetailExcelAsync(IncidentDetailDTO incident);

    }
}
