using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models.View_Models.Uploader;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Operational_Risk_Management.Models.Filtering
{
    /// <summary>
    /// Admin-specific filter model that extends the unified filter model with admin-only filtering capabilities
    /// </summary>
    public class AdminSubmissionFilterModel : FilterModelForSubmissions
    {
        // Admin-specific filtering fields
        [ModelBinder(BinderType = typeof(CommaDelimitedModelBinder<Guid>))]
        [DefaultValue(typeof(List<Guid>), "")]
        public List<Guid> DepartmentIds { get; set; } = new List<Guid>();

        public ThresholdStatus BreachStatus { get; set; } = ThresholdStatus.None;
    }
    public enum ThresholdStatus
    {
        None,
        Breached,
        NotBreached,
        TolerableBreachesExceeded
    }
}
