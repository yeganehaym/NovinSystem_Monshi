using Microsoft.AspNetCore.Localization;

namespace Monshi.Web.Resources;

public class CustomLocalizationProvider:RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        throw new NotImplementedException();
    }
}