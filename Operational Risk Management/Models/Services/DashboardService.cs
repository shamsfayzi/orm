using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Operational_Risk_Management.Services
{
    public class IncidentDashboardService : IIncidentDashboardService
    {
        private readonly IApplicationDbContext _context;

        public IncidentDashboardService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IncidentDashboardDTO> GetDashboardDataAsync(DateTime? startDate, DateTime? endDate, string requestingRole, string requestingUserId, string requestingDepartment)
        {
            var query = _context.Incidents.AsQueryable();

            // Actual TODO: Apply filtering based on requestingRole, requestingUserId, requestingDepartment
            // if (requestingRole == ViewStaticState.RoleUploader) // Assuming "Uploader" maps to a non-admin user role
            // {
            //    // Example: Filter incidents created by the user or for their department
            //    query = query.Where(i => i.CreateBy == requestingUserId || i.BranchDepartmentUnit == requestingDepartment);
            // }
            // else if (requestingRole == "SomeOtherLimitedRole") { ... }

            if (startDate.HasValue)
            {
                query = query.Where(i => i.ReportDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.ReportDate <= endDate.Value);
            }

            // Get counts for dashboard metrics
            var totalIncidents = await query.CountAsync();
            var openIncidents = await query.Where(i => i.IncidentStatus == IncidentStatus.Open).CountAsync();
            var pendingReviewIncidents = await query.Where(i => i.IncidentStatus == IncidentStatus.Open && i.LastReviewDate == null).CountAsync();
            var requiresRevisionIncidents = await query.Where(i => i.RequiresRevision).CountAsync();
            var closedIncidents = await query.Where(i => i.IncidentStatus == IncidentStatus.Closed).CountAsync();

            // Get incidents by department
            var incidentsByDepartment = await query
                .GroupBy(i => i.BranchDepartmentUnit)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.Department, x => x.Count);

            // Get incidents by category
            var incidentsByCategory = await query
                .GroupBy(i => i.IncidentCategoryLevel)
                .Select(g => new { Category = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.Category, x => x.Count);

            // Get incidents by risk level
            var incidentsByRiskLevel = await query
                .GroupBy(i => i.RiskAssessment)
                .Select(g => new { RiskLevel = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.RiskLevel, x => x.Count);

            // Get incidents trend by month
            var incidentsTrend = await GetIncidentsTrendAsync(startDate, endDate, "month");

            return new IncidentDashboardDTO
            {
                TotalIncidents = totalIncidents,
                OpenIncidents = openIncidents,
                PendingReviewIncidents = pendingReviewIncidents,
                RequiringRevisionIncidents = requiresRevisionIncidents,
                ClosedIncidents = closedIncidents,
                IncidentsByDepartment = incidentsByDepartment,
                IncidentsByCategory = incidentsByCategory,
                IncidentsByRiskLevel = incidentsByRiskLevel,
                IncidentsTrend = incidentsTrend
            };
        }

        public async Task<Dictionary<string, int>> GetIncidentsTrendAsync(DateTime? startDate, DateTime? endDate, string interval = "month")
        {
            // Default date range if not provided
            var start = startDate ?? DateTime.Now.AddYears(-1);
            var end = endDate ?? DateTime.Now;

            // Apply date filters
            var query = _context.Incidents
                .Where(i => i.ReportDate >= start && i.ReportDate <= end)
                .AsQueryable();

            // Group by the specified interval
            Dictionary<string, int> result;

            switch (interval.ToLower())
            {
                case "day":
                    result = await query
                        .GroupBy(i => i.ReportDate.Date)
                        .Select(g => new { Date = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Date)
                        .ToDictionaryAsync(x => x.Date.ToString("yyyy-MM-dd"), x => x.Count);
                    break;

                case "week":
                    // Pull dates only (minimal projection)
                    var weekData = await query
                        .Select(i => i.ReportDate)
                        .ToListAsync();

                    result = weekData
                        .GroupBy(date => new { date.Year, Week = GetIsoWeek(date) })
                        .Select(g => new { Date = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Date.Year).ThenBy(x => x.Date.Week)
                        .ToDictionary(x => $"{x.Date.Year}-W{x.Date.Week:D2}", x => x.Count);
                    break;

                case "month":
                default:
                    var monthData = await query
                        .Select(i => new { i.ReportDate.Year, i.ReportDate.Month })
                        .ToListAsync();

                    result = monthData
                        .GroupBy(x => new { x.Year, x.Month })
                        .Select(g => new
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            Count = g.Count()
                        })
                        .OrderBy(x => x.Date)
                        .ToDictionary(x => x.Date.ToString("yyyy-MM"), x => x.Count);
                    break;
            }

            return result;
        }

        public async Task<PaginatedModel<IncidentListDTO>> GetFilteredIncidentsAsync(IncidentFilterDTO filter)
        {
            var query = _context.Incidents.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(i =>
                    i.TitleOfIncident.Contains(filter.SearchTerm) ||
                    i.BranchDepartmentUnit.Contains(filter.SearchTerm) ||
                    i.EventIncidentDescription.Contains(filter.SearchTerm)
                );
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(i => i.ReportDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(i => i.ReportDate <= filter.EndDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.BranchDepartmentUnit))
            {
                query = query.Where(i => i.BranchDepartmentUnit == filter.BranchDepartmentUnit);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(i => i.IncidentStatus == filter.Status.Value);
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(i => i.IncidentType == filter.Type.Value);
            }

            if (filter.Category.HasValue)
            {
                query = query.Where(i => i.IncidentCategoryLevel == filter.Category.Value);
            }

            if (filter.RiskLevel.HasValue)
            {
                query = query.Where(i => i.RiskAssessment == filter.RiskLevel.Value);
            }

            if (filter.RequiresRevision.HasValue)
            {
                query = query.Where(i => i.RequiresRevision == filter.RequiresRevision.Value);
            }

            // Get total count for pagination
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderByDescending(i => i.ReportDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(i => new IncidentListDTO
                {
                    Id = i.Id,
                    TitleOfIncident = i.TitleOfIncident,
                    BranchDepartmentUnit = i.BranchDepartmentUnit,
                    ReportDate = i.ReportDate,
                    IncidentStatus = i.IncidentStatus,
                    IncidentType = i.IncidentType,
                    IncidentCategoryLevel = i.IncidentCategoryLevel,
                    RiskAssessment = i.RiskAssessment,
                    RequiresRevision = i.RequiresRevision
                })
                .ToListAsync();

            return new PaginatedModel<IncidentListDTO>
            {
                Items = items,
                TotalRecords = totalItems,
                CurrentPage = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize)
            };
        }

        public async Task<List<string>> GetDepartmentsAsync()
        {
            return await _context.Incidents
                .Select(i => i.BranchDepartmentUnit)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<PaginatedModel<IncidentListDTO>> GetPendingReviewIncidentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new IncidentFilterDTO
            {
                Status = IncidentStatus.Open,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var query = _context.Incidents
                .Where(i => i.IncidentStatus == IncidentStatus.Open && i.LastReviewDate == null);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.ReportDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new IncidentListDTO
                {
                    Id = i.Id,
                    TitleOfIncident = i.TitleOfIncident,
                    BranchDepartmentUnit = i.BranchDepartmentUnit,
                    ReportDate = i.ReportDate,
                    IncidentStatus = i.IncidentStatus,
                    IncidentType = i.IncidentType,
                    IncidentCategoryLevel = i.IncidentCategoryLevel,
                    RiskAssessment = i.RiskAssessment,
                    RequiresRevision = i.RequiresRevision
                })
                .ToListAsync();

            return new PaginatedModel<IncidentListDTO>
            {
                Items = items,
                TotalRecords = totalItems,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<PaginatedModel<IncidentListDTO>> GetRequiringRevisionIncidentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new IncidentFilterDTO
            {
                RequiresRevision = true,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var query = _context.Incidents
                .Where(i => i.RequiresRevision);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.LastReviewDate ?? i.ReportDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new IncidentListDTO
                {
                    Id = i.Id,
                    TitleOfIncident = i.TitleOfIncident,
                    BranchDepartmentUnit = i.BranchDepartmentUnit,
                    ReportDate = i.ReportDate,
                    IncidentStatus = i.IncidentStatus,
                    IncidentType = i.IncidentType,
                    IncidentCategoryLevel = i.IncidentCategoryLevel,
                    RiskAssessment = i.RiskAssessment,
                    RequiresRevision = i.RequiresRevision
                })
                .ToListAsync();

            return new PaginatedModel<IncidentListDTO>
            {
                Items = items,
                TotalRecords = totalItems,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        // Helper method to get ISO week number
        private int GetIsoWeek(DateTime date)
        {
            // ISO week starts on Monday
            var day = (int)System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date.AddDays(day == 0 ? -6 : 1 - day),
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }
    }

   

}
