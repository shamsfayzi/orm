using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Operational_Risk_Management.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Entities; // Added for IncidentReviewComment
using Operational_Risk_Management.Models.View_Models.Common; // Added for FileContentDownloadViewModel
using Operational_Risk_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operational_Risk_Management.Services.Interfaces
{
    public interface IIncidentService
    {
        Task<IncidentDetailDTO> CreateIncidentAsync(CreateIncidentDTO model, string userId);
        Task<bool> UpdateIncidentAsync(Guid id, CreateIncidentDTO model, string userId);
        Task<IncidentDetailDTO> GetIncidentDetailAsync(Guid id);
        Task<PaginatedModel<IncidentListDTO>> GetIncidentsAsync(IncidentFilterDTO filter, string userId, bool isAdmin);
        Task<bool> ReviewIncidentAsync(ReviewIncidentDTO model, string reviewedBy);
        Task<bool> CloseIncidentAsync(Guid id, string userId);
        Task<bool> UploadDocumentAsync(Guid incidentId, IFormFile file, string description, string userId);
        Task<bool> RemoveDocumentAsync(Guid documentId, string userId);
        Task<FileDownloadDTO> DownloadDocumentAsync(Guid documentId);
        Task<Dictionary<string, int>> GetIncidentStatisticsAsync(DateTime? startDate, DateTime? endDate, string departmentUnit = null);
        Task<byte[]> ExportIncidentToPdfAsync(Guid id);
        Task<byte[]> ExportIncidentsToExcelAsync(IncidentFilterDTO filter, string userId, bool isAdmin);

        Task<List<IncidentReviewComment>> GetIncidentCommentsAsync(Guid incidentId);
        Task AddIncidentCommentAsync(Guid incidentId, string commentText, string commenterId, string commenterName, bool isRevisionNote, bool isVisibleToDepartment);
        Task<FileContentDownloadViewModel> GetDocumentForDownloadAsync(Guid documentId);
    }

    public class IncidentHistoryDTO
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FileDownloadDTO
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Url { get; set; }
    }
}

