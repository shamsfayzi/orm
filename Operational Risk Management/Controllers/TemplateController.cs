using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models; // For ViewStaticState
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.View_Models.Indicator;
using Operational_Risk_Management.Models.View_Models.Templates;

namespace Operational_Risk_Management.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ITemplateRepository _kriTemplateRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly IIndicatorRepository _kriIndicatorRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IApplicationUserRepository _users;
        private readonly IMapper mapper;
        public TemplateController(ITemplateRepository KRIRepo, IMapper mapper, IDepartmentRepository departmentRepository, ISubmissionRepository submissionRepository,IApplicationUserRepository users)
        {
            this.departmentRepository = departmentRepository;
            _users = users;
            _kriTemplateRepository = KRIRepo;
            this.mapper = mapper;
            _submissionRepository = submissionRepository;
        }
        public async Task<IActionResult> Index(PaginatedModel<Template> model)
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();
            return View(await _kriTemplateRepository.GetTableAsync(model));
        }
        [HttpGet]
        public async Task<IActionResult> CreateTemplate()
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();
            ViewBag.users = await _users.GetAllAsync(); ;
            ViewBag.Departments = await this.departmentRepository.GetAllAsync(); 
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplate(VM_KRITemplateCreate model)
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();
            // FluentValidation runs automatically due to Program.cs setup
            if (!ModelState.IsValid) // Rely on ModelStates
            {
                var departments = await this.departmentRepository.GetAllAsync();
                ViewBag.Departments = departments;
                return View(model);
            }
            await _kriTemplateRepository.AddAsync(model);
            TempData["msg-success"] = "New KRITemplate added successfully.";
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<bool> Delete(Guid id) // This is an AJAX endpoint, Forbid() might not be ideal. Client should check.
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return false; // Or throw exception for AJAX
            if (!await _kriTemplateRepository.IsExistedByIdAsync(id))
            {
                return false;
            }
            await _kriTemplateRepository.DeleteAsync(id);
            return true;
        }
        [HttpGet]
        public async Task<IActionResult> EditTemplate(Guid Id)
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();
            if (!await _kriTemplateRepository.IsExistedByIdAsync(Id, out var kri))
            {
                return NotFound();
            }
            ViewBag.users = await _users.GetAllAsync(); ;
            ViewBag.Departments = await this.departmentRepository.GetAllAsync();
            var model = mapper.Map<VM_KRITemplateUpdate>(kri);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTemplate(VM_KRITemplateUpdate model)
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();
            // FluentValidation runs automatically due to Program.cs setup
            if (!ModelState.IsValid) // Rely on ModelState
            {
                var departments = await this.departmentRepository.GetAllAsync();
                ViewBag.Departments = departments;
                return View(model);
            }

            await _kriTemplateRepository.UpdateAsync(model);
            TempData["msg-success"] = "KRITemplate updated successfully.";
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Details(PaginatedModel<Template> model, Guid id)
        {
            // Assuming Details view might be accessible to more roles if they need to see template structure
            // If not, add role check: if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();
            if (!await _kriTemplateRepository.IsExistedByIdAsync(id))
            {
                TempData["msg-error"] = "Template not found.";
                return NotFound();
            }
            return View(await _kriTemplateRepository.TemplateDetails(model, id));
        }
        public async Task<IActionResult> AddIndicator(Guid id) // id is TemplateId (GET)
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();
            var template = await _kriTemplateRepository.GetByIdAsync(id);
            if (template != null)
            {
                var model = new VM_KRIIndicatorCreate { TemplateId = id };
                ViewBag.TemplateName = template.FocalPoint ?? $"Template ({template.Id.ToString().Substring(0,8)})";
                // The AddIndicator.cshtml view should be updated to use @model VM_KRIIndicatorCreate
                return View("AddIndicator",model); // Explicitly name view if it's not already AddIndicator.cshtml
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIndicator(VM_KRIIndicatorCreate model) // POST
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();

            if (!ModelState.IsValid)
            {
                var template = await _kriTemplateRepository.GetByIdAsync(model.TemplateId);
                ViewBag.TemplateName = template?.FocalPoint ?? $"Template ({model.TemplateId.ToString().Substring(0,8)})";
                // The AddIndicator.cshtml view should be updated to use @model VM_KRIIndicatorCreate
                return View("AddIndicator",model);
            }
            await _kriTemplateRepository.AddTemplateIndicatorAsync(model);
            TempData["msg-success"] = "New Indicator added successfully."; // Corrected message
            // Redirect back to the indicator list for the specific template
            return RedirectToAction("Index", "TemplateIndicator", new { templateId = model.TemplateId });
        }
        public async Task<IActionResult> SubmissionDetails(Guid id) // This seems to be for viewing a KRI submission, might belong elsewhere or be uploader focused
        {
           Submission submission = await _submissionRepository.SubmissionDetails(id);
            if (submission != null)
            {
                return View(submission);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
