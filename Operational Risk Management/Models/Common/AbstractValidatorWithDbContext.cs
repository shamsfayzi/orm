using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Common
{
    public abstract class AbstractValidatorWithDbContext<T> : AbstractValidator<T> where T : class
    {
        // Inject DbContext and HttpContextAccessor
        protected readonly IApplicationDbContext _dbContext;
        protected readonly HttpContext _httpContext;

        // Constructor updated for DI
        public AbstractValidatorWithDbContext(HttpContext httpContext)
        {
            _dbContext = httpContext.RequestServices.GetRequiredService<IApplicationDbContext>();
            _httpContext = httpContext;
        }

    }
}
