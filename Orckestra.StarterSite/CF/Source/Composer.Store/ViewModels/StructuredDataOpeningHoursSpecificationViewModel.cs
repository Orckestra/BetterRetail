
using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StructuredDataOpeningHoursSpecificationViewModel: BaseViewModel
    {
        public List<DayOfWeek> DayOfWeeks { get; set; }
        /// <summary>
        /// The time the business location opens, in hh:mm:ss format.
        /// </summary>
        public string Opens { get; set; }
        /// <summary>
        /// The time the business location closes, in hh:mm:ss format.
        /// </summary>
        public string Closes { get; set; }
        /// <summary>
        /// The start date of a seasonal business closure, in YYYY-MM-DD format.
        /// </summary>
        public string ValidFrom { get; set; }
        /// <summary>
        /// The end date of a seasonal business closure, in YYYY-MM-DD format.
        /// </summary>
        public string ValidThrough { get; set; }
    }
}
