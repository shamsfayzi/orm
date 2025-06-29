using System;

namespace Operational_Risk_Management.Models.View_Models.Override
{
    public class PendingOverrideRequestViewModel
    {
        public Guid RequestId { get; set; }
        public Guid SubmissionId { get; set; }
        public string IndicatorName { get; set; }
        public string DepartmentName { get; set; }
        public string RequestedByFullName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
    }
}
