using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.View_Models.KRITemplates;

namespace Operational_Risk_Management.Models.View_Models.Uploader.Indicator
{
    public class VM_Indicator 
    {
        public Guid Id { get; set; }
        public string KRIType { get; set; }

        public string RfNo { get; set; }

        public string Process { get; set; }
        public string RiskArea { get; set; }
        public string IndicatorName { get; set; }
        public string FrequencyOfReview { get; set; } = "Monthly";
        public int TolerableBreaches { get; set; } = 0;
        public int? TotalProcess { get; set; }
        public Guid TemplateId { get; set; }
        public VM_Template Template { get;set; }
    }
}
