using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models.View_Models.Uploader;
using Operational_Risk_Management.Services.Interfaces;

namespace Operational_Risk_Management.Controllers
{
    public class KriManagementController : Controller
    {
        private readonly IKRIDashboardSercice _dashboardService;
        public KriManagementController(IKRIDashboardSercice dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            // Set default date range if not provided (last 30 days)
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-30);
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }

            // Store date range in ViewBag for form repopulation
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Get dashboard data
            var dashboardData = await _dashboardService.GetRiskManagementDashboardDataAsync(startDate, endDate);

            return View(dashboardData);
        }

        public async Task<IActionResult> Details(string type, FilterModelForSubmissions filterModel)
        {
            // Initialize filter model if null
            filterModel ??= new FilterModelForSubmissions();

            // Store filter model in ViewBag for form repopulation
            ViewBag.FilterModel = filterModel;
            ViewBag.SubmissionType = type;
            ViewBag.ShowFilters = true; // Show filter panel by default

            // Get departments and indicators for filter dropdowns
            ViewBag.Departments = await _dashboardService.GetAllDepartmentsAsync();
            ViewBag.Indicators = await _dashboardService.GetAllIndicatorsAsync();

            // Get years for filter (current year - 5 to current year)
            var currentYear = DateTime.Now.Year;
            ViewBag.Years = Enumerable.Range(currentYear - 5, 6).ToList();

            // Get submissions with filters
            var submissions = await _dashboardService.GetSubmissionsWithFiltersAsync(filterModel, type);

            return View("Details",submissions);
        }

    }
}
