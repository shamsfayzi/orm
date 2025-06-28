using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Services;
using Operational_Risk_Management.Services.Interfaces;
using Operational_Risk_Management.Models.Entities; // Added for IncidentReviewComment
using Operational_Risk_Management.Models.View_Models.Common; // Added for FileContentDownloadViewModel
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Operational_Risk_Management.Models.Services;

namespace Operational_Risk_Management.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationService _notificationService;
        private readonly IPdfService _pdfService;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationDbContext _context; // Added context

        public IncidentService(
            IIncidentRepository incidentRepository,
            IFileStorageService fileStorageService,
            INotificationService notificationService, // Assuming this is for internal notifications
            IPdfService pdfService,
            IMapper mapper,
            IExcelService excelService,
            IApplicationDbContext context, // Ensure this is present and assigned
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _incidentRepository = incidentRepository;
            _fileStorageService = fileStorageService;
            _notificationService = notificationService;
            _pdfService = pdfService;
            _excelService = excelService;
            _mapper = mapper;
            _context = context; // Assign context
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IncidentDetailDTO> CreateIncidentAsync(CreateIncidentDTO model, string userId)
        {
            var user = "Muqtader";
            if (user == null)
                return null;

            var incident = _mapper.Map<Incident>(model);

            var staffConcerned = model.StaffConcerned?.Select(s => new StaffConcerned
            {
                Name = s.Name,
                TitleFunction = s.TitleFunction,
                CreateBy = userId,
                CreateDate = DateTime.UtcNow
            }).ToList();

            var createdIncident = await _incidentRepository.CreateIncidentWithRelatedDataAsync(incident, staffConcerned);

            //// Notify Risk Management team about new incident
            //await _notificationService.NotifyRiskManagementTeamAsync(
            //    "New Incident Report",
            //    $"A new incident '{model.TitleOfIncident}' has been reported by {model.BranchDepartmentUnit}."
            //);

            return await _incidentRepository.GetIncidentDetailAsync(createdIncident.Id);
        }

        public async Task<bool> UpdateIncidentAsync(Guid id, CreateIncidentDTO model, string userId)
        {
            var user = "Muqtader";
            if (user == null)
                return false;

            // Check if incident exists
            var existingIncidentOriginalState = await _incidentRepository.GetByIdAsync(id); // Get original state for RequiresRevision check
            if (existingIncidentOriginalState == null)
                return false;

            // Map DTO to a new Incident object that will be passed to repository
            var incidentToUpdate = _mapper.Map<Incident>(model);
            incidentToUpdate.Id = id; // Ensure Id is set for the update operation
            incidentToUpdate.UpdateBy = userId; // Set UpdateBy
            incidentToUpdate.UpdatedDate = DateTime.UtcNow; // Set UpdatedDate

            // If a user is submitting an update, it's considered a revision fulfillment.
            incidentToUpdate.RequiresRevision = false;
            incidentToUpdate.LastReviewDate = null; // Clear last review details
            incidentToUpdate.ReviewedBy = null;     // Clear reviewer

            var staffConcerned = model.StaffConcerned?.Select(s => new StaffConcerned
            {
                Name = s.Name,
                TitleFunction = s.TitleFunction,
                CreateBy = userId,
                CreateDate = DateTime.UtcNow
            }).ToList();

            // The UpdateIncidentWithRelatedDataAsync should correctly take values from incidentToUpdate
            // including RequiresRevision, LastReviewDate, ReviewedBy, UpdateBy, UpdatedDate.
            var result = await _incidentRepository.UpdateIncidentWithRelatedDataAsync(incidentToUpdate, staffConcerned);

            if (result && existingIncidentOriginalState.RequiresRevision) // Check original state for notification
            {
                // Notify Risk Management team that a previously requested revision has been submitted
                await _notificationService.NotifyRiskManagementTeamAsync(
                    "Incident Report Revised by User",
                    $"The incident report '{incidentToUpdate.TitleOfIncident}' (ID: {incidentToUpdate.Id}), which previously required revision, has been updated by the user."
                );
            }

            return result;
        }

        public async Task<IncidentDetailDTO> GetIncidentDetailAsync(Guid id)
        {
            return await _incidentRepository.GetIncidentDetailAsync(id);
        }

        public async Task<PaginatedModel<IncidentListDTO>> GetIncidentsAsync(IncidentFilterDTO filter, string userId, bool isAdmin)
        {
            return await _incidentRepository.GetIncidentsAsync(filter, userId, isAdmin);
        }

        public async Task<bool> ReviewIncidentAsync(ReviewIncidentDTO model, string reviewedBy)
        {
            var result = await _incidentRepository.ReviewIncidentAsync(model.IncidentId, model.RiskManagementNotes, model.RequiresRevision, reviewedBy);

            if (result && model.RequiresRevision)
            {
                // Get incident details for notification
                var incident = await _incidentRepository.GetIncidentDetailAsync(model.IncidentId);

                if (incident != null)
                {
                    // Find users in the department to notify
                    //var departmentUsers = await _userManager.GetUsersInRoleAsync("User");
                    //var departmentUserIds = departmentUsers
                    //    .Where(u => u.Department == incident.BranchDepartmentUnit)
                    //    .Select(u => u.Id)
                    //    .ToList();
                    var departmentUserIds = new List<string>{ "1","2" };
                    // Notify department about revision request
                    await _notificationService.NotifyUsersAsync(
                        departmentUserIds,
                        "Incident Report Requires Revision",
                        $"Your incident report '{incident.TitleOfIncident}' requires revision. Please check the Risk Management notes."
                    );
                }
            }

            return result;
        }

        public async Task<bool> CloseIncidentAsync(Guid id, string userId)
        {
            return await _incidentRepository.CloseIncidentAsync(id, userId);
        }

        public async Task<bool> UploadDocumentAsync(Guid incidentId, IFormFile file, string description, string userId)
        {
            // Validate incident exists
            var incident = await _incidentRepository.GetByIdAsync(incidentId);
            if (incident == null)
                return false;

            // Upload file to storage
            var path = await _fileStorageService.SaveFileAsync(file, "incidents");

            if (path == null)
                return false;

            // Create document record
            var document = new IncidentDocument
            {
                IncidentId = incidentId,
                FileName = path.Split("/")[2],
                FilePath = path,
                FileType = file.ContentType,
                FileSize = (long)(file.ContentType.Length / (1024m * 1024m)),
                Description = description,
                CreateBy = userId,
                CreateDate = DateTime.UtcNow
            };

            return await _incidentRepository.AddDocumentAsync(document);
        }

        public async Task<bool> RemoveDocumentAsync(Guid documentId, string userId)
        {
            var document = await _incidentRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
                return false;

            // Update document for tracking
            document.UpdateBy = userId;
            document.UpdatedDate = DateTime.UtcNow;

            return await _incidentRepository.RemoveDocumentAsync(documentId);
        }
        public async Task<FileDownloadDTO> DownloadDocumentAsync(Guid documentId)
        {
            var document = await _incidentRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
                return null;
            // for better security we should send file content not a link to wwwroot
            //var fileData = await _fileStorageService.GetFileAsync(document.FilePath);

            //if (fileData == null)
            //    return null;

            return new FileDownloadDTO
            {
                FileName = document.FileName,
                ContentType = document.FileType,
                Url = document.FilePath
            };
        }

        public async Task<Dictionary<string, int>> GetIncidentStatisticsAsync(DateTime? startDate, DateTime? endDate, string departmentUnit = null)
        {
            return await _incidentRepository.GetIncidentStatisticsAsync(startDate, endDate, departmentUnit);
        }

        public async Task<byte[]> ExportIncidentToPdfAsync(Guid id)
        {
            var incident = await _incidentRepository.GetIncidentDetailAsync(id);
            if (incident == null)
                return null;
            return await _pdfService.GenerateIncidentReportPdfAsync(incident);
        }

        public async Task<byte[]> ExportIncidentsToExcelAsync(IncidentFilterDTO filter, string userId, bool isAdmin)
        {
            var incidents = await _incidentRepository.GetIncidentsAsync(filter, userId, isAdmin);
            if (incidents == null || incidents.Items == null || !incidents.Items.Any())
                return null;

            return await _excelService.GenerateIncidentReportExcelAsync(incidents.Items.ToList());
        }

        public async Task<List<IncidentReviewComment>> GetIncidentCommentsAsync(Guid incidentId)
        {
            return await _incidentRepository.GetCommentsByIncidentIdAsync(incidentId);
        }

        public async Task AddIncidentCommentAsync(Guid incidentId, string commentText, string commenterId, string commenterName, bool isRevisionNote, bool isVisibleToDepartment)
        {
            var incident = await _incidentRepository.GetByIdAsync(incidentId);
            if (incident == null)
            {
                // Or throw an exception
                return;
            }

            var comment = new IncidentReviewComment
            {
                IncidentId = incidentId,
                CommentText = commentText,
                CommenterId = commenterId,
                CommenterName = commenterName,
                CommentDate = DateTime.UtcNow,
                IsRevisionNote = isRevisionNote,
                IsVisibleToDepartment = isVisibleToDepartment
                // BaseEntity properties like Id, CreateDate will be handled by BaseEntity or DB
            };

            await _incidentRepository.AddCommentAsync(comment); // This does NOT save changes now

            if (isRevisionNote && isVisibleToDepartment)
            {
                incident.RequiresRevision = true;
                incident.RiskManagementNotes = commentText;
                incident.LastReviewDate = DateTime.UtcNow;
                incident.ReviewedBy = commenterName;
                // No explicit _incidentRepository.UpdateAsync(incident) needed if incident is tracked by _context
            }

            await _context.SaveChangesAsync(CancellationToken.None); // Save all tracked changes

            if (isRevisionNote && isVisibleToDepartment)
            {
                if (incident != null && !string.IsNullOrEmpty(incident.CreateBy))
                {
                    var user = await _userManager.FindByNameAsync(incident.CreateBy); // Find user by username
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        string subject = $"Incident Report Requires Revision: {incident.TitleOfIncident}";
                        string emailBody = $@"
                        <p>Dear {user.UserName},</p>
                        <p>Your incident report titled ""<strong>{incident.TitleOfIncident}</strong>"" (ID: {incident.Id}) requires revision.</p>
                        <p><strong>Reviewer's Comment:</strong></p>
                        <p><em>{comment.CommentText}</em></p>
                        <p>Please log in to the Operational Risk Management system to view the full details and take necessary action.</p>
                        <p>Thank you.</p>";

                        try
                        {
                            bool emailSent = await _emailSender.SendAsync_SMTP(user.Email, subject, emailBody, isBodyHtml: true);
                            if (emailSent)
                            {
                                // Log success if ILogger is available
                            }
                            else
                            {
                                // Log failure if ILogger is available
                            }
                        }
                        catch (Exception) // Consider logging ex if ILogger is available
                        {
                            // Log exception if ILogger is available
                        }
                    }
                    else
                    {
                        // Log if user or email not found
                    }
                }
            }
        }

        public async Task<FileContentDownloadViewModel> GetDocumentForDownloadAsync(Guid documentId)
        {
            var document = await _incidentRepository.GetDocumentByIdAsync(documentId);
            if (document == null || string.IsNullOrEmpty(document.FilePath))
            {
                return null; // Or throw FileNotFoundException
            }

            byte[] fileBytes = await _fileStorageService.ReadFileBytesAsync(document.FilePath);
            if (fileBytes == null)
            {
                return null; // Or throw an exception indicating file content could not be read
            }

            return new FileContentDownloadViewModel
            {
                FileContents = fileBytes,
                ContentType = document.FileType ?? "application/octet-stream", // Default content type
                FileName = document.FileName
            };
        }
    }
}