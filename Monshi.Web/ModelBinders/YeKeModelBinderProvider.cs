using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WebApplication2.ModelBinders;

public class YeKeModelBinderProvider:IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.IsComplexType)
        {
            return null;
        }

        if (context.Metadata.ModelType == typeof(string))
        {
            return new YeKeModelBinder();
        }

        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        return new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
    }
}