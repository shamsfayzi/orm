namespace Operational_Risk_Management.Models
{
    public static class ViewStaticState
    {
        // --- Role Placeholders ---
        public static bool IsAdmin { get; private set; } = false;
        public static bool IsRiskManager { get; private set; } = false;
        public static bool IsDepartmentManager { get; private set; } = false; // Example role
        public static bool IsUploader { get; private set; } = true; // Default to Uploader for broad visibility initially

        // --- User Metadata Placeholders ---
        public static string CurrentUserName { get; private set; } = "Default Uploader";
        public static string CurrentUserDepartment { get; private set; } = "Default Department";
        public static Guid CurrentUserId { get; private set; } = Guid.Empty; // Example

        // --- Available Roles (for logic if needed) ---
        public const string RoleAdmin = "Admin";
        public const string RoleRiskManager = "RiskManager";
        public const string RoleDepartmentManager = "DepartmentManager";
        public const string RoleUploader = "Uploader";

        /// <summary>
        /// Call this at the top of a view or in _ViewStart for testing different perspectives.
        /// In a real app, these would be set by the authentication system.
        /// </summary>
        public static void SimulateLogin(string role, string userName = "Test User", string department = "Test Department", Guid? userId = null)
        {
            IsAdmin = role == RoleAdmin;
            IsRiskManager = role == RoleRiskManager;
            IsDepartmentManager = role == RoleDepartmentManager;
            IsUploader = role == RoleUploader;

            // Ensure only one primary role is true, or adjust logic as needed
            if (IsAdmin) IsRiskManager = IsDepartmentManager = IsUploader = false;
            else if (IsRiskManager) IsAdmin = IsDepartmentManager = IsUploader = false;
            else if (IsDepartmentManager) IsAdmin = IsRiskManager = IsUploader = false;
            else if (IsUploader) IsAdmin = IsRiskManager = IsDepartmentManager = false;


            CurrentUserName = userName;
            CurrentUserDepartment = department;
            CurrentUserId = userId ?? Guid.NewGuid(); // Assign a new Guid if null for testing
        }

        public static void ResetLogin()
        {
            IsAdmin = false;
            IsRiskManager = false;
            IsDepartmentManager = false;
            IsUploader = true; // Default
            CurrentUserName = "Default Uploader";
            CurrentUserDepartment = "Default Department";
            CurrentUserId = Guid.Empty;
        }

        // Example: Initialize with a default role for easier initial testing
        static ViewStaticState()
        {
            ResetLogin(); // Start with a default role
            // To test as Admin by default:
            // SimulateLogin(RoleAdmin, "Admin User", "IT Department");
        }
    }
}
