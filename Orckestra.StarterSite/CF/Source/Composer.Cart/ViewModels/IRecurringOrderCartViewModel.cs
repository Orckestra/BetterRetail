using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public interface IRecurringOrderCartViewModel : IExtensionOf<CartViewModel>
    {
        /// <summary>
        /// Date used to identify the cart date
        /// </summary>
        DateTime NextOccurence { get; set; }

        /// <summary>
        ///  Date formatted used to identify the cart date
        /// </summary>
        string FormatedNextOccurence { get; set; }
    }
}
