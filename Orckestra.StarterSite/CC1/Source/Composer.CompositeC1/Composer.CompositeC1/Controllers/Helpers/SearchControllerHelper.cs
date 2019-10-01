using System.Web;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1.Controllers.Helpers
{
    public static class SearchControllerHelper
    {
        public static bool AreKeywordsValid(string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                return false;
            }

            var strippedKeywords = keywords.Trim();

            if (SearchConfiguration.BlockStarSearchs)
            {
                strippedKeywords = strippedKeywords.Replace("*", "");
            }

            var isInvalid = string.IsNullOrWhiteSpace(strippedKeywords);
            return !isInvalid;
        }

    };
}