using System.Linq.Expressions;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class EntityExtensions
    {

        
        /// <summary>
        /// find an entity base on Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T? Find<T>(this IQueryable<T> entities, Guid id) where T : BaseEntity
        {
            return entities.Find(id);
        }
        /// <summary>
        /// check if an entity base on Id is existed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="id">unique primary key id</param>
        /// <returns>true if exists and false if not</returns>
        public static bool IsExists<T>(this IQueryable<T> entities, Guid id) where T : BaseEntity
        {
            return entities.Any(a => a.Id == id);
        }

        /// <summary>
        /// check if the property name with the value is unique, except the id matched record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUnique<T>(this IQueryable<T> entities, Guid id, string propertyName, object value) where T : BaseEntity
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var idProperty = Expression.Property(parameter, "Id");
            var idValue = Expression.Constant(id);
            var idEquals = Expression.NotEqual(idProperty, idValue);

            var property = Expression.Property(parameter, propertyName);
            var propertyValue = Expression.Constant(value);
            var propertyNotEquals = Expression.Equal(property, propertyValue);

            var andExpression = Expression.AndAlso(idEquals, propertyNotEquals);
            var lambda = Expression.Lambda<Func<T, bool>>(andExpression, parameter);

            return !entities.Any(lambda);
        }
        /// <summary>
        /// only check if the property name with the value is unique
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUnique<T>(this IQueryable<T> entities, string propertyName, object value) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, propertyName);
            var propertyValue = Expression.Constant(value);
            var equalsExpression = Expression.Equal(property, propertyValue);
            var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

            return !entities.Any(lambda);
        }

        /// <summary>
        /// check if an entity base on Id is existed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="id">unique primary key id</param>
        /// <param name="targetEntity">target row</param>
        /// <returns>true if exists and false if not</returns>
        public static bool IsExists<T>(this IQueryable<T> entities, Guid id, out T? targetEntity) where T : BaseEntity
        {
            targetEntity = entities.FirstOrDefault(a => a.Id == id);
            return targetEntity != null;
        }
        public static void LogAndSave(this ILogger logger, IApplicationDbContext dbContext, string message, params object[] args)
        {
            dbContext.Logs.Add(new Log() // Changed ActivityLog to Log
            {
                Content = string.Format(message, args)
            });
            dbContext.SaveChanges();
            logger.LogInformation(message, args);
        }
    }
}
