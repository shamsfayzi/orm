using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models; // For ViewStaticState
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.Services.Repositories;
using Operational_Risk_Management.Models.View_Models.Departments;

namespace Operational_Risk_Management.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository departmentRepository;
        private readonly IMapper mapper;
        public DepartmentController(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            this.departmentRepository = departmentRepository;
            this.mapper = mapper;
        }
        
        public async Task<IActionResult> Index(PaginatedModel<Department> model)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            return View(await departmentRepository.GetTableAsync(model));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM_DepartmentCreate model)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            await departmentRepository.AddAsync(model);
            TempData["msg-success"] = "New Department added successfully.";
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<bool> Delete(Guid id) // AJAX endpoint
        {
            if (!ViewStaticState.IsAdmin) return false; // Or throw for AJAX
            if (!await departmentRepository.IsExistedByIdAsync(id))
            {
                return false;
            }
            await departmentRepository.DeleteAsync(id);
            return true;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            if(!await departmentRepository.IsExistedByIdAsync(Id,out var dep))
            {
                return NotFound();
            }
            return View(mapper.Map<VM_DepartmentUpdate>(dep));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VM_DepartmentUpdate model)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await departmentRepository.UpdateAsync(model);
            TempData["msg-success"] = "Department updated successfully.";
            return RedirectToAction("index");
        }
        public bool CheckDepartmentNameAvailability(string departmentName, Guid? id)
        {
            // AJAX utility, access control might be less strict or handled by context
            return departmentRepository.IsDepartmentExists(departmentName, id);
        }
    }
}
