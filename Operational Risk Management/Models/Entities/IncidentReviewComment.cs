using System;
using Operational_Risk_Management.Models.Context; // For BaseEntity if used, or just for namespace
using Operational_Risk_Management.Models.Entities; // For Incident

namespace Operational_Risk_Management.Models.Entities
{
    public class IncidentReviewComment : BaseEntity // Assuming BaseEntity provides Id, CreateDate etc.
    {
        public Guid IncidentId { get; set; }
        public virtual Incident.Incident Incident { get; set; }

        public string CommentText { get; set; }

        public string CommenterId { get; set; } // Store User ID from Claims or Identity
        public string CommenterName { get; set; } // Store User's Full Name for display

        public DateTime CommentDate { get; set; }

        public bool IsRevisionNote { get; set; } = false;
        public bool IsVisibleToDepartment { get; set; } = false; // Default to not visible unless specified
    }
}
