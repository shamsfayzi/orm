using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.Interfaces.Services;
using Operational_Risk_Management.Models.View_Models.Uploader; // For IndicatorCardViewModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // For ToListAsync, Include etc.

namespace Operational_Risk_Management.Models.Services
{
    public class SubmissionCycleService : ISubmissionCycleService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IGenericRepository<Indicator> _indicatorRepository;
        private readonly IGenericRepository<Submission> _submissionRepository;
        private readonly IGenericRepository<SubmissionWindows> _submissionWindowsRepository;
        private readonly IMapper _mapper; // Assuming IMapper might be needed for complex mappings later

        public SubmissionCycleService(
            ITemplateRepository templateRepository,
            IGenericRepository<Indicator> indicatorRepository,
            IGenericRepository<Submission> submissionRepository,
            IGenericRepository<SubmissionWindows> submissionWindowsRepository,
            IMapper mapper)
        {
            _templateRepository = templateRepository;
            _indicatorRepository = indicatorRepository;
            _submissionRepository = submissionRepository;
            _submissionWindowsRepository = submissionWindowsRepository;
            _mapper = mapper;
        }

        public async Task<List<IndicatorCardViewModel>> GetIndicatorCardViewModelsAsync(string uploaderUserNameOrFocalPoint, string uploaderDepartmentIdentifier)
        {
            // This is a placeholder implementation.
            // Real logic would involve:
            // 1. Fetching relevant indicators based on uploaderUserNameOrFocalPoint and/or uploaderDepartmentIdentifier.
            //    This means querying Templates by FocalPoint or Department, then their Indicators.
            // 2. For each indicator, determining its current submission state (NextDueDate, CurrentPeriodStatus, CanSubmitOrEdit).
            //    This involves its FrequencyOfReview, the SubmissionWindows configuration, and existing Submissions.

            var templates = await _templateRepository.GetAsync(
                filter: t => t.FocalPoint == uploaderUserNameOrFocalPoint && !t.IsDeleted, // Simplified: only by focal point for now
                includeProperties: "Department,Indicators,Indicators.Submissions"
            );

            var result = new List<IndicatorCardViewModel>();

            foreach (var template in templates)
            {
                foreach (var indicator in template.Indicators.Where(i => !i.IsDeleted && i.IsActive))
                {
                    var state = await GetIndicatorSubmissionStateAsync(indicator.Id, uploaderUserNameOrFocalPoint); // userId might be more appropriate here

                    result.Add(new IndicatorCardViewModel
                    {
                        IndicatorId = indicator.Id,
                        IndicatorName = indicator.IndicatorName,
                        KRIType = indicator.KRIType,
                        RiskArea = indicator.RiskArea,
                        Process = indicator.Process,
                        FrequencyOfReview = indicator.FrequencyOfReview,
                        DepartmentName = template.Department?.DepartmentName,
                        TemplateFocalPoint = template.FocalPoint,
                        LastSubmissionDate = indicator.Submissions?.OrderByDescending(s => s.ReportingMonth).FirstOrDefault()?.CreateDate,
                        NextDueDate = state.NextDueDate,
                        CurrentStatus = state.CurrentPeriodStatus,
                        IsDueForSubmission = state.CanSubmitOrEdit,
                        LastSubmissionId = indicator.Submissions?.OrderByDescending(s => s.ReportingMonth).FirstOrDefault()?.Id ?? Guid.Empty
                    });
                }
            }
            return result.OrderBy(i => i.IndicatorName).ToList();
        }

        public async Task<IndicatorSubmissionStateDTO> GetIndicatorSubmissionStateAsync(Guid indicatorId, string userId)
        {
            // This is a placeholder implementation.
            // Real logic is complex and depends on business rules for submission cycles.
            var indicator = await _indicatorRepository.GetByIdAsync(indicatorId);
            if (indicator == null)
            {
                return new IndicatorSubmissionStateDTO { IndicatorId = indicatorId, CurrentPeriodStatus = "Error: Not Found", CanSubmitOrEdit = false };
            }

            // Simplified placeholder logic:
            var currentReportingPeriodStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastSubmissionForCurrentPeriod = await _submissionRepository.GetAsync(
                filter: s => s.IndicatorId == indicatorId && s.ReportingMonth == currentReportingPeriodStart,
                orderBy: o => o.OrderByDescending(s => s.CreateDate)
            ).ContinueWith(t => t.Result.FirstOrDefault());

            string status = "Open"; // Default
            bool canEdit = true;    // Default
            DateTime? nextDueDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month)); // End of current month


            if (lastSubmissionForCurrentPeriod != null)
            {
                status = lastSubmissionForCurrentPeriod.Status;
                canEdit = status == "Draft"; // Can only edit drafts
            }

            // TODO: Implement actual logic using indicator.FrequencyOfReview, SubmissionWindows, and current date.
            // Example: Check if window is locked based on SubmissionWindows configuration for the indicator's frequency.
            // var submissionWindowConfig = await _submissionWindowsRepository.GetAsync(f => f.Frequency == indicator.FrequencyOfReview && f.IsActive);
            // if (submissionWindowConfig.Any()) { ... calculate lock status ... }


            return new IndicatorSubmissionStateDTO
            {
                IndicatorId = indicatorId,
                NextDueDate = nextDueDate, // Placeholder
                CurrentPeriodStatus = status, // Placeholder
                CanSubmitOrEdit = canEdit, // Placeholder
                CurrentPeriodSubmissionId = lastSubmissionForCurrentPeriod?.Id,
                ReportingPeriodName = currentReportingPeriodStart.ToString("MMMM yyyy")
            };
        }
    }
}
