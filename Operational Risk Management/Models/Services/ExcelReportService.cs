using ClosedXML.Excel;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Interfaces.Services;

namespace Operational_Risk_Management.Services
{
    public class ExcelService : IExcelService
    {
        private readonly ILogger<ExcelService> _logger;

        public ExcelService(ILogger<ExcelService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> GenerateIncidentReportExcelAsync(List<IncidentListDTO> incidents)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Incidents");

                        // Set column headers with styling
                        var headerRow = worksheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        worksheet.Cell(1, 1).Value = "ID";
                        worksheet.Cell(1, 2).Value = "Title";
                        worksheet.Cell(1, 3).Value = "Department/Branch/Unit";
                        worksheet.Cell(1, 4).Value = "Report Date";
                        worksheet.Cell(1, 5).Value = "Status";
                        worksheet.Cell(1, 6).Value = "Type";
                        worksheet.Cell(1, 7).Value = "Category";
                        worksheet.Cell(1, 8).Value = "Risk Assessment";
                        worksheet.Cell(1, 9).Value = "Requires Revision";

                        // Auto-fit columns
                        worksheet.Column(1).Width = 36; // ID column (GUID)
                        worksheet.Column(2).Width = 30; // Title
                        worksheet.Column(3).Width = 25; // Department
                        worksheet.Column(4).Width = 12; // Date
                        worksheet.Column(5).Width = 12; // Status
                        worksheet.Column(6).Width = 15; // Type
                        worksheet.Column(7).Width = 25; // Category
                        worksheet.Column(8).Width = 15; // Risk Assessment
                        worksheet.Column(9).Width = 15; // Requires Revision

                        // Add data rows
                        int row = 2;
                        foreach (var incident in incidents)
                        {
                            worksheet.Cell(row, 1).Value = incident.Id.ToString();
                            worksheet.Cell(row, 2).Value = incident.TitleOfIncident;
                            worksheet.Cell(row, 3).Value = incident.BranchDepartmentUnit;
                            worksheet.Cell(row, 4).Value = incident.ReportDate.ToString("yyyy-MM-dd");
                            worksheet.Cell(row, 5).Value = incident.IncidentStatus.ToString();
                            worksheet.Cell(row, 6).Value = incident.IncidentType.ToString();
                            worksheet.Cell(row, 7).Value = incident.IncidentCategoryLevel.ToString();
                            worksheet.Cell(row, 8).Value = incident.RiskAssessment.ToString();
                            worksheet.Cell(row, 9).Value = incident.RequiresRevision ? "Yes" : "No";

                            // Style alternating rows
                            if (row % 2 == 0)
                            {
                                worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                            }

                            row++;
                        }

                        // Create table
                        var table = worksheet.Range(1, 1, incidents.Count + 1, 9).CreateTable();
                        table.Theme = XLTableTheme.TableStyleMedium2;

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return stream.ToArray();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating incident report Excel");
                return null;
            }
        }

        public async Task<byte[]> GenerateIncidentDetailExcelAsync(IncidentDetailDTO incident)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Incident Report");

                        // Set header style
                        var headerStyle = workbook.Style;
                        headerStyle.Font.Bold = true;
                        headerStyle.Fill.BackgroundColor = XLColor.LightGray;

                        // Set title
                        worksheet.Cell("A1").Value = "Operational Risk Incident Report";
                        worksheet.Range("A1:I1").Merge();
                        worksheet.Cell("A1").Style.Font.Bold = true;
                        worksheet.Cell("A1").Style.Font.FontSize = 16;
                        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Basic Information
                        int currentRow = 3;
                        worksheet.Cell(currentRow, 1).Value = "Basic Information";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Add basic info data
                        AddLabelValuePair(worksheet, ref currentRow, "Reference Number", incident.Id.ToString());
                        AddLabelValuePair(worksheet, ref currentRow, "Report Date", incident.ReportDate.ToString("yyyy-MM-dd"));
                        AddLabelValuePair(worksheet, ref currentRow, "Status", incident.IncidentStatus.ToString());
                        AddLabelValuePair(worksheet, ref currentRow, "Title of Incident", incident.TitleOfIncident);
                        AddLabelValuePair(worksheet, ref currentRow, "Reported By", incident.ReportedBy);
                        AddLabelValuePair(worksheet, ref currentRow, "Title/Role", incident.TitleRole);
                        AddLabelValuePair(worksheet, ref currentRow, "Department/Branch/Unit", incident.BranchDepartmentUnit);

                        // Add spacing
                        currentRow += 1;

                        // Incident/Event Reported
                        worksheet.Cell(currentRow, 1).Value = "Incident/Event Reported";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Add incident data
                        AddLabelValuePair(worksheet, ref currentRow, "Start Date", incident.StartDate.ToString("yyyy-MM-dd"));
                        AddLabelValuePair(worksheet, ref currentRow, "End Date", incident.EndDate.ToString("yyyy-MM-dd"));
                        AddLabelValuePair(worksheet, ref currentRow, "Discovery Date", incident.DiscoveryDate.ToString("yyyy-MM-dd"));
                        AddLabelValuePair(worksheet, ref currentRow, "Discovered By", incident.DiscoveredBy);
                        AddLabelValuePair(worksheet, ref currentRow, "Activity/Process", incident.ActivityProcess);
                        AddLabelValuePair(worksheet, ref currentRow, "First Line (Risk Owner)", incident.FirstLineRiskOwner);

                        // Add spacing
                        currentRow += 1;

                        // Staff Concerned
                        worksheet.Cell(currentRow, 1).Value = "Staff Concerned";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Staff table headers
                        worksheet.Cell(currentRow, 1).Value = "Name";
                        worksheet.Cell(currentRow, 2).Value = "Title/Function";
                        worksheet.Range(currentRow, 1, currentRow, 2).Style.Font.Bold = true;
                        worksheet.Range(currentRow, 1, currentRow, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                        currentRow++;

                        // Staff data
                        if (incident.StaffConcerned != null && incident.StaffConcerned.Count > 0)
                        {
                            foreach (var staff in incident.StaffConcerned)
                            {
                                worksheet.Cell(currentRow, 1).Value = staff.Name;
                                worksheet.Cell(currentRow, 2).Value = staff.TitleFunction;
                                currentRow++;
                            }
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 1).Value = "No staff concerned listed";
                            worksheet.Range(currentRow, 1, currentRow, 2).Merge();
                            currentRow++;
                        }

                        // Add spacing
                        currentRow += 1;

                        // Incident/Event Details
                        worksheet.Cell(currentRow, 1).Value = "Incident/Event Details";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Add incident details
                        AddLabelValuePair(worksheet, ref currentRow, "Incident Type", incident.IncidentType.ToString());
                        AddLabelValuePair(worksheet, ref currentRow, "Incident Category", incident.IncidentCategoryLevel.ToString());
                        AddLabelValuePair(worksheet, ref currentRow, "Risk Type", incident.RiskType.ToString());
                        AddLabelValuePair(worksheet, ref currentRow, "Risk Assessment", incident.RiskAssessment.ToString());

                        // Add spacing
                        currentRow += 1;

                        // Financial Impact
                        worksheet.Cell(currentRow, 1).Value = "Incident/Event Impact - Financial";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Financial table headers
                        worksheet.Cell(currentRow, 1).Value = "";
                        worksheet.Cell(currentRow, 2).Value = "Currency";
                        worksheet.Cell(currentRow, 3).Value = "Gross Amount";
                        worksheet.Cell(currentRow, 4).Value = "Legal Costs";
                        worksheet.Cell(currentRow, 5).Value = "Other Costs";
                        worksheet.Cell(currentRow, 6).Value = "Recovery from Client";
                        worksheet.Cell(currentRow, 7).Value = "Recovery from Insurance";
                        worksheet.Cell(currentRow, 8).Value = "Net Amount";
                        worksheet.Range(currentRow, 1, currentRow, 8).Style.Font.Bold = true;
                        worksheet.Range(currentRow, 1, currentRow, 8).Style.Fill.BackgroundColor = XLColor.LightGray;
                        currentRow++;

                        // Financial data - Loss
                        worksheet.Cell(currentRow, 1).Value = "Loss";
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 2).Value = incident.FinancialLossCurrency ?? "";
                        worksheet.Cell(currentRow, 3).Value = incident.FinancialLossGrossAmount ?? 0;
                        worksheet.Cell(currentRow, 4).Value = incident.FinancialLossLegalCosts ?? 0;
                        worksheet.Cell(currentRow, 5).Value = incident.FinancialLossOtherCosts ?? 0;
                        worksheet.Cell(currentRow, 6).Value = incident.FinancialLossRecoveryFromClient ?? 0;
                        worksheet.Cell(currentRow, 7).Value = incident.FinancialLossRecoveryFromInsurance ?? 0;
                        worksheet.Cell(currentRow, 8).Value = incident.FinancialLossNetLossToDate ?? 0;

                        // Format financial cells
                        worksheet.Range(currentRow, 3, currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                        currentRow++;

                        // Financial data - Gain
                        worksheet.Cell(currentRow, 1).Value = "Gain/Opp. Cost";
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 2).Value = incident.GainCurrency ?? "";
                        worksheet.Cell(currentRow, 3).Value = incident.GainGrossAmount ?? 0;
                        worksheet.Cell(currentRow, 4).Value = incident.GainLegalCosts ?? 0;
                        worksheet.Cell(currentRow, 5).Value = incident.GainOtherCosts ?? 0;
                        worksheet.Cell(currentRow, 6).Value = incident.GainRecoveryFromClient ?? 0;
                        worksheet.Cell(currentRow, 7).Value = incident.GainRecoveryFromInsurance ?? 0;
                        worksheet.Cell(currentRow, 8).Value = incident.GainNetAmount ?? 0;

                        // Format financial cells
                        worksheet.Range(currentRow, 3, currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                        currentRow += 2;

                        // Summaries
                        worksheet.Cell(currentRow, 1).Value = "Summaries";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Add summaries
                        AddLabelValuePair(worksheet, ref currentRow, "Event/Incident Description", incident.EventIncidentDescription);
                        AddLabelValuePair(worksheet, ref currentRow, "Event/Incident Cause", incident.EventIncidentCause);
                        AddLabelValuePair(worksheet, ref currentRow, "Event/Incident Discovery", incident.EventIncidentDiscovery);
                        AddLabelValuePair(worksheet, ref currentRow, "Corrective Actions Already Implemented", incident.CorrectiveActionsImplemented);

                        // Add spacing
                        currentRow += 1;

                        // Controls
                        worksheet.Cell(currentRow, 1).Value = "Controls";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Add controls
                        AddLabelValuePair(worksheet, ref currentRow, "Written Procedure Available", incident.HasWrittenProcedure ? "Yes" : "No");
                        AddLabelValuePair(worksheet, ref currentRow, "Procedure Includes Controls", incident.ProcedureIncludesControls ? "Yes" : "No");
                        AddLabelValuePair(worksheet, ref currentRow, "Existing Control Measures", incident.ExistingControlMeasures);
                        AddLabelValuePair(worksheet, ref currentRow, "Reasons for Failure", incident.ReasonsForFailure);

                        // Add spacing
                        currentRow += 1;

                        // Proposed Corrective Measures
                        worksheet.Cell(currentRow, 1).Value = "Proposed Corrective Measures";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Add proposed measures
                        AddLabelValuePair(worksheet, ref currentRow, "Policies & Procedures", incident.ProposedPoliciesProcedures ?? "N/A");
                        AddLabelValuePair(worksheet, ref currentRow, "Controls", incident.ProposedControls ?? "N/A");
                        AddLabelValuePair(worksheet, ref currentRow, "Human Resources", incident.ProposedHumanResources ?? "N/A");
                        AddLabelValuePair(worksheet, ref currentRow, "Systems", incident.ProposedSystems ?? "N/A");
                        AddLabelValuePair(worksheet, ref currentRow, "Others", incident.ProposedOthers ?? "N/A");

                        // Add spacing
                        currentRow += 1;

                        // Other Comments
                        worksheet.Cell(currentRow, 1).Value = "Other Comments";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Add comments
                        worksheet.Cell(currentRow, 1).Value = incident.OtherComments;
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Alignment.WrapText = true;
                        currentRow += 2;

                        // Risk Management Review (if applicable)
                        if (!string.IsNullOrEmpty(incident.RiskManagementNotes) || incident.LastReviewDate.HasValue)
                        {
                            worksheet.Cell(currentRow, 1).Value = "Risk Management Review";
                            worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                            currentRow += 1;

                            if (incident.LastReviewDate.HasValue)
                            {
                                AddLabelValuePair(worksheet, ref currentRow, "Review Date", incident.LastReviewDate.Value.ToString("yyyy-MM-dd"));
                                AddLabelValuePair(worksheet, ref currentRow, "Reviewed By", incident.ReviewedBy ?? "N/A");
                            }

                            if (!string.IsNullOrEmpty(incident.RiskManagementNotes))
                            {
                                AddLabelValuePair(worksheet, ref currentRow, "Notes", incident.RiskManagementNotes);
                                AddLabelValuePair(worksheet, ref currentRow, "Requires Revision", incident.RequiresRevision ? "Yes" : "No");
                            }

                            // Add spacing
                            currentRow += 1;
                        }

                        // Supporting Documents
                        worksheet.Cell(currentRow, 1).Value = "Supporting Documents";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                        currentRow += 1;

                        // Document table
                        if (incident.Documents != null && incident.Documents.Count > 0)
                        {
                            worksheet.Cell(currentRow, 1).Value = "File Name";
                            worksheet.Cell(currentRow, 2).Value = "File Type";
                            worksheet.Cell(currentRow, 3).Value = "Size";
                            worksheet.Cell(currentRow, 4).Value = "Upload Date";
                            worksheet.Range(currentRow, 1, currentRow, 4).Style.Font.Bold = true;
                            worksheet.Range(currentRow, 1, currentRow, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                            currentRow++;

                            foreach (var doc in incident.Documents)
                            {
                                worksheet.Cell(currentRow, 1).Value = doc.FileName;
                                worksheet.Cell(currentRow, 2).Value = doc.FileType;
                                worksheet.Cell(currentRow, 3).Value = FormatFileSize(doc.FileSize);
                                worksheet.Cell(currentRow, 4).Value = doc.CreateDate.ToString("yyyy-MM-dd");
                                currentRow++;
                            }
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 1).Value = "No supporting documents attached.";
                            worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                            currentRow++;
                        }

                        // Add footer
                        currentRow += 2;
                        worksheet.Cell(currentRow, 1).Value = $"Generated: {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")}";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;

                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = $"Reference: {incident.Id}";
                        worksheet.Range(currentRow, 1, currentRow, 9).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;

                        // Auto-fit columns for better readability
                        worksheet.Columns().AdjustToContents();

                        // Set page settings for printing
                        worksheet.PageSetup.Margins.Top = 0.5;
                        worksheet.PageSetup.Margins.Bottom = 0.5;
                        worksheet.PageSetup.Margins.Left = 0.5;
                        worksheet.PageSetup.Margins.Right = 0.5;
                        worksheet.PageSetup.Margins.Header = 0.25;
                        worksheet.PageSetup.Margins.Footer = 0.25;
                        worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                        worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;

                        // Create memory stream and save workbook
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return stream.ToArray();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Log error (you might want to add proper error logging here)
                Console.WriteLine($"Error generating Excel: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
            }
        }

        private void AddLabelValuePair(IXLWorksheet worksheet, ref int currentRow, string label, string value)
        {
            worksheet.Cell(currentRow, 1).Value = label;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = value ?? "N/A";

            // Merge cells for the value if it's likely to be long text
            if (value != null && value.Length > 50)
            {
                worksheet.Range(currentRow, 2, currentRow, 9).Merge();
                worksheet.Cell(currentRow, 2).Style.Alignment.WrapText = true;
            }

            currentRow++;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}