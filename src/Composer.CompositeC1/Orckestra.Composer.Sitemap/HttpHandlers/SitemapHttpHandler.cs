using Composite.Core;
using Orckestra.Composer.Services;
using Orckestra.Composer.Sitemap.Config;
using System.IO;
using System.Web;
using System.Xml;

namespace Orckestra.Composer.Sitemap.HttpHandlers
{
    public class SitemapHttpHandler: IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var sitemapGeneratorConfig = ServiceLocator.GetService<ISitemapGeneratorConfig>();
            var WebsiteContext = ServiceLocator.GetService<IWebsiteContext>();

            var sitemapDirectory = sitemapGeneratorConfig.GetSitemapDirectory(WebsiteContext.WebsiteId);
            var sitemapFilepath = Path.Combine(sitemapDirectory, SitemapGenerator.SitemapIndexFilename);

            if (File.Exists(sitemapFilepath))
            {
                HttpResponse Response = context.Response;
                Response.ContentType = "text/xml";
                Response.WriteFile(sitemapFilepath);
            }
        }
    }
}
