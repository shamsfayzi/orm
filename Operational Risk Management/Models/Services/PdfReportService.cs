using DinkToPdf;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Extensions.Logging;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Interfaces.Services;
using Operational_Risk_Management.Services.Interfaces;
using System;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Operational_Risk_Management.Services
{
    public class PdfService : IPdfService
    {
        private readonly IConverter _converter;
        private readonly ILogger<PdfService> _logger;

        public PdfService(IConverter converter, ILogger<PdfService> logger)
        {
            _converter = converter;
            _logger = logger;
        }

        public async Task<byte[]> GenerateIncidentReportPdfAsync(IncidentDetailDTO incident)
        {
            try
            {
                if (incident == null)
                    return null;

                // Generate HTML content for PDF
                var htmlContent = await Task.Run(() => GenerateIncidentReportHtml(incident));

                // Create PDF document
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = DinkToPdf.ColorMode.Color,
                        Orientation = DinkToPdf.Orientation.Portrait,
                        PaperSize = DinkToPdf.PaperKind.A4,
                        Margins = new MarginSettings { Top = 25, Bottom = 25, Left = 25, Right = 25 }
                    },
                    Objects = {
                        new ObjectSettings {
                            PagesCount = true,
                            HtmlContent = htmlContent,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                            FooterSettings = { FontSize = 9, Line = true, Center = "Operational Risk Management System" }
                        }
                    }
                };

                // Convert HTML to PDF
                return _converter.Convert(doc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating incident report PDF");
                return null;
            }
        }

        public async Task<byte[]> GenerateDashboardReportPdfAsync(IncidentDashboardDTO dashboardData)
        {
            try
            {
                if (dashboardData == null)
                    return null;

                // Generate HTML content for PDF
                var htmlContent = await Task.Run(() => GenerateDashboardReportHtml(dashboardData));

                // Create PDF document
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = DinkToPdf.ColorMode.Color,
                        Orientation = DinkToPdf.Orientation.Landscape,
                        PaperSize = DinkToPdf.PaperKind.A4,
                        Margins = new MarginSettings { Top = 25, Bottom = 25, Left = 25, Right = 25 }
                    },
                    Objects = {
                        new ObjectSettings {
                            PagesCount = true,
                            HtmlContent = htmlContent,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                            FooterSettings = { FontSize = 9, Line = true, Center = "Operational Risk Management System" }
                        }
                    }
                };

                // Convert HTML to PDF
                return _converter.Convert(doc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard report PDF");
                return null;
            }
        }

        private string GenerateIncidentReportHtml(IncidentDetailDTO incident)
        {
            var sb = new StringBuilder();

            // Add CSS styles
            sb.Append(@"
            <style>
                body { font-family: Arial, sans-serif; font-size: 12px; }
                .header { text-align: center; margin-bottom: 20px; }
                .logo { max-width: 100px; max-height: 100px; }
                h1 { color: #003366; font-size: 24px; margin-bottom: 20px; }
                h2 { color: #003366; font-size: 18px; margin-top: 30px; margin-bottom: 10px; }
                .section { margin-bottom: 25px; }
                .table-container { margin-top: 10px; margin-bottom: 10px; }
                table { width: 100%; border-collapse: collapse; }
                th { background-color: #f2f2f2; text-align: left; padding: 8px; border: 1px solid #ddd; }
                td { padding: 8px; border: 1px solid #ddd; }
                .label { font-weight: bold; width: 30%; }
                .value { width: 70%; }
                .row:nth-child(even) { background-color: #f9f9f9; }
            </style>");

            // Header
            sb.Append(@"
            <div class='header'>
                <h1>Operational Risk Incident Report</h1>
            </div>");

            // Basic Information
            sb.Append(@"
            <h2>Basic Information</h2>
            <div class='section'>
                <table>
                    <tr>
                        <td class='label'>Reference Number:</td>
                        <td class='value'>").Append(incident.Id).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Report Date:</td>
                        <td class='value'>").Append(incident.ReportDate.ToString("yyyy-MM-dd")).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Status:</td>
                        <td class='value'>").Append(incident.IncidentStatus).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Title of Incident:</td>
                        <td class='value'>").Append(incident.TitleOfIncident).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Reported By:</td>
                        <td class='value'>").Append(incident.ReportedBy).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Title/Role:</td>
                        <td class='value'>").Append(incident.TitleRole).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Department/Branch/Unit:</td>
                        <td class='value'>").Append(incident.BranchDepartmentUnit).Append(@"</td>
                    </tr>
                </table>
            </div>");

            // Incident/Event Reported
            sb.Append(@"
            <h2>Incident/Event Reported</h2>
            <div class='section'>
                <table>
                    <tr>
                        <td class='label'>Start Date:</td>
                        <td class='value'>").Append(incident.StartDate.ToString("yyyy-MM-dd")).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>End Date:</td>
                        <td class='value'>").Append(incident.EndDate.ToString("yyyy-MM-dd")).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Discovery Date:</td>
                        <td class='value'>").Append(incident.DiscoveryDate.ToString("yyyy-MM-dd")).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Discovered By:</td>
                        <td class='value'>").Append(incident.DiscoveredBy).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Activity/Process:</td>
                        <td class='value'>").Append(incident.ActivityProcess).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>First Line (Risk Owner):</td>
                        <td class='value'>").Append(incident.FirstLineRiskOwner).Append(@"</td>
                    </tr>
                </table>
            </div>");

            // Staff Concerned
            sb.Append(@"
            <h2>Staff Concerned</h2>
            <div class='section'>
                <div class='table-container'>
                    <table>
                        <tr>
                            <th>Name</th>
                            <th>Title/Function</th>
                        </tr>");

            if (incident.StaffConcerned != null && incident.StaffConcerned.Count > 0)
            {
                foreach (var staff in incident.StaffConcerned)
                {
                    sb.Append(@"
                        <tr>
                            <td>").Append(staff.Name).Append(@"</td>
                            <td>").Append(staff.TitleFunction).Append(@"</td>
                        </tr>");
                }
            }
            else
            {
                sb.Append(@"
                        <tr>
                            <td colspan='2'>No staff concerned listed</td>
                        </tr>");
            }

            sb.Append(@"
                    </table>
                </div>
            </div>");

            // Incident/Event Details
            sb.Append(@"
            <h2>Incident/Event Details</h2>
            <div class='section'>
                <table>
                    <tr>
                        <td class='label'>Incident Type:</td>
                        <td class='value'>").Append(incident.IncidentType).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Incident Category:</td>
                        <td class='value'>").Append(incident.IncidentCategoryLevel).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Risk Type:</td>
                        <td class='value'>").Append(incident.RiskType).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Risk Assessment:</td>
                        <td class='value'>").Append(incident.RiskAssessment).Append(@"</td>
                    </tr>
                </table>
            </div>");

            // Financial Impact
            sb.Append(@"
            <h2>Incident/Event Impact - Financial</h2>
            <div class='section'>
                <div class='table-container'>
                    <table>
                        <tr>
                            <th></th>
                            <th>Currency</th>
                            <th>Gross Amount</th>
                            <th>Legal Costs</th>
                            <th>Other Costs</th>
                            <th>Recovery from Client</th>
                            <th>Recovery from Insurance</th>
                            <th>Net Amount</th>
                        </tr>
                        <tr>
                            <td><strong>Loss</strong></td>
                            <td>").Append(incident.FinancialLossCurrency ?? "").Append(@"</td>
                            <td>").Append(FormatAmount(incident.FinancialLossGrossAmount)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.FinancialLossLegalCosts)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.FinancialLossOtherCosts)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.FinancialLossRecoveryFromClient)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.FinancialLossRecoveryFromInsurance)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.FinancialLossNetLossToDate)).Append(@"</td>
                        </tr>
                        <tr>
                            <td><strong>Gain/Opp. Cost</strong></td>
                            <td>").Append(incident.GainCurrency ?? "").Append(@"</td>
                            <td>").Append(FormatAmount(incident.GainGrossAmount)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.GainLegalCosts)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.GainOtherCosts)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.GainRecoveryFromClient)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.GainRecoveryFromInsurance)).Append(@"</td>
                            <td>").Append(FormatAmount(incident.GainNetAmount)).Append(@"</td>
                        </tr>
                    </table>
                </div>
            </div>");

            // Summaries
            sb.Append(@"
            <h2>Summaries</h2>
            <div class='section'>
                <table>
                    <tr>
                        <td class='label'>Event/Incident Description:</td>
                        <td class='value'>").Append(incident.EventIncidentDescription).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Event/Incident Cause:</td>
                        <td class='value'>").Append(incident.EventIncidentCause).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Event/Incident Discovery:</td>
                        <td class='value'>").Append(incident.EventIncidentDiscovery).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Corrective Actions Already Implemented:</td>
                        <td class='value'>").Append(incident.CorrectiveActionsImplemented).Append(@"</td>
                    </tr>
                </table>
            </div>");

            // Controls
            sb.Append(@"
            <h2>Controls</h2>
            <div class='section'>
                <table>
                    <tr>
                        <td class='label'>Written Procedure Available:</td>
                        <td class='value'>").Append(incident.HasWrittenProcedure ? "Yes" : "No").Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Procedure Includes Controls:</td>
                        <td class='value'>").Append(incident.ProcedureIncludesControls ? "Yes" : "No").Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Existing Control Measures:</td>
                        <td class='value'>").Append(incident.ExistingControlMeasures).Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Reasons for Failure:</td>
                        <td class='value'>").Append(incident.ReasonsForFailure).Append(@"</td>
                    </tr>
                </table>
            </div>");

            // Proposed Corrective Measures
            sb.Append(@"
            <h2>Proposed Corrective Measures</h2>
            <div class='section'>
                <table>
                    <tr>
                        <td class='label'>Policies & Procedures:</td>
                        <td class='value'>").Append(incident.ProposedPoliciesProcedures ?? "N/A").Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Controls:</td>
                        <td class='value'>").Append(incident.ProposedControls ?? "N/A").Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Human Resources:</td>
                        <td class='value'>").Append(incident.ProposedHumanResources ?? "N/A").Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Systems:</td>
                        <td class='value'>").Append(incident.ProposedSystems ?? "N/A").Append(@"</td>
                    </tr>
                    <tr>
                        <td class='label'>Others:</td>
                        <td class='value'>").Append(incident.ProposedOthers ?? "N/A").Append(@"</td>
                    </tr>
                </table>
            </div>");

            // Other Comments
            sb.Append(@"
            <h2>Other Comments</h2>
            <div class='section'>
                <p>").Append(incident.OtherComments).Append(@"</p>
            </div>");

            // Risk Management Review (if applicable)
            if (!string.IsNullOrEmpty(incident.RiskManagementNotes) || incident.LastReviewDate.HasValue)
            {
                sb.Append(@"
                <h2>Risk Management Review</h2>
                <div class='section'>
                    <table>");

                if (incident.LastReviewDate.HasValue)
                {
                    sb.Append(@"
                        <tr>
                            <td class='label'>Review Date:</td>
                            <td class='value'>").Append(incident.LastReviewDate.Value.ToString("yyyy-MM-dd")).Append(@"</td>
                        </tr>
                        <tr>
                            <td class='label'>Reviewed By:</td>
                            <td class='value'>").Append(incident.ReviewedBy ?? "N/A").Append(@"</td>
                        </tr>");
                }

                if (!string.IsNullOrEmpty(incident.RiskManagementNotes))
                {
                    sb.Append(@"
                        <tr>
                            <td class='label'>Notes:</td>
                            <td class='value'>").Append(incident.RiskManagementNotes).Append(@"</td>
                        </tr>
                        <tr>
                            <td class='label'>Requires Revision:</td>
                            <td class='value'>").Append(incident.RequiresRevision ? "Yes" : "No").Append(@"</td>
                        </tr>");
                }

                sb.Append(@"
                    </table>
                </div>");
            }

            // Supporting Documents
            sb.Append(@"
            <h2>Supporting Documents</h2>
            <div class='section'>");

            if (incident.Documents != null && incident.Documents.Count > 0)
            {
                sb.Append(@"
                <div class='table-container'>
                    <table>
                        <tr>
                            <th>File Name</th>
                            <th>File Type</th>
                            <th>Size</th>
                            <th>Upload Date</th>
                        </tr>");

                // Services/PdfService.cs (continued)
                foreach (var doc in incident.Documents)
                {
                    sb.Append(@"
                        <tr>
                            <td>").Append(doc.FileName).Append(@"</td>
                            <td>").Append(doc.FileType).Append(@"</td>
                            <td>").Append(FormatFileSize(doc.FileSize)).Append(@"</td>
                            <td>").Append(doc.CreateDate.ToString("yyyy-MM-dd")).Append(@"</td>
                        </tr>");
                }

                sb.Append(@"
                    </table>
                </div>");
            }
            else
            {
                sb.Append(@"<p>No supporting documents attached.</p>");
            }

            sb.Append(@"
            </div>");

            // Footer - Metadata
            sb.Append(@"
            <div style='margin-top: 50px; font-size: 9px;'>
                <p>Generated: ").Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")).Append(@"</p>
                <p>Reference: ").Append(incident.Id).Append(@"</p>
            </div>");

            return sb.ToString();
        }

        private string GenerateDashboardReportHtml(IncidentDashboardDTO dashboardData)
        {
            var sb = new StringBuilder();

            // Add CSS styles
            sb.Append(@"
            <style>
                body { font-family: Arial, sans-serif; font-size: 12px; }
                .header { text-align: center; margin-bottom: 20px; }
                h1 { color: #003366; font-size: 24px; margin-bottom: 20px; }
                h2 { color: #003366; font-size: 18px; margin-top: 30px; margin-bottom: 10px; }
                .section { margin-bottom: 25px; }
                .table-container { margin-top: 10px; margin-bottom: 10px; }
                table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
                th { background-color: #f2f2f2; text-align: left; padding: 8px; border: 1px solid #ddd; }
                td { padding: 8px; border: 1px solid #ddd; }
                .summary-box { margin-bottom: 20px; }
                .summary-grid { display: grid; grid-template-columns: 1fr 1fr 1fr 1fr; grid-gap: 15px; }
                .summary-item { border: 1px solid #ddd; padding: 15px; text-align: center; background-color: #f9f9f9; }
                .summary-number { font-size: 24px; font-weight: bold; color: #003366; }
                .summary-label { font-size: 14px; color: #666; }
            </style>");

            // Header
            sb.Append(@"
            <div class='header'>
                <h1>Operational Risk Management Dashboard Report</h1>
                <p>Generated on ").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Append(@"</p>
            </div>");

            // Summary Statistics
            sb.Append(@"
            <div class='summary-box'>
                <div class='summary-grid'>
                    <div class='summary-item'>
                        <div class='summary-number'>").Append(dashboardData.TotalIncidents).Append(@"</div>
                        <div class='summary-label'>Total Incidents</div>
                    </div>
                    <div class='summary-item'>
                        <div class='summary-number'>").Append(dashboardData.OpenIncidents).Append(@"</div>
                        <div class='summary-label'>Open Incidents</div>
                    </div>
                    <div class='summary-item'>
                        <div class='summary-number'>").Append(dashboardData.ClosedIncidents).Append(@"</div>
                        <div class='summary-label'>Closed Incidents</div>
                    </div>
                    <div class='summary-item'>
                        <div class='summary-number'>").Append(dashboardData.RequiringRevisionIncidents).Append(@"</div>
                        <div class='summary-label'>Requiring Revision</div>
                    </div>
                </div>
            </div>");

            // Incidents by Department
            sb.Append(@"
            <h2>Incidents by Department</h2>
            <div class='section'>
                <div class='table-container'>
                    <table>
                        <tr>
                            <th>Department</th>
                            <th>Count</th>
                        </tr>");

            if (dashboardData.IncidentsByDepartment != null && dashboardData.IncidentsByDepartment.Count > 0)
            {
                foreach (var dept in dashboardData.IncidentsByDepartment)
                {
                    sb.Append(@"
                        <tr>
                            <td>").Append(dept.Key).Append(@"</td>
                            <td>").Append(dept.Value).Append(@"</td>
                        </tr>");
                }
            }
            else
            {
                sb.Append(@"
                        <tr>
                            <td colspan='2'>No data available</td>
                        </tr>");
            }

            sb.Append(@"
                    </table>
                </div>
            </div>");

            // Incidents by Category
            sb.Append(@"
            <h2>Incidents by Category</h2>
            <div class='section'>
                <div class='table-container'>
                    <table>
                        <tr>
                            <th>Category</th>
                            <th>Count</th>
                        </tr>");

            if (dashboardData.IncidentsByCategory != null && dashboardData.IncidentsByCategory.Count > 0)
            {
                foreach (var category in dashboardData.IncidentsByCategory)
                {
                    sb.Append(@"
                        <tr>
                            <td>").Append(category.Key).Append(@"</td>
                            <td>").Append(category.Value).Append(@"</td>
                        </tr>");
                }
            }
            else
            {
                sb.Append(@"
                        <tr>
                            <td colspan='2'>No data available</td>
                        </tr>");
            }

            sb.Append(@"
                    </table>
                </div>
            </div>");

            // Incidents by Risk Level
            sb.Append(@"
            <h2>Incidents by Risk Level</h2>
            <div class='section'>
                <div class='table-container'>
                    <table>
                        <tr>
                            <th>Risk Level</th>
                            <th>Count</th>
                        </tr>");

            if (dashboardData.IncidentsByRiskLevel != null && dashboardData.IncidentsByRiskLevel.Count > 0)
            {
                foreach (var riskLevel in dashboardData.IncidentsByRiskLevel)
                {
                    sb.Append(@"
                        <tr>
                            <td>").Append(riskLevel.Key).Append(@"</td>
                            <td>").Append(riskLevel.Value).Append(@"</td>
                        </tr>");
                }
            }
            else
            {
                sb.Append(@"
                        <tr>
                            <td colspan='2'>No data available</td>
                        </tr>");
            }

            sb.Append(@"
                    </table>
                </div>
            </div>");

            // Incidents Trend
            sb.Append(@"
            <h2>Incidents Trend</h2>
            <div class='section'>
                <div class='table-container'>
                    <table>
                        <tr>
                            <th>Period</th>
                            <th>Count</th>
                        </tr>");

            if (dashboardData.IncidentsTrend != null && dashboardData.IncidentsTrend.Count > 0)
            {
                foreach (var trend in dashboardData.IncidentsTrend)
                {
                    sb.Append(@"
                        <tr>
                            <td>").Append(trend.Key).Append(@"</td>
                            <td>").Append(trend.Value).Append(@"</td>
                        </tr>");
                }
            }
            else
            {
                sb.Append(@"
                        <tr>
                            <td colspan='2'>No data available</td>
                        </tr>");
            }

            sb.Append(@"
                    </table>
                </div>
            </div>");

            // Footer
            sb.Append(@"
            <div style='margin-top: 30px; text-align: center; font-size: 10px;'>
                <p>Confidential - For internal use only</p>
                <p>Operational Risk Management System</p>
            </div>");

            return sb.ToString();
        }

        private string FormatAmount(decimal? amount)
        {
            if (amount.HasValue)
            {
                return amount.Value.ToString("N2");
            }
            return "0.00";
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{Math.Round(size, 2)} {sizes[order]}";
        }
    }
}
