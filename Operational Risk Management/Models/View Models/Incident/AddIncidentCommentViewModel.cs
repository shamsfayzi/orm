using System;
using System.ComponentModel.DataAnnotations;

namespace Operational_Risk_Management.Models.View_Models.Incident
{
    public class AddIncidentCommentViewModel
    {
        [Required]
        public Guid IncidentId { get; set; }

        [Required(ErrorMessage = "Comment text cannot be empty.")]
        [StringLength(2000, ErrorMessage = "Comment text cannot exceed 2000 characters.")]
        public string CommentText { get; set; }

        public bool IsRevisionNote { get; set; }
        public bool IsVisibleToDepartment { get; set; }
    }
}
