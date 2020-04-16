using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Providers.PageUrl
{
    public interface IPageUrlProvider
    {
        List<PageUrl> GetAvailableLanguageUrl();
  }

    public class PageUrl
    {
        string Url { get; set; }
        string DisplayName { get; set; }
    }
}