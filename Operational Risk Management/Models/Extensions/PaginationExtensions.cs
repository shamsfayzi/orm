using System;
using System.Linq;
using System.Threading.Tasks;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginatedModel<T>> ToPaginatedAsync<T>(this IQueryable<T> source, PaginatedModel<T> model) where T : BaseEntity
        {
            
            // Ensure filter is not null
            if (model.Filter == null)
            {
                model.Filter = Activator.CreateInstance<T>(); // Creating a new instance of the filter
            }
            if (model.Pagination == null)
            {
                model.Pagination = new Pagination(); // Creating a new instance of the filter
            }
            // Calculate total records and pages
            model.TotalRecords = await source.CountAsync();
            model.TotalPages = (int)Math.Ceiling(model.TotalRecords / (double)model.PageSize);

            // Ensure current page is within range
            if (model.CurrentPage < 1)
                model.CurrentPage = 1;
            if (model.CurrentPage > model.TotalPages)
                model.CurrentPage = model.TotalPages;
            if (source.Any())
            {

                if(model.Pagination.IgnorePagination)
                {
                    model.Items=await source.ToListAsync();
                }
                else
                {
                    // Skip and take records for pagination
                    model.Items = await source.Skip((model.CurrentPage - 1) * model.PageSize)
                                         .Take(model.PageSize)
                                         .ToListAsync();
                }
            }

            // Set Pagination properties


            model.Pagination.HasNext = model.CurrentPage < model.TotalPages;
            model.Pagination.HasPrevious = model.CurrentPage > 1;
            model.Pagination.CurrentPage = model.CurrentPage;
            model.Pagination.TotalPage = model.TotalPages;
            model.Pagination.PageSize = model.PageSize;
            model.Pagination.TotalRecords = model.TotalRecords.ToString();
            model.Pagination.CurrentRecords = model.Items.Count();
            return model;
        }
    }
}
