using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Composite.Core;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class PageService : IPageService
    {

        protected ISiteConfiguration SiteConfiguration { get; private set; }

        public PageService()
        {

        }
        /// <summary>
        /// Returns a page in the given locale.
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual IPage GetPage(Guid pageId, CultureInfo cultureInfo = null)
        {
            using (new DataConnection(cultureInfo))
            {
                return PageManager.GetPageById(pageId);
            }
        }

        public virtual Guid GetParentPageId(IPage page)
        {
            return page.GetParentId();
        }

        /// <summary>
        /// Returns localized URL. It was changed from original implementation to fix 6520: Previewing unpublished category pages is C1 console shows error in the preview, and on frontend once the page is published 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual string GetRendererPageUrl(Guid pageId, CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = DataLocalizationFacade.DefaultLocalizationCulture;
            }

            var pageUrlData = new PageUrlData(pageId, PublicationScope.Published, cultureInfo);

            return PageUrls.BuildUrl(pageUrlData, UrlKind.Renderer);
        }

        public virtual string GetPageUrl(Guid pageId, CultureInfo cultureInfo = null, HttpContext httpContext = null)
        {
            if (pageId == Guid.Empty)
            {
                throw new ArgumentException(nameof(pageId));
            }
            var page = GetPage(pageId, cultureInfo);
            if (page == null)
            {
                return null;
            }

            if (httpContext != null)
                return PageUrls.BuildUrl(page, UrlKind.Public, new UrlSpace(httpContext));
            else
                return PageUrls.BuildUrl(page);
        }

        public virtual string GetPageUrl(IPage page)
        {
            var url = PageUrls.BuildUrl(page);
            return url;
        }
    }
}
