using FluentValidation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.View_Models.Users
{
    public record VM_AppUserCreate : IValidationModel
    {
        [DisplayName("Full Name")]
        [Required]
        public string? FullName { get; set; }

        [Required]
        [DisplayName("User Name")]
        public string? UserName { get; set; }

        [DisplayName("Phone 1")]
        public string? Phone1 { get; set; }

        [DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }
        [DisplayName("Access to KRI Submission")]
        public bool KriSubmissionAccess { get; set; } = false;
        [DisplayName("Department")]
        public Guid DepartmentId { get; set; }
    }
    /// <summary>
    /// Create AppUser model validation
    /// </summary>
    public class VLD_AppUserCreate : AbstractValidatorWithDbContext<VM_AppUserCreate>
    {
        public VLD_AppUserCreate(HttpContext context) : base(context)
        {
            RuleFor(a => a.UserName).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must(username=>_dbContext.ApplicationUsers.IsUnique("UserName",username)).WithMessage("User name already exists");
            RuleFor(a => a.FullName).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must(flname => flname.Split(' ').Length > 1).WithMessage("Full name should contain name and last name seprated by space");
            RuleFor(a => a.DepartmentId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must(id => _dbContext.Departments.Any(d => d.Id == id)).WithMessage("Department is required and must exist.");
        }
    }

}
