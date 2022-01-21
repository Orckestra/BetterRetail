using System;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public class EditingOrderViewModel
    {
        public string Scope { get; set; }

        public string OrderNumber { get; set; }

        public DateTime? EditableUntil { get; set; }

        public bool IsEditingOrder => !string.IsNullOrEmpty(OrderNumber);

        public string CartUrl { get; set; }
    }
}
