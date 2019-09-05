using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Globalization;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class ImageViewService : IImageViewService
    {
        protected IMediaService MediaService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public ImageViewService(IMediaService mediaService, IComposerContext composerContext)
        {
            if (mediaService == null) { throw new ArgumentNullException("mediaService"); }

            MediaService = mediaService;
            ComposerContext = composerContext;
        }

        public virtual ImageViewModel GetCheckoutTrustImageViewModel(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, ComposerContext.WebsiteId);
            var imageInfo = MediaService.GetImageInfo(pagesConfiguration.CreditCardsTrustIconId, cultureInfo);

            return new ImageViewModel
            {
                Url = imageInfo.Url,
                Alt = imageInfo.Alt
            };
        }
    }
}
