﻿using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Globalization;
using System.Web;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class PageService : IPageService
    {
        private readonly HttpContext _currentHttpContext;
        protected ISiteConfiguration SiteConfiguration { get; private set; }
        public PageService() {
            _currentHttpContext = HttpContext.Current;
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
            if (pageId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(), nameof(pageId)); }

            var page = GetPage(pageId, cultureInfo);
            if (page == null) return null;

            var context = httpContext ?? _currentHttpContext;
            var urlSpace = context != null ? new UrlSpace(context) : null;

            return PageUrls.BuildUrl(page, UrlKind.Public, urlSpace);
        }

        public virtual string GetPageUrl(IPage page)
        {
            var url = PageUrls.BuildUrl(page);
            return url;
        }
    }
}