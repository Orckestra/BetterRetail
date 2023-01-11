using System;
using System.Linq;
using System.Web;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Services;

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

        private Guid? _rootPageVersionId;

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
                    var pageUrlData = C1Helper.GetPageUrlDataFromUrl(_httpRequest.Url.ToString());
                    _websiteId = C1Helper.GetWebsiteIdFromPageUrlData(pageUrlData);
                }

                return _websiteId;
            }
        }

        public T GetRootPageMetaData<T>() where T : class, IPageMetaData
        {
            using (var data = new DataConnection())
            {
                if (_rootPageVersionId == null)
                {
                    var websiteId = WebsiteId;

                    var rootPage = PageManager.GetPageById(websiteId)
                        ?? throw new InvalidOperationException($"Failed to get the website root page by ID: '{websiteId}'");

                    _rootPageVersionId = rootPage.VersionId;
                }

                var versionId = _rootPageVersionId;

                // Version resolution through DataConnection is slow, so we're disabling the "version resolution service" and selecting the needed version by ourselves
                data.DisableServices();

                return data.Get<T>()
                    .AsEnumerable()
                    .FirstOrDefault(d => d.PageId == _websiteId && d.VersionId == versionId);
            }
        }
    }
}
