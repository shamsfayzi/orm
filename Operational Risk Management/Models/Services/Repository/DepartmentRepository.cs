using AutoMapper;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.View_Models.Departments;
using Microsoft.IdentityModel.Tokens;

namespace Operational_Risk_Management.Models.Services.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly IApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILogger<DepartmentRepository> _logger;
        public DepartmentRepository(ApplicationDBContext context, IMapper mapper, ILogger<DepartmentRepository> logger, IHttpContextAccessor httpAccessor) : base(context)
        {
            _dbcontext = context;
            _mapper = mapper;
            _logger = logger;
            _httpAccessor = httpAccessor;
        }

        public async Task<PaginatedModel<Department>> GetTableAsync(PaginatedModel<Department> model)
        {
            var departments = _dbcontext.Departments.AsQueryable();
            if (!model.Search.IsNullOrEmpty())
            {
                departments = departments.Where(a => a.DepartmentName.Contains(model.Search) ||
                a.Description.Contains(model.Search));
            }

            return await departments.ToPaginatedAsync(model);
        }

        public bool IsDepartmentExists(string departmentName, Guid? id)
        {
            if (id is not null)
            {
                return _dbcontext.Departments.IsUnique(id.ToGuid(), "DepartmentName", departmentName);
            }
            return _dbcontext.Departments.IsUnique("DepartmentName", departmentName);
        }

        public async Task<bool> AddAsync(VM_DepartmentCreate model)
        {
            try
            {
                var newDepartment = await base.AddAsync(_mapper.Map<Department>(model));

                await _dbcontext.SaveChangesAsync(CancellationToken.None);

                _logger.LogAndSave(_dbcontext, "{0}: {1} created the Department with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), newDepartment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the department creation", DateTime.Now);
                _logger.LogError(ex.Message);
            }
            return true;
        }
        public async Task UpdateAsync(VM_DepartmentUpdate model)
        {
            try
            {
                var targetDepartment = await _dbcontext.Departments.FindAsync(model.Id);
                _mapper.Map(model, targetDepartment);
                _dbcontext.Departments.Update(targetDepartment);
                _dbcontext.SaveChanges();
                _logger.LogAndSave(_dbcontext, "{0}: {1} updated the department with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), model.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the department update with userId: {1}", DateTime.Now, model.Id);
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
                _logger.LogAndSave(_dbcontext, "{0}: {1} deleted the department with Id {2}", DateTime.Now, _httpAccessor.HttpContext.User.GetFullName(), id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: an error occured during the department deletation with departmetn id : {1}", DateTime.Now, id);
                _logger.LogError(ex.Message);
            }
            await Task.CompletedTask;
        }
    }
}