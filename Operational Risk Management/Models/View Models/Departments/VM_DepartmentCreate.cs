using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Operational_Risk_Management.Models.Common;
using FluentValidation;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.View_Models.Departments
{
    public class VM_DepartmentCreate : IValidationModel
    {
        [DisplayName("Department Name")]
        [Required]
        public string? DepartmentName { get; set; }
        [DisplayName("Description")]
        public string? Description { get; set; }
    }
    public class VLD_DepartmentCreate : AbstractValidatorWithDbContext<VM_DepartmentCreate>
    {
        public VLD_DepartmentCreate(HttpContext context) : base(context)
        {
            RuleFor(a => a.DepartmentName).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must(departmentName => _dbContext.Departments.IsUnique("DepartmentName", departmentName)).WithMessage("Department already exists");
        }
    }
}