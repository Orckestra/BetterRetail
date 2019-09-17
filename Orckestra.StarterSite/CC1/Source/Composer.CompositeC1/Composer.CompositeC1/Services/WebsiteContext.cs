using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Autofac.Integration.Mvc;
using Composite.Core.Routing;
using Composite.Data;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.ServiceModel.Requests.Orders;

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
                        _websiteId = pageUrlData.PageId;
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
