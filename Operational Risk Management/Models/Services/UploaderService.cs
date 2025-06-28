using AutoMapper;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.Interfaces.Services;
using Operational_Risk_Management.Models.Services.Repository;
using Operational_Risk_Management.Models.View_Models.Uploader;

namespace Operational_Risk_Management.Models.Services
{
    public class UploaderService : IUploaderService
    {
        private readonly IIndicatorRepository _indicatorRepo;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISubmissionRepository _submissionsRepo;
        private readonly IMapper _mapper;

        public UploaderService(IIndicatorRepository indicatorRepo, IMapper mapper, ITemplateRepository templateRepository, ISubmissionRepository submissionRepo)
        {
            _submissionsRepo = submissionRepo;
            _indicatorRepo = indicatorRepo;
            _templateRepository = templateRepository;
            _mapper = mapper;
        }

        public void BuildDashboardAsync(string username)
        {
            //    var indicators = await _indicatorRepo.GetAsync(
            //         filter: s => s.Template.FocalPoint == username && !s.IsDeleted,
            //includeProperties: "Submissions,Template,Template.Department",
            //orderBy: o => o.OrderByDescending(s => s.CreateDate)
            //);
            //    //var dueSoon = await GetDueSoonIndicatorsAsync(username);

            //    //var merged = MergeIndicators(indicators, dueSoon);
            //    var (approved, pending, rejected) = CountStatuses(indicators);
            //    var mappedIndicators = _mapper.Map<List<IndicatorViewModel>>(indicators);
            //    int openSubmissionCount = mappedIndicators.Count(a => a.CanSubmitNewReport == true);
            //    //int dueSoonCount = merged.Count(i => i.IsDueSoon);
            //    var last = GetLastSubmittedVM(indicators);
            //    //dueSoonCount -= openSubmissionCount;

            //    return new VM_UploaderDashboard
            //    {
            //        TemplateFocalPoint = indicators.FirstOrDefault()?.Template?.FocalPoint ?? username,
            //        DepartmentName = indicators.FirstOrDefault()?.Template?.Department?.DepartmentName ?? "Unknown",
            //        Indicators = mappedIndicators,
            //        ApprovedCount = approved,
            //        PendingCount = pending,
            //        RejectedCount = rejected,
            //        //DueSoonCount = dueSoonCount,
            //        LastSubmitted = last,
            //        OpenSubmissionCount = openSubmissionCount
            //    };
        }
        public async Task<List<VM_Submission>> SubmissionVM(FilterModelForSubmissions model)
        {
            // Initialize model if null
            model ??= new FilterModelForSubmissions();

            // Normalize filter values
            if (!model.Statuses.Any())
            {
                model.Statuses = new List<string> { "Open", "Submitted", "Closed", "Draft" };
            }

            if (!model.Months.Any())
            {
                model.Months = Enumerable.Range(1, 12).ToList();
            }

            if (!model.Years.Any())
            {
                var currentYear = DateTime.Now.Year;
                model.Years = Enumerable.Range(currentYear - 5, 6).ToList();
            }

            // Build query with proper template-indicator handling
            var submissions = await _submissionsRepo.GetAsync(
                filter: s =>
                    // Handle indicator filtering:
                    // 1. If specific indicators are selected, filter by those indicators
                    // 2. If no indicators but a template is specified, filter by template ID
                    // 3. If neither indicators nor template specified, return all
                    (
                        (model.IndicatorIds.Any() && model.IndicatorIds.Contains(s.IndicatorId)) ||
                        (!model.IndicatorIds.Any() && model.TemplateId != Guid.Empty && s.Indicator.TemplateId == model.TemplateId) 
                    ) &&

                    // Filter by status
                    (model.Statuses.Count == 0 || model.Statuses.Contains(s.Status)) &&

                    // Filter by month
                    (model.Months.Count == 0 || model.Months.Contains(s.CreateDate.Month)) &&

                    // Filter by year
                    (model.Years.Count == 0 || model.Years.Contains(s.CreateDate.Year)) &&

                    // Filter by search term
                    (string.IsNullOrEmpty(model.SearchTerm) ||
                     (s.Notes != null && s.Notes.Contains(model.SearchTerm, StringComparison.OrdinalIgnoreCase))) &&

                    // Exclude deleted
                    !s.IsDeleted,

                includeProperties: "Indicator,Indicator.Template,Indicator.Template.Department"
            );
            submissions.ForEach(a =>
            {
                if (!model.IndicatorIds.Exists(id => id == a.IndicatorId))
                {
                    model.IndicatorIds.Add(a.IndicatorId);
                }
            });
            return _mapper.Map<List<VM_Submission>>(submissions);
        }

    }
}
