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

        public DateTime? EndTime => (RequestStatus == OverrideRequestStatus.Approved && ApprovedDate.HasValue)
            ? ApprovedDate.Value.AddMinutes(30)
            : null;

        public void Approve(string approver)
        {
            RequestStatus = OverrideRequestStatus.Approved;
            ApprovedBy = approver;
            ApprovedDate = DateTime.UtcNow;
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
