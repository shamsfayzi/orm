using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.Services.Repositories;
using Operational_Risk_Management.Models.View_Models.Indicator;
using Operational_Risk_Management.Models.View_Models.KRIIndicator;
using Operational_Risk_Management.Models.View_Models.Templates;

namespace Operational_Risk_Management.Models.Services.Repository
{
    public class TemplateRepository : GenericRepository<Template>, ITemplateRepository
    {
        private readonly IApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILogger<TemplateRepository> _logger;
        public TemplateRepository(ApplicationDBContext context, IMapper mapper, ILogger<TemplateRepository> logger, IHttpContextAccessor httpAccessor) : base(context)
        {
            _dbcontext = context;
            _mapper = mapper;
            _logger = logger;
            _httpAccessor = httpAccessor;
        }
        public async Task<PaginatedModel<Template>> GetTableAsync(PaginatedModel<Template> model)
        {
            var KRITemplates = _dbcontext.Templates.Include(a => a.Department).AsQueryable();
            if (!model.Search.IsEmptyOrNull())
            {
                KRITemplates = KRITemplates.Where(a => a.FocalPoint.Contains(model.Search) ||
                a.Department.DepartmentName.Contains(model.Search));
            }
            return await KRITemplates.ToPaginatedAsync(model);
        }
        public async Task<bool> AddAsync(VM_KRITemplateCreate model)
        {
            try
            {
                var newKRITemplate = _mapper.Map<Template>(model);
                var user = await _dbcontext.ApplicationUsers.FindAsync(model.ApplicationUserId);
                newKRITemplate.FocalPoint = user.FullName;
                newKRITemplate = await base.AddAsync(newKRITemplate);
                await _dbcontext.SaveChangesAsync(CancellationToken.None);
                _logger.LogAndSave(_dbcontext, "{0}: {1} created the KRITemplate with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), newKRITemplate.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the KRITemplate creation", DateTime.Now);
                return false;
            }
        }
        public async Task UpdateAsync(VM_KRITemplateUpdate model)
        {
            try
            {
                var targetTemplate = await _dbcontext.Templates.FindAsync(model.Id);
                var user = await _dbcontext.ApplicationUsers.FindAsync(model.ApplicationUserId);
                targetTemplate.FocalPoint = user.FullName;
                _mapper.Map(model, targetTemplate);
                _dbcontext.Templates.Update(targetTemplate);
                _dbcontext.SaveChanges();
                _logger.LogAndSave(_dbcontext, "{0}: {1} updated the user with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), model.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the user update with userId: {1}", DateTime.Now, model.Id);
                _logger.LogError(ex.Message);
            }

            await Task.CompletedTask;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                base.DeleteAsync(id);
                await _dbcontext.SaveChangesAsync(CancellationToken.None);
                _logger.LogAndSave(_dbcontext, "{0}: {1} deleted the KRITemplate with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the KRITemplate deletion", DateTime.Now);
                return false;
            }
        }
        public async Task<PaginatedModel<Template>> TemplateDetails(PaginatedModel<Template> model, Guid id)
        {

            try
            {
                if (await base.GetByIdAsync(id) == null)
                {
                    return model;
                }
                else
                {
                    return await
                        _dbcontext.Templates.Where
                        (a => a.Id == id)
                        .Include(a => a.Department)
                        .Include(a => a.Indicators)
                            .ThenInclude(a => a.Submissions)
                        .ToPaginatedAsync(model);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<bool> AddTemplateIndicatorAsync(VM_KRIIndicatorCreate vM_KRIIndicatorCreate)
        {
            try
            {
                var newIndicator =
                    await _dbcontext.Indicators.AddAsync(_mapper.Map<Indicator>(vM_KRIIndicatorCreate));
                await _dbcontext.SaveChangesAsync(CancellationToken.None);

                _logger.LogAndSave(_dbcontext, "{0}: {1} created the indicator with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), newIndicator.Entity.Id);

            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the indicator creation", DateTime.Now);
                _logger.LogError(ex.Message);
            }
            return true;
        }
        public async Task UpdateTemplateIndicatorAsync(VM_IndicatorEdit vM_KRIIndicatorUpdate)
        {
            try
            {
                var indicator = _dbcontext.Indicators.Update(_mapper.Map<Indicator>(vM_KRIIndicatorUpdate));
                await _dbcontext.SaveChangesAsync(CancellationToken.None);
                _logger.LogAndSave(_dbcontext, "{0}: {1} updated the indicator with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), vM_KRIIndicatorUpdate.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the indicator update", DateTime.Now);
                _logger.LogError(ex.Message);
            }
            await Task.CompletedTask;
        }
        public async Task DeleteTemplateIndicatorAsync(Guid id)
        {
            try
            {
                await base.DeleteAsync(id);
                await _dbcontext.SaveChangesAsync(CancellationToken.None);
                _logger.LogAndSave(_dbcontext, "{0}: {1} deleted the indicator with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the indicator deletation with indicator id : {1}", DateTime.Now, id);
                _logger.LogError(ex.Message);
            }
            await Task.CompletedTask;
        }
        public async Task<Template> GetTemplateByFocalPoint(string FocalPoint)
        {
            try
            {
                return await _dbcontext.Templates
                    .FirstOrDefaultAsync(a => a.FocalPoint == FocalPoint && !a.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured while fetching the template by focal point: {1}", DateTime.Now, FocalPoint);
                _logger.LogError(ex.Message);
                return null;
            }
        }

    }
}
