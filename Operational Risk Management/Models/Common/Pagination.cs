namespace Operational_Risk_Management.Models.Common
{
    public class Pagination
    {
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPage { get; set; }
        public int PageSize { get; set; } = 20;
        public string TotalRecords { get; set; }
        public int CurrentRecords { get; set; }
        public bool IgnorePagination { get; set; }
    }

}
