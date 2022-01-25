using System;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public class EditingOrderViewModel
    {
        public string Scope { get; set; }

        public Guid OrderId { get; set; }

        public string CartUrl { get; set; }
    }
}
