using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Repositories;

namespace Operational_Risk_Management.Models.Services.Repository
{
    public class AttachmentRepository:  GenericRepository<Attachment>, IAttachmentRepository
    {
        private readonly IApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILogger<AttachmentRepository> _logger;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IFileStorageService _fileStorageService; // Added

        public AttachmentRepository(
            ApplicationDBContext context,
            IMapper mapper,
            ILogger<AttachmentRepository> logger,
            IHttpContextAccessor httpAccessor,
            ISubmissionRepository submissionRepository,
            IFileStorageService fileStorageService) : base(context) // Added
        {
            _dbcontext = context;
            _mapper = mapper;
            _logger = logger;
            _httpAccessor = httpAccessor;
            _submissionRepository = submissionRepository;
            _fileStorageService = fileStorageService; // Added
        }

        public Task<bool> DeleteAttachmentAsync(Guid attachmentId)
        {
            throw new NotImplementedException();
        }

        public Task<Attachment> GetAttachmentByIdAsync(Guid attachmentId)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, string Message, List<string> SavedFiles)> UploadSubmissionAttachmentAsync(Guid submissionId, IFormFileCollection files)
        {
            var submission = await _context.Submissions.FindAsync(submissionId);
            if (submission == null || !(submission.Status == "In Progress" || submission.Status == "Submitted"))
            {
                return (false, "Submission not found or not in a state that allows uploads.", null);
            }

            if (files == null || files.Count == 0)
            {
                return (false, "No files uploaded.", null);
            }

            var savedFiles = new List<string>();

            foreach (var file in files.Where(f => f.Length > 0))
            {
                // Use IFileStorageService to save the file
                var storedFilePath = await _fileStorageService.SaveFileAsync(file, "submissions_attachments"); // Using a dedicated folder

                if (string.IsNullOrEmpty(storedFilePath))
                {
                    _logger.LogError($"Failed to save file '{file.FileName}' to storage for submission {submissionId}.");
                    // Optionally decide if one file failing should stop all, or collect errors.
                    // For now, let's assume we continue and report overall success based on what was saved.
                    continue;
                }

                var attachment = new Attachment
                {
                    FileName = file.FileName, // Original user-friendly name
                    Size = (file.Length / 1024.0).ToString("F1") + " KB",
                    FileType = file.ContentType,
                    SubmissionId = submission.Id,
                    FilePath = storedFilePath // Path/key from IFileStorageService
                };

                await base.AddAsync(attachment);
                // Let the main context save changes, typically at end of controller action or service unit of work
                // await base.SaveChangesAsync(); // Removed to avoid multiple SaveChanges calls if this repo is part of larger UoW

                // It's unusual for a repository method to modify a related entity (submission.Attachments.Add)
                // and then call another repository to update that entity.
                // This suggests submission might need to be re-fetched or its Attachments collection reloaded
                // by the caller if this AddAsync doesn't immediately reflect in the DbContext's tracking of submission.
                // However, if submission is tracked, adding to its collection should be fine before a single SaveChanges.
                
                // The line `submission.Attachments.Add(attachment);` might not be necessary if EF Core tracks the relationship
                // via `attachment.SubmissionId`. If `submission.Attachments` is null, it needs initialization.
                if (submission.Attachments == null) submission.Attachments = new List<Attachment>();
                // EF Core should automatically associate this attachment with the submission when SaveChanges is called
                // due to the SubmissionId foreign key. Explicitly adding to collection might be for immediate in-memory state.

                savedFiles.Add(file.FileName); // Add original filename for user feedback
            }

            // Single SaveChanges at the end of the operation (e.g., in controller or service method that calls this)
            // For now, assuming this method should ensure its own additions are persisted if it's a complete unit of work.
            await _context.SaveChangesAsync(CancellationToken.None);


            // The lines below to update submission via submissionRepository seem redundant if all changes are in one DbContext.
            // If Attachments were added to the submission object and it's tracked, _context.SaveChangesAsync() covers it.
            // await submissionRepository.UpdateAsync(submission);
            // await submissionRepository.SaveChangesAsync();

            return (true, $"{savedFiles.Count} file(s) uploaded successfully.", savedFiles);
        }
    }
}
