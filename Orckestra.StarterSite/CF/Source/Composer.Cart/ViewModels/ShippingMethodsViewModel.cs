using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class ShippingMethodsViewModel : BaseViewModel
    {
        public IList<ShippingMethodViewModel> ShippingMethods { get; set; }

        public ShippingMethodsViewModel()
        {
            ShippingMethods = new List<ShippingMethodViewModel>();
        }
    }
}
