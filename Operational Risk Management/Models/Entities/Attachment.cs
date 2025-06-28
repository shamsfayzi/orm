using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Entities
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Size { get; set; }
        public Guid SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }
    }
}