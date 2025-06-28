using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Common;
using FluentValidation;
using Operational_Risk_Management.Models.Extensions;

namespace Operational_Risk_Management.Models.View_Models.Departments
{
    public class VM_DepartmentUpdate : IValidationModel
    {
        [Required]
        public Guid? Id { get; set; }
        [DisplayName("Department Name")]
        [Required]
        public string? DepartmentName { get; set; }
        [DisplayName("Description")]
        public string? Description { get; set; }
    }
    public class VLD_DepartmentUpdate : AbstractValidatorWithDbContext<VM_DepartmentUpdate>
    {
        public VLD_DepartmentUpdate(HttpContext context) : base(context)
        {
            RuleFor(a => a.Id).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, id) => _dbContext.Departments.IsExists(model.Id.ToGuid())).WithMessage("Invalid request");
            RuleFor(a => a.DepartmentName).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, departmentName) => _dbContext.Departments.IsUnique(model.Id.ToGuid(), "DepartmentName", departmentName)).WithMessage("Department already exists.");
        }
    }

}
