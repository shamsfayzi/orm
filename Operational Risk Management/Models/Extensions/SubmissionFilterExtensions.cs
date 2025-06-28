using Operational_Risk_Management.Models.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class SubmissionFilterExtensions
    {
        public static Expression<Func<Submission, bool>> ApplyFilters(
            this Expression<Func<Submission, bool>> predicate,
            string searchTerm,
            int? year,
            int? month)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                predicate = predicate.And(s => (s.Notes != null && s.Notes.Contains(searchTerm)) || 
                                               (s.CreateBy != null && s.CreateBy.Contains(searchTerm)));
            }

            int? filterYear = year;
            if (year.HasValue && year == 0) // 0 might represent 'All Years'
            {
                filterYear = null; 
            }

            if (filterYear.HasValue)
            {
                predicate = predicate.And(s => s.CreateDate.Year == filterYear.Value);
            }

            // 0 or null might represent 'All Months'
            if (month.HasValue && month > 0) 
            {
                predicate = predicate.And(s => s.CreateDate.Month == month.Value);
            }

            return predicate;
        }
    }
}