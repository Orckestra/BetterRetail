using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class MinicartViewModel : BaseViewModel
    {
        public string Url { get; set; }
        public string TotalQuantity { get; set; }
        public int NotificationTimeInMilliseconds { get; set; }
    }
}
