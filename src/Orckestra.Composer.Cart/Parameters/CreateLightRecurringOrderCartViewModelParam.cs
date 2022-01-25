using Orckestra.Composer.Providers.Dam;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CreateLightRecurringOrderCartViewModelParam
    {
        /// <summary>
        /// Cart to map.
        /// </summary>
        public Overture.ServiceModel.Orders.Cart Cart { get; set; }

        /// <summary>
        /// Culture Info for the ViewModel.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Product Image information
        /// </summary>
        public ProductImageInfo ProductImageInfo { get; set; }
                
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
