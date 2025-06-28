using Operational_Risk_Management.Models.View_Models.KRIIndicator;
using Operational_Risk_Management.Services.Interfaces;

namespace Operational_Risk_Management.Models.View_Models
{
    public class DashboardDTO
    {
        public KRIDashboardDataDTO KRIDashboardDataDTO { get; set; }
        public IncidentDashboardDTO IncidentDashboardDTO { get; set; }
    }
}
