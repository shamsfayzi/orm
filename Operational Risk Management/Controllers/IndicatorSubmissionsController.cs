using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Repositories;

namespace Operational_Risk_Management.Controllers
{
    public class IndicatorSubmissionsController : Controller
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IGenericRepository<Feedback> _feedbackGenericRepo;
        public IndicatorSubmissionsController(ISubmissionRepository submissionRepository, IGenericRepository<Feedback> feedbackGenericRepo)
        {
            _submissionRepository = submissionRepository;
            _feedbackGenericRepo = feedbackGenericRepo;
        }
        public async Task<IActionResult> Details(Guid Id)
        {
            return View(await _submissionRepository.SubmissionDetails(Id));
             
        }
        public async Task<IActionResult> SubmitFeedback(string content , Guid Id)
        {
            var feedback = new Feedback
            {
                SubmissionId = Id,
                FeedbackContent = content,
                CreateDate = DateTime.UtcNow,
                CreateBy = User.Identity.Name,
            };
            await _feedbackGenericRepo.AddAsync(feedback);
            await _feedbackGenericRepo.SaveChangesAsync();
            TempData["msg-success"] = "Feedback submitted successfully.";
            return RedirectToAction("Details", new { id = Id });
        }
    }
}
    