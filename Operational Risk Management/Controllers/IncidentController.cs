using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Services.Interfaces;
using System.Security.Claims; // Added for ClaimTypes
using System.Linq; // Added for SelectMany and FirstOrDefault
using Microsoft.AspNetCore.Mvc.Rendering; // Added for SelectListItem
using Operational_Risk_Management.Models.Incident; // Added for Enums like IncidentType
using Operational_Risk_Management.Models.Extensions; // Assuming GetDisplayName() is here
using Operational_Risk_Management.Models.View_Models.Common; // Added for FileContentDownloadViewModel


namespace Operational_Risk_Management.Controllers
{

    using AutoMapper; // Added for IMapper

    namespace Operational_Risk_Management.Controllers
    {

        public class IncidentController : Controller // Changed from ControllerBase
        {
            private readonly IIncidentService _incidentService;
            private readonly IMapper _mapper; // Added IMapper field
                                              //private readonly UserManager<ApplicationUser> _userManager;

            public IncidentController(
                IIncidentService incidentService,
                IMapper mapper) // Added IMapper injection
                                //UserManager<ApplicationUser> userManager)
            {
                _incidentService = incidentService;
                _mapper = mapper; // Assigned IMapper
                                  //_userManager = userManager;
            }

            [Authorize]
            [HttpGet]
            public IActionResult CreateIncident()
            {
                var model = new CreateIncidentPageViewModel();
                // Populate SelectListItems for Enums
                model.IncidentTypes = Enum.GetValues(typeof(IncidentType)).Cast<IncidentType>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList(); // Using ToString as GetDisplayName() source is unknown
                model.IncidentCategoryLevels = Enum.GetValues(typeof(IncidentCategoryLevel)).Cast<IncidentCategoryLevel>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                model.RiskTypes = Enum.GetValues(typeof(RiskType)).Cast<RiskType>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                model.RiskAssessments = Enum.GetValues(typeof(RiskAssessment)).Cast<RiskAssessment>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                model.YesNoOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };
                return View(model);
            }

            [HttpPost]
            [Authorize(Roles = "Admin,RiskManagement")]
            // [ValidateAntiForgeryToken] // Consider adding if you have @Html.AntiForgeryToken() in the form
            public async Task<IActionResult> AddComment(AddIncidentCommentViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    // Option 1: Redirect back with TempData for error (simpler for now)
                    TempData["ErrorMessage"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Invalid comment submission.";
                    // Assuming IncidentDetails is served from HomeController.GetIncidentDetails or similar.
                    // The view is actually /Home/IncidentDetails/{id}, which implies an action like GetIncidentDetails in HomeController.
                    return RedirectToAction("IncidentDetails", "Home", new { id = model.IncidentId });
                }

                var commenterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var commenterName = User.Identity.Name;

                if (string.IsNullOrEmpty(commenterId) || string.IsNullOrEmpty(commenterName))
                {
                    TempData["ErrorMessage"] = "Could not identify commenter.";
                    return RedirectToAction("IncidentDetails", "Home", new { id = model.IncidentId });
                }

                await _incidentService.AddIncidentCommentAsync(
                    model.IncidentId,
                    model.CommentText,
                    commenterId,
                    commenterName,
                    model.IsRevisionNote,
                    model.IsVisibleToDepartment
                );

                TempData["SuccessMessage"] = "Comment added successfully.";
                return RedirectToAction("IncidentDetails", "Home", new { id = model.IncidentId });
            }

            [HttpPost]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CreateIncident(CreateIncidentPageViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    // Re-populate dropdowns for the view if returning the model
                    model.IncidentTypes = Enum.GetValues(typeof(IncidentType)).Cast<IncidentType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.IncidentCategoryLevels = Enum.GetValues(typeof(IncidentCategoryLevel)).Cast<IncidentCategoryLevel>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskTypes = Enum.GetValues(typeof(RiskType)).Cast<RiskType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskAssessments = Enum.GetValues(typeof(RiskAssessment)).Cast<RiskAssessment>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.YesNoOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Yes" },
                    new SelectListItem { Value = "false", Text = "No" }
                };
                    return View(model); // Return the view with validation errors
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var incidentDto = _mapper.Map<CreateIncidentDTO>(model);
                // The ReportDate from CreateIncidentPageViewModel is now mapped to CreateIncidentDTO by AutoMapper
                // as ReportDate property was added to CreateIncidentDTO in the previous subtask.

                var createdIncidentDetails = await _incidentService.CreateIncidentAsync(incidentDto, userId);

                if (createdIncidentDetails != null && createdIncidentDetails.Id != Guid.Empty)
                {
                    TempData["SuccessMessage"] = $"Incident '{createdIncidentDetails.TitleOfIncident}' reported successfully with ID {createdIncidentDetails.Id}!";

                    // Handle Supporting Document Uploads
                    if (model.SupportingDocuments != null && model.SupportingDocuments.Any())
                    {
                        // string userId is already available in this action context
                        foreach (var file in model.SupportingDocuments)
                        {
                            if (file.Length > 0) // Basic check for empty file
                            {
                                // TODO: Add more robust error handling for individual file uploads if needed.
                                // For example, collect results of UploadDocumentAsync and inform user if some files failed.
                                await _incidentService.UploadDocumentAsync(createdIncidentDetails.Id, file, file.FileName, userId);
                            }
                        }
                        TempData["SuccessMessage"] += " Supporting documents uploaded."; // Append to existing success message
                    }

                    return RedirectToAction("GetIncidentDetails", "Home", new { incidentId = createdIncidentDetails.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to report incident. Please try again or contact support.";
                    // Re-populate dropdowns before returning view
                    model.IncidentTypes = Enum.GetValues(typeof(IncidentType)).Cast<IncidentType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.IncidentCategoryLevels = Enum.GetValues(typeof(IncidentCategoryLevel)).Cast<IncidentCategoryLevel>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskTypes = Enum.GetValues(typeof(RiskType)).Cast<RiskType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskAssessments = Enum.GetValues(typeof(RiskAssessment)).Cast<RiskAssessment>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.YesNoOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Yes" },
                    new SelectListItem { Value = "false", Text = "No" }
                };
                    return View(model);
                }
            }

            [HttpPost("api/incidents")] // Differentiating route for API endpoint
            public async Task<IActionResult> CreateIncidentApi([FromBody] CreateIncidentDTO model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst("sub")?.Value;
                var result = await _incidentService.CreateIncidentAsync(model, userId);

                if (result != null)
                {
                    return CreatedAtAction(nameof(GetIncidentById), new { id = result.Id }, result);
                }

                return BadRequest("Failed to create incident report");
            }


            public async Task<IActionResult> UpdateIncident(Guid id, [FromBody] CreateIncidentDTO model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst("sub")?.Value;
                var result = await _incidentService.UpdateIncidentAsync(id, model, userId);

                if (result)
                {
                    return Ok();
                }

                return NotFound("Incident report not found");
            }

            [HttpGet("{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> GetIncidentById(Guid id)
            {
                var incident = await _incidentService.GetIncidentDetailAsync(id);

                if (incident != null)
                {
                    return Ok(incident);
                }

                return NotFound("Incident report not found");
            }

            public async Task<IActionResult> GetIncidents([FromQuery] IncidentFilterDTO filter)
            {
                var userId = User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("RiskManagement");

                var incidents = await _incidentService.GetIncidentsAsync(filter, userId, isAdmin);
                return Ok(incidents);
            }

            [Authorize(Roles = "Admin,RiskManagement")]

            public async Task<IActionResult> ReviewIncident([FromBody] ReviewIncidentDTO model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst("sub")?.Value;
                var result = await _incidentService.ReviewIncidentAsync(model, userId);

                if (result)
                {
                    return Ok();
                }

                return NotFound("Incident report not found");
            }

            [HttpPost("{id}/close")]
            [Authorize(Roles = "Admin,RiskManagement")]

            public async Task<IActionResult> CloseIncident(Guid id)
            {
                var userId = User.FindFirst("sub")?.Value;
                var result = await _incidentService.CloseIncidentAsync(id, userId);

                if (result)
                {
                    return Ok();
                }

                return NotFound("Incident report not found");
            }

            [HttpPost("{id}/documents")]

            public async Task<IActionResult> UploadDocument(Guid id, IFormFile file, [FromForm] string description)
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                // Validate file size (e.g., 10MB limit)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest("File size exceeds limit (10MB)");
                }

                // Validate file type
                var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
                if (!Array.Exists(allowedTypes, type => type == file.ContentType))
                {
                    return BadRequest("File type not supported");
                }

                var userId = User.FindFirst("sub")?.Value;
                var result = await _incidentService.UploadDocumentAsync(id, file, description, userId);

                if (result)
                {
                    return Ok();
                }

                return BadRequest("Failed to upload document");
            }

            [HttpDelete("documents/{documentId}")]

            public async Task<IActionResult> DeleteDocument(Guid documentId)
            {
                var userId = User.FindFirst("sub")?.Value;
                var result = await _incidentService.RemoveDocumentAsync(documentId, userId);

                if (result)
                {
                    return Ok();
                }

                return NotFound("Document not found");
            }

            // TODO : add a anchor tag in html that navigates user to document
            //public async Task<IActionResult> DownloadDocument(Guid documentId)
            //{
            //    var document = await _incidentService.DownloadDocumentAsync(documentId);

            //    if (document != null)
            //    {
            //        return File(document.FileData, document.ContentType, document.FileName);
            //    }

            //    return NotFound("Document not found");
            //}


            [Authorize(Roles = "Admin,RiskManagement")]

            public async Task<IActionResult> GetIncidentStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string departmentUnit)
            {
                var statistics = await _incidentService.GetIncidentStatisticsAsync(startDate, endDate, departmentUnit);
                return Ok(statistics);
            }



            public async Task<IActionResult> ExportIncidentToPdf(Guid id)
            {
                var pdfData = await _incidentService.ExportIncidentToPdfAsync(id);

                if (pdfData != null)
                {
                    return File(pdfData, "application/pdf", $"Incident_Report_{id}.pdf");
                }

                return NotFound("Incident report not found");
            }


            public async Task<IActionResult> ExportIncidentsToExcel([FromQuery] IncidentFilterDTO filter)
            {
                var userId = User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("RiskManagement");

                var excelData = await _incidentService.ExportIncidentsToExcelAsync(filter, userId, isAdmin);

                if (excelData != null)
                {
                    // TODO : return the url;
                    return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Incident_Reports_{DateTime.Now:yyyyMMdd}.xlsx");
                }

                return NotFound("No incident reports found");
            }

            [HttpGet]
            //[Authorize] // Decide if authorization is needed for downloads
            public async Task<IActionResult> DownloadAttachedDocument(Guid documentId)
            {
                if (documentId == Guid.Empty)
                {
                    return BadRequest("Invalid document ID.");
                }

                var fileDownloadViewModel = await _incidentService.GetDocumentForDownloadAsync(documentId);

                if (fileDownloadViewModel == null || fileDownloadViewModel.FileContents == null)
                {
                    // Optionally, add a TempData message here if redirecting, or return a specific error view.
                    return NotFound("Document not found or unable to read file content.");
                }

                return File(fileDownloadViewModel.FileContents, fileDownloadViewModel.ContentType, fileDownloadViewModel.FileName);
            }

            [Authorize]
            [HttpGet]
            public async Task<IActionResult> EditIncident(Guid id)
            {
                if (id == Guid.Empty)
                {
                    return NotFound();
                }

                var incidentDetailDto = await _incidentService.GetIncidentDetailAsync(id);
                if (incidentDetailDto == null)
                {
                    return NotFound();
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isAdmin = User.IsInRole("Admin") || User.IsInRole("RiskManagement");

                if (!isAdmin && incidentDetailDto.CreateBy != currentUserId)
                {
                    return Forbid();
                }

                var model = _mapper.Map<EditIncidentPageViewModel>(incidentDetailDto);
                model.Id = id;

                model.IncidentTypes = Enum.GetValues(typeof(IncidentType)).Cast<IncidentType>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                model.IncidentCategoryLevels = Enum.GetValues(typeof(IncidentCategoryLevel)).Cast<IncidentCategoryLevel>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                model.RiskTypes = Enum.GetValues(typeof(RiskType)).Cast<RiskType>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                model.RiskAssessments = Enum.GetValues(typeof(RiskAssessment)).Cast<RiskAssessment>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                model.YesNoOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };

                return View("EditIncident", model);
            }

            [HttpPost]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditIncident(EditIncidentPageViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    model.IncidentTypes = Enum.GetValues(typeof(IncidentType)).Cast<IncidentType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.IncidentCategoryLevels = Enum.GetValues(typeof(IncidentCategoryLevel)).Cast<IncidentCategoryLevel>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskTypes = Enum.GetValues(typeof(RiskType)).Cast<RiskType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskAssessments = Enum.GetValues(typeof(RiskAssessment)).Cast<RiskAssessment>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.YesNoOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Yes" },
                    new SelectListItem { Value = "false", Text = "No" }
                };
                    if (model.Id != Guid.Empty)
                    {
                        var originalIncidentDto = await _incidentService.GetIncidentDetailAsync(model.Id);
                        if (originalIncidentDto != null)
                        {
                            model.ExistingDocuments = originalIncidentDto.Documents; // Keep existing documents list
                        }
                    }
                    return View("EditIncident", model);
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var incidentToUpdate = await _incidentService.GetIncidentDetailAsync(model.Id);

                if (incidentToUpdate == null) return NotFound();

                bool isAdmin = User.IsInRole("Admin") || User.IsInRole("RiskManagement");
                if (!isAdmin && incidentToUpdate.CreateBy != currentUserId)
                {
                    return Forbid();
                }

                var incidentDto = _mapper.Map<CreateIncidentDTO>(model);
                bool updateResult = await _incidentService.UpdateIncidentAsync(model.Id, incidentDto, currentUserId);

                if (updateResult)
                {
                    if (model.DocumentsToDelete != null && model.DocumentsToDelete.Any())
                    {
                        foreach (var docId in model.DocumentsToDelete)
                        {
                            await _incidentService.RemoveDocumentAsync(docId, currentUserId);
                        }
                    }

                    if (model.NewSupportingDocuments != null && model.NewSupportingDocuments.Any())
                    {
                        foreach (var file in model.NewSupportingDocuments)
                        {
                            if (file.Length > 0)
                            {
                                await _incidentService.UploadDocumentAsync(model.Id, file, file.FileName, currentUserId);
                            }
                        }
                    }

                    TempData["SuccessMessage"] = "Incident updated successfully.";
                    return RedirectToAction("GetIncidentDetails", "Home", new { incidentId = model.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update incident.";
                    model.IncidentTypes = Enum.GetValues(typeof(IncidentType)).Cast<IncidentType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.IncidentCategoryLevels = Enum.GetValues(typeof(IncidentCategoryLevel)).Cast<IncidentCategoryLevel>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskTypes = Enum.GetValues(typeof(RiskType)).Cast<RiskType>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.RiskAssessments = Enum.GetValues(typeof(RiskAssessment)).Cast<RiskAssessment>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList();
                    model.YesNoOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Yes" },
                    new SelectListItem { Value = "false", Text = "No" }
                };
                    if (model.Id != Guid.Empty)
                    {
                        var originalIncidentDto = await _incidentService.GetIncidentDetailAsync(model.Id);
                        if (originalIncidentDto != null)
                        {
                            model.ExistingDocuments = originalIncidentDto.Documents;
                        }
                    }
                    return View("EditIncident", model);
                }
            }
        }
    }
}