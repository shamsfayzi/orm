using System.Collections.Generic;
using System.Threading.Tasks;
using Operational_Risk_Management.Models.Entities;

namespace Operational_Risk_Management.Models.Interfaces.Services
{
    public interface INotificationService
    {
        Task<bool> NotifyUserAsync(string userId, string title, string message);
        Task<bool> NotifyUsersAsync(List<string> userIds, string title, string message);
        Task<bool> NotifyRiskManagementTeamAsync(string title, string message);
        Task<bool> NotifyDepartmentAsync(string department, string title, string message);
        Task<bool> SendEmailNotificationAsync(string email, string subject, string body);
        Task<List<NotificationDTO>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 10);
        Task<bool> MarkNotificationAsReadAsync(string notificationId);
    }
}
