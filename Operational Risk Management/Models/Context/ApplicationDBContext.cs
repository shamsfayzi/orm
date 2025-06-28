using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Extensions;
using Operational_Risk_Management.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Attachment = Operational_Risk_Management.Models.Entities.Attachment;
namespace Operational_Risk_Management.Models.Context
{
    public class ApplicationDBContext : DbContext, IApplicationDbContext
    {
        private readonly IHttpContextAccessor _accessor;
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options, IHttpContextAccessor accessor) : base(options)
        {
            _accessor = accessor;
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<ActivityLog> Logs { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<SubmissionWindows> SubmissionWindows { get; set; }
        public DbSet<Incident.Incident> Incidents { get; set; }
        public DbSet<Incident.IncidentDocument> IncidentDocuments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<IncidentReviewComment> IncidentReviewComments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyFilter<ISoftDelete>(a => !a.IsDeleted);

            //modelBuilder.ApplyFilter<Tag>(a => a.CreateBy==_accessor.HttpContext.User.GetUserName());
            //InitDB.Init(modelBuilder);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IncidentReviewComment>()
                .HasOne(c => c.Incident)
                .WithMany(i => i.ReviewComments)
                .HasForeignKey(c => c.IncidentId)
                .OnDelete(DeleteBehavior.Cascade); // Or DeleteBehavior.Restrict if preferred
        }

        public override int SaveChanges()
        {

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is not IIgnoreCustomeSaveChange)
                {

                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Entity is not IIgnoreAutoDate)
                        {
                            entry.Entity.GetType().GetProperty("CreateDate").SetValue(entry.Entity, DateTime.Now);
                            entry.Entity.GetType().GetProperty("UpdatedDate").SetValue(entry.Entity, DateTime.Now);
                        }

                        entry.Entity.GetType().GetProperty("CreateBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetUserName());
                        entry.Entity.GetType().GetProperty("CreatedByFullName").SetValue(entry.Entity, _accessor.HttpContext.User.GetFullName());

                    }
                    else if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete)
                    {
                        entry.Entity.GetType().GetProperty("IsDeleted").SetValue(entry.Entity, true);
                        entry.Entity.GetType().GetProperty("DeletedDate").SetValue(entry.Entity, DateTime.Now);
                        entry.State = EntityState.Modified;
                        entry.Entity.GetType().GetProperty("UpdatedByFullName").SetValue(entry.Entity, _accessor.HttpContext.User.GetFullName());
                        entry.Entity.GetType().GetProperty("UpdateBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetUserName());
                        entry.Entity.GetType().GetProperty("DeletedBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetFullName());
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.GetType().GetProperty("UpdatedDate").SetValue(entry.Entity, DateTime.Now);
                        entry.State = EntityState.Modified;
                        entry.Entity.GetType().GetProperty("UpdatedByFullName").SetValue(entry.Entity, _accessor.HttpContext.User.GetFullName());
                        entry.Entity.GetType().GetProperty("UpdateBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetUserName());
                    }
                }
            }
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellation)
        {
            if (_accessor.HttpContext != null)
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.Entity is not IIgnoreCustomeSaveChange)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            if (entry.Entity is not IIgnoreAutoDate)
                            {
                                entry.Entity.GetType().GetProperty("CreateDate").SetValue(entry.Entity, DateTime.Now);
                                entry.Entity.GetType().GetProperty("UpdatedDate").SetValue(entry.Entity, DateTime.Now);
                            }
                            entry.Entity.GetType().GetProperty("CreateBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetUserName());
                            entry.Entity.GetType().GetProperty("CreatedByFullName").SetValue(entry.Entity, _accessor.HttpContext.User.GetFullName());

                        }
                        else if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete)
                        {
                            entry.Entity.GetType().GetProperty("IsDeleted").SetValue(entry.Entity, true);
                            entry.Entity.GetType().GetProperty("DeletedDate").SetValue(entry.Entity, DateTime.Now);
                            entry.State = EntityState.Modified;
                            entry.Entity.GetType().GetProperty("UpdateBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetUserName());
                            entry.Entity.GetType().GetProperty("DeletedBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetUserName());

                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entry.Entity.GetType().GetProperty("UpdatedDate").SetValue(entry.Entity, DateTime.Now);
                            entry.State = EntityState.Modified;
                            entry.Entity.GetType().GetProperty("UpdateBy").SetValue(entry.Entity, _accessor.HttpContext.User.GetUserName());
                        }

                    }

                }
            }

            return base.SaveChangesAsync(cancellation);
        }


    }
}
