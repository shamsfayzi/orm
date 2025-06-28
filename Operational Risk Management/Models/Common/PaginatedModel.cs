using Operational_Risk_Management.Models.Context;
using System.Collections.Generic;

namespace Operational_Risk_Management.Models.Common
{
    public class PaginatedModel<T> where T : BaseEntity
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool IsFiltered { get; set; } = false;
        public string Search { get; set; }
        public T Filter { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public Pagination Pagination { get; set; }
        public int? SelectedMonth { get; set; }
        public int? SelectedYear { get; set; }
    }
}
