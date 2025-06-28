using System.ComponentModel.DataAnnotations;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.View_Models.Uploader.Indicator;

namespace Operational_Risk_Management.Models.View_Models.Uploader
{
    public class Update_DetailVM {
        public SubmissionDetailViewModel Submission { get; set; }
        public VM_SubmissionUpdate UpdateModel { get; set; }
    }

    public class VM_Submission
    {
        public Guid Id { get; set; }
        public DateTime ReportingMonth { get; set; }

        public string Status { get; set; }

        public int Breaches { get; set; }

        public string Notes { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdatedByFullName { get; set; }
        public Guid IndicatorId { get; set; }
        public virtual VM_Indicator Indicator { get; set; }

    }
    public class VM_SubmissionUpdate
    {
        [Required]
        public Guid Id { get; set; }
        [Required]

        public string Notes;
        [Required]

        public int Breaches { get; set; }

    }
}
