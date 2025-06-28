using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Repositories;

namespace Operational_Risk_Management.Models.Services.Repository
{
    public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
    {
        private readonly IApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILogger<SubmissionRepository> _logger;
        public SubmissionRepository(ApplicationDBContext context, IMapper mapper, ILogger<SubmissionRepository> logger, IHttpContextAccessor httpAccessor) : base(context)
        {
            _dbcontext = context;
            _mapper = mapper;
            _logger = logger;
            _httpAccessor = httpAccessor;
        }
        public async Task<PaginatedModel<Submission>> GetTableAsync(PaginatedModel<Submission> model)
        {
            var submissions = _dbcontext.Submissions.Where(a=>a.CreateBy != "System_Automation").AsQueryable();
            if (!model.Search.IsEmptyOrNull())
            {
                submissions = submissions.Where(a => a.Notes.Contains(model.Search) ||
                a.Indicator.IndicatorName.Contains(model.Search));
            }
            return await submissions.ToPaginatedAsync(model);
        }

        public async Task<Submission> SubmissionDetails(Guid id)
        {
            var submission = await _dbcontext.Submissions.Include(a => a.Attachments).Include(a => a.Feedbacks).Include(a=>a.Indicator).ThenInclude(a=>a.Template).ThenInclude(Template => Template.Department)
                .FirstOrDefaultAsync(a => a.Id == id);
            return submission;
        }
    }
}
