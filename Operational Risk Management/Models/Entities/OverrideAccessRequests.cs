using Operational_Risk_Management.Models.Context;

namespace Operational_Risk_Management.Models.Entities
{
    public class OverrideAccessRequest : BaseEntity
    {
        public Guid SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }

        public string Reason { get; set; }

        public OverrideRequestStatus RequestStatus { get; set; } = OverrideRequestStatus.Pending;

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // StartTime and EndTime are now settable properties to match the SQL schema
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public void Approve(string approver, DateTime? startTime = null, DateTime? endTime = null)
        {
            RequestStatus = OverrideRequestStatus.Approved;
            ApprovedBy = approver;
            ApprovedDate = DateTime.UtcNow;
            // Admins might set specific start/end times for the override
            StartTime = startTime ?? ApprovedDate; // Default StartTime to ApprovedDate if not provided
            EndTime = endTime ?? ApprovedDate.Value.AddMinutes(30); // Default EndTime to 30 mins from approval if not provided
        }

        public void Revoke()
        {
            RequestStatus = OverrideRequestStatus.Revoked;
            // Optionally clear EndTime or set it to now if it was in the future
            if (EndTime.HasValue && EndTime > DateTime.UtcNow)
            {
                EndTime = DateTime.UtcNow;
            }
        }

        public void Reject()
        {
            RequestStatus = OverrideRequestStatus.Rejected;
        }
    }
    public enum OverrideRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Revoked
    }

}
