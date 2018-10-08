using Orckestra.Composer.Parameters;
using Orckestra.Composer.SearchQuery.Parameters;
using System.Collections.Specialized;

namespace Orckestra.Composer.SearchQuery.Providers
{
    public interface ISearchQueryUrlProvider
    {
        string BuildSearchQueryUrl(BuildSearchQueryUrlParam param);

        NameValueCollection BuildSearchQueryString(BuildSearchUrlParam param);
    }
}
