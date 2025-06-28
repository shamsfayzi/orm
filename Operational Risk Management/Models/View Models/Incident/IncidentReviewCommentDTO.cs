using System;

namespace Operational_Risk_Management.Models.View_Models.Incident
{
    public class IncidentReviewCommentDTO
    {
        public string CommentText { get; set; }
        public string CommenterName { get; set; }
        public DateTime CommentDate { get; set; }
        public bool IsRevisionNote { get; set; }
        public bool IsVisibleToDepartment { get; set; }
    }
}
