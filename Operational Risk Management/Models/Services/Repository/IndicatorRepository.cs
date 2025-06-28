using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
using Operational_Risk_Management.Models.View_Models.Uploader.Indicator;

namespace Operational_Risk_Management.Models.Services.Repository
{
    public class IndicatorRepository : GenericRepository<Indicator>, IIndicatorRepository
    {
        private readonly IApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILogger<IndicatorRepository> _logger;
        public IndicatorRepository(ApplicationDBContext context, IMapper mapper, ILogger<IndicatorRepository> logger, IHttpContextAccessor httpAccessor) : base(context)
        {
            _dbcontext = context;
            _mapper = mapper;
            _logger = logger;
            _httpAccessor = httpAccessor;
        }

        public async Task<PaginatedModel<Indicator>> GetTableAsync(PaginatedModel<Indicator> model)
        {
            var indicators = _dbcontext.Indicators.AsQueryable();
            if (!model.Search.IsNullOrEmpty())
            {
                indicators = indicators.Where(a => a.IndicatorName.Contains(model.Search) ||
                a.Process.Contains(model.Search));
            }

            return await indicators.ToPaginatedAsync(model);
        }
        public async Task<PaginatedModel<Indicator>> GetByTemplateId(Guid Tid, PaginatedModel<Indicator> model)
        {
            var indicators = _dbcontext.Indicators.Where(a => a.TemplateId == Tid).AsQueryable();
            if (!model.Search.IsNullOrEmpty())
            {
                indicators = indicators.Where(a => a.IndicatorName.Contains(model.Search) ||
                a.Process.Contains(model.Search));
            }

            return await indicators.ToPaginatedAsync(model);
        }
        public async Task<bool> AddAsync(VM_KRIIndicatorCreate vM_KRIIndicatorCreate)
        {
            try
            {
                var newIndicator = await base.AddAsync(_mapper.Map<Indicator>(vM_KRIIndicatorCreate));
                await _dbcontext.SaveChangesAsync(CancellationToken.None);

                _logger.LogAndSave(_dbcontext, "{0}: {1} created the indicator with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), newIndicator.Id);

            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the indicator creation", DateTime.Now);
                _logger.LogError(ex.Message);
            }
            return true;
        }
        public async Task UpdateAsync(VM_IndicatorEdit vM_KRIIndicatorUpdate)
        {
            try
            {
                var indicator =  _dbcontext.Indicators.Update(_mapper.Map<Indicator>(vM_KRIIndicatorUpdate));
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
        public override async Task DeleteAsync(Guid id)
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
        public async Task<PaginatedModel<Submission>> GetSubmissionsByIID(Guid id, PaginatedModel<Submission> model)
        {
            var submissions = _dbcontext.Submissions.Where(a => a.IndicatorId == id).AsQueryable();
            if (!model.Search.IsNullOrEmpty())
            {
                submissions = submissions.Where(a => a.CreateDate.ToString().Contains(model.Search) ||
                a.Status.ToString().Contains(model.Search));
            }
            return await submissions.ToPaginatedAsync(model);
        }
        //public async Task<PaginatedModel<VM_Indicator>> GetPaginatedWithDueSoon(PaginatedModel<VM_Indicator> model,string focalPoint)
        //{
        //    var indicators = _dbcontext.Indicators.Include(a=> a.Submissions).Where(a=>a.Template.FocalPoint == focalPoint).AsQueryable();
        //    if (!model.Search.IsNullOrEmpty())
        //    {
        //        indicators = indicators.Where(a => a.IndicatorName.Contains(model.Search) ||
        //        a.Process.Contains(model.Search));
        //    }
        //    var indicatorVMs = indicators.Select(ind => _mapper.Map<VM_Indicator>(ind)).AsQueryable();

        //    return await indicatorVMs.ToPaginatedAsync(model);
        //}


    }

}
