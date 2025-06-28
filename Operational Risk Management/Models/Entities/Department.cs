using System.ComponentModel;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Entities;

public class Department : BaseEntity, ISoftDelete
{
    [DisplayName("Department Name")]
    public string? DepartmentName { get; set; }
    [DisplayName("Description")]
    public string Description { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }

}
