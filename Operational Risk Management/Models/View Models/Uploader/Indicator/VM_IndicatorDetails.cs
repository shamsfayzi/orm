namespace Operational_Risk_Management.Models.View_Models.Uploader.Indicator
{
    public class VM_IndicatorDetails
    {
        // All original properties
        public Guid Id { get; set; }
        public string KRIType { get; set; }
        public string RfNo { get; set; }
        public string Process { get; set; }
        public string RiskArea { get; set; }
        public string IndicatorName { get; set; }
        public string FrequencyOfReview { get; set; }
        public int? TotalProcess { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string TemplateName { get; set; }

        // Calculated properties
        public DateTime? NextDueDate { get; set; }
        public int SubmissionCount { get; set; }
        public string StatusBadgeClass { get; set; }
        public bool CanUpload { get; set; }

        // View helpers
        public string FormattedUpdatedAt => UpdatedAt?.ToString("dd MMM yyyy HH:mm") ?? "Never";
        public string DueStatusClass => NextDueDate < DateTime.Now ? "text-danger fw-bold" : "";
    }
}
