using System.Globalization;

namespace Orckestra.Composer.Grocery.Providers
{
    public interface IGroceryUrlProvider
    {
        string GetSearchUrl(CultureInfo cultureInfo);
    }
}
