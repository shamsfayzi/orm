using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

            return View(await departmentRepository.GetTableAsync(model));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM_DepartmentCreate model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            await departmentRepository.AddAsync(model);
            TempData["msg-success"] = "New Department added successfully.";
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<bool> Delete(Guid id)
        {
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
            return departmentRepository.IsDepartmentExists(departmentName, id);
        }
    }
}
