using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Entities
{
    public class Submission: BaseEntity,ISoftDelete
    {
        public DateTime ReportingMonth { get; set; }
            
        public string Status { get; set; } = "Draft";

        public int Breaches { get; set; }

        public string Notes { get; set; }
        public bool WindowLocked { get; set; }
        public string? WindowLockReason { get; set; }
        public bool? WindowOverride { get;set; } = false; // 0 = No, 1 = Yes
        public string? WindowOverrideBy { get; set; }
        public DateTime? WindowOverrideDate { get; set; }  

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
        public Guid IndicatorId { get; set; }
        public virtual Indicator Indicator { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    }
}
