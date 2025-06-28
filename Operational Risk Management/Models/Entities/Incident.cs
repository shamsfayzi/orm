using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.Incident
{
    public class Incident : BaseEntity, ISoftDelete
    {
        public string ReportedBy { get; set; }

        public string TitleRole { get; set; }

        public string BranchDepartmentUnit { get; set; }

        public DateTime ReportDate { get; set; }

        // Incident/Event Reported
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime DiscoveryDate { get; set; }

        public string DiscoveredBy { get; set; }

        public string TitleOfIncident { get; set; }

        public string ActivityProcess { get; set; }

        public string FirstLineRiskOwner { get; set; }

        // Staff concerned - Collection of StaffConcerned
        public virtual ICollection<StaffConcerned> StaffConcerned { get; set; }  = new List<StaffConcerned>();

        // Incident/Event Details
        public IncidentType IncidentType { get; set; }

        public IncidentStatus IncidentStatus { get; set; }

        public IncidentCategoryLevel IncidentCategoryLevel { get; set; }

        public RiskType RiskType { get; set; }

        public RiskAssessment RiskAssessment { get; set; }

        // Incident/Event Impact - Financial
        public string FinancialLossCurrency { get; set; }
        public decimal? FinancialLossGrossAmount { get; set; }
        public decimal? FinancialLossLegalCosts { get; set; }
        public decimal? FinancialLossOtherCosts { get; set; }
        public decimal? FinancialLossRecoveryFromClient { get; set; }
        public decimal? FinancialLossRecoveryFromInsurance { get; set; }
        public decimal? FinancialLossNetLossToDate { get; set; }

        // Gain/Opportunity Cost
        public string GainCurrency { get; set; }
        public decimal? GainGrossAmount { get; set; }
        public decimal? GainLegalCosts { get; set; }
        public decimal? GainOtherCosts { get; set; }
        public decimal? GainRecoveryFromClient { get; set; }
        public decimal? GainRecoveryFromInsurance { get; set; }
        public decimal? GainNetAmount { get; set; }
        public string NonFinancialImpact { get; set; }


        // Summaries
        public string EventIncidentDescription { get; set; }

        public string EventIncidentCause { get; set; }

        public string EventIncidentDiscovery { get; set; }

        public string CorrectiveActionsImplemented { get; set; }

        // Controls
        public bool HasWrittenProcedure { get; set; }

        public bool ProcedureIncludesControls { get; set; }

        public string ExistingControlMeasures { get; set; }

        public string ReasonsForFailure { get; set; }

        // Proposed corrective measures
        public string ProposedPoliciesProcedures { get; set; }
        public string ProposedControls { get; set; }
        public string ProposedHumanResources { get; set; }
        public string ProposedSystems { get; set; }
        public string ProposedOthers { get; set; }

        public string OtherComments { get; set; }

        // Supporting documents
        public virtual ICollection<IncidentDocument> Documents { get; set; } = new List<IncidentDocument>();

        // Risk Management review
        public string RiskManagementNotes { get; set; }
        public bool RequiresRevision { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public string ReviewedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }

        // Navigation property for comments
        public virtual ICollection<IncidentReviewComment> ReviewComments { get; set; } = new List<IncidentReviewComment>();
    }

    public enum IncidentType
    {
        Loss,
        NearMiss,
        Gain,
        OpportunityCost,
        Undetermined
    }

    public enum IncidentStatus
    {
        Open,
        Closed
    }

    public enum IncidentCategoryLevel
    {
        InternalFraud,
        ExternalFraud,
        EmploymentPracticeAndWorkplaceSafety,
        ClientsProductsAndBusinessPractices,
        DamageToPhysicalAssets,
        BusinessDisruptionAndSystemsFailure,
        ExecutionDeliveryAndProcessManagement
    }

    public enum RiskType
    {
        Process,
        People,
        System,
        ExternalEvent
    }

    public enum RiskAssessment
    {
        VeryLowRisk,
        LowRisk,
        MediumRisk,
        HighRisk,
        VeryHighRisk
    }
}
