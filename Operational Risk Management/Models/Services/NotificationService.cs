using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces.Services;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Services;

namespace Operational_Risk_Management.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailSender _emailService;
        public NotificationService(IApplicationDbContext context, IEmailSender emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> NotifyUserAsync(string userId, string title, string message)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = NotificationType.UserSpecific,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync(CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> NotifyUsersAsync(List<string> userIds, string title, string message)
        {
            try
            {
                var notifications = userIds.Select(userId => new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = NotificationType.UserSpecific,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync(CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> NotifyRiskManagementTeamAsync(string title, string message)
        {
            try
            {
                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    Type = NotificationType.RiskManagementTeam,
                    IsRiskManagementNotification = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync(CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> NotifyDepartmentAsync(string department, string title, string message)
        {
            try
            {
                var notification = new Notification
                {
                    Department = department,
                    Title = title,
                    Message = message,
                    Type = NotificationType.DepartmentWide,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync(CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(string email, string subject, string body)
        {
            try
            {
                // Save notification record
                var notification = new Notification
                {
                    Email = email,
                    Subject = subject,
                    Body = body,
                    Type = NotificationType.Email,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);

                // Send actual email
                var emailSent = await _emailService.SendAsync_SMTP(email, subject, body);

                if (emailSent)
                {
                    notification.IsEmailSent = true;
                    notification.EmailSentAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync(CancellationToken.None);
                return emailSent;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<NotificationDTO>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 10)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId ||
                           n.Department == null ||
                           n.IsRiskManagementNotification)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                    Type = n.Type
                })
                .ToListAsync();
        }

        public async Task<bool> MarkNotificationAsReadAsync(string notificationId)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(notificationId);
                if (notification == null) return false;

                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}