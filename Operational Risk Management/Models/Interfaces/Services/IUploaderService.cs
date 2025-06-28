using Operational_Risk_Management.Models.View_Models.Uploader;

namespace Operational_Risk_Management.Models.Interfaces.Services
{
    public interface IUploaderService
    {
        //Task<VM_UploaderDashboard> BuildDashboardAsync(string username);
        Task<List<VM_Submission>> SubmissionVM(FilterModelForSubmissions model);
        //Task<List<IndicatorViewModel>> GetOpenForSubmissionIndicators(string focalPoint);
        //Task<FilteredSubmissions> GetSubmissionByStatus(string focalPont,string status);
    }
}
