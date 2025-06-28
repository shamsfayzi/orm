using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Entities
{
    public class Template : BaseEntity, ISoftDelete
    {
        public string? FocalPoint { get; set; }
        public Guid ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Indicator> Indicators { get; set; } = [];
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}
