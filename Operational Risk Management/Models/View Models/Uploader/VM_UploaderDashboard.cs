using Operational_Risk_Management.Models.Entities;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Operational_Risk_Management.Models.View_Models.Uploader.Indicator;

namespace Operational_Risk_Management.Models.View_Models.Uploader
{
    public class VM_UploaderDashboard
    {
         //public Guid TemplateId { get; set; }
        public string TemplateFocalPoint { get; set; } 
        //public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } 
        public List<IndicatorViewModel> Indicators { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int RejectedCount { get; set; }
        //public int DueSoonCount { get; set; }
        public int OpenSubmissionCount { get; set; }

        public IndicatorViewModel LastSubmitted { get; set; } // Optional
        public VM_UploaderDashboard()
        {
            Indicators = new List<IndicatorViewModel>();
        }
    }



    public class IndicatorViewModel
    {
        public Guid IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public string KRIType { get; set; }
        public string RfNo { get; set; }
        public string Process { get; set; }
        public string RiskArea { get; set; }
        public string FrequencyOfReview { get; set; }
        public int TolerableBreaches { get; set; }
        public DateTime? LastSubmissionDate { get; set; }
        public string LastSubmissionStatus { get; set; }
        public bool CanSubmitNewReport { get; set; }
        public Guid? LastSubmissionId { get; set; }
        public DateTime? DueDate { get; set; }
        //public bool IsDueSoon { get;  set; }
        public List<SubmissionDetailViewModel> Submissions { get; set; } = new();

    }
    public class FilteredSubmissions
    {
        public List<IndicatorViewModel> Indicators { get; set; } = new List<IndicatorViewModel> { };
        public List<Submission> Submissions { get; set; } = new List<Submission> { };
        public string StatusFilter { get; set; } // To store "Approved", "Pending", etc.


    }
    public class SubmissionDetailViewModel
    {
        public Guid Id { get; set; }
        public VM_Indicator Indicator { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int Breaches { get; set; } // Changed from BreachLevel
        public DateTime ReportingMonth { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdatedByFullName { get; set; }
        public bool WindowLocked { get; set; }
        public string? WindowLockReason { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }=new List<Feedback>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }

    public class SubmissionUpdateModel
    {
        public Guid SubmissionId { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter only numbers")]
        [Display(Name = "Total Breaches")]
        public string TotalBreaches { get; set; }

        [Display(Name = "Add Attachments")]
        public List<IFormFile>? NewAttachments { get; set; }
    }
    public class SendFeedbackDto
    {
        [Required]
        public string SubmissionId { get; set; }
        [Required]
        [MinLength(1)]
        public string Content { get; set; }
    }
}