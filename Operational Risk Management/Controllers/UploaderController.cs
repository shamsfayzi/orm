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
        private readonly ISubmissionCycleService _submissionCycleService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UploaderController> _logger; // Added
        public UploaderController(
            IHubContext<OverrideHub> hubContext,
            ITemplateRepository customeTemplateRepo,
            IGenericRepository<OverrideAccessRequest> overrideAcessRepo,
            IUploaderService uploaderService,
            IMapper mapper,
            IGenericRepository<Indicator> indicatorRepository,
            IGenericRepository<Submission> submissionRepository,
            IAttachmentRepository attachmentRepository,
            ISubmissionCycleService submissionCycleService,
            IFileStorageService fileStorageService,
            ILogger<UploaderController> logger) { // Added
            _uploaderService = uploaderService;
            _customeTemplateRepo = customeTemplateRepo;
            _overrideAccessRepository = overrideAcessRepo;
            _logger = logger; // Added
            _submissionCycleService = submissionCycleService;
            _fileStorageService = fileStorageService; // Added
            _mapper = mapper;
            _indicatorRepository = indicatorRepository;
            _submissionRepository = submissionRepository;
            _attachmentRepo = attachmentRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();
            var uploaderUserName = ViewStaticState.CurrentUserName;
            var uploaderDepartment = ViewStaticState.CurrentUserDepartment; // Assuming this is relevant for the service

            // Use the new service to get indicator card view models
            // The service will handle fetching templates, indicators, submissions, and calculating statuses/due dates.
            List<IndicatorCardViewModel> userIndicators = await _submissionCycleService.GetIndicatorCardViewModelsAsync(uploaderUserName, uploaderDepartment);

            var dashboard = new VM_UploaderDashboard
            {
                TemplateFocalPoint = uploaderUserName, // Or derive from service if more appropriate
                DepartmentName = uploaderDepartment, // Or derive from service
                Indicators = _mapper.Map<List<IndicatorViewModel>>(userIndicators), // Assuming direct map or similar structure
                TotalAssignedIndicators = userIndicators.Count
            };

            // Calculate counts based on the detailed info from IndicatorCardViewModel
            foreach (var indicatorVM in userIndicators)
            {
                if (indicatorVM.CurrentStatus == "Approved") dashboard.ApprovedCount++;
                else if (indicatorVM.CurrentStatus == "Submitted" || indicatorVM.CurrentStatus == "Pending") dashboard.PendingCount++;
                else if (indicatorVM.CurrentStatus == "Rejected" || indicatorVM.CurrentStatus == "Requires Revision") dashboard.RejectedCount++;

                if (indicatorVM.IsDueForSubmission) dashboard.OpenSubmissionCount++; // Or SubmissionsDueCount based on new VM
            }

            // Assuming IndicatorViewModel in VM_UploaderDashboard is similar enough to IndicatorCardViewModel
            // or that a mapping configuration exists.
            // If VM_UploaderDashboard.Indicators expects IndicatorViewModel, and service returns IndicatorCardViewModel,
            // an explicit mapping or adjustment is needed. For now, assuming _mapper can handle it or structure is compatible.
             dashboard.LastSubmitted = dashboard.Indicators
                .Where(i => i.LastSubmissionDate.HasValue)
                .OrderByDescending(i => i.LastSubmissionDate)
                .FirstOrDefault();


            return View(dashboard);
        }

        public async Task<IActionResult> Indicators()
        {
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();
            var uploaderUserName = ViewStaticState.CurrentUserName;
            var uploaderDepartment = ViewStaticState.CurrentUserDepartment; // Assuming this is relevant for the service

            // Use the new service to get indicator card view models
            List<IndicatorCardViewModel> userIndicators = await _submissionCycleService.GetIndicatorCardViewModelsAsync(uploaderUserName, uploaderDepartment);

            var model = new VM_UploaderIndicatorList { Indicators = userIndicators.OrderBy(i => i.IndicatorName).ToList() };
            return View(model);
        }

        public async Task<IActionResult> IndicatorDetails(Guid id)
        {
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();
            var indicator = await _indicatorRepository.GetAsync(filter: a => a.Id == id, includeProperties: "Template,Submissions");
            return indicator  == null ?  NotFound() : View(_mapper.Map<VM_IndicatorDetails>(indicator.First()));
        }
        public async Task<IActionResult> Submissions(FilterModelForSubmissions model)
        {
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();
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
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestOverride([FromBody] VM_CR_OverrideAccessRequest overrideAccessRequest)
        {
            if (!ViewStaticState.IsUploader) return Forbid(); // Strictly Uploader
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
            if (!ViewStaticState.IsUploader) return Forbid();
            var currentUser = ViewStaticState.CurrentUserName; // Using ViewStaticState

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
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();
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
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();
            var attachment = await _attachmentRepo.GetByIdAsync(id); // Assuming IAttachmentRepository has GetByIdAsync

            if (attachment == null)
            {
                return NotFound("Attachment not found.");
            }

            // In a real scenario, you'd use IFileStorageService to get the file bytes
            // For now, assuming FilePath is a direct wwwroot path for simplicity, which is NOT secure for production.
            // This needs to be replaced with proper file storage retrieval.
            if (string.IsNullOrEmpty(attachment.FilePath))
            {
                TempData["msg-error"] = "File path is missing for this attachment.";
                return RedirectToAction("ViewSubmission", new { id = attachment.SubmissionId });
            }

            try
            {
                var fileBytes = await _fileStorageService.ReadFileBytesAsync(attachment.FilePath);
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    TempData["msg-error"] = "File not found or is empty in storage.";
                    return RedirectToAction("ViewSubmission", new { id = attachment.SubmissionId });
                }
                return File(fileBytes, attachment.FileType ?? "application/octet-stream", attachment.FileName);
            }
            catch (Exception ex) // Catch potential exceptions from file service (e.g., file not found, access issues)
            {
                // Log the exception ex
                TempData["msg-error"] = "Error accessing file. It may have been moved or deleted from storage.";
                return RedirectToAction("ViewSubmission", new { id = attachment.SubmissionId });
            }
        }

        // GET: Uploader/ManageSubmission?indicatorId=GUID
        // GET: Uploader/ManageSubmission?submissionId=GUID
        public async Task<IActionResult> ManageSubmission(Guid? indicatorId, Guid? submissionId)
        {
            if (!ViewStaticState.IsUploader) return Forbid(); // Or appropriate access denied

            VM_ManageSubmission model = new VM_ManageSubmission();
            Submission existingSubmission = null;
            Indicator indicator = null;

            if (submissionId.HasValue)
            {
                existingSubmission = await _submissionRepository.GetAsync(
                    filter: s => s.Id == submissionId.Value,
                    includeProperties: "Indicator,Indicator.Template,Attachments"
                ).ContinueWith(t => t.Result.FirstOrDefault());

                if (existingSubmission == null) return NotFound("Submission not found.");
                indicator = existingSubmission.Indicator;
                model.SubmissionId = existingSubmission.Id;
                model.Notes = existingSubmission.Notes;
                model.Breaches = existingSubmission.Breaches;
                model.ReportingMonth = existingSubmission.ReportingMonth;
            }
            else if (indicatorId.HasValue)
            {
                indicator = await _indicatorRepository.GetAsync(
                    filter: i => i.Id == indicatorId.Value,
                    includeProperties: "Template,Submissions"
                ).ContinueWith(t => t.Result.FirstOrDefault());

                if (indicator == null) return NotFound("Indicator not found.");

                // Check if a draft for the current reporting period already exists
                var currentReportingMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                existingSubmission = indicator.Submissions
                    .FirstOrDefault(s => s.ReportingMonth.Year == currentReportingMonthStart.Year &&
                                         s.ReportingMonth.Month == currentReportingMonthStart.Month &&
                                         (s.Status == "Draft" || s.Status == "Open")); // Or whatever logic defines an editable current submission

                if (existingSubmission != null)
                {
                    // Load this existing draft/open submission
                    return RedirectToAction(nameof(ManageSubmission), new { submissionId = existingSubmission.Id });
                }
                model.ReportingMonth = currentReportingMonthStart;
            }
            else
            {
                return BadRequest("Indicator ID or Submission ID must be provided.");
            }

            if (indicator == null) return NotFound("Associated indicator could not be determined.");

            model.IndicatorId = indicator.Id;
            model.IndicatorName = indicator.IndicatorName;
            model.IndicatorDescription = indicator.Process; // Assuming process is a good description substitute
            model.KRIType = indicator.KRIType;
            model.Process = indicator.Process;
            model.RiskArea = indicator.RiskArea;
            model.FrequencyOfReview = indicator.FrequencyOfReview;

            model.ExistingAttachments = (existingSubmission?.Attachments ?? new List<Attachment>())
                .Select(a => new ExistingAttachmentViewModel
                {
                    AttachmentId = a.Id,
                    FileName = a.FileName,
                    DownloadUrl = Url.Action("DownloadAttachment", "Uploader", new { id = a.Id })
                }).ToList();

            // Use SubmissionCycleService to get current state
            var submissionState = await _submissionCycleService.GetIndicatorSubmissionStateAsync(indicator.Id, ViewStaticState.CurrentUserId.ToString());

            model.IsWindowLocked = (submissionState.CurrentPeriodStatus == "Locked"); // Or however the service defines locked
            model.HasActiveOverride = false; // Default, check for explicit override below

            if (model.SubmissionId.HasValue) // Only check for active overrides if we are editing an existing submission
            {
                var activeOverrideRequest = await _context.OverrideAccessRequests
                    .FirstOrDefaultAsync(o => o.SubmissionId == model.SubmissionId.Value &&
                                         o.RequestStatus == OverrideRequestStatus.Approved &&
                                         o.EndTime.HasValue && o.EndTime.Value > DateTime.UtcNow);
                model.HasActiveOverride = activeOverrideRequest != null;
                model.OverrideEndTime = activeOverrideRequest?.EndTime;
            }

            // model.CanEdit is a computed property in VM, it will use IsWindowLocked and HasActiveOverride.
            // If not CanEdit, redirect or show message
            if (!model.CanEdit)
            {
                 TempData["msg-info"] = $"The submission window for {model.ReportingMonth:MMMM yyyy} for '{indicator.IndicatorName}' is currently locked or not open for submission.";
                 if(model.SubmissionId.HasValue)
                 {
                    // If there's an existing submission, allow viewing it (which has override request option)
                    return RedirectToAction("ViewSubmission", new { id = model.SubmissionId.Value });
                 }
                 // If new submission and window is locked, perhaps redirect to indicators list or show a specific message page
                 return RedirectToAction(nameof(Indicators));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageSubmission(VM_ManageSubmission model)
        {
            if (!ViewStaticState.IsUploader) return Forbid();

            var indicator = await _indicatorRepository.GetByIdAsync(model.IndicatorId);
            if (indicator == null) ModelState.AddModelError("", "Associated indicator not found.");

            // Re-check window lock & override status before saving using the service
            var submissionState = await _submissionCycleService.GetIndicatorSubmissionStateAsync(model.IndicatorId, ViewStaticState.CurrentUserId.ToString());
            bool isActuallyWindowLocked = (submissionState.CurrentPeriodStatus == "Locked");
            bool hasActuallyActiveOverride = false;
            if (model.SubmissionId.HasValue)
            {
                 var activeOverrideRequest = await _context.OverrideAccessRequests
                    .FirstOrDefaultAsync(o => o.SubmissionId == model.SubmissionId.Value &&
                                         o.RequestStatus == OverrideRequestStatus.Approved &&
                                         o.EndTime.HasValue && o.EndTime.Value > DateTime.UtcNow);
                hasActuallyActiveOverride = activeOverrideRequest != null;
            }

            if(isActuallyWindowLocked && !hasActuallyActiveOverride)
            {
                ModelState.AddModelError("", "Submission window is locked, and no active override found. Cannot save changes.");
            }

            if (!ModelState.IsValid)
            {
                // Repopulate necessary fields for the view if returning due to error
                if(indicator != null) {
                    model.IndicatorName = indicator.IndicatorName;
                    model.IndicatorDescription = indicator.Process;
                    model.KRIType = indicator.KRIType;
                    model.Process = indicator.Process;
                    model.RiskArea = indicator.RiskArea;
                    model.FrequencyOfReview = indicator.FrequencyOfReview;
                }
                 // Need to repopulate existing attachments if they were part of the GET
                if (model.SubmissionId.HasValue) {
                    var existingSubmission = await _submissionRepository.GetAsync(filter: s => s.Id == model.SubmissionId.Value, includeProperties: "Attachments").ContinueWith(t => t.Result.FirstOrDefault());
                    model.ExistingAttachments = (existingSubmission?.Attachments ?? new List<Attachment>())
                        .Select(a => new ExistingAttachmentViewModel { AttachmentId = a.Id, FileName = a.FileName, DownloadUrl = Url.Action("DownloadAttachment", "Uploader", new {id = a.Id})}).ToList();
                } else {
                    model.ExistingAttachments = new List<ExistingAttachmentViewModel>();
                }
                model.IsWindowLocked = isWindowLocked;
                model.HasActiveOverride = hasActiveOverride;
                model.OverrideEndTime = activeOverride?.EndTime;
                return View(model);
            }

            Submission submission;
            if (model.SubmissionId.HasValue && model.SubmissionId != Guid.Empty) // Editing existing
            {
                submission = await _submissionRepository.GetByIdAsync(model.SubmissionId.Value);
                if (submission == null) return NotFound("Submission to update not found.");

                submission.Notes = model.Notes;
                submission.Breaches = model.Breaches;
                submission.ReportingMonth = new DateTime(model.ReportingMonth.Year, model.ReportingMonth.Month, 1);
                submission.Status = "Submitted"; // Or "Draft" if saving as draft
                // UpdateBy and UpdatedDate are handled by BaseEntity logic in DbContext
                await _submissionRepository.UpdateAsync(submission);
            }
            else // Creating new
            {
                submission = new Submission
                {
                    IndicatorId = model.IndicatorId,
                    ReportingMonth = new DateTime(model.ReportingMonth.Year, model.ReportingMonth.Month, 1),
                    Notes = model.Notes,
                    Breaches = model.Breaches,
                    Status = "Submitted", // Or "Draft"
                    // CreateBy, CreateDate, etc., handled by BaseEntity logic
                    WindowLocked = isWindowLocked, // Initial lock status based on current check
                    WindowOverride = hasActiveOverride
                };
                if(hasActiveOverride && activeOverride != null) {
                    submission.WindowOverrideBy = activeOverride.ApprovedBy;
                    submission.WindowOverrideDate = activeOverride.ApprovedDate;
                }
                await _submissionRepository.AddAsync(submission);
            }

            // Handle attachments to delete (if VM_ManageSubmission is updated to include this)
            if (model.ExistingAttachments != null)
            {
                foreach (var attVM in model.ExistingAttachments.Where(a => a.IsMarkedForDeletion))
                {
                    // TODO: Add IFileStorageService.DeleteFileAsync for physical file
                    await _attachmentRepo.DeleteAsync(attVM.AttachmentId);
                }
            }

            // Handle new attachments
            if (model.NewAttachments != null && model.NewAttachments.Any())
            {
                foreach (var file in model.NewAttachments)
                {
                    if (file.Length > 0)
                    {
                        // Ideal usage with IFileStorageService:
                        var storedFilePath = await _fileStorageService.SaveFileAsync(file, "submissions");
                        // SaveFileAsync should return a unique path/key for the stored file.

                        if (!string.IsNullOrEmpty(storedFilePath))
                        {
                            var attachment = new Attachment
                            {
                                SubmissionId = submission.Id,
                                FileName = file.FileName, // Original user-friendly name
                                FileType = file.ContentType,
                                Size = (file.Length / 1024.0).ToString("F1") + " KB",
                                FilePath = storedFilePath // Path/key from IFileStorageService
                            };
                            await _attachmentRepo.AddAsync(attachment);
                        }
                        else
                        {
                            // Log failure to save file via service
                            _logger.LogError($"Failed to save attachment '{file.FileName}' for submission {submission.Id} using IFileStorageService.");
                            // Optionally add a model error or TempData message
                        }
                    }
                }
            }

            await _context.SaveChangesAsync(CancellationToken.None); // Use main context save changes

            TempData["msg-success"] = $"Submission for '{indicator.IndicatorName}' ({model.ReportingMonth:MMMM yyyy}) saved successfully.";
            return RedirectToAction("ViewSubmission", new { id = submission.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttachment(Guid id)
        {
            // Uploader can delete if submission window is open/override active. Admin might have broader delete.
            // For now, basic check. More granular permission would be better.
            if (!ViewStaticState.IsUploader && !ViewStaticState.IsAdmin) return Forbid();


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

            bool fileDeletedFromStorage = false;
            if (!string.IsNullOrEmpty(attachment.FilePath))
            {
                try
                {
                    fileDeletedFromStorage = await _fileStorageService.DeleteFileAsync(attachment.FilePath);
                    if (!fileDeletedFromStorage)
                    {
                        // Log this failure, but proceed to delete DB record as file might be orphaned.
                        _logger.LogWarning($"Failed to delete file from storage: {attachment.FilePath} for attachment ID {id}. It might be orphaned.");
                    }
                }
                catch (Exception ex)
                {
                    // Log this exception ex
                     _logger.LogError(ex, $"Error deleting file from storage: {attachment.FilePath} for attachment ID {id}.");
                    // Depending on policy, you might choose to not delete the DB record if file deletion fails critically.
                    // For now, proceeding to delete DB record.
                }
            }
            else
            {
                _logger.LogWarning($"Attachment ID {id} had no FilePath. Cannot delete from storage.");
            }

            await _attachmentRepo.DeleteAsync(id); // Assuming hard delete for attachment record
            // _context.SaveChanges() should be called at a higher level or after all operations in a unit of work.
            // If _attachmentRepo.DeleteAsync doesn't call SaveChanges, it should be called here or after.
            // For consistency with ManageSubmission, using _context.SaveChangesAsync().
            await _context.SaveChangesAsync(CancellationToken.None);


            TempData["msg-success"] = "Attachment deleted successfully.";
            return RedirectToAction("ViewSubmission", new { id = attachment.SubmissionId });
        }
      
    }
}