using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Models.Entities; // Added for IncidentReviewComment
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operational_Risk_Management.Models.Interfaces
{
    public interface IIncidentRepository : IGenericRepository<Incident.Incident>
    {
        Task<PaginatedModel<IncidentListDTO>> GetIncidentsAsync(IncidentFilterDTO filter, string userId, bool isAdmin);
        Task<IncidentDetailDTO> GetIncidentDetailAsync(Guid id);
        Task<Incident.Incident> CreateIncidentWithRelatedDataAsync(Incident.Incident incident, List<StaffConcerned> staffConcerned);
        Task<bool> UpdateIncidentWithRelatedDataAsync(Incident.Incident incident, List<StaffConcerned> staffConcerned);
        Task<bool> ReviewIncidentAsync(Guid id, string notes, bool requiresRevision, string reviewedBy);
        Task<bool> AddDocumentAsync(IncidentDocument document);
        Task<bool> RemoveDocumentAsync(Guid documentId);
        Task<IncidentDocument> GetDocumentByIdAsync(Guid documentId);
        Task<bool> CloseIncidentAsync(Guid id, string closedBy);
        Task<Dictionary<string, int>> GetIncidentStatisticsAsync(DateTime? startDate, DateTime? endDate, string departmentUnit = null);

        Task AddCommentAsync(IncidentReviewComment comment);
        Task<List<IncidentReviewComment>> GetCommentsByIncidentIdAsync(Guid incidentId);
    }
}
