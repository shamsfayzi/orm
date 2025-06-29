using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.View_Models;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.View_Models.Uploader;
using Operational_Risk_Management.Services.Interfaces;

namespace Operational_Risk_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIncidentDashboardService _IncidentDashboardService;
        private readonly IKRIDashboardSercice _KRIdashboardService;
        private readonly IIncidentRepository _incidentRepository;

        public HomeController(IIncidentDashboardService IncidentdashboardService,IKRIDashboardSercice KRIDashboardService,IIncidentRepository incidentRepository)
        {
            _IncidentDashboardService = IncidentdashboardService;
            _KRIdashboardService = KRIDashboardService;
            _incidentRepository = incidentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            ViewBag.StartDate = startDate ?? DateTime.Now.AddMonths(-6);
            ViewBag.EndDate = endDate ?? DateTime.Now;

            // Prepare user context for service calls
            var userRole = ViewStaticState.IsAdmin ? ViewStaticState.RoleAdmin :
                           ViewStaticState.IsRiskManager ? ViewStaticState.RoleRiskManager :
                           ViewStaticState.IsUploader ? ViewStaticState.RoleUploader :
                           "User"; // Default or define as needed
            var userName = ViewStaticState.CurrentUserName;
            // Assuming CurrentUserDepartment might be an ID or Name. Services would need to handle.
            var userDepartmentIdentifier = ViewStaticState.CurrentUserDepartment;

            // TODO: Update service method signatures to accept user context for filtering
            // var IncidentdashboardData = await _IncidentDashboardService.GetDashboardDataAsync(startDate, endDate, userRole, userName, userDepartmentIdentifier);
            // var KRIdashboardData = await _KRIdashboardService.GetRiskManagementDashboardDataAsync(startDate, endDate, userRole, userName, userDepartmentIdentifier);

            // Using existing service calls for now, highlighting the need for them to be role-aware internally or via parameters
            var IncidentdashboardData = await _IncidentDashboardService.GetDashboardDataAsync(startDate, endDate);
            var KRIdashboardData = await _KRIdashboardService.GetRiskManagementDashboardDataAsync(startDate, endDate);


            return View(new DashboardDTO
            {
                IncidentDashboardDTO = IncidentdashboardData,
                KRIDashboardDataDTO = KRIdashboardData
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetIncidentsTrend(DateTime? startDate, DateTime? endDate, string interval = "month")
        {
            var trend = await _IncidentDashboardService.GetIncidentsTrendAsync(startDate, endDate, interval);
            return Json(trend);
        }
        [HttpGet]
        public async Task<IActionResult> GetIncidentDetails(Guid incidentId)
        {
            if (incidentId == Guid.Empty)
            {
                return BadRequest("Invalid incident ID.");
            }
            var incidentDetails = await _incidentRepository.GetIncidentDetailAsync(incidentId);
            if (incidentDetails == null)
            {
                return NotFound("Incident not found.");
            }
            return View("IncidentDetails", incidentDetails);
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

            // Populate user context for filtering
            filter.RequestingUserId = ViewStaticState.CurrentUserId.ToString(); // Assuming CurrentUserId is Guid
            filter.IsRequestingUserAdminOrManager = ViewStaticState.IsAdmin || ViewStaticState.IsRiskManager;
            filter.RequestingUserDepartment = ViewStaticState.CurrentUserDepartment;

            var departments = await _IncidentDashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _IncidentDashboardService.GetFilteredIncidentsAsync(filter); // Service needs to use these new filter props
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
            filter.RequestingUserId = ViewStaticState.CurrentUserId.ToString();
            filter.IsRequestingUserAdminOrManager = ViewStaticState.IsAdmin || ViewStaticState.IsRiskManager;
            filter.RequestingUserDepartment = ViewStaticState.CurrentUserDepartment;

            ViewBag.FilterModel = filter;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.IncidentType = "open";

            var departments = await _IncidentDashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _IncidentDashboardService.GetFilteredIncidentsAsync(filter);
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
            filter.RequestingUserId = ViewStaticState.CurrentUserId.ToString();
            filter.IsRequestingUserAdminOrManager = ViewStaticState.IsAdmin || ViewStaticState.IsRiskManager;
            filter.RequestingUserDepartment = ViewStaticState.CurrentUserDepartment;

            ViewBag.FilterModel = filter;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.IncidentType = "closed";

            var departments = await _IncidentDashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _IncidentDashboardService.GetFilteredIncidentsAsync(filter);
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
            filter.RequestingUserId = ViewStaticState.CurrentUserId.ToString();
            filter.IsRequestingUserAdminOrManager = ViewStaticState.IsAdmin || ViewStaticState.IsRiskManager;
            filter.RequestingUserDepartment = ViewStaticState.CurrentUserDepartment;
            ViewBag.FilterModel = filter;

            var departments = await _IncidentDashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _IncidentDashboardService.GetPendingReviewIncidentsAsync(pageNumber, pageSize);
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
            filter.RequestingUserId = ViewStaticState.CurrentUserId.ToString();
            filter.IsRequestingUserAdminOrManager = ViewStaticState.IsAdmin || ViewStaticState.IsRiskManager;
            filter.RequestingUserDepartment = ViewStaticState.CurrentUserDepartment;
            ViewBag.FilterModel = filter;

            var departments = await _IncidentDashboardService.GetDepartmentsAsync();
            ViewBag.Departments = departments;

            var result = await _IncidentDashboardService.GetRequiringRevisionIncidentsAsync(pageNumber, pageSize);
            ViewBag.TotalItems = result.TotalRecords;

            return View("IncidentList", result.Items);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
            ViewBag.Departments = await _KRIdashboardService.GetAllDepartmentsAsync();
            ViewBag.Indicators = await _KRIdashboardService.GetAllIndicatorsAsync();

            // Get years for filter (current year - 5 to current year)
            var currentYear = DateTime.Now.Year;
            ViewBag.Years = Enumerable.Range(currentYear - 5, 6).ToList();

            // Populate user context for filtering in filterModel (if it supports it)
            // Assuming FilterModelForSubmissions might need properties like RequestingUserId, IsAdmin etc.
            // For now, this highlights that _KRIdashboardService.GetSubmissionsWithFiltersAsync needs to be user-aware.
            // filterModel.RequestingUserId = ViewStaticState.CurrentUserId.ToString();
            // filterModel.IsUserAdminOrManager = ViewStaticState.IsAdmin || ViewStaticState.IsRiskManager;
            // filterModel.RequestingUserDepartment = ViewStaticState.CurrentUserDepartment;

            // Get submissions with filters
            var submissions = await _KRIdashboardService.GetSubmissionsWithFiltersAsync(filterModel, type /*, user context if passed directly */);

            return View("SubmissionDetails", submissions);
        }

    }
}
