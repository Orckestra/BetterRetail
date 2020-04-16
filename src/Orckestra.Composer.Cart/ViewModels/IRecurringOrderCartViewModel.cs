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
        ///  NextOccurence formatted for visual string display
        /// </summary>
        string FormatedNextOccurence { get; set; }

        /// <summary>
        ///  NextOccurence formatted in YYYY/MM/DD
        /// </summary>
        string NextOccurenceValue { get; set; }

        /// <summary>
        /// Link to recurring schedule page to change the templates
        /// </summary>
        string RecurringScheduleUrl { get; set; }

        /// <summary>
        /// Cart name
        /// </summary>
        string Name { get; set; }
    }
}
