using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Incident;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.View_Models.Incident
{
    public class CreateIncidentDTO
    {
        [Required]
        public string ReportedBy { get; set; }

        [Required]
        public string TitleRole { get; set; }

        [Required]
        public string BranchDepartmentUnit { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime DiscoveryDate { get; set; }

        [Required]
        public string DiscoveredBy { get; set; }

        [Required]
        public string TitleOfIncident { get; set; }

        [Required]
        public string ActivityProcess { get; set; }

        [Required]
        public string FirstLineRiskOwner { get; set; }

        [Required]
        public List<StaffConcernedDTO> StaffConcerned { get; set; }

        [Required]
        public IncidentType IncidentType { get; set; }

        [Required]
        public IncidentCategoryLevel IncidentCategoryLevel { get; set; }

        [Required]
        public RiskType RiskType { get; set; }

        [Required]
        public RiskAssessment RiskAssessment { get; set; }

        // Financial impact
        public string FinancialLossCurrency { get; set; }
        public decimal? FinancialLossGrossAmount { get; set; }
        public decimal? FinancialLossLegalCosts { get; set; }
        public decimal? FinancialLossOtherCosts { get; set; }
        public decimal? FinancialLossRecoveryFromClient { get; set; }
        public decimal? FinancialLossRecoveryFromInsurance { get; set; }

        // Gain
        public string GainCurrency { get; set; }
        public decimal? GainGrossAmount { get; set; }
        public decimal? GainLegalCosts { get; set; }
        public decimal? GainOtherCosts { get; set; }
        public decimal? GainRecoveryFromClient { get; set; }
        public decimal? GainRecoveryFromInsurance { get; set; }

        [Required]
        public string EventIncidentDescription { get; set; }

        [Required]
        public string EventIncidentCause { get; set; }

        [Required]
        public string EventIncidentDiscovery { get; set; }

        [Required]
        public string CorrectiveActionsImplemented { get; set; }

        [Required]
        public bool HasWrittenProcedure { get; set; }

        [Required]
        public bool ProcedureIncludesControls { get; set; }

        [Required]
        public string ExistingControlMeasures { get; set; }

        [Required]
        public string ReasonsForFailure { get; set; }

        public string ProposedPoliciesProcedures { get; set; }
        public string ProposedControls { get; set; }
        public string ProposedHumanResources { get; set; }
        public string ProposedSystems { get; set; }
        public string ProposedOthers { get; set; }

        [Required]
        public string OtherComments { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        public string NonFinancialImpact { get; set; }
    }

    public class StaffConcernedDTO
    {
        public string Name { get; set; }
        public string TitleFunction { get; set; }
    }

    public class IncidentDocumentDTO
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class IncidentDetailDTO
    {
        public Guid Id { get; set; }
        public string ReportedBy { get; set; }
        public string TitleRole { get; set; }
        public string BranchDepartmentUnit { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DiscoveryDate { get; set; }
        public string DiscoveredBy { get; set; }
        public string TitleOfIncident { get; set; }
        public string ActivityProcess { get; set; }
        public string FirstLineRiskOwner { get; set; }
        public List<StaffConcernedDTO> StaffConcerned { get; set; }
        public IncidentType IncidentType { get; set; }
        public IncidentStatus IncidentStatus { get; set; }
        public IncidentCategoryLevel IncidentCategoryLevel { get; set; }
        public RiskType RiskType { get; set; }
        public RiskAssessment RiskAssessment { get; set; }

        // Financial impact
        public string FinancialLossCurrency { get; set; }
        public decimal? FinancialLossGrossAmount { get; set; }
        public decimal? FinancialLossLegalCosts { get; set; }
        public decimal? FinancialLossOtherCosts { get; set; }
        public decimal? FinancialLossRecoveryFromClient { get; set; }
        public decimal? FinancialLossRecoveryFromInsurance { get; set; }
        public decimal? FinancialLossNetLossToDate { get; set; }

        // Gain
        public string GainCurrency { get; set; }
        public decimal? GainGrossAmount { get; set; }
        public decimal? GainLegalCosts { get; set; }
        public decimal? GainOtherCosts { get; set; }
        public decimal? GainRecoveryFromClient { get; set; }
        public decimal? GainRecoveryFromInsurance { get; set; }
        public decimal? GainNetAmount { get; set; }

        public string EventIncidentDescription { get; set; }
        public string EventIncidentCause { get; set; }
        public string EventIncidentDiscovery { get; set; }
        public string CorrectiveActionsImplemented { get; set; }
        public bool HasWrittenProcedure { get; set; }
        public bool ProcedureIncludesControls { get; set; }
        public string ExistingControlMeasures { get; set; }
        public string ReasonsForFailure { get; set; }
        public string ProposedPoliciesProcedures { get; set; }
        public string ProposedControls { get; set; }
        public string ProposedHumanResources { get; set; }
        public string ProposedSystems { get; set; }
        public string ProposedOthers { get; set; }
        public string OtherComments { get; set; }
        public List<IncidentDocumentDTO> Documents { get; set; }
        public string RiskManagementNotes { get; set; }
        public bool RequiresRevision { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public string ReviewedBy { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<IncidentReviewCommentDTO> ReviewComments { get; set; } = new List<IncidentReviewCommentDTO>();
    }

    public class IncidentListDTO:BaseEntity
    {
        public Guid Id { get; set; }
        public string TitleOfIncident { get; set; }
        public string BranchDepartmentUnit { get; set; }
        public DateTime ReportDate { get; set; }
        public IncidentStatus IncidentStatus { get; set; }
        public IncidentType IncidentType { get; set; }
        public IncidentCategoryLevel IncidentCategoryLevel { get; set; }
        public RiskAssessment RiskAssessment { get; set; }
        public bool RequiresRevision { get; set; }
    }

    public class ReviewIncidentDTO
    {
        [Required]
        public Guid IncidentId { get; set; }

        [Required]
        public string RiskManagementNotes { get; set; }

        [Required]
        public bool RequiresRevision { get; set; }
    }

    public class IncidentFilterDTO
    {
        public string SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string BranchDepartmentUnit { get; set; }
        public IncidentStatus? Status { get; set; }
        public IncidentType? Type { get; set; }
        public IncidentCategoryLevel? Category { get; set; }
        public RiskAssessment? RiskLevel { get; set; }
        public bool? RequiresRevision { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Properties for user context based filtering
        public string RequestingUserId { get; set; }
        public string RequestingUserDepartment { get; set; } // Added for department-based filtering
        public bool IsRequestingUserAdminOrManager { get; set; }
    }
}