using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Operational_Risk_Management.Models.View_Models.KRIIndicator // Corrected namespace
{
    public class VM_IndicatorEdit : IValidationModel // Renamed class
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid TemplateId { get; set; }
        [Required]
        [DisplayName("KRI Type")]
        public string KRIType { get; set; }
        [Required]
        [DisplayName("Reference No")]
        public string RfNo { get; set; }
        //[Required]
        [DisplayName("Process")]
        public string? Process { get; set; }
        //[Required]
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

        // Added IsActive to match the entity and edit form
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
    }
    public class VLD_IndicatorEdit : AbstractValidatorWithDbContext<VM_IndicatorEdit> // Renamed class and generic type
    {
        // Constructor now matches the base class, accepting HttpContext
        public VLD_IndicatorEdit(HttpContext context) : base(context)
        {
            RuleFor(a => a.TemplateId).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().MustAsync(async (id, cancellation) => await _dbContext.Templates.AnyAsync(t => t.Id == id && !t.IsDeleted, cancellation)).WithMessage("Selected Template does not exist.");
            RuleFor(a => a)
            .Must(model =>
                (!string.IsNullOrWhiteSpace(model.Process) != (!string.IsNullOrWhiteSpace(model.RiskArea))))
            .WithMessage("Exactly one of Process or Risk Area must be filled (not both, not neither).");
            //// Add rules for other properties to match VLD_IndicatorCreate (adjusting for edit context)
            //RuleFor(x => x.KRIType)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.");

            //RuleFor(x => x.RfNo)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");
            //    // Add uniqueness check if needed, excluding the current item:
            //    // .MustAsync(async (model, rfNo, cancellation) => 
            //    //     !await _dbContext.Indicators.AnyAsync(i => i.RfNo == rfNo && i.Id != model.Id && !i.IsDeleted, cancellation))
            //    // .WithMessage("{PropertyName} must be unique.");

            //RuleFor(x => x.IndicatorName) // Use renamed property
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(250).WithMessage("{PropertyName} cannot exceed 250 characters.");

            //RuleFor(x => x.FrequencyOfReview)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");

            //RuleFor(x => x.TolerableBreaches)
            //    .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.");//// Add rules for other properties to match VLD_IndicatorCreate (adjusting for edit context)
            //RuleFor(x => x.KRIType)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.");

            //RuleFor(x => x.RfNo)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");
            //    // Add uniqueness check if needed, excluding the current item:
            //    // .MustAsync(async (model, rfNo, cancellation) => 
            //    //     !await _dbContext.Indicators.AnyAsync(i => i.RfNo == rfNo && i.Id != model.Id && !i.IsDeleted, cancellation))
            //    // .WithMessage("{PropertyName} must be unique.");

            //RuleFor(x => x.IndicatorName) // Use renamed property
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(250).WithMessage("{PropertyName} cannot exceed 250 characters.");

            //RuleFor(x => x.FrequencyOfReview)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");

            //RuleFor(x => x.TolerableBreaches)
            //    .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.");


            // check if a department already has a template
            //RuleFor(a=> a.DepartmentId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, departmentId) => _dbContext.Templates.IsUnique(model.DepartmentId, "DepartmentId",departmentId)).WithMessage("Department already exists.");
        }
    }
}
