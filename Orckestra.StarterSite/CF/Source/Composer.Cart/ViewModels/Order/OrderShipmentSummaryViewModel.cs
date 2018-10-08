using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderShipmentSummaryViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the tracking infos.
        /// </summary>
        public TrackingInfoViewModel TrackingInfo { get; set; }

        /// <summary>
        /// Gets or sets the scheduled ship date.
        /// </summary>
        /// <value>
        /// The scheduled ship date.
        /// </value>
        public string ScheduledShipDate { get; set; }
    }
}
