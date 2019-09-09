using System;
using System.Globalization;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    public interface IOrderUrlProvider
    {
        /// <summary>
        /// Gets the order detail base URL.
        /// </summary>
        /// <param name="cultureInfo">The culture info</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        string GetOrderDetailsBaseUrl(CultureInfo cultureInfo, Guid websiteId);

        /// <summary>
        /// Get the order history URL
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetOrderHistoryUrl(GetOrderUrlParameter param);

        /// <summary>
        /// Gets the guest order detail URL.
        /// </summary>
        /// <param name="cultureInfo">The culture info</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        string GetGuestOrderDetailsUrl(CultureInfo cultureInfo, Guid websiteId);

        /// <summary>
        /// Gets the find my order URL.
        /// </summary>
        /// <param name="cultureInfo">The culture info</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        string GetFindMyOrderUrl(CultureInfo cultureInfo, Guid websiteId);
    }
}
