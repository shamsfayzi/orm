using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Operational_Risk_Management.Models; // For ViewStaticState
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.View_Models.Users;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Enums;

namespace Operational_Risk_Management.Controllers
{
    //[Authorize(nameof(UserRoles.Admin))]
    public class UsersController : Controller
    {

        private readonly IApplicationUserRepository _appUserRepo;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public UsersController(IApplicationUserRepository appUserRepo,IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _appUserRepo = appUserRepo;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(PaginatedModel<ApplicationUser> model)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            return View(await _appUserRepo.GetTableAsync(model));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM_AppUserCreate model)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepository.GetAllAsync(); // Repopulate for view
                return View(model);
            }

            await _appUserRepo.AddAsync(model);
            TempData["msg-success"] = "New user added successfully.";
            return RedirectToAction("index");
        }

        public bool CheckUserNameAvailability(string userName, Guid? id)
        {
            // This is a utility endpoint, often called by JS.
            // Access control might be less strict or handled by context if JS only runs for admin.
            // For now, assuming it's fine as it doesn't modify data.
            return _appUserRepo.IsUserNameExists(userName, id);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            if (!await _appUserRepo.IsExistedByIdAsync(id, out var targetAppUser))
            {
                return NotFound();
            }
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View(_mapper.Map<VM_AppUserUpdate>(targetAppUser));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VM_AppUserUpdate model)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepository.GetAllAsync(); // Repopulate
                return View(model);
            }
            await _appUserRepo.UpdateAsync(model);
            TempData["msg-success"] = "User update successfully.";
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<bool> Delete(Guid id) // AJAX endpoint
        {
            if (!ViewStaticState.IsAdmin) return false; // Or throw for AJAX
            if (!await _appUserRepo.IsExistedByIdAsync(id))
            {
                return false;
            }
            await _appUserRepo.DeleteAsync(id);
            return true;
        }
    }

}
