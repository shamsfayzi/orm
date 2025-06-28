using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CommaDelimitedModelBinder<T> : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(value))
        {
            bindingContext.Result = ModelBindingResult.Success(new List<T>());
            return Task.CompletedTask;
        }

        try
        {
            var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<T>();

            foreach (var item in values)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    result.Add((T)Convert.ChangeType(item.Trim(), typeof(T)));
                }
            }

            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch
        {
            bindingContext.Result = ModelBindingResult.Success(new List<T>());
        }

        return Task.CompletedTask;
    }
}
