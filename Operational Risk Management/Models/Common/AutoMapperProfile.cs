using AutoMapper;
using Operational_Risk_Management.Controllers;
using Operational_Risk_Management.Models.View_Models.Incident;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Incident;


//using Operational_Risk_Management.Models.View_Models.Database;
//using Operational_Risk_Management.Models.View_Models.Network;
//using Operational_Risk_Management.Models.View_Models.PenTests;
using Operational_Risk_Management.Models.View_Models.Departments;
using Operational_Risk_Management.Models.View_Models.KRIIndicator;
using Operational_Risk_Management.Models.View_Models.KRITemplates;
using Operational_Risk_Management.Models.View_Models.Templates;
using Operational_Risk_Management.Models.View_Models.Uploader;
using Operational_Risk_Management.Models.View_Models.Uploader.Indicator;
// Ensure Entities is used if IncidentReviewComment is directly referenced.
// using Operational_Risk_Management.Models.Entities; // Already present via other usings indirectly
// using Operational_Risk_Management.Models.View_Models.Incident; // Already present
using Operational_Risk_Management.Models.View_Models.Users;
//using Operational_Risk_Management.Models.View_Models.VA;


namespace Operational_Risk_Management.Models.Common
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            MapDepartments();
            MapAppUsers();
            MapKRITemplates();
            MapKRIIndicators();
            MapSubmissions();
            MapOverrideAccess();
            MapIncidents();
        }

        private void MapDepartments()
        {
            CreateMap<VM_DepartmentCreate, Department>().ReverseMap();
            CreateMap<VM_DepartmentUpdate, Department>().ReverseMap();
            //CreateMap<VM_AppUserUpdate, ApplicationUser>();
        }
        private void MapAppUsers()
        {
            CreateMap<VM_AppUserCreate, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, VM_AppUserUpdate>();
            CreateMap<VM_AppUserUpdate, ApplicationUser>();
        }
        private void MapKRITemplates()
        {
            CreateMap<VM_KRITemplateCreate, Template>().ReverseMap();
            CreateMap<VM_KRITemplateUpdate, Template>().ReverseMap();
            CreateMap<Template, VM_Template>();
            //CreateMap<KRIIndicator, VM_KRIIndicatorUpdate>().ReverseMap();
            //CreateMap<KRIIndicator, VM_KRIIndicatorCreate>().ReverseMap();
        }
        private void MapKRIIndicators()
        {
            // Map VM_IndicatorCreate to Indicator
            CreateMap<VM_IndicatorCreate, Indicator>(); // One way for creation
            // Map Indicator to VM_IndicatorEdit (for displaying edit form)
            CreateMap<Indicator, VM_IndicatorEdit>();
            CreateMap<Indicator, VM_Indicator>();

                
        }
        private void MapSubmissions()
        {
            CreateMap<Submission, VM_Submission>();
            CreateMap<Submission, SubmissionDetailViewModel>();
            
        }
        private void MapOverrideAccess()
        {
            CreateMap<VM_CR_OverrideAccessRequest, OverrideAccessRequest>();
        }
        private void MapIncidents()
        {
            // ViewModel to DTO (for form submission to API)
            CreateMap<StaffConcernedFormViewModel, StaffConcernedDTO>();
            CreateMap<CreateIncidentPageViewModel, CreateIncidentDTO>()
                .ForMember(dest => dest.StaffConcerned, opt => opt.MapFrom(src => src.StaffConcerned));
            // ReportDate and NonFinancialImpact will be mapped by convention as names match.

            // DTO to Entity (for saving to DB)
            CreateMap<StaffConcernedDTO, StaffConcerned>(); // Ensure this exists for nested mapping
            CreateMap<CreateIncidentDTO, Incident.Incident>()
                .ForMember(dest => dest.StaffConcerned, opt => opt.MapFrom(src => src.StaffConcerned));
            // ReportDate and NonFinancialImpact will be mapped by convention.

            // Entity to DTO (for display in lists/details)
            CreateMap<IncidentReviewComment, IncidentReviewCommentDTO>();
            CreateMap<IncidentDocument, IncidentDocumentDTO>(); // Ensure this exists
            CreateMap<Incident.Incident, IncidentDetailDTO>()
                .ForMember(dest => dest.StaffConcerned, opt => opt.MapFrom(src =>
                    src.StaffConcerned.Select(s => new StaffConcernedDTO
                    {
                        Name = s.Name,
                        TitleFunction = s.TitleFunction
                    }).ToList()))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents)) // AutoMapper will use IncidentDocument -> IncidentDocumentDTO
                .ForMember(dest => dest.ReviewComments, opt => opt.MapFrom(src => src.ReviewComments)); // AutoMapper will use IncidentReviewComment -> IncidentReviewCommentDTO

            // Mappings for Edit Page:
            CreateMap<StaffConcernedDTO, StaffConcernedFormViewModel>(); // DTO to FormVM (for StaffConcerned in Edit)
            CreateMap<IncidentDetailDTO, EditIncidentPageViewModel>()
                .ForMember(dest => dest.ExistingDocuments, opt => opt.MapFrom(src => src.Documents))
                .ForMember(dest => dest.NewSupportingDocuments, opt => opt.Ignore()) // This is for file uploads, not mapping from DTO
                .ForMember(dest => dest.DocumentsToDelete, opt => opt.Ignore());     // This is for form submission, not mapping from DTO
                // StaffConcerned should map from IncidentDetailDTO.StaffConcerned (List<StaffConcernedDTO>)
                // to EditIncidentPageViewModel.StaffConcerned (List<StaffConcernedFormViewModel>)
                // using the StaffConcernedDTO -> StaffConcernedFormViewModel map above.

            // Entity to EditViewModel (for populating the form - alternative if not using IncidentDetailDTO as source)
            CreateMap<StaffConcerned, StaffConcernedFormViewModel>();
            CreateMap<Incident.Incident, EditIncidentPageViewModel>()
                .ForMember(dest => dest.ExistingDocuments, opt => opt.MapFrom(src => src.Documents))
                .ForMember(dest => dest.StaffConcerned, opt => opt.MapFrom(src => src.StaffConcerned));

            // EditViewModel back to DTO (for processing the update)
            CreateMap<EditIncidentPageViewModel, CreateIncidentDTO>()
                .ForMember(dest => dest.StaffConcerned, opt => opt.MapFrom(src => src.StaffConcerned));
                // Other properties like ReportDate, NonFinancialImpact etc., map by convention.
                // NewSupportingDocuments and DocumentsToDelete are handled by the controller, not AutoMapper directly to CreateIncidentDTO.

            CreateMap<CreateIncidentDTO, Incident.Incident>();
            CreateMap<Incident.Incident, IncidentDetailDTO>()
            .ForMember(dest => dest.StaffConcerned, opt => opt.MapFrom(src =>
                src.StaffConcerned.Select(s => new StaffConcernedDTO
                {
             Name = s.Name,
             TitleFunction = s.TitleFunction
                 }).ToList() ?? new List<StaffConcernedDTO>()))
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src =>
                src.Documents.Select(d => new IncidentDocumentDTO
                 {
                 Id = d.Id,
                 FileName = d.FileName,
                 FileType = d.FileType,
                 FileSize = d.FileSize,
                 Description = d.Description,
                 CreateDate = d.CreateDate
            }).ToList() ?? new List<IncidentDocumentDTO>()));

        }
    }
}