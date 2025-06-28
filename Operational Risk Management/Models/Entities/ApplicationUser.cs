using System.ComponentModel;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Entities
{
    public class ApplicationUser : BaseEntity, ISoftDelete
    {
        [DisplayName("Full Name")]
        public string? FullName { get; set; }


        [DisplayName("Disabled")]
        public bool Disabled { get; set; }
        [DisplayName("Department")]
        public Guid? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        [DisplayName("User Name")]
        public string? UserName { get; set; }

        [DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }
        [DisplayName("Access to KRI Submission ")]
        public bool KriSubmissionAccess { get; set; } = false;
        [DisplayName("Phone 1")]
        public string? Phone1 { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}