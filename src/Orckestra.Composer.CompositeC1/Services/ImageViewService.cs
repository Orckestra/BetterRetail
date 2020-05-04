using System;
using System.Globalization;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class ImageViewService : IImageViewService
    {
        protected IMediaService MediaService { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }

        public ImageViewService(IMediaService mediaService, IWebsiteContext websiteContext, ISiteConfiguration siteConfiguration)
        {
            MediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            WebsiteContext = websiteContext;
            SiteConfiguration = siteConfiguration;
        }

        public virtual ImageViewModel GetCheckoutTrustImageViewModel(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, WebsiteContext.WebsiteId);
            var imageInfo = MediaService.GetImageInfo(pagesConfiguration.CreditCardsTrustIconId, cultureInfo);

            return new ImageViewModel
            {
                Url = imageInfo.Url,
                Alt = imageInfo.Alt
            };
        }
    }
}
