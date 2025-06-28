using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Incident;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Services.Interfaces;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Enums;
using Operational_Risk_Management.Models.View_Models.Uploader;
using AutoMapper;
using Operational_Risk_Management.Models.View_Models.KRIIndicator;

namespace Operational_Risk_Management.Services
{
    public class KRIDashboardService : IKRIDashboardSercice
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public KRIDashboardService(IApplicationDbContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<KRIDashboardDataDTO> GetRiskManagementDashboardDataAsync(DateTime? startDate, DateTime? endDate)
        {
            var SubmissionsQuery = ApplyDateFilter(_context.Submissions, startDate, endDate);

            var dashboardData = new KRIDashboardDataDTO
            {
                AllSubmissions = await SubmissionsQuery.CountAsync(),
                BreachedSubmissions = await SubmissionsQuery.CountAsync(i => i.Breaches > i.Indicator.TolerableBreaches),
                OverDueSubmissions = await SubmissionsQuery.CountAsync(i => i.Status == "Overdue"),
                RevissionSubmissions = await SubmissionsQuery.CountAsync(i => i.UpdatedDate > _context.Feedbacks.Where(a => a.SubmissionId == i.Id).OrderBy(a => a.CreateDate).FirstOrDefault().CreateDate),
                MostBreachedByDepartment = await GetMostBreachedByDepartment(SubmissionsQuery, startDate, endDate),
                MostBreachedByIndicator = await GetMostBreachedByIndicator(SubmissionsQuery, startDate, endDate)
            };
            return dashboardData;
        }

        public async Task<List<VM_Submission>> GetSubmissionsWithFiltersAsync(FilterModelForSubmissions model, string submissionType = "all")
        {
            // Initialize model if null
            model ??= new FilterModelForSubmissions();

            // Normalize filter values
            if (!model.Statuses.Any())
            {
                model.Statuses = new List<string> { "In Progress", "Submitted", "Overdue","Opened" };
            }

            // Start with base query
            var query = _context.Submissions.AsQueryable();

            // Apply submission type filter
            query = submissionType switch
            {
                "breached" => query.Where(s => s.Breaches > s.Indicator.TolerableBreaches),
                "overdue" => query.Where(s => s.Status == "Overdue"),
                "revision" => query.Where(s => s.UpdatedDate > _context.Feedbacks
                    .Where(a => a.SubmissionId == s.Id)
                    .OrderBy(a => a.CreateDate)
                    .FirstOrDefault().CreateDate),
                _ => query // "all" - no additional filter
            };

            // Apply indicator filters
            if (model.IndicatorIds.Any())
            {
                query = query.Where(s => model.IndicatorIds.Contains(s.IndicatorId));
            }
            else if (model.TemplateId != Guid.Empty)
            {
                query = query.Where(s => s.Indicator.TemplateId == model.TemplateId);
            }

            // Apply status filter
            if (model.Statuses.Any())
            {
                query = query.Where(s => model.Statuses.Contains(s.Status));
            }

            // Apply search term filter
            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                query = query.Where(s => s.Notes.Contains(model.SearchTerm));
            }

            // Apply date range filter
            if (model.DateFrom.HasValue)
            {
                query = query.Where(s => s.ReportingMonth >= model.DateFrom.Value);
            }

            if (model.DateTo.HasValue)
            {
                // Add 1 day to DateTo to make the range inclusive of the selected end date
                query = query.Where(s => s.ReportingMonth < model.DateTo.Value.AddDays(1));
            }

            // Exclude deleted submissions
            query = query.Where(s => !s.IsDeleted);

            // Include related entities
            query = query.Include(s => s.Indicator)
                         .Include(s => s.Indicator.Template)
                         .Include(s => s.Indicator.Template.Department);

            // Execute query and map to view model
            var submissions = await query.ToListAsync();
            return MapToViewModel(submissions);
        }

        public async Task<List<string>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Where(d => !d.IsDeleted)
                .Select(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<List<Indicator>> GetAllIndicatorsAsync()
        {
            return await _context.Indicators
                .Include(i => i.Template)
                    .ThenInclude(t => t.Department)
                .Where(i => !i.IsDeleted)
                .ToListAsync();
        }

        private IQueryable<Submission> ApplyDateFilter(IQueryable<Submission> query, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                query = query.Where(i => i.ReportingMonth >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.ReportingMonth <= endDate.Value);
            }

            return query.Where(i => !i.IsDeleted);
        }

        private async Task<List<DepartmentCountResult>> GetMostBreachedByDepartment(IQueryable<Submission> query, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                query = query.Where(i => i.ReportingMonth >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(i => i.ReportingMonth <= endDate.Value);
            }

            return await query.
                GroupBy(x => x.Indicator.Template.Department.DepartmentName ?? "Unknown")
                .OrderByDescending(g => g.Count())
                .Select(g => new DepartmentCountResult
                {
                    Count = g.Count(),
                    Department = g.Key
                })
                .ToListAsync();
        }

        private async Task<List<IndicatorCountResult>> GetMostBreachedByIndicator(IQueryable<Submission> query, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                query = query.Where(i => i.ReportingMonth >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(i => i.ReportingMonth <= endDate.Value);
            }

            return await query.
                GroupBy(x => x.Indicator.IndicatorName ?? "Unknown")
                .OrderByDescending(g => g.Count())
                .Select(g => new IndicatorCountResult
                {
                    Count = g.Count(),
                    IndicatorName = g.Key
                })
                .ToListAsync();
        }

        private List<VM_Submission> MapToViewModel(List<Submission> submissions)
        {
            var result = new List<VM_Submission>();

            foreach (var submission in submissions)
            {
                result.Add(_mapper.Map<VM_Submission>(submission));
            }

            return result;
        }

        private int GetWeekOfYear(DateTime date)
        {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return ci.Calendar.GetWeekOfYear(date, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
        }
    }
}