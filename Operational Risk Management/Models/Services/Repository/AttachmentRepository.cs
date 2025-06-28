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
        private readonly ISubmissionRepository submissionRepository;
        public AttachmentRepository(ApplicationDBContext context, IMapper mapper, ILogger<AttachmentRepository> logger, IHttpContextAccessor httpAccessor, ISubmissionRepository submissionRepository) : base(context)
        {
            _dbcontext = context;
            _mapper = mapper;
            _logger = logger;
            _httpAccessor = httpAccessor;
            this.submissionRepository = submissionRepository;   
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
                var fileName = await FileExtensions.SaveToAsync(file, "attachments");

                var attachment = new Attachment
                {
                    FileName = fileName,
                    Size = file.Length.ToString(),
                    FileType = file.ContentType,
                    SubmissionId = submission.Id
                };

                await base.AddAsync(attachment);
                await base.SaveChangesAsync();
                
                submission.Attachments.Add(attachment); // Make sure Attachments is a modifiable list
                savedFiles.Add(fileName);
            }

             await submissionRepository.UpdateAsync(submission);
            await submissionRepository.SaveChangesAsync();

            return (true, $"{savedFiles.Count} file(s) uploaded successfully.", savedFiles);
        }
    }
}
