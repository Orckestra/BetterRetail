using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class ImageViewService : IImageViewService
    {
        protected IMediaService MediaService { get; private set; }

        public ImageViewService(IMediaService mediaService)
        {
            if (mediaService == null) { throw new ArgumentNullException("mediaService"); }

            MediaService = mediaService;
        }

        public ImageViewModel GetCheckoutTrustImageViewModel(CultureInfo cultureInfo)
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
