using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public interface IRecurringOrderLineItemViewModel : IExtensionOf<LineItemDetailViewModel>
    {
        /// <summary>
        /// Link to recurring schedule detail page to change the templates
        /// </summary>
        string RecurringScheduleDetailUrl { get; set; }
    }
}
