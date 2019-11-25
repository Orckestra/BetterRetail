using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class RecurringOrderCartsViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether this view model is being fetched via AJAX.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }
        public RecurringOrderCartsViewModel()
        {
            RecurringOrderCartViewModelList = new List<CartViewModel>();
        }
        public List<CartViewModel> RecurringOrderCartViewModelList { get; set; }
    }
}
