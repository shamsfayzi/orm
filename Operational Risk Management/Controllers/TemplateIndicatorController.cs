using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Operational_Risk_Management.Models.View_Models.KRIIndicator;
using Operational_Risk_Management.Models.Extensions; // For validation extension and ExpressionExtensions
using Microsoft.AspNetCore.Mvc.Rendering;
using Operational_Risk_Management.Models.Common; // For SelectList and PaginatedModel
using System.Linq.Expressions; // For Expression<Func<T, bool>>

namespace Operational_Risk_Management.Controllers
{
    public class TemplateIndicatorController : Controller
    {
        private readonly IGenericRepository<Indicator> _indicatorRepository;
        private readonly IGenericRepository<Template> _templateRepository;
        private readonly IGenericRepository<Submission> _submissionRepository;
        private readonly ApplicationDBContext _context; // Keep for potential complex queries/includes not covered by generic repo
        private readonly IMapper _mapper;

        public TemplateIndicatorController(
            IGenericRepository<Indicator> indicatorRepository,
            IGenericRepository<Template> templateRepository,
            IGenericRepository<Submission> submissionRepository, // Added Submission Repository
            ApplicationDBContext context,
            IMapper mapper
            )
        {
            _indicatorRepository = indicatorRepository;
            _templateRepository = templateRepository;
            _submissionRepository = submissionRepository; // Initialize Submission Repository
            _context = context;
            _mapper = mapper;
        }

        // GET: TemplateIndicator?templateId={templateId}
        public async Task<IActionResult> ManageIndicators(Guid templateId)
        {   
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                return NotFound("Template not found.");
            }

            // Eagerly load indicators for the specific template
            var indicators = await _context.Indicators
                                        .Where(i => i.TemplateId == templateId && !i.IsDeleted)
                                        .ToListAsync();
            return View(indicators);
        }

        // GET: TemplateIndicator/Details/5
        public async Task<IActionResult> Details(Guid id, string searchTerm, int? year, int? month, int page = 1, int pageSize = 10)
        {
            // Fetch indicator using repository, including its Template
            var indicators = await _indicatorRepository.GetAsync(filter: i => i.Id == id && !i.IsDeleted, includeProperties: "Template");
            var indicator = indicators.FirstOrDefault();

            if (indicator == null) // GetAsync returns a list, so check if the first item is null (or list is empty)
            {
                return NotFound();
            }

            // Prepare base filter for submissions
            Expression<Func<Submission, bool>> submissionFilter = s => s.IndicatorId == indicator.Id && !s.IsDeleted;

            // Apply filters using the extension method
            submissionFilter = submissionFilter.ApplyFilters(searchTerm, year, month);

            // Initialize PaginatedModel for the repository call
            var paginatedModelInput = new PaginatedModel<Submission>
            {
                CurrentPage = page,
                PageSize = pageSize
            };

            // Use the submission repository to get paginated submissions
            var paginatedSubmissions = await _submissionRepository.GetPaginatedAsync(
                paginatedModelInput,
                submissionFilter,
                orderBy: q => q.OrderByDescending(s => s.CreateDate)
            );

            // Fetch distinct years for the year dropdown
            // Using _context directly here for a specific aggregation query that might be simpler than a generic repo method.
            var submissionYears = await _context.Submissions 
                .Where(s => s.IndicatorId == indicator.Id && !s.IsDeleted)
                .Select(s => s.CreateDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var yearOptions = submissionYears.Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() }).ToList();
            // Ensure 'All Years' option allows clearing the filter (empty string value for 'year' parameter)
            yearOptions.Insert(0, new SelectListItem { Value = "", Text = "All Years" }); 

            // Month options
            var monthOptions = Enumerable.Range(1, 12).Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = new DateTime(DateTime.UtcNow.Year, m, 1).ToString("MMMM") 
            }).ToList();
            // Ensure 'All Months' option allows clearing the filter (empty string value for 'month' parameter)
            monthOptions.Insert(0, new SelectListItem { Value = "", Text = "All Months" }); 

            // Fetch sibling indicators using the indicator repository
            var siblingIndicators = await _indicatorRepository.GetAsync(
                filter: i => i.TemplateId == indicator.TemplateId && !i.IsDeleted && i.Id != indicator.Id,
                orderBy: q => q.OrderBy(i => i.IndicatorName)
            );

            var viewModel = new VM_IndicatorDetails
            {
                Indicator = indicator,
                Submissions = paginatedSubmissions, // Assign the PaginatedModel directly
                SiblingIndicators = siblingIndicators.ToList(), 
                TemplateFocalPoint = indicator.Template?.FocalPoint,
                SearchTerm = searchTerm,
                SelectedYear = year, // Pass back the original year for the dropdown selection
                SelectedMonth = month, // Pass back the original month for the dropdown selection
                YearOptions = yearOptions,
                MonthOptions = monthOptions
            };

            ViewBag.IndicatorSelectList = new SelectList(
                siblingIndicators, 
                nameof(Indicator.Id),
                nameof(Indicator.IndicatorName)
            );

            return View(viewModel);
        }

        // GET: TemplateIndicator/Create?templateId={templateId}
        public async Task<IActionResult> Create(Guid templateId)
        {
             var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                return NotFound("Template not found.");
            }
            ViewBag.TemplateId = templateId;
            // Initialize a new ViewModel with the TemplateId pre-filled
            var viewModel = new VM_IndicatorCreate { TemplateId = templateId };
            return View(viewModel);
        }

        // POST: TemplateIndicator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM_IndicatorCreate viewModel)
            {
            // FluentValidation runs automatically due to Program.cs setup
            if (!viewModel.IsValid(new VLD_IndicatorCreate(HttpContext), ModelState))
            {
                ViewBag.TemplateId = viewModel.TemplateId; // Keep templateId in ViewBag
                return View(viewModel);
            }

            var indicator = _mapper.Map<Indicator>(viewModel);
            await _indicatorRepository.AddAsync(indicator);
            await _indicatorRepository.SaveChangesAsync();

            TempData["msg-success"] = "Indicator created successfully.";
            return RedirectToAction(nameof(ManageIndicators), new { templateId = viewModel.TemplateId });
        }

        // GET: TemplateIndicator/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var indicator = await _indicatorRepository.GetByIdAsync(id);
            if (indicator == null || indicator.IsDeleted)
            {
                return NotFound();
            }
            // Map Entity to ViewModel
            var viewModel = _mapper.Map<VM_IndicatorEdit>(indicator);
            ViewBag.TemplateId = indicator.TemplateId; // Pass TemplateId for context
            return View(viewModel);
        }

        // POST: TemplateIndicator/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VM_IndicatorEdit viewModel)
        {
            if (id != viewModel.Id)
            {
                // Consider returning BadRequest() if the ID mismatch is a client error
                return NotFound(); 
            }

            // FluentValidation runs automatically due to Program.cs setup
            if (!ModelState.IsValid) // Rely on ModelState
            {
                ViewBag.TemplateId = viewModel.TemplateId; // Keep templateId in ViewBag
                return View(viewModel);
            }

            try
            {
                var indicator = await _indicatorRepository.GetByIdAsync(id);
                // Check if deleted as well, depending on business logic
                if (indicator == null || indicator.IsDeleted) 
                {
                    return NotFound();
                }

                _mapper.Map(viewModel, indicator);
                await _indicatorRepository.UpdateAsync(indicator);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Use the local check method which considers IsDeleted
                if (!await IndicatorExists(viewModel.Id)) 
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["msg-success"] = "Indicator updated successfully.";
            return RedirectToAction(nameof(Index), new { templateId = viewModel.TemplateId });
        }

        // GET: TemplateIndicator/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var indicator = await _indicatorRepository.GetByIdAsync(id);
            if (indicator == null || indicator.IsDeleted)
            {
                return NotFound();
            }
             // Optionally include Template details if needed in the view
            // var indicatorWithTemplate = await _context.Indicators.Include(i => i.Template).FirstOrDefaultAsync(i => i.Id == id);
            return View(indicator);
        }

        // POST: TemplateIndicator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var indicator = await _indicatorRepository.GetByIdAsync(id);
            if (indicator == null)
            {
                // Already deleted or never existed
                return RedirectToAction(nameof(Index)); // Redirect to a safe place, maybe a general error page or template list?
            }

            var templateId = indicator.TemplateId; // Store before deleting
            // Implement soft delete
            await _indicatorRepository.SoftDeleteAsync(id, "SYSTEM_USER"); // Assuming a user context is available later

            return RedirectToAction(nameof(Index), new { templateId = templateId });
        }

        private async Task<bool> IndicatorExists(Guid id)
        {
            var indicator = await _indicatorRepository.GetByIdAsync(id);
            return indicator != null && !indicator.IsDeleted;
        }
    }
}