using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.View_Models.Users;

namespace Operational_Risk_Management.Models.Services.Repositories
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly IApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILogger<ApplicationUserRepository> _logger;
        public ApplicationUserRepository(ApplicationDBContext context, IMapper mapper, ILogger<ApplicationUserRepository> logger, IHttpContextAccessor httpAccessor) : base(context)
        {
            _dbcontext = context;
            _mapper = mapper;
            _logger = logger;
            _httpAccessor = httpAccessor;
        }

        public async Task<PaginatedModel<ApplicationUser>> GetTableAsync(PaginatedModel<ApplicationUser> model)
        {
            var users = _dbcontext.ApplicationUsers.Include(a=>a.Department).AsQueryable();
            if (!model.Search.IsEmptyOrNull())
            {
                users = users.Where(a => a.UserName.Contains(model.Search) ||
                a.FullName.Contains(model.Search));
            }

            return await users.ToPaginatedAsync(model);
        }

        public async Task UpdateAsync(VM_AppUserUpdate model)
        {
            try
            {
                var targetApplciationUser = await _dbcontext.ApplicationUsers.Include(a=>a.Department).Where(a=>a.Id == model.Id).FirstOrDefaultAsync();
                _mapper.Map(model, targetApplciationUser);
                _dbcontext.ApplicationUsers.Update(targetApplciationUser);
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

        public bool IsUserNameExists(string userName, Guid? id)
        {
            if (id is not null)
            {
                return _dbcontext.ApplicationUsers.IsUnique(id.ToGuid(),"UserName",userName);
            }
            return _dbcontext.ApplicationUsers.IsUnique("UserName", userName);
        }

        public async Task<bool> AddAsync(VM_AppUserCreate model)
        {
            try
            {
                var newApplicationUser = await base.AddAsync(_mapper.Map<ApplicationUser>(model));

                await _dbcontext.SaveChangesAsync(CancellationToken.None);
              
                _logger.LogAndSave(_dbcontext, "{0}: {1} created the user with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), newApplicationUser.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the user creation", DateTime.Now);
                _logger.LogError(ex.Message);
            }
            return true;
        }

        public override async Task DeleteAsync(Guid id)
        {
            try
            {
                await base.DeleteAsync(id);
                await _dbcontext.SaveChangesAsync(CancellationToken.None);
                _logger.LogAndSave(_dbcontext, "{0}: {1} deleted the user with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the user deletation with userId: {1}", DateTime.Now, id);
                _logger.LogError(ex.Message);
            }
            await Task.CompletedTask;
        }
    }
}