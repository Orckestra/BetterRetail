using System;
using System.Linq;
using System.Web;
using Composite.Core.Routing;
using Composite.Data;
using Orckestra.Composer.Services;

using Composite.Core.WebClient.Renderings.Page;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Services.Cache;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class WebsiteContext : IWebsiteContext
    {
        public WebsiteContext(HttpRequestBase httpRequest, ICacheService cacheService)
        {
            _httpRequest = httpRequest;
            _cache = cacheService.GetStoreWithDependencies<string, Guid>("Hostname Bindings", 
                new CacheDependentEntry<IHostnameBinding>()
            );
        }

        private readonly HttpRequestBase _httpRequest;
        private readonly ICacheStore<string, Guid> _cache;

        private Guid _websiteId;
        public Guid WebsiteId
        {
            get
            {
                if (_websiteId == Guid.Empty)
                {
                    var host = _httpRequest.Url?.Host;
                    _websiteId = _cache.GetOrAdd(host, h => DataFacade
                            .GetData<IHostnameBinding>(d => d.Hostname == h)
                            .Select(d => d.HomePageId)
                            .FirstOrDefault());
                }

                if (_websiteId == Guid.Empty)
                {
                    if (SitemapNavigator.CurrentHomePageId != Guid.Empty)
                    {
                        _websiteId = SitemapNavigator.CurrentHomePageId;
                    }
                    else
                    {
                        try
                        {
                            var websiteId = _httpRequest.Headers["WebsiteId"];
                            Guid.TryParse(websiteId, out _websiteId);
                        }
                        catch (Exception e)
                        {

                        }

                    }
                }

                if (_websiteId == Guid.Empty)
                {
                    var pageUrlData = GetPageUrldata(_httpRequest.Url);
                    if (pageUrlData != null)
                    {
                        _websiteId = PageStructureInfo.GetAssociatedPageIds(pageUrlData.PageId, SitemapScope.AncestorsAndCurrent).LastOrDefault();
                    }

                }

                return _websiteId;
            }
        }

        private static PageUrlData GetPageUrldata(Uri url)
        {
            PageUrlData pageUrlData = null;
            var urlStr = url.ToString();
            while (pageUrlData == null && urlStr.LastIndexOf('/') > 0)
            {
                urlStr = urlStr.Substring(0, urlStr.LastIndexOf('/'));
                pageUrlData = PageUrls.ParseUrl(urlStr.ToString());
            }

            return pageUrlData;
        }
    }
}
