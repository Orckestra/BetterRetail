using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CouponViewModel : BaseViewModel
    {
        public string CouponCode { get; set; }

        public string DisplayText { get; set; }

        public Guid PromotionId { get; set; }

        public string PromotionName { get; set; }

        public decimal Amount { get; set; }
    }
}
