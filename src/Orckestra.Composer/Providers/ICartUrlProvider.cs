using System.Collections.Generic;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers.Checkout;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Provider for Cart Url
    /// </summary>
    public interface ICartUrlProvider
    {
        /// <summary>
        /// Get the Url of the Cart page.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The Cart url</returns>
        string GetCartUrl(BaseUrlParameter parameters);

        /// <summary>
        /// Get the Url of the Checkout SignIn page.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The Cart url</returns>
        string GetCheckoutSignInUrl(BaseUrlParameter parameters);

        /// <summary>
        /// Get the Url of the Checkout page.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The Cart url</returns>
        string GetCheckoutPageUrl(BaseUrlParameter parameters);

        /// <summary>
        /// Gets the URL of a specified StepNumber checkout step.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Url of the specified step number.</returns>
        string GetCheckoutStepUrl(GetCheckoutStepUrlParam parameters);

        /// <summary>
        /// Gets the URLs and infos of all checkout step.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Url of the specified step number.</returns>
        Dictionary<int, CheckoutStepPageInfo> GetCheckoutStepPageInfos(BaseUrlParameter parameters);

        /// <summary>
        /// Url to the Add new address in checkout page
        /// </summary>
        /// <returns>localized url</returns>
        string GetCheckoutAddAddressUrl(BaseUrlParameter param);

        /// <summary>
        /// Base Url to the Update address in checkout page
        /// </summary>
        /// <returns>localized url</returns>
        string GetCheckoutUpdateAddressBaseUrl(BaseUrlParameter param);

        /// <summary>
        /// Get the Url of the Homepage
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetHomepageUrl(BaseUrlParameter param);

        string GetCheckoutConfirmationPageUrl(BaseUrlParameter param);
    }
}
