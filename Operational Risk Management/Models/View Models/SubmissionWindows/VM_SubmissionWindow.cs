using System;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.View_Models.SubmissionWindows
{
    public class VM_SubmissionWindow
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; } // E.g., "Monthly", "Quarterly", "Annually" - Could be an enum later

        [Required]
        [Range(0, 30, ErrorMessage = "Must be between 0 and 30.")]
        [Display(Name = "Open Days Before Period End")]
        public int OpenDaysBeforePeriodEnd { get; set; }

        [Required]
        [Range(0, 90, ErrorMessage = "Must be between 0 and 90.")]
        [Display(Name = "Close Days After Period End")]
        public int CloseDaysAfterPeriodEnd { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; } = true;

        // Consider adding validation to ensure CloseDaysAfterPeriodEnd is reasonable relative to OpenDaysBeforePeriodEnd for a given frequency
        // For example, OpenDaysBeforePeriodEnd for "Monthly" shouldn't be greater than ~28-30.
    }
}
