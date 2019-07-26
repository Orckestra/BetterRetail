using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class RecurringOrderCartsRescheduleResultViewModel : BaseViewModel
    {
        public RecurringOrderCartsViewModel RecurringOrderCartsViewModel { get; set; }

        /// <summary>
        /// Indicates if the cart used for the reschedule has merged with another cart so it no longer should exists
        /// </summary>
        public bool RescheduledCartHasMerged { get; set; }

        /// <summary>
        /// Gets or Sets the url to the recurring carts page
        /// </summary>
        public string RecurringCartsUrl { get; set; }
    }
}
