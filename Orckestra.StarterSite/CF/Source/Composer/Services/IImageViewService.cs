using System;
using System.Globalization;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Services
{
    public interface IImageViewService
    {
        /// <summary>
        /// Get the ImageViewModel of the Credit card payment trust image.
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns>The ImageViewModel</returns>
        ImageViewModel GetCheckoutTrustImageViewModel(CultureInfo cultureInfo, Guid websiteId);
    }
}
