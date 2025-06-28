using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Services;
using Operational_Risk_Management.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Operational_Risk_Management.Controllers
{
    public class IncidentManagementController : Controller
    {
        private readonly IIncidentDashboardService _dashboardService;

        public IncidentManagementController(IIncidentDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(DateTime? startDate, DateTime? endDate)
        {
            ViewBag.StartDate = startDate ?? DateTime.Now.AddMonths(-6);
            ViewBag.EndDate = endDate ?? DateTime.Now;

            var dashboardData = await _dashboardService.GetDashboardDataAsync(startDate, endDate);
            return View(dashboardData);
        }

        [HttpGet]
        public async Task<IActionResult> GetIncidentsTrend(DateTime? startDate, DateTime? endDate, string interval = "month")
        {
            var trend = await _dashboardService.GetIncidentsTrendAsync(startDate, endDate, interval);
            return Json(trend);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIncidents(IncidentFilterDTO filter)
        {
            if (filter == null)
            {
                filter = new IncidentFilterDTO();
            }

            ViewBag.FilterModel = filter;
            ViewBag.CurrentPage = filter.PageNumber;
            ViewBag.PageSize = filter.PageSize;

            var departments = await _dashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _dashboardService.GetFilteredIncidentsAsync(filter);
            ViewBag.TotalItems = result.TotalRecords;

            return View("IncidentList", result.Items);
        }

        [HttpGet]
        public async Task<IActionResult> GetOpenIncidents(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new IncidentFilterDTO
            {
                Status = IncidentStatus.Open,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            ViewBag.FilterModel = filter;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.IncidentType = "open";

            var departments = await _dashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _dashboardService.GetFilteredIncidentsAsync(filter);
            ViewBag.TotalItems = result.TotalRecords;

            return View("IncidentList", result.Items);
        }

        [HttpGet]
        public async Task<IActionResult> GetClosedIncidents(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new IncidentFilterDTO
            {
                Status = IncidentStatus.Closed,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            ViewBag.FilterModel = filter;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.IncidentType = "closed";

            var departments = await _dashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _dashboardService.GetFilteredIncidentsAsync(filter);
            ViewBag.TotalItems = result.TotalRecords;

            return View("IncidentList", result.Items);
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingReviewIncidents(int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.IncidentType = "pending";

            var filter = new IncidentFilterDTO
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            ViewBag.FilterModel = filter;

            var departments = await _dashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _dashboardService.GetPendingReviewIncidentsAsync(pageNumber, pageSize);
            ViewBag.TotalItems = result.TotalRecords;

            return View("IncidentList", result.Items);
        }

        [HttpGet]
        public async Task<IActionResult> GetRequiringRevisionIncidents(int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.IncidentType = "revision";

            var filter = new IncidentFilterDTO
            {
                RequiresRevision = true,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            ViewBag.FilterModel = filter;

            var departments = await _dashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _dashboardService.GetRequiringRevisionIncidentsAsync(pageNumber, pageSize);
            ViewBag.TotalItems = result.TotalRecords;

            return View("IncidentList", result.Items);
        }

        [HttpGet]
        public async Task<IActionResult> ViewIncident(Guid id)
        {
            // This would be implemented to fetch and display incident details
            // For now, we'll just return a placeholder
            return Content($"View Incident with ID: {id}");
        }

        [HttpGet]
        public async Task<IActionResult> EditIncident(Guid id)
        {
            // This would be implemented to fetch and edit incident details
            // For now, we'll just return a placeholder
            return Content($"Edit Incident with ID: {id}");
        }
    }
}
