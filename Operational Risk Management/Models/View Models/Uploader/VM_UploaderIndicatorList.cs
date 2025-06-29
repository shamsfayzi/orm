using System.Collections.Generic;
using System;

namespace Operational_Risk_Management.Models.View_Models.Uploader
{
    public class VM_UploaderIndicatorList
    {
        public List<IndicatorCardViewModel> Indicators { get; set; } = new List<IndicatorCardViewModel>();
    }

    public class IndicatorCardViewModel
    {
        public Guid IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public string KRIType { get; set; }
        public string RiskArea { get; set; }
        public string Process { get; set; }
        public string FrequencyOfReview { get; set; }
        public string DepartmentName { get; set; } // From Template.Department
        public string TemplateFocalPoint { get; set; } // From Template
        public DateTime? LastSubmissionDate { get; set; }
        public DateTime? NextDueDate { get; set; } // This would need calculation logic
        public string CurrentStatus { get; set; } // e.g., "Submitted", "Pending", "Overdue"
        public bool IsDueForSubmission { get; set; } // Logic to determine if action is needed
        public Guid LastSubmissionId { get; set; }
    }
}
