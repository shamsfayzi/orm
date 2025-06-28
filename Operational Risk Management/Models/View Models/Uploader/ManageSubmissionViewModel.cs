using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.View_Models.Uploader
{
    public class ManageSubmissionViewModel
    {
        public Guid IndicatorId { get; set; }
        public string? IndicatorName { get; set; }
        public string? FrequencyOfReview { get; set; } // For display

        [Display(Name = "Comments")]
        [Required]
        public string Comments { get; set; }
        [Display(Name = "Breaches")]
        [Required]
        public int Breaches{ get; set; }

        // To list existing attachments if editing, or to show uploaded files before final submit.
        // For a new submission, the actual file upload will be handled by IFormFile in the controller.
        public List<AttachmentViewModel> ExistingAttachments { get; set; } = new List<AttachmentViewModel>();
    }

    // A simple view model for displaying attachment info
    public class AttachmentViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; } // URL or path to download/view
    }
}