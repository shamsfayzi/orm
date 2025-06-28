using Operational_Risk_Management.Models.Entities;

namespace Operational_Risk_Management.Models.View_Models.KRITemplates
{
    public class VM_Template
    {
        public string FocalPoint { get; set; }
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}
