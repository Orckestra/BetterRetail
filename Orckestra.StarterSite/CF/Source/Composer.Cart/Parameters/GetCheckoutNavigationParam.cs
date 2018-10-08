using System.Collections.Generic;
using Orckestra.Composer.Providers.Checkout;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCheckoutNavigationParam
    {
        /// <summary>
        /// The information for all the checkout steps.
        /// </summary>
        public Dictionary<int, CheckoutStepPageInfo> StepUrls { get; set; }

        /// <summary>
        /// The current step number.
        /// </summary>
        public int CurrentStep { get; set; }
    }
}
