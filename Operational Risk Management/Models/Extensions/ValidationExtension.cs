using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Operational_Risk_Management.Models.Interfaces;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class ValidationExtension
    {
        public static IEnumerable<string> GetErrors(this ModelStateDictionary modelState)
        {
            foreach (var item in modelState.Values)
            {
                if (item.Errors.Any())
                {
                    yield return item.Errors.FirstOrDefault().ErrorMessage;
                }
            }
        }
        public static bool IsValid<T>(this T entity, AbstractValidator<T> validator, ModelStateDictionary modelState) where T : IValidationModel
        {
            if (entity == null)
            {
                return false;
            }
            var validationResult = validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    if (modelState.ContainsKey(error.PropertyName))
                    {
                        modelState[error.PropertyName].Errors.Clear();
                    }

                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return false;
            }
            return true;
        }
        public static async Task<bool> IsValidAsync<T>(this T entity, AbstractValidator<T> validator, ModelStateDictionary modelState) where T : IValidationModel
        {
            var validationResult = await validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    if (modelState.ContainsKey(error.PropertyName))
                    {
                        modelState[error.PropertyName].Errors.Clear();
                    }

                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return false;
            }
            return true;
        }

    }
}
