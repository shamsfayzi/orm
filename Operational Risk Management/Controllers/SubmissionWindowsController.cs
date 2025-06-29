using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models; // For ViewStaticState
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.View_Models.SubmissionWindows; // Will create this
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // For List

namespace Operational_Risk_Management.Controllers
{
    // Conceptual Authorization: [Authorize(Roles = "Admin")]
    public class SubmissionWindowsController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SubmissionWindowsController(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: SubmissionWindows
        public async Task<IActionResult> Index()
        {
            if (!ViewStaticState.IsAdmin)
            {
                // In a real app, might redirect to an AccessDenied page
                return Content("Access Denied. Only Admins can manage Submission Windows.");
            }
            var windows = await _context.SubmissionWindows.OrderBy(sw => sw.Frequency).ToListAsync();
            // For now, directly passing entities. Could map to a list ViewModel if needed for display.
            return View(windows);
        }

        // GET: SubmissionWindows/Create
        public IActionResult Create()
        {
            if (!ViewStaticState.IsAdmin) return Content("Access Denied.");
            var model = new VM_SubmissionWindow();
            // You might want to pre-populate or provide SelectLists for Frequency if it's an enum or fixed set
            // ViewBag.Frequencies = new SelectList(new[] { "Monthly", "Quarterly", "Annually" });
            return View(model);
        }

        // POST: SubmissionWindows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM_SubmissionWindow vmWindow)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();

            if (ModelState.IsValid)
            {
                // Check for duplicate frequency
                if (await _context.SubmissionWindows.AnyAsync(sw => sw.Frequency == vmWindow.Frequency && !sw.IsDeleted)) // Assuming ISoftDelete
                {
                    ModelState.AddModelError("Frequency", "A submission window for this frequency already exists.");
                    // ViewBag.Frequencies = new SelectList(new[] { "Monthly", "Quarterly", "Annually" });
                    return View(vmWindow);
                }

                var submissionWindow = _mapper.Map<SubmissionWindows>(vmWindow);
                // Audit fields like CreateDate, CreateBy will be set by ApplicationDBContext SaveChanges override
                _context.SubmissionWindows.Add(submissionWindow);
                await _context.SaveChangesAsync(CancellationToken.None);
                TempData["msg-success"] = "Submission window created successfully.";
                return RedirectToAction(nameof(Index));
            }
            // ViewBag.Frequencies = new SelectList(new[] { "Monthly", "Quarterly", "Annually" });
            return View(vmWindow);
        }

        // GET: SubmissionWindows/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!ViewStaticState.IsAdmin) return Content("Access Denied.");

            var submissionWindow = await _context.SubmissionWindows.FindAsync(id);
            if (submissionWindow == null || submissionWindow.IsDeleted) // Assuming ISoftDelete
            {
                return NotFound();
            }
            var vmWindow = _mapper.Map<VM_SubmissionWindow>(submissionWindow);
            // ViewBag.Frequencies = new SelectList(new[] { "Monthly", "Quarterly", "Annually" });
            return View(vmWindow);
        }

        // POST: SubmissionWindows/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VM_SubmissionWindow vmWindow)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();
            if (id != vmWindow.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Check for duplicate frequency if the frequency is being changed
                var originalWindow = await _context.SubmissionWindows.AsNoTracking().FirstOrDefaultAsync(sw => sw.Id == id);
                if (originalWindow.Frequency != vmWindow.Frequency)
                {
                    if (await _context.SubmissionWindows.AnyAsync(sw => sw.Frequency == vmWindow.Frequency && sw.Id != id && !sw.IsDeleted))
                    {
                        ModelState.AddModelError("Frequency", "A submission window for this frequency already exists.");
                        // ViewBag.Frequencies = new SelectList(new[] { "Monthly", "Quarterly", "Annually" });
                        return View(vmWindow);
                    }
                }

                try
                {
                    var submissionWindow = await _context.SubmissionWindows.FindAsync(id);
                    if (submissionWindow == null || submissionWindow.IsDeleted) return NotFound();

                    _mapper.Map(vmWindow, submissionWindow);
                    // Audit fields like UpdatedDate, UpdateBy will be set by ApplicationDBContext SaveChanges override
                    await _context.SaveChangesAsync(CancellationToken.None);
                    TempData["msg-success"] = "Submission window updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.SubmissionWindows.AnyAsync(e => e.Id == id && !e.IsDeleted))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // ViewBag.Frequencies = new SelectList(new[] { "Monthly", "Quarterly", "Annually" });
            return View(vmWindow);
        }

        // POST: SubmissionWindows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (!ViewStaticState.IsAdmin) return Forbid();

            var submissionWindow = await _context.SubmissionWindows.FindAsync(id);
            if (submissionWindow == null)
            {
                TempData["msg-error"] = "Submission window not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if this window is linked to any indicators (via frequency on indicator)
            // This is a conceptual check; direct FK might not exist from SubmissionWindow to Indicator.
            // Instead, indicators store frequency as a string.
            // A more robust check would see if any Indicator.FrequencyOfReview matches submissionWindow.Frequency.
            // For simplicity now, we'll allow deletion. Consider impact on SPs that use SubmissionWindows.

            _context.SubmissionWindows.Remove(submissionWindow);
            await _context.SaveChangesAsync(CancellationToken.None);
            TempData["msg-success"] = "Submission window deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
