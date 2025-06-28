using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.Entities
{

        public class Notification
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();

            public string Title { get; set; }

            public string Message { get; set; }

            // Nullable for system-wide notifications
            public string? UserId { get; set; }

            // Nullable for user-specific notifications
            public string? Department { get; set; }

            // For email notifications
            public string? Email { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? ReadAt { get; set; }

            public bool IsRead { get; set; } = false;

            public NotificationType Type { get; set; }

            public bool IsEmailSent { get; set; } = false;
            public DateTime? EmailSentAt { get; set; }

            public bool IsSystemNotification { get; set; } = false;

            public bool IsRiskManagementNotification { get; set; } = false;
        }

        public enum NotificationType
        {
            UserSpecific,
            DepartmentWide,
            RiskManagementTeam,
            Email,
            SystemAlert
        }

        public class NotificationDTO
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsRead { get; set; }
            public NotificationType Type { get; set; }
        }
    }

