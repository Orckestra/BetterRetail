using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class ImageViewService : IImageViewService
    {
        protected IMediaService MediaService { get; private set; }
        protected PagesConfiguration PagesConfiguration;

        public ImageViewService(IMediaService mediaService)
        {
            if (mediaService == null) { throw new ArgumentNullException("mediaService"); }

            MediaService = mediaService;
            PagesConfiguration = SiteConfiguration.GetPagesConfiguration();
        }

        public virtual ImageViewModel GetCheckoutTrustImageViewModel(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }

            var imageInfo = MediaService.GetImageInfo(PagesConfiguration.CreditCardsTrustIconId, cultureInfo);

            return new ImageViewModel
            {
                Url = imageInfo.Url,
                Alt = imageInfo.Alt
            };
        }
    }
}
