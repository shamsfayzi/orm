using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Operational_Risk_Management.Models.View_Models.KRIIndicator
{
    public class VM_IndicatorDetails
    {
        public Models.Entities.Indicator Indicator { get; set; }
        public PaginatedModel<Submission> Submissions { get; set; }
        public IEnumerable<Models.Entities.Indicator> SiblingIndicators { get; set; } // For the dropdown
        public string TemplateFocalPoint { get; set; } // Optional: To display template info

        // Filtering and Searching Properties
        public string SearchTerm { get; set; }
        public int? SelectedYear { get; set; }
        public int? SelectedMonth { get; set; }

        public IEnumerable<SelectListItem> YearOptions { get; set; }
        public IEnumerable<SelectListItem> MonthOptions { get; set; }
    }
}