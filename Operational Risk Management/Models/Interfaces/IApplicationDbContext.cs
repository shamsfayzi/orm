using Operational_Risk_Management.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Attachment = Operational_Risk_Management.Models.Entities.Attachment;

namespace Operational_Risk_Management.Models.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ActivityLog> Logs { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<SubmissionWindows> SubmissionWindows { get; set; }
        public DbSet<Incident.Incident> Incidents { get; set; }
        public DbSet<Incident.IncidentDocument> IncidentDocuments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        public int SaveChanges();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
