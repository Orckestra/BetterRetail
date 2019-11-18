using System;
using System.Linq;
using System.Web;
using Autofac.Integration.Mvc;
using Composite.Core.Routing;
using Composite.Data;
using Orckestra.Composer.Services;

using Composite.Core.WebClient.Renderings.Page;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class WebsiteContext: IWebsiteContext
    {
        //private HttpRequestBase HttpRequest;


        public WebsiteContext()
        {
            //HttpRequest = httpRequest;
        }

        private Guid _websiteId;
        public Guid WebsiteId {
            get
            {
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
                            var websiteId = HttpContext.Current.Request.Headers["WebsiteId"];
                            Guid.TryParse(websiteId, out _websiteId);
                        }
                        catch (Exception e)
                        {

                        }

                    }
                }

                if (_websiteId == Guid.Empty)
                {
                    var HttpRequest = (HttpRequestBase)AutofacDependencyResolver.Current.GetService(typeof(HttpRequestBase));

                    var pageUrlData = GetPageUrldata(HttpRequest.Url);
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
