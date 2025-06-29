using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.View_Models.Uploader
{
    public class ExistingAttachmentViewModel
    {
        public Guid AttachmentId { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; } // Or just use AttachmentId to build URL in view
        public bool IsMarkedForDeletion { get; set; }
    }

    public class VM_ManageSubmission
    {
        public Guid IndicatorId { get; set; }
        [Display(Name = "Indicator")]
        public string IndicatorName { get; set; }
        [Display(Name = "Indicator Description")]
        public string IndicatorDescription { get; set; } // Or full Indicator object
        [Display(Name = "KRI Type")]
        public string KRIType { get; set; }
        [Display(Name = "Process")]
        public string Process { get; set; }
        [Display(Name = "Risk Area")]
        public string RiskArea { get; set; }
        [Display(Name = "Frequency Of Review")]
        public string FrequencyOfReview { get; set; }


        public Guid? SubmissionId { get; set; } // Null if new submission

        [Required]
        [Display(Name = "Reporting Month")]
        [DataType(DataType.Date)] // To ensure only date part is considered for month
        public DateTime ReportingMonth { get; set; }

        [Display(Name = "Notes / Comments")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Breaches cannot be negative.")]
        [Display(Name = "Number of Breaches/Exceptions")]
        public int Breaches { get; set; }

        public List<IFormFile> NewAttachments { get; set; }
        public List<ExistingAttachmentViewModel> ExistingAttachments { get; set; }

        public bool IsWindowLocked { get; set; }
        public bool HasActiveOverride { get; set; }
        public DateTime? OverrideEndTime { get; set; }

        public bool CanEdit => !IsWindowLocked || HasActiveOverride;

        public VM_ManageSubmission()
        {
            NewAttachments = new List<IFormFile>();
            ExistingAttachments = new List<ExistingAttachmentViewModel>();
            ReportingMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1); // Default to current month
        }
    }
}
