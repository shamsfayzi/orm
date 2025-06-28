using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using System;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.Incident
{
    public class StaffConcerned : BaseEntity
    {
        public Guid IncidentId { get; set; }
        public string Name { get; set; }
        public string TitleFunction { get; set; }

        // Navigation property
        public virtual Incident Incident { get; set; }
    }
}