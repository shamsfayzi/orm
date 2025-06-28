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
        public async Task<IActionResult> IndicatorDetails(Guid id)
        {
            var indicator = await _indicatorRepository.GetAsync(filter: a => a.Id == id, includeProperties: "Template,Submissions");
            return indicator  == null ?  NotFound() : View(_mapper.Map<VM_IndicatorDetails>(indicator.First()));
        }
        public async Task<IActionResult> Submissions(FilterModelForSubmissions model)
        {
            var username = User.Identity?.Name ?? "Muqtader";
            var template = await _customeTemplateRepo.GetTemplateByFocalPoint(username);
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
      
    }
}