using System.ComponentModel;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Entities;

public class Feedback : BaseEntity
{
    public string FeedbackContent { get; set; } = string.Empty;
    public Guid SubmissionId { get; set; }
    public virtual Submission Submission { get; set; }

}
