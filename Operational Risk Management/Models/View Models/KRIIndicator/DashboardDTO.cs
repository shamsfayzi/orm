namespace Operational_Risk_Management.Models.View_Models.KRIIndicator
{

    public class KRIDashboardDataDTO
    {
        public int AllSubmissions { get; set; }
        public int BreachedSubmissions { get; set; }
        public int OverDueSubmissions { get; set; }
        public int RevissionSubmissions { get; set; }
        public List<DepartmentCountResult> MostBreachedByDepartment { get; set; } // get two most  breached by department
        public List<IndicatorCountResult> MostBreachedByIndicator { get; set; } // get 3 most breched indicator 
    }
    public class DepartmentCountResult
    {
        public string Department { get; set; }
        public int Count { get; set; }
    }
    public class IndicatorCountResult
    {
        public string IndicatorName { get; set; }
        public int Count { get; set; }
    }


}
