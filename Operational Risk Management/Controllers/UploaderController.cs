using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.View_Models.Uploader;
using AutoMapper;
using Operational_Risk_Management.Models.View_Models.Uploader.Indicator;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.SignalR;
using Operational_Risk_Management.Models;
using Operational_Risk_Management.Models.Interfaces.Services;


namespace Operational_Risk_Management.Controllers
{
    // [Authorize(Roles = "Uploader")] 
    public class UploaderController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUploaderService _uploaderService;
        private readonly IGenericRepository<OverrideAccessRequest> _overrideAccessRepository;
        private readonly IHubContext<OverrideHub> _hubContext;
        private readonly ITemplateRepository _customeTemplateRepo;
        private readonly IGenericRepository<Indicator> _indicatorRepository;
        private readonly IGenericRepository<Submission> _submissionRepository;
        private readonly IAttachmentRepository _attachmentRepo;
        public UploaderController(
            IHubContext<OverrideHub> hubContext,
            ITemplateRepository customeTemplateRepo,
            IGenericRepository<OverrideAccessRequest> overrideAcessRepo,
            IUploaderService uploaderService,
        IMapper mapper,
            IGenericRepository<Indicator> indicatorRepository,
            IGenericRepository<Submission> submissionRepository,
            IAttachmentRepository attachmentRepository) {
            _uploaderService = uploaderService;
            _customeTemplateRepo = customeTemplateRepo;
            _overrideAccessRepository = overrideAcessRepo;
            _mapper = mapper;
            _indicatorRepository = indicatorRepository;
            _submissionRepository = submissionRepository;
            _attachmentRepo = attachmentRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var uploaderUserName = ViewStaticState.CurrentUserName;
            // In a real app, this might also use department context from ViewStaticState.CurrentUserDepartment
            // For now, focusing on FocalPoint
            var templates = await _customeTemplateRepo.GetAsync(
                filter: t => t.FocalPoint == uploaderUserName && !t.IsDeleted,
                includeProperties: "Department,Indicators,Indicators.Submissions"
            );

            var dashboard = new VM_UploaderDashboard
            {
                TemplateFocalPoint = uploaderUserName,
                DepartmentName = templates.FirstOrDefault()?.Department?.DepartmentName ?? "N/A", // Assuming one department per focal point for simplicity
                Indicators = new List<IndicatorViewModel>()
            };

            if (!templates.Any())
            {
                return View(dashboard); // Return empty dashboard if no templates found
            }

            List<IndicatorViewModel> allIndicatorViewModels = new List<IndicatorViewModel>();

            foreach (var template in templates)
            {
                foreach (var indicator in template.Indicators.Where(i => !i.IsDeleted && i.IsActive))
                {
                    var lastSubmission = indicator.Submissions
                                        .OrderByDescending(s => s.ReportingMonth)
                                        .FirstOrDefault();

                    // Simplified DueDate & CanSubmitNewReport logic
                    // This needs to be robustly implemented, likely in a service, considering SubmissionWindows, frequency, etc.
                    DateTime? dueDate = null; // Placeholder
                    bool canSubmitNewReport = true; // Placeholder: default to true, logic would check current period submission.
                    string lastStatus = lastSubmission?.Status ?? "Not Submitted";

                    // Example for DueDate (very basic)
                    if (indicator.FrequencyOfReview?.ToLower() == "monthly")
                    {
                        dueDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddDays(-1); // End of current month
                        if (lastSubmission != null && lastSubmission.ReportingMonth.Year == DateTime.UtcNow.Year && lastSubmission.ReportingMonth.Month == DateTime.UtcNow.Month)
                        {
                            // Already submitted for current month
                           // canSubmitNewReport = (lastSubmission.Status == "Draft"); // Can edit draft
                        }
                    }
                    // Add more frequency logic (Quarterly, Annually) for DueDate

                    var indicatorVM = new IndicatorViewModel
                    {
                        IndicatorId = indicator.Id,
                        IndicatorName = indicator.IndicatorName,
                        KRIType = indicator.KRIType,
                        RfNo = indicator.RfNo,
                        Process = indicator.Process,
                        RiskArea = indicator.RiskArea,
                        FrequencyOfReview = indicator.FrequencyOfReview,
                        TolerableBreaches = indicator.TolerableBreaches,
                        LastSubmissionDate = lastSubmission?.CreateDate,
                        LastSubmissionStatus = lastStatus,
                        CanSubmitNewReport = canSubmitNewReport, // Needs real logic
                        LastSubmissionId = lastSubmission?.Id,
                        DueDate = dueDate, // Needs real logic
                        Submissions = _mapper.Map<List<SubmissionDetailViewModel>>(indicator.Submissions?.ToList() ?? new List<Submission>())
                    };
                    allIndicatorViewModels.Add(indicatorVM);

                    if (lastSubmission != null)
                    {
                        if (lastSubmission.Status == "Approved") dashboard.ApprovedCount++;
                        else if (lastSubmission.Status == "Submitted" || lastSubmission.Status == "Pending") dashboard.PendingCount++;
                        else if (lastSubmission.Status == "Rejected" || lastSubmission.Status == "Requires Revision") dashboard.RejectedCount++;
                    }
                    if(canSubmitNewReport) dashboard.OpenSubmissionCount++;
                }
            }
            dashboard.Indicators = allIndicatorViewModels.OrderBy(i => i.IndicatorName).ToList();
            dashboard.LastSubmitted = allIndicatorViewModels.Where(i=>i.LastSubmissionDate.HasValue).OrderByDescending(i => i.LastSubmissionDate).FirstOrDefault();

            return View(dashboard);
        }

        public async Task<IActionResult> Indicators()
        {
            var uploaderUserName = ViewStaticState.CurrentUserName;
            // In a real app, get this from User.Identity.Name or a claims helper

            var templates = await _customeTemplateRepo.GetAsync(
                filter: t => t.FocalPoint == uploaderUserName && !t.IsDeleted,
                includeProperties: "Department,Indicators,Indicators.Submissions"
            );

            var indicatorViewModels = new List<IndicatorCardViewModel>();

            foreach (var template in templates)
            {
                foreach (var indicator in template.Indicators.Where(i => !i.IsDeleted && i.IsActive))
                {
                    var lastSubmission = indicator.Submissions
                                        .OrderByDescending(s => s.ReportingMonth)
                                        .FirstOrDefault();

                    // Simplified due date & status logic - this needs proper business rules
                    DateTime? nextDueDate = null; // Placeholder - requires complex calculation based on frequency and last submission
                    string currentStatus = lastSubmission?.Status ?? "Not Submitted";
                    bool isDue = true; // Placeholder

                    if (lastSubmission != null) {
                        // Example: If monthly, and last submission was for last month, next is this month.
                        // This is highly simplified.
                        if(indicator.FrequencyOfReview.ToLower() == "monthly")
                        {
                            var reportingMonthForNext = lastSubmission.ReportingMonth.AddMonths(1);
                            if(reportingMonthForNext.Year > DateTime.UtcNow.Year || (reportingMonthForNext.Year == DateTime.UtcNow.Year && reportingMonthForNext.Month > DateTime.UtcNow.Month))
                            {
                                // If the next theoretical submission is for a future month, it's not "due" yet in a simple sense.
                                // isDue = false; // This logic needs to be much more robust.
                            }
                            // A more robust 'nextDueDate' would consider SubmissionWindows configuration.
                        }
                    } else {
                        // No submission yet, might be due based on indicator creation date and frequency.
                    }


                    indicatorViewModels.Add(new IndicatorCardViewModel
                    {
                        IndicatorId = indicator.Id,
                        IndicatorName = indicator.IndicatorName,
                        KRIType = indicator.KRIType,
                        RiskArea = indicator.RiskArea,
                        Process = indicator.Process,
                        FrequencyOfReview = indicator.FrequencyOfReview,
                        DepartmentName = template.Department?.DepartmentName,
                        TemplateFocalPoint = template.FocalPoint,
                        LastSubmissionDate = lastSubmission?.CreateDate,
                        NextDueDate = nextDueDate, // Placeholder
                        CurrentStatus = currentStatus,
                        IsDueForSubmission = isDue, // Placeholder
                        LastSubmissionId = lastSubmission?.Id ?? Guid.Empty
                    });
                }
            }

            var model = new VM_UploaderIndicatorList { Indicators = indicatorViewModels.OrderBy(i => i.IndicatorName).ToList() };
            return View(model);
        }

        public async Task<IActionResult> IndicatorDetails(Guid id)
        {
            var indicator = await _indicatorRepository.GetAsync(filter: a => a.Id == id, includeProperties: "Template,Submissions");
            return indicator  == null ?  NotFound() : View(_mapper.Map<VM_IndicatorDetails>(indicator.First()));
        }
        public async Task<IActionResult> Submissions(FilterModelForSubmissions model)
        {
            var username = ViewStaticState.CurrentUserName; // Using ViewStaticState
            var template = await _customeTemplateRepo.GetTemplateByFocalPoint(username);

            if (template == null)
            {
                // If no template found for this focal point, return empty list or handle appropriately
                return View((new List<VM_Submission>(), model));
            }

            model.TemplateId = template.Id;
            var submissions = await _uploaderService.SubmissionVM(model);
            return View((submissions, model));
        }

        [HttpGet]
        public async Task<IActionResult> ViewSubmission(Guid id)
        {
            var submissionFromDb = await _submissionRepository.GetAsync(
                filter: a => a.Id == id,
                includeProperties: "Indicator,Attachments,Indicator.Template,Indicator.Template.Department,Feedbacks");

            var submission = submissionFromDb.FirstOrDefault();
            if (submission == null)
                return NotFound();

            return View(new Update_DetailVM()
            {
                Submission = _mapper.Map<SubmissionDetailViewModel>(submission),
                UpdateModel = new VM_SubmissionUpdate()
                {
                    Id = submission.Id,
                    Breaches = submission.Breaches,
                    Notes = submission.Notes
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSubmission(VM_SubmissionUpdate model)
        {
            if (ModelState.IsValid && await _submissionRepository.IsExistedByIdAsync(model.Id, out var submission))
            {
                submission.Notes = model.Notes;
                submission.Breaches = model.Breaches;
                await _submissionRepository.UpdateAsync(submission);

                TempData["msg-success"] = "Submission updated successfully.";
                return RedirectToAction("ViewSubmission", new { id = model.Id });
            }

            TempData["msg-error"] = "Invalid submission data or submission not found.";
            return RedirectToAction("ViewSubmission", new { id = model.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Added for AJAX, client must send token
        public async Task<IActionResult> UploadAttachments(IFormCollection form)
        {
            if (!Guid.TryParse(form["submissionId"], out var submissionId))
            {
                return BadRequest(new { success = false, message = "Invalid or missing submissionId." });
            }

            var (success, message, savedFiles) = await _attachmentRepo.UploadSubmissionAttachmentAsync(submissionId, form.Files);

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message, files = savedFiles });
        
        }
        [HttpPost] // Assuming this should be POST, and thus needs AntiForgeryToken
        [ValidateAntiForgeryToken] // Added for AJAX, client must send token
        public async Task<IActionResult> RequestOverride([FromBody] VM_CR_OverrideAccessRequest overrideAccessRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }
            if (await _submissionRepository.IsExistedByIdAsync(overrideAccessRequest.SubmissionId, out var submissoin) && submissoin.WindowLocked)
            {
                overrideAccessRequest.CreatedByFullName = User.Identity.Name ?? "Muqtader";
                var model = _mapper.Map<OverrideAccessRequest>(overrideAccessRequest);
                await  _overrideAccessRepository.AddAsync(model);
                await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOverrideRequest", model);
                return Ok(new { success = true, message = "Request Sent please. once approved you can submit." });
            }
            return BadRequest(new { success = false, message = "Invalid request data." });
        }
        public async Task<IActionResult> CheckOverrideStatus(string submissionId)
        {
            var currentUser = User.Identity.Name;

            var now = DateTime.UtcNow;
            var Id = Guid.TryParse(submissionId, out var guid) ? guid : Guid.Empty;
            var query = await _overrideAccessRepository.GetAsync(filter:a => a.SubmissionId == Id&& a.CreatedByFullName == currentUser);
            var request =  query.OrderBy(a => a.CreateDate).FirstOrDefault();

            bool isPending = request.RequestStatus == OverrideRequestStatus.Pending && (request.EndTime == null || request.EndTime > now);
            bool hasAccess = request.RequestStatus == OverrideRequestStatus.Approved && request.EndTime > now;

            return Ok(new
            {
                isPending,
                hasAccess
            });
        }

        public async Task<IActionResult> GetFeedbacks(string SubmissionId)
        {
            var guid = Guid.Parse(SubmissionId);
            var submissionFromDb = await _submissionRepository.GetAsync(filter: a => a.Id == guid, includeProperties: "Feedbacks");
            var submission = submissionFromDb.FirstOrDefault();
            if (submission == null)
            {
                return RedirectToAction("Index");
            }
            else {
                return Json(submission.Feedbacks);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(Guid id)
        {
            var attachment = await _attachmentRepo.GetByIdAsync(id); // Assuming IAttachmentRepository has GetByIdAsync

            if (attachment == null)
            {
                return NotFound("Attachment not found.");
            }

            // In a real scenario, you'd use IFileStorageService to get the file bytes
            // For now, assuming FilePath is a direct wwwroot path for simplicity, which is NOT secure for production.
            // This needs to be replaced with proper file storage retrieval.
            // Example with IFileStorageService (conceptual - if FilePath is an abstract path):
            // var fileBytes = await _fileStorageService.ReadFileBytesAsync(attachment.FilePath);
            // if (fileBytes == null)
            // {
            //     return NotFound("File not found in storage.");
            // }
            // return File(fileBytes, attachment.FileType ?? "application/octet-stream", attachment.FileName);

            // TEMPORARY Placeholder for direct wwwroot file access (INSECURE - REPLACE)
            // This assumes attachment.FilePath is like "~/uploads/submissions/file.pdf"
            // And that the FileStorageService has stored them in wwwroot/uploads/submissions
            if (string.IsNullOrEmpty(attachment.FilePath) || !attachment.FilePath.StartsWith("wwwroot/"))
            {
                 //This indicates FilePath is not a direct wwwroot path as assumed by this placeholder
                TempData["msg-error"] = "File path is invalid or not accessible directly.";
                // Redirect back to submission view or an error page
                // Need submissionId to redirect properly, which is not directly available here.
                // This part highlights the need for a proper FileStorageService.
                if (attachment.SubmissionId != Guid.Empty)
                {
                    return RedirectToAction("ViewSubmission", new { id = attachment.SubmissionId });
                }
                return RedirectToAction("Index"); // Fallback
            }

            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), attachment.FilePath);
            if (!System.IO.File.Exists(physicalPath))
            {
                TempData["msg-error"] = "File not found on server.";
                 if (attachment.SubmissionId != Guid.Empty)
                {
                    return RedirectToAction("ViewSubmission", new { id = attachment.SubmissionId });
                }
                return RedirectToAction("Index");
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
            return File(fileBytes, attachment.FileType ?? "application/octet-stream", attachment.FileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice, ensure form in ViewSubmission.cshtml has this for the delete action.
        public async Task<IActionResult> DeleteAttachment(Guid id)
        {
            // In a real app, ensure user is authorized to delete this attachment (e.g., owns the submission or is admin)
            // And that the submission is not in a state that prevents modification.

            var attachment = await _attachmentRepo.GetByIdAsync(id);
            if (attachment == null)
            {
                TempData["msg-error"] = "Attachment not found.";
                // Try to redirect back to the submission if possible, otherwise to a general page.
                // This requires knowing the submissionId. If not available, a more generic redirect is needed.
                // For now, assuming we might not have submissionId directly here without another query.
                return RedirectToAction("Index"); // Fallback
            }

            var submission = await _submissionRepository.GetByIdAsync(attachment.SubmissionId);
            if (submission == null)
            {
                TempData["msg-error"] = "Associated submission not found.";
                return RedirectToAction("Index");
            }

            // Prevent deletion if submission window is locked and no override is active
            // This check might need to be more sophisticated based on override logic.
            if (submission.WindowLocked && !(submission.WindowOverride == true))
            {
                TempData["msg-error"] = "Cannot delete attachments for a locked submission without an active override.";
                return RedirectToAction("ViewSubmission", new { id = submission.Id });
            }

            // TODO: Use IFileStorageService to delete the actual file from storage
            // await _fileStorageService.DeleteFileAsync(attachment.FilePath);

            // For now, just deleting the record. Physical file deletion needs IFileStorageService.
            await _attachmentRepo.DeleteAsync(id); // Assuming hard delete for attachment record
            await _attachmentRepo.SaveChangesAsync(); // If IAttachmentRepository doesn't save changes itself

            TempData["msg-success"] = "Attachment deleted successfully.";
            return RedirectToAction("ViewSubmission", new { id = attachment.SubmissionId });
        }
      
    }
}