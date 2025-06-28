using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Operational_Risk_Management.Models.View_Models.Uploader
{
    public class FilterModelForSubmissions
    {
        [ModelBinder(BinderType = typeof(CommaDelimitedModelBinder<Guid>))]
        [DefaultValue(typeof(List<Guid>), "")]
        public List<Guid> IndicatorIds { get; set; } = new List<Guid>();

        public Guid TemplateId { get; set; } = Guid.Empty;

        [ModelBinder(BinderType = typeof(CommaDelimitedModelBinder<string>))]
        [DefaultValue(typeof(List<string>), "")]
        public List<string> Statuses { get; set; } = new List<string>();

        [ModelBinder(BinderType = typeof(CommaDelimitedModelBinder<int>))]
        [DefaultValue(typeof(List<int>), "")]
        public List<int> Months { get; set; } = new List<int>();

        [ModelBinder(BinderType = typeof(CommaDelimitedModelBinder<int>))]
        [DefaultValue(typeof(List<int>), "")]
        public List<int> Years { get; set; } = new List<int>();

        public string SearchTerm { get; set; } = "";

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
