using System;
using Operational_Risk_Management.Models.Context;

namespace Operational_Risk_Management.Models.Entities
{
    public class SubmissionAuditLog : BaseEntity // Inherits Id, CreateDate, CreateBy etc.
    {
        public Guid SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }

        [Required]
        public string Action { get; set; } // e.g., "Status changed to Open", "Override Approved"

        // If ActionDate and ActionBy are different from BaseEntity.CreateDate/CreateBy
        // For now, assuming BaseEntity covers the who/when of the audit log entry itself.
        // If SPs specifically populate distinct ActionDate/ActionBy, these would be needed:
        // public DateTime ActionDate {get; set;}
        // public string ActionBy {get; set;} // Or Guid ActionByUserId

        public string? Details { get; set; } // Additional details for the audit entry
    }
}
