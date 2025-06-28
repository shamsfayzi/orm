using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.View_Models.Uploader
{
    public class VM_CR_OverrideAccessRequest
    {
        [Required]
        public Guid SubmissionId { get; set; }
        [Required]

        public string Reason { get; set; }
        public string? CreatedByFullName { get; set; }

    }
}
