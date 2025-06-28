using Microsoft.AspNetCore.Mvc.ModelBinding;

public class CommaDelimitedModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType.IsGenericType &&
            context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var elementType = context.Metadata.ModelType.GetGenericArguments()[0];

            // Only handle simple types that can be converted from string
            if (elementType == typeof(string) ||
                elementType == typeof(int) ||
                elementType == typeof(decimal) ||
                elementType == typeof(Guid) ||
                elementType == typeof(DateTime) ||
                elementType == typeof(bool))
            {
                var binderType = typeof(CommaDelimitedModelBinder<>).MakeGenericType(elementType);
                return (IModelBinder)Activator.CreateInstance(binderType);
            }
        }

        return null;
    }
}
