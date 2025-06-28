using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.View_Models.Templates
{
    public class VM_KRITemplateCreate : IValidationModel
    {
        [Required]
        [DisplayName("Focal Point")]
        public Guid ApplicationUserId { get; set; }
        [Required]
        [DisplayName("Department")]
        public Guid DepartmentId { get; set; }
    }
    public class VLD_KRITEmplateCreate : AbstractValidatorWithDbContext<VM_KRITemplateCreate>
    {
        public VLD_KRITEmplateCreate(HttpContext context) : base(context)
        {
            RuleFor(a => a.DepartmentId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, departmentId) => _dbContext.Departments.IsExists(model.DepartmentId)).WithMessage("Invalid request");
            RuleFor(a => a.ApplicationUserId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, applicationUserId) => _dbContext.ApplicationUsers.IsExists(model.ApplicationUserId)).WithMessage("Invalid request");
            // check if a department already has a template
            //RuleFor(a=> a.DepartmentId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, departmentId) => _dbContext.Templates.IsUnique(model.DepartmentId, "DepartmentId",departmentId)).WithMessage("Department already exists.");
        }
    }
}
