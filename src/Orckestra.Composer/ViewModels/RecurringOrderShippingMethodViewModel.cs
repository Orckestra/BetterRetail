using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    public class RecurringOrderShippingMethodViewModel : BaseViewModel
    {
        /// <summary>
        /// The Shipping Method UI-friendly display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The Shipping Method Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Shipping Provider unique Identifier.
        /// </summary>
        public string ShippingProviderId { get; set; }
    }
}
