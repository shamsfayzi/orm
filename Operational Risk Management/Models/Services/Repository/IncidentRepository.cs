using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Incident;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Services;
using Operational_Risk_Management.Models.Entities; // Added for IncidentReviewComment
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Operational_Risk_Management.Models.Repositories
{
    public class IncidentRepository : GenericRepository<Incident.Incident>, IIncidentRepository
    {
        private readonly IMapper _mapper;
        public IncidentRepository(ApplicationDBContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedModel<IncidentListDTO>> GetIncidentsAsync(IncidentFilterDTO filter, string userId, bool isAdmin)
        {
            IQueryable<Incident.Incident> query = _context.Set<Incident.Incident>()
                .Include(i => i.StaffConcerned)
                .Where(i => !i.IsDeleted);

            if (!isAdmin)
            {
                var UserId =  Guid.Parse(userId);
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == UserId);

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Department.DepartmentName))
                    {
                        query = query.Where(i => i.BranchDepartmentUnit == user.Department.DepartmentName || i.CreateBy == userId);
                    }
                    else
                    {
                        // User has no department, so can only see incidents they created
                        query = query.Where(i => i.CreateBy == userId);
                    }
                }
                else
                {
                    // User not found in DB, should not happen for an authenticated user.
                    // Return no incidents for safety.
                    query = query.Where(i => false);
                }
            }

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(i =>
                    i.TitleOfIncident.Contains(filter.SearchTerm) ||
                    i.EventIncidentDescription.Contains(filter.SearchTerm) ||
                    i.ReportedBy.Contains(filter.SearchTerm));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(i => i.ReportDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(i => i.ReportDate <= filter.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(filter.BranchDepartmentUnit))
            {
                query = query.Where(i => i.BranchDepartmentUnit == filter.BranchDepartmentUnit);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(i => i.IncidentStatus == filter.Status.Value);
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(i => i.IncidentType == filter.Type.Value);
            }

            if (filter.Category.HasValue)
            {
                query = query.Where(i => i.IncidentCategoryLevel == filter.Category.Value);
            }

            if (filter.RiskLevel.HasValue)
            {
                query = query.Where(i => i.RiskAssessment == filter.RiskLevel.Value);
            }

            if (filter.RequiresRevision.HasValue)
            {
                query = query.Where(i => i.RequiresRevision == filter.RequiresRevision.Value);
            }

            // Count total
            var totalCount = await query.CountAsync();

            // Apply pagination
            var incidents = await query
                .OrderByDescending(i => i.CreateDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(i => new IncidentListDTO
                {
                    Id = i.Id,
                    TitleOfIncident = i.TitleOfIncident,
                    BranchDepartmentUnit = i.BranchDepartmentUnit,
                    ReportDate = i.ReportDate,
                    IncidentStatus = i.IncidentStatus,
                    IncidentType = i.IncidentType,
                    IncidentCategoryLevel = i.IncidentCategoryLevel,
                    RiskAssessment = i.RiskAssessment,
                    RequiresRevision = i.RequiresRevision
                })
                .ToListAsync();
            
            return new PaginatedModel<IncidentListDTO>
            {
                Items = incidents,
                CurrentPage = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };
        }

        public async Task<IncidentDetailDTO> GetIncidentDetailAsync(Guid id)
        {
            var incident = await _context.Set<Incident.Incident>()
                .Include(i => i.StaffConcerned)
                .Include(i => i.Documents)
                .Include(i => i.ReviewComments) // <-- Added this line
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

            if (incident == null)
                return null;
            var incidentDetail = _mapper.Map<Incident.Incident, IncidentDetailDTO>(incident);

            return incidentDetail;
        }

        public async Task<Incident.Incident> CreateIncidentWithRelatedDataAsync(Incident.Incident incident, List<StaffConcerned> staffConcerned)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Calculate net loss automatically
                    if (incident.FinancialLossGrossAmount.HasValue)
                    {
                        incident.FinancialLossNetLossToDate = incident.FinancialLossGrossAmount.Value +
                            (incident.FinancialLossLegalCosts ?? 0) +
                            (incident.FinancialLossOtherCosts ?? 0) -
                            (incident.FinancialLossRecoveryFromClient ?? 0) -
                            (incident.FinancialLossRecoveryFromInsurance ?? 0);
                    }

                    // Calculate net gain automatically
                    if (incident.GainGrossAmount.HasValue)
                    {
                        incident.GainNetAmount = incident.GainGrossAmount.Value +
                            (incident.GainLegalCosts ?? 0) +
                            (incident.GainOtherCosts ?? 0) -
                            (incident.GainRecoveryFromClient ?? 0) -
                            (incident.GainRecoveryFromInsurance ?? 0);
                    }

                    // Set default status to Open
                    incident.IncidentStatus = IncidentStatus.Open;

                    // Add incident
                    var addedIncident = await base.AddAsync(incident);

                    // Add staff concerned
                    if (staffConcerned != null && staffConcerned.Any())
                    {
                        foreach (var staff in staffConcerned)
                        {
                            staff.IncidentId = addedIncident.Id;
                            _context.Set<StaffConcerned>().Add(staff);
                        }
                    }

                    // Add incident history
                    
                    await _context.SaveChangesAsync(CancellationToken.None);
                    transaction.Commit();

                    return addedIncident;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> UpdateIncidentWithRelatedDataAsync(Incident.Incident incident, List<StaffConcerned> staffConcerned)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var existingIncident = await _context.Set<Incident.Incident>()
                        .Include(i => i.StaffConcerned)
                        .FirstOrDefaultAsync(i => i.Id == incident.Id && !i.IsDeleted);

                    if (existingIncident == null)
                        return false;

                    // Check if incident is requiring revision
                    if (existingIncident.RequiresRevision)
                    {
                        existingIncident.RequiresRevision = false;
                    }

                    // Calculate net loss automatically
                    if (incident.FinancialLossGrossAmount.HasValue)
                    {
                        incident.FinancialLossNetLossToDate = incident.FinancialLossGrossAmount.Value +
                            (incident.FinancialLossLegalCosts ?? 0) +
                            (incident.FinancialLossOtherCosts ?? 0) -
                            (incident.FinancialLossRecoveryFromClient ?? 0) -
                            (incident.FinancialLossRecoveryFromInsurance ?? 0);
                    }

                    // Calculate net gain automatically
                    if (incident.GainGrossAmount.HasValue)
                    {
                        incident.GainNetAmount = incident.GainGrossAmount.Value +
                            (incident.GainLegalCosts ?? 0) +
                            (incident.GainOtherCosts ?? 0) -
                            (incident.GainRecoveryFromClient ?? 0) -
                            (incident.GainRecoveryFromInsurance ?? 0);
                    }

                    // Update incident properties
                    existingIncident.ReportedBy = incident.ReportedBy;
                    existingIncident.TitleRole = incident.TitleRole;
                    existingIncident.BranchDepartmentUnit = incident.BranchDepartmentUnit;
                    existingIncident.StartDate = incident.StartDate;
                    existingIncident.EndDate = incident.EndDate;
                    existingIncident.DiscoveryDate = incident.DiscoveryDate;
                    existingIncident.DiscoveredBy = incident.DiscoveredBy;
                    existingIncident.TitleOfIncident = incident.TitleOfIncident;
                    existingIncident.ActivityProcess = incident.ActivityProcess;
                    existingIncident.FirstLineRiskOwner = incident.FirstLineRiskOwner;
                    existingIncident.IncidentType = incident.IncidentType;
                    existingIncident.IncidentCategoryLevel = incident.IncidentCategoryLevel;
                    existingIncident.RiskType = incident.RiskType;
                    existingIncident.RiskAssessment = incident.RiskAssessment;

                    existingIncident.FinancialLossCurrency = incident.FinancialLossCurrency;
                    existingIncident.FinancialLossGrossAmount = incident.FinancialLossGrossAmount;
                    existingIncident.FinancialLossLegalCosts = incident.FinancialLossLegalCosts;
                    existingIncident.FinancialLossOtherCosts = incident.FinancialLossOtherCosts;
                    existingIncident.FinancialLossRecoveryFromClient = incident.FinancialLossRecoveryFromClient;
                    existingIncident.FinancialLossRecoveryFromInsurance = incident.FinancialLossRecoveryFromInsurance;
                    existingIncident.FinancialLossNetLossToDate = incident.FinancialLossNetLossToDate;

                    existingIncident.GainCurrency = incident.GainCurrency;
                    existingIncident.GainGrossAmount = incident.GainGrossAmount;
                    existingIncident.GainLegalCosts = incident.GainLegalCosts;
                    existingIncident.GainOtherCosts = incident.GainOtherCosts;
                    existingIncident.GainRecoveryFromClient = incident.GainRecoveryFromClient;
                    existingIncident.GainRecoveryFromInsurance = incident.GainRecoveryFromInsurance;
                    existingIncident.GainNetAmount = incident.GainNetAmount;

                    existingIncident.EventIncidentDescription = incident.EventIncidentDescription;
                    existingIncident.EventIncidentCause = incident.EventIncidentCause;
                    existingIncident.EventIncidentDiscovery = incident.EventIncidentDiscovery;
                    existingIncident.CorrectiveActionsImplemented = incident.CorrectiveActionsImplemented;
                    existingIncident.HasWrittenProcedure = incident.HasWrittenProcedure;
                    existingIncident.ProcedureIncludesControls = incident.ProcedureIncludesControls;
                    existingIncident.ExistingControlMeasures = incident.ExistingControlMeasures;
                    existingIncident.ReasonsForFailure = incident.ReasonsForFailure;
                    existingIncident.ProposedPoliciesProcedures = incident.ProposedPoliciesProcedures;
                    existingIncident.ProposedControls = incident.ProposedControls;
                    existingIncident.ProposedHumanResources = incident.ProposedHumanResources;
                    existingIncident.ProposedSystems = incident.ProposedSystems;
                    existingIncident.ProposedOthers = incident.ProposedOthers;
                    existingIncident.OtherComments = incident.OtherComments;

                    existingIncident.UpdateBy = incident.UpdateBy;
                    existingIncident.UpdatedDate = DateTime.UtcNow;

                    // Update staff concerned
                    if (existingIncident.StaffConcerned != null && existingIncident.StaffConcerned.Any())
                    {
                        _context.Set<StaffConcerned>().RemoveRange(existingIncident.StaffConcerned);
                    }

                    if (staffConcerned != null && staffConcerned.Any())
                    {
                        foreach (var staff in staffConcerned)
                        {
                            staff.IncidentId = existingIncident.Id;
                            _context.Set<StaffConcerned>().Add(staff);
                        }
                    }

                  
                   

                    await _context.SaveChangesAsync(CancellationToken.None);
                    transaction.Commit();

                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> ReviewIncidentAsync(Guid id, string notes, bool requiresRevision, string reviewedBy)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var incident = await _context.Set<Incident.Incident>()
                        .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

                    if (incident == null)
                        return false;

                    incident.RiskManagementNotes = notes;
                    incident.RequiresRevision = requiresRevision;
                    incident.LastReviewDate = DateTime.UtcNow;
                    incident.ReviewedBy = reviewedBy;
                    incident.UpdateBy = reviewedBy;
                    incident.UpdatedDate = DateTime.UtcNow;


                    await _context.SaveChangesAsync(CancellationToken.None);
                    transaction.Commit();

                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public async Task<bool> AddDocumentAsync(IncidentDocument document)
        {
            try
            {
                _context.Set<IncidentDocument>().Add(document);
                await _context.SaveChangesAsync(CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveDocumentAsync(Guid documentId)
        {
            var document = await _context.Set<IncidentDocument>()
                .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);

            if (document == null)
                return false;

            try
            {
                // Soft delete
                document.IsDeleted = true;
                document.DeletedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(CancellationToken.None);


                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IncidentDocument> GetDocumentByIdAsync(Guid documentId)
        {
            return await _context.Set<IncidentDocument>()
                .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);
        }

        public async Task<bool> CloseIncidentAsync(Guid id, string closedBy)
        {
            var incident = await _context.Set<Incident.Incident>()
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

            if (incident == null)
                return false;

            try
            {
                incident.IncidentStatus = IncidentStatus.Closed;
                incident.UpdateBy = closedBy;
                incident.UpdatedDate = DateTime.UtcNow;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, int>> GetIncidentStatisticsAsync(DateTime? startDate, DateTime? endDate, string departmentUnit = null)
        {
            var query = _context.Set<Incident.Incident>()
                .Where(i => !i.IsDeleted);

            if (startDate.HasValue)
            {
                query = query.Where(i => i.ReportDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.ReportDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(departmentUnit))
            {
                query = query.Where(i => i.BranchDepartmentUnit == departmentUnit);
            }

            var statistics = new Dictionary<string, int>
            {
                ["Total"] = await query.CountAsync(),
                ["Open"] = await query.CountAsync(i => i.IncidentStatus == IncidentStatus.Open),
                ["Closed"] = await query.CountAsync(i => i.IncidentStatus == IncidentStatus.Closed),
                ["RequiresRevision"] = await query.CountAsync(i => i.RequiresRevision),
                ["InternalFraud"] = await query.CountAsync(i => i.IncidentCategoryLevel == IncidentCategoryLevel.InternalFraud),
                ["ExternalFraud"] = await query.CountAsync(i => i.IncidentCategoryLevel == IncidentCategoryLevel.ExternalFraud),
                ["WorkplaceSafety"] = await query.CountAsync(i => i.IncidentCategoryLevel == IncidentCategoryLevel.EmploymentPracticeAndWorkplaceSafety),
                ["BusinessPractices"] = await query.CountAsync(i => i.IncidentCategoryLevel == IncidentCategoryLevel.ClientsProductsAndBusinessPractices),
                ["PhysicalAssets"] = await query.CountAsync(i => i.IncidentCategoryLevel == IncidentCategoryLevel.DamageToPhysicalAssets),
                ["SystemsFailure"] = await query.CountAsync(i => i.IncidentCategoryLevel == IncidentCategoryLevel.BusinessDisruptionAndSystemsFailure),
                ["ProcessManagement"] = await query.CountAsync(i => i.IncidentCategoryLevel == IncidentCategoryLevel.ExecutionDeliveryAndProcessManagement),
                ["VeryHighRisk"] = await query.CountAsync(i => i.RiskAssessment == RiskAssessment.VeryHighRisk),
                ["HighRisk"] = await query.CountAsync(i => i.RiskAssessment == RiskAssessment.HighRisk),
                ["MediumRisk"] = await query.CountAsync(i => i.RiskAssessment == RiskAssessment.MediumRisk),
                ["LowRisk"] = await query.CountAsync(i => i.RiskAssessment == RiskAssessment.LowRisk),
                ["VeryLowRisk"] = await query.CountAsync(i => i.RiskAssessment == RiskAssessment.VeryLowRisk)
            };

            return statistics;
        }

        public async Task AddCommentAsync(IncidentReviewComment comment)
        {
            await _context.IncidentReviewComments.AddAsync(comment);
            // SaveChangesAsync will be called by the service layer (Unit of Work)
        }

        public async Task<List<IncidentReviewComment>> GetCommentsByIncidentIdAsync(Guid incidentId)
        {
            return await _context.IncidentReviewComments
                                 .Where(c => c.IncidentId == incidentId)
                                 .OrderByDescending(c => c.CommentDate) // Or Ascending, based on desired display
                                 .ToListAsync();
        }
    }
}
