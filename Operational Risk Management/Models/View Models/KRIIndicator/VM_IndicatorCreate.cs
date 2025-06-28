using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Operational_Risk_Management.Models.View_Models.KRIIndicator // Corrected namespace
{
    public class VM_IndicatorCreate : IValidationModel // Renamed class
    {
        [Required]
        public Guid TemplateId { get; set; }
        [Required]
        [DisplayName("KRI Type")]
        public string KRIType { get; set; }
        [Required]
        [DisplayName("Reference No")]
        public string RfNo { get; set; }
        [DisplayName("Process")]
        public string? Process { get; set; }
        
        [DisplayName("Risk Area")]

        public string? RiskArea { get; set; }    
        [Required]
        [DisplayName("Indicator ")] // Changed DisplayName to match entity/other VM
        public string IndicatorName { get; set; } // Renamed property to match entity/other VM
        [Required]
        [DisplayName("Frequency of Review")]
        public string FrequencyOfReview { get; set; } = "Monthly";
        [Required]
        [DisplayName("Total Process")]
        public int? TotalProcess { get; set; }
        [Required]
        [DisplayName("Tolerable Breaches")] // Corrected DisplayName
        public int TolerableBreaches { get; set; } = 0;

        // Added IsActive to match the entity and create form
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; } = true;
    }
    public class VLD_IndicatorCreate :  AbstractValidatorWithDbContext<VM_IndicatorCreate> // Renamed class and generic type
    {
        // Constructor now matches the base class, accepting HttpContext
        public VLD_IndicatorCreate(HttpContext context) : base(context)
        {
                RuleFor(a => a.TemplateId).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().Must( (id, cancellation) =>   _dbContext.Templates.Any(t => t.Id== id.TemplateId && !t.IsDeleted)).WithMessage("Selected Template does not exist.");

            // Add rules for other properties
            //RuleFor(x => x.KRIType)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.");

            //RuleFor(x => x.RfNo)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");
            //// Add uniqueness check if needed:
            //// .MustAsync(async (rfNo, cancellation) => !await _dbContext.Indicators.AnyAsync(i => i.RfNo == rfNo && !i.IsDeleted, cancellation))
            //// .WithMessage("{PropertyName} must be unique.");

            //RuleFor(x => x.IndicatorName) // Use renamed property
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(250).WithMessage("{PropertyName} cannot exceed 250 characters.");

            //RuleFor(x => x.FrequencyOfReview)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");

            //RuleFor(x => x.TolerableBreaches)
            //    .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.");

            // Keep the rule for Process/RiskArea if still applicable
            RuleFor(a => a)
            .Must(model =>
                (!string.IsNullOrWhiteSpace(model.Process) != (!string.IsNullOrWhiteSpace(model.RiskArea))))
            .WithMessage("Exactly one of Process or Risk Area must be filled (not both, not neither).");


            // check if a department already has a template
            //RuleFor(a=> a.DepartmentId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, departmentId) => _dbContext.Templates.IsUnique(model.DepartmentId, "DepartmentId",departmentId)).WithMessage("Department already exists.");

        }
    }
}
