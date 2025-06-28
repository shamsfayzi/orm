
using Operational_Risk_Management.Models.Interfaces.Repositories;
using Operational_Risk_Management.Models.Services.Repositories;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Services.Repository;
using FluentValidation.AspNetCore;
using Operational_Risk_Management.Models.Interfaces.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Operational_Risk_Management.Services.Interfaces;
using Operational_Risk_Management.Services;
using Operational_Risk_Management.Models.Repositories;
using DinkToPdf.Contracts;
using DinkToPdf;
using Operational_Risk_Management.Models.Common;

namespace Operational_Risk_Management.Models.Services
{
    public static class ServicesHome
    {
        /// <summary>  
        /// Register custom services to application builder  
        /// </summary>  
        /// <param name="builder"></param>  
        /// <returns></returns>  
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            #region Independent Services
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            #endregion
            //register your services here  
            #region Repositories  
            builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
            builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IIndicatorRepository, IndicatorRepository>();
            builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            // Register the open generic repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            #endregion

            #region Services  
            //builder.Services.AddSignalR();  
            //builder.Services.AddScoped<IAuthorizationHandler, DefaultWindowsAuthorizationHandler>();  
            //builder.Services.AddScoped<IEmailSender, EmailSender>();  
            builder.Services.AddScoped<IUploaderService, UploaderService>();
            builder.Services.AddScoped<IApplicationDbContext, ApplicationDBContext>();
            //builder.Services.AddScoped<IEventPublisher, EventPublisher>();  
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            builder.Services.AddScoped<IIncidentService, IncidentService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IPdfService, PdfService>();
            builder.Services.AddScoped<IExcelService, ExcelService>();
            builder.Services.AddScoped<IIncidentDashboardService, IncidentDashboardService>();
            builder.Services.AddScoped<IKRIDashboardSercice,KRIDashboardService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            #endregion

            return builder;
        }
    }
}