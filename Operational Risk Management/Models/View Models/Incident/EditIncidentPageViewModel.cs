using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // For IFormFile
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using Operational_Risk_Management.Models.Incident; // For enums

namespace Operational_Risk_Management.Models.View_Models.Incident
{
    // StaffConcernedFormViewModel is assumed to be in CreateIncidentPageViewModel.cs or another shared location.
    // If not, it should be defined here or referenced from its actual location.
    // For this subtask, we'll assume it's accessible.
    // IncidentDocumentDTO is assumed to be in IncidentVMs.cs or another shared location.

    public class EditIncidentPageViewModel
    {
        [Required]
        public Guid Id { get; set; } // ID of the incident being edited

        // This Report Section
        [Required]
        [Display(Name = "Reported by")]
        public string ReportedBy { get; set; }

        [Required]
        [Display(Name = "Title/Role")]
        public string TitleRole { get; set; }

        [Required]
        [Display(Name = "Branch/Department/Unit (Reporting From)")]
        public string BranchDepartmentUnit { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; }

        // Incident/Event Reported Section
        [Required]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Discovery Date & Time")]
        public DateTime DiscoveryDate { get; set; }

        [Required]
        [Display(Name = "Discovered by")]
        public string DiscoveredBy { get; set; }

        [Required]
        [Display(Name = "Title of Incident")]
        [StringLength(200)]
        public string TitleOfIncident { get; set; }

        [Required]
        [Display(Name = "Activity/Process Where Incident Occurred")]
        public string ActivityProcess { get; set; }

        [Required]
        [Display(Name = "First Line (Risk Owner)")]
        public string FirstLineRiskOwner { get; set; }

        [Display(Name = "Staff Concerned")]
        public List<StaffConcernedFormViewModel> StaffConcerned { get; set; } = new List<StaffConcernedFormViewModel>();

        // Incident/Event Details Section
        [Required]
        [Display(Name = "Incident Type")]
        public IncidentType IncidentType { get; set; }

        [Required]
        [Display(Name = "Incident Category Level")]
        public IncidentCategoryLevel IncidentCategoryLevel { get; set; }

        [Required]
        [Display(Name = "Risk Type")]
        public RiskType RiskType { get; set; }

        [Required]
        [Display(Name = "Risk Assessment")]
        public RiskAssessment RiskAssessment { get; set; }

        // Financial Impact - Loss
        [Display(Name = "Currency (Loss)")]
        [StringLength(3)]
        public string FinancialLossCurrency { get; set; }
        [Display(Name = "Gross Amount (Loss)")]
        public decimal? FinancialLossGrossAmount { get; set; }
        [Display(Name = "Legal Costs (Loss)")]
        public decimal? FinancialLossLegalCosts { get; set; }
        [Display(Name = "Other Costs (Loss)")]
        public decimal? FinancialLossOtherCosts { get; set; }
        [Display(Name = "Recovery from Client (Loss)")]
        public decimal? FinancialLossRecoveryFromClient { get; set; }
        [Display(Name = "Recovery from Insurance (Loss)")]
        public decimal? FinancialLossRecoveryFromInsurance { get; set; }

        // Financial Impact - Gain/Opp. Cost
        [Display(Name = "Currency (Gain/Opp. Cost)")]
        [StringLength(3)]
        public string GainCurrency { get; set; }
        [Display(Name = "Gross Amount (Gain/Opp. Cost)")]
        public decimal? GainGrossAmount { get; set; }
        [Display(Name = "Legal Costs (Gain/Opp. Cost)")]
        public decimal? GainLegalCosts { get; set; }
        [Display(Name = "Other Costs (Gain/Opp. Cost)")]
        public decimal? GainOtherCosts { get; set; }
        [Display(Name = "Recovery from Client (Gain/Opp. Cost)")]
        public decimal? GainRecoveryFromClient { get; set; }
        [Display(Name = "Recovery from Insurance (Gain/Opp. Cost)")]
        public decimal? GainRecoveryFromInsurance { get; set; }

        [Display(Name = "Non-financial Impact")]
        [DataType(DataType.MultilineText)]
        public string NonFinancialImpact { get; set; }

        // Summaries Section
        [Required]
        [Display(Name = "Event/Incident Description")]
        [DataType(DataType.MultilineText)]
        public string EventIncidentDescription { get; set; }

        [Required]
        [Display(Name = "Event/Incident Cause")]
        [DataType(DataType.MultilineText)]
        public string EventIncidentCause { get; set; }

        [Required]
        [Display(Name = "Event/Incident Discovery")]
        [DataType(DataType.MultilineText)]
        public string EventIncidentDiscovery { get; set; }

        [Required]
        [Display(Name = "Corrective Actions Already Implemented")]
        [DataType(DataType.MultilineText)]
        public string CorrectiveActionsImplemented { get; set; }

        // Controls (in summary) Section
        [Required]
        [Display(Name = "Is there written procedure for activity concerned? (Y/N)")]
        public bool HasWrittenProcedure { get; set; }

        [Required]
        [Display(Name = "Does the procedure include controls? (Y/N)")]
        public bool ProcedureIncludesControls { get; set; }

        [Required]
        [Display(Name = "Existing Control Measures")]
        [DataType(DataType.MultilineText)]
        public string ExistingControlMeasures { get; set; }

        [Required]
        [Display(Name = "Reasons for Control Failure")]
        [DataType(DataType.MultilineText)]
        public string ReasonsForFailure { get; set; }

        // Proposed Corrective Measures Section
        [Display(Name = "Policies & Procedures")]
        [DataType(DataType.MultilineText)]
        public string ProposedPoliciesProcedures { get; set; }

        [Display(Name = "Controls")]
        [DataType(DataType.MultilineText)]
        public string ProposedControls { get; set; }
        [Display(Name = "Human Resources")]
        [DataType(DataType.MultilineText)]
        public string ProposedHumanResources { get; set; }
        [Display(Name = "Systems")]
        [DataType(DataType.MultilineText)]
        public string ProposedSystems { get; set; }
        [Display(Name = "Others")]
        [DataType(DataType.MultilineText)]
        public string ProposedOthers { get; set; }

        [Required]
        [Display(Name = "Other Comments (Overall)")]
        [DataType(DataType.MultilineText)]
        public string OtherComments { get; set; }

        // For displaying existing documents
        public List<IncidentDocumentDTO> ExistingDocuments { get; set; } = new List<IncidentDocumentDTO>();
        // For uploading new documents
        [Display(Name = "Attach More Supporting Documents")]
        public List<IFormFile> NewSupportingDocuments { get; set; }
        // For marking documents to be deleted
        public List<Guid> DocumentsToDelete { get; set; } = new List<Guid>();

        // For dropdowns
        public IEnumerable<SelectListItem> IncidentTypes { get; set; }
        public IEnumerable<SelectListItem> IncidentCategoryLevels { get; set; }
        public IEnumerable<SelectListItem> RiskTypes { get; set; }
        public IEnumerable<SelectListItem> RiskAssessments { get; set; }
        public IEnumerable<SelectListItem> YesNoOptions { get; set; }
    }
}
