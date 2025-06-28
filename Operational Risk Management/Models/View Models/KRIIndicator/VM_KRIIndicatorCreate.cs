using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.View_Models.Indicator
{
    public class VM_KRIIndicatorCreate : IValidationModel
    {
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
        public string Process { get; set; }
        //[Required]
        [DisplayName("Risk Area")]

        public string RiskArea { get; set; }
        [Required]
        [DisplayName("Indicator")]
        public string Indicator { get; set; }
        [Required]
        [DisplayName("Frequency of Review")]
        public string FrequencyOfReview { get; set; } = "Monthly";
        [Required]
        [DisplayName("Total Process")]
        public int? TotalProcess { get; set; }
        [Required]
        [DisplayName("Breach Treshold")]
        public int TolerableBreaches { get; set; } = 0;
    }
    public class VLD_KRIIndicatorCreate :  AbstractValidatorWithDbContext<VM_KRIIndicatorCreate>
    {
        public VLD_KRIIndicatorCreate(HttpContext context) : base(context)
        {
        RuleFor(a => a.TemplateId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, templateId) => _dbContext.Templates.IsExists(model.TemplateId)).WithMessage("Invalid request");
            RuleFor(a => a)
                .Must(model => !string.IsNullOrWhiteSpace(model.Process) || !string.IsNullOrWhiteSpace(model.RiskArea))
                .WithMessage("Either Process or Risk Area must be provided.");


            // check if a department already has a template
            //RuleFor(a=> a.DepartmentId).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Must((model, departmentId) => _dbContext.Templates.IsUnique(model.DepartmentId, "DepartmentId",departmentId)).WithMessage("Department already exists.");
        }
    }
}
