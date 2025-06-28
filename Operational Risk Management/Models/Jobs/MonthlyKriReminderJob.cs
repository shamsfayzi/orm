using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Enums;
using Operational_Risk_Management.Models.Extensions;
using AutoMapper.Internal.Mappers;
using Operational_Risk_Management.Models.Services;

namespace Operational_Risk_Management.Models.Jobs
{
    public class MonthlyKriReminderJob
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<MonthlyKriReminderJob> _logger;
        private readonly ApplicationDBContext _db;

        public MonthlyKriReminderJob(IEmailSender emailSender, ILogger<MonthlyKriReminderJob> logger, ApplicationDBContext db)
        {
            _emailSender = emailSender;
            _logger = logger;
            _db = db;
        }

        public async Task SendKriRemindersAsync()
        {
            _logger.LogInformation("KRI reminder job started...");

            var today = DateTime.UtcNow.Date;
            var endOfTheMonth = DateTime.Now.ToLastDateOfMonth().Day;
            var submissionWindows = await _db.SubmissionWindows.Where(a => a.IsActive).ToListAsync();
            var openSubmissions = await _db.Submissions
                .Where(s => s.Status == "Open" && IsWithinSubmissionWindow(s.Indicator.FrequencyOfReview, today, endOfTheMonth, submissionWindows)).Include(a=>a.Indicator).ThenInclude(a=>a.Template)
                .ToListAsync();
            if (!openSubmissions.Any())
            {
                _logger.LogInformation("No active KRI windows. No reminders sent.");
                return;
            }

            var grouped = openSubmissions
                .GroupBy(k => k.Indicator.Template.FocalPoint);
            // get email
            foreach (var group in grouped)
            {
                var email = group.Key;
                var indicatorNames = string.Join(", ", group.Select(i => i.Indicator.IndicatorName));

                var body = $"Dear staff,\n\nThe following KRI indicators assigned to you are open for submission:\n\n{indicatorNames}\n\nPlease complete your updates before the deadline.\n\nThank you.";

                await _emailSender.SendAsync_SMTP(email, "KRI Submission Reminder", body);
            }

            _logger.LogInformation("Reminder emails sent to: {0}", string.Join(", ", grouped.Select(g => g.Key)));
        }
        private static int GetDaysUntilEndOfPeriod(List<SubmissionWindows> submissionWindows, string freq)
        {
            return submissionWindows.Where(a => a.Frequency == freq).First().OpenDaysBeforePeriodEnd;
        }
        private static int GetDaysUntilStartOfPeriod(List<SubmissionWindows> submissionWindows, string freq)
        {
            return submissionWindows.Where(a => a.Frequency == freq).First().OpenDaysBeforePeriodEnd;
        }
        private bool IsWithinSubmissionWindow(string frequency, DateTime today, int endOfTheMonth, List<SubmissionWindows> submissionWindows)
        {
            return frequency switch
            {
                "Monthly" => IsMonthlyWindowOpen(today, endOfTheMonth, submissionWindows),
                "Quarterly" => IsQuarterlyWindowOpen(today, endOfTheMonth, submissionWindows),
                "Annual" => IsAnnualWindowOpen(today, endOfTheMonth, submissionWindows),
                _ => false
            };
        }

        private bool IsMonthlyWindowOpen(DateTime today, int endOfTheMonth, List<SubmissionWindows> submissionWindows)
        {
            return today.Day >= (endOfTheMonth - GetDaysUntilEndOfPeriod(submissionWindows, "Monthly")) ||
                   today.Day <= GetDaysUntilStartOfPeriod(submissionWindows, "Monthly");
        }

        private bool IsQuarterlyWindowOpen(DateTime today, int endOfTheMonth, List<SubmissionWindows> submissionWindows)
        {
            var isQuarterEndMonth = today.Month % 3 == 0; // March, June, September, December
            if (!isQuarterEndMonth) return false;

            return today.Day >= (endOfTheMonth - GetDaysUntilEndOfPeriod(submissionWindows, "Quarterly")) ||
                   today.Day <= GetDaysUntilStartOfPeriod(submissionWindows, "Quarterly");
        }

        private bool IsAnnualWindowOpen(DateTime today, int endOfTheMonth, List<SubmissionWindows> submissionWindows)
        {
            if (today.Month != 12) return false;

            return today.Day >= (endOfTheMonth - GetDaysUntilEndOfPeriod(submissionWindows, "Annual")) ||
                   today.Day <= GetDaysUntilStartOfPeriod(submissionWindows, "Annual");
        }
    }
}