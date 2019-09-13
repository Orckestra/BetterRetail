using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class LightRecurringOrderCartsViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the recurring carts.
        /// </summary>
        /// <value>
        /// The orders.
        /// </value>
        public IList<LightRecurringOrderCartViewModel> RecurringOrderCarts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this view model is being fetched via AJAX.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }

        public LightRecurringOrderCartsViewModel()
        {
            RecurringOrderCarts = new List<LightRecurringOrderCartViewModel>();
        }
    }
}
