using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
            return View(await _kriTemplateRepository.GetTableAsync(model));
        }
        [HttpGet]
        public async Task<IActionResult> CreateTemplate()
        {
            ViewBag.users = await _users.GetAllAsync(); ;
            ViewBag.Departments = await this.departmentRepository.GetAllAsync(); 
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplate(VM_KRITemplateCreate model)
        {
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
        public async Task<bool> Delete(Guid id)
        {
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
            if (!await _kriTemplateRepository.IsExistedByIdAsync(id))
            {
                TempData["msg-error"] = "Template not found.";
                return NotFound();
            }
            return View(await _kriTemplateRepository.TemplateDetails(model, id));
        }
        public async Task<IActionResult> AddIndicator(Guid id)
        {
            var template = await _kriTemplateRepository.GetByIdAsync(id);
            if (template != null)
            {
                return View(template);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIndicator(VM_KRIIndicatorCreate model)
        {
            // FluentValidation runs automatically due to Program.cs setup
            if (!ModelState.IsValid) // Rely on ModelState
            {
                // Need to fetch the template again if returning the view
                var template = await _kriTemplateRepository.GetByIdAsync(model.TemplateId);
                if (template == null)
                {
                    return NotFound(); // Or handle appropriately
                }
                // Pass the template back to the view if needed, or just return View(model)
                return View(template); // Assuming AddIndicator view needs the Template object
            }
            await _kriTemplateRepository.AddTemplateIndicatorAsync(model);
            TempData["msg-success"] = "New Indicator added successfully."; // Corrected message
            // Redirect back to the indicator list for the specific template
            return RedirectToAction("Index", "TemplateIndicator", new { templateId = model.TemplateId });
        }
        public async Task<IActionResult> SubmissionDetails(Guid id)
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
