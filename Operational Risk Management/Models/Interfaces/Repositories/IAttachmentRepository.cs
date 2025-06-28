
using Operational_Risk_Management.Models.Entities;

namespace Operational_Risk_Management.Models.Interfaces.Repositories
{
    public interface IAttachmentRepository : IGenericRepository<Attachment>
    {
        Task<(bool Success, string Message, List<string> SavedFiles)> UploadSubmissionAttachmentAsync(Guid submissionId, IFormFileCollection files);
        //Task<PaginatedModel<Attachment>> GetAttachmentsBySubmissionIdAsync(Guid submissionId, PaginatedModel<Attachment> model);
        Task<bool> DeleteAttachmentAsync(Guid attachmentId);
        Task<Attachment> GetAttachmentByIdAsync(Guid attachmentId);
    }
}
