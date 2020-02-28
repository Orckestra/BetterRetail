using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class ShippingMethodTypesViewModel : BaseViewModel
    {
        public IList<ShippingMethodTypeViewModel> ShippingMethodTypes { get; set; }

        public ShippingMethodTypesViewModel()
        {
            ShippingMethodTypes = new List<ShippingMethodTypeViewModel>();
        }
    }
}
