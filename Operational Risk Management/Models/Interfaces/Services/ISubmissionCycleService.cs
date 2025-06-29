using System;
using Operational_Risk_Management.Models.View_Models.Uploader; // For IndicatorCardViewModel or a new DTO

namespace Operational_Risk_Management.Models.Interfaces.Services
{
    public class IndicatorSubmissionStateDTO
    {
        public Guid IndicatorId { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string CurrentPeriodStatus { get; set; } // e.g., "Open", "Submitted", "Approved", "Draft", "Overdue", "Locked"
        public bool CanSubmitOrEdit { get; set; } // True if user can currently make/edit a submission for the current period
        public Guid? CurrentPeriodSubmissionId { get; set; } // ID of submission for current period, if exists (draft or submitted)
        public string ReportingPeriodName { get; set; } // e.g., "March 2024"
    }

    public interface ISubmissionCycleService
    {
        /// <summary>
        /// Gets the current submission state for a specific indicator.
        /// Considers frequency, submission windows, existing submissions, and current date.
        /// </summary>
        Task<IndicatorSubmissionStateDTO> GetIndicatorSubmissionStateAsync(Guid indicatorId, string userId);

        /// <summary>
        /// Gets the submission state for multiple indicators, typically for a dashboard.
        /// </summary>
        Task<List<IndicatorCardViewModel>> GetIndicatorCardViewModelsAsync(string uploaderUserNameOrFocalPoint, string uploaderDepartmentIdentifier);
    }
}
