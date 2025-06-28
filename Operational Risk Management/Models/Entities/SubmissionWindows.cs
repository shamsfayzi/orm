using Operational_Risk_Management.Models.Context;

namespace Operational_Risk_Management.Models.Entities
{
    public class SubmissionWindows : BaseEntity
    {
        public string Frequency { get;set; } = "Monthly"; // Monthly, Quarterly, Yearly
        public int OpenDaysBeforePeriodEnd { get;set; }
        public int CloseDaysAfterPeriodEnd { get; set; }
        public bool IsActive { get; set; } = true; // Indicates if the submission window is currently active

    }
}
