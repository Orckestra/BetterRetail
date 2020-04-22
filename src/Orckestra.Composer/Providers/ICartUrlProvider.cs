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
        /// Get the Url of the Checkout page.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The Cart url</returns>
        string GetCheckoutPageUrl(BaseUrlParameter parameters);

 
        /// <summary>
        /// Get the Url of the Homepage
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetHomepageUrl(BaseUrlParameter param);

        string GetCheckoutConfirmationPageUrl(BaseUrlParameter param);

        string GetCheckoutSignInUrl(BaseUrlParameter parameters);
    }
}
