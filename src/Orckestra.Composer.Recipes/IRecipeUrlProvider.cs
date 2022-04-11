using System;
using System.Globalization;

namespace Orckestra.Composer.Recipes
{
    public interface IRecipeUrlProvider
    {
        string BuildRecipeMealTypeUrl(Guid mealType, CultureInfo cultureInfo);
        string GetSearchPageUrl(CultureInfo cultureInfo);
    }
}
