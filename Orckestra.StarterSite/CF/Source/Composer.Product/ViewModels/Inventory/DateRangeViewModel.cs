using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels.Inventory
{
    public sealed class DateRangeViewModel : BaseViewModel
    {
        /// <summary>
        /// The starting date of the range
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// The ending date of the range
        /// </summary>
        public DateTime End { get; set; }
    }
}
