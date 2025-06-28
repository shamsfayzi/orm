using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Operational_Risk_Management.Models.Context
{
    public static class FilterDatabase
    {

        public static void ApplyFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> filter)
        {
            var entities = modelBuilder.Model
           .GetEntityTypes()
           .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
           .Select(e => e.ClrType);
            foreach (var entity in entities)
            {
                var newParam = Expression.Parameter(entity);
                var newbody = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), newParam, filter.Body);
                modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
            }
        }
    }
}
