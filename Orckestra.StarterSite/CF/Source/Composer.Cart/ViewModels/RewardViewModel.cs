using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class RewardViewModel : BaseViewModel
    {
        public string Description { get; set; }
        public Guid PromotionId { get; set; }
        public decimal Amount { get; set; }
        public string PromotionName { get; set; }
    }
}
