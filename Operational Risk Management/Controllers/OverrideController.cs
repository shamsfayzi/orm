using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Operational_Risk_Management.Models;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.View_Models.Override; // Will create this ViewModel
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // For Include/ThenInclude

namespace Operational_Risk_Management.Controllers
{
    // Conceptual Authorization: [Authorize(Roles = "Admin,RiskManager")]
    public class OverrideController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<OverrideHub> _hubContext;

        public OverrideController(IApplicationDbContext context, IMapper mapper, IHubContext<OverrideHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        // GET: Override
        public async Task<IActionResult> Index()
        {
            // For ViewStaticState testing, ensure an Admin/RiskManager is simulated
            // if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager)
            // {
            //     return Forbid(); // Or RedirectToAction("AccessDenied", "Error");
            // }

            var pendingRequests = await _context.OverrideAccessRequests
                .Include(r => r.Submission)
                    .ThenInclude(s => s.Indicator)
                        .ThenInclude(i => i.Template)
                            .ThenInclude(t => t.Department)
                .Where(r => r.RequestStatus == OverrideRequestStatus.Pending)
                .OrderByDescending(r => r.CreateDate)
                .ToListAsync();

            // Manual mapping for now, or create a specific ViewModel if complex
            var viewModelList = pendingRequests.Select(r => new PendingOverrideRequestViewModel
            {
                RequestId = r.Id,
                SubmissionId = r.SubmissionId,
                IndicatorName = r.Submission?.Indicator?.IndicatorName,
                DepartmentName = r.Submission?.Indicator?.Template?.Department?.DepartmentName,
                RequestedByFullName = r.CreatedByFullName, // Assuming BaseEntity has this
                RequestDate = r.CreateDate,
                Reason = r.Reason
            }).ToList();

            return View(viewModelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Conceptual Authorization: [Authorize(Roles = "Admin,RiskManager")]
        public async Task<IActionResult> Approve(Guid requestId)
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();

            var request = await _context.OverrideAccessRequests.Include(r => r.Submission).FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null || request.RequestStatus != OverrideRequestStatus.Pending)
            {
                TempData["msg-error"] = "Request not found or already processed.";
                return RedirectToAction(nameof(Index));
            }

            // Logic for setting StartTime and EndTime for the override
            // For simplicity, let's make it 1 hour from approval. This can be made configurable.
            var overrideStartTime = DateTime.UtcNow;
            var overrideEndTime = overrideStartTime.AddHours(1);

            request.Approve(ViewStaticState.CurrentUserName, overrideStartTime, overrideEndTime); // Assuming CurrentUserName from ViewStaticState is admin/manager

            if (request.Submission != null)
            {
                request.Submission.WindowLocked = false; // Unlock the window
                request.Submission.WindowOverride = true;
                request.Submission.WindowOverrideBy = ViewStaticState.CurrentUserName;
                request.Submission.WindowOverrideDate = DateTime.UtcNow;
                 // Optionally set a specific reason or append to existing
                request.Submission.WindowLockReason = $"Override approved by {ViewStaticState.CurrentUserName}. Valid until {overrideEndTime:g}. Original reason: {request.Submission.WindowLockReason}";
            }

            await _context.SaveChangesAsync(CancellationToken.None);

            // Notify the specific user who made the request
            if (!string.IsNullOrEmpty(request.CreateBy)) // Assuming CreateBy stores the requester's identifier (e.g., username)
            {
                 // The client-side needs to listen for an event targeted to them or a general event they can filter
                await _hubContext.Clients.User(request.CreateBy).SendAsync("OverrideStatusChanged", new
                {
                    submissionId = request.SubmissionId,
                    isApproved = true,
                    message = $"Your override request for submission ID {request.SubmissionId} has been approved. You can now edit until {overrideEndTime:g}.",
                    newEndTime = overrideEndTime
                });
                 // Fallback or additional notification to a group if direct user targeting is complex
                 await _hubContext.Clients.Group(request.SubmissionId.ToString()).SendAsync("OverrideStatusChanged", new { submissionId = request.SubmissionId, isApproved = true, message = "Override Approved" });

            }

            TempData["msg-success"] = "Override request approved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Conceptual Authorization: [Authorize(Roles = "Admin,RiskManager")]
        public async Task<IActionResult> Reject(Guid requestId)
        {
            if (!ViewStaticState.IsAdmin && !ViewStaticState.IsRiskManager) return Forbid();

            var request = await _context.OverrideAccessRequests.FindAsync(requestId);
            if (request == null || request.RequestStatus != OverrideRequestStatus.Pending)
            {
                TempData["msg-error"] = "Request not found or already processed.";
                return RedirectToAction(nameof(Index));
            }

            request.Reject(); // Using the method defined in the entity
            // request.RequestStatus = OverrideRequestStatus.Rejected; // Alternative if no method
            // request.UpdateBy = ViewStaticState.CurrentUserName; // Assuming BaseEntity handles this
            // request.UpdatedDate = DateTime.UtcNow;


            await _context.SaveChangesAsync(CancellationToken.None);

            // Notify the specific user
            if (!string.IsNullOrEmpty(request.CreateBy))
            {
                await _hubContext.Clients.User(request.CreateBy).SendAsync("OverrideStatusChanged", new
                {
                    submissionId = request.SubmissionId,
                    isApproved = false,
                    message = $"Your override request for submission ID {request.SubmissionId} has been rejected."
                });
                await _hubContext.Clients.Group(request.SubmissionId.ToString()).SendAsync("OverrideStatusChanged", new { submissionId = request.SubmissionId, isApproved = false, message = "Override Rejected" });
            }

            TempData["msg-success"] = "Override request rejected successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
