using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.Incident
{
    public class IncidentDocument : BaseEntity,ISoftDelete
    {
        public Guid IncidentId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; }

        public long FileSize { get; set; }

        public string Description { get; set; }

        // Navigation property
        public virtual Incident Incident { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}
