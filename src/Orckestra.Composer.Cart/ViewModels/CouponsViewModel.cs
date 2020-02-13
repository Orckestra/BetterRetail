using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CouponsViewModel : BaseViewModel
    {
        public List<CouponViewModel> ApplicableCoupons { get; set; }

        public List<CartMessageViewModel> Messages { get; set; }

        public CouponsViewModel()
        {
            ApplicableCoupons = new List<CouponViewModel>();
            Messages = new List<CartMessageViewModel>();
        }
    }
}
