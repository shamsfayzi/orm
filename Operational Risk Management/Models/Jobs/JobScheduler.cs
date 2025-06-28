using Hangfire;
using Operational_Risk_Management.Models.Services;
namespace Operational_Risk_Management.Models.Jobs
{
        public static class JobScheduler
        {
            public static IApplicationBuilder RunJobs(this IApplicationBuilder app)
            {
                // Register your Hangfire jobs here
                RecurringJob.AddOrUpdate<MonthlyKriReminderJob>(
                    "kri-monthly-reminder",
                    job => job.SendKriRemindersAsync(),
                    Cron.Daily); // use Cron.Minutely for testing

                return app;
            }
        }
    }


