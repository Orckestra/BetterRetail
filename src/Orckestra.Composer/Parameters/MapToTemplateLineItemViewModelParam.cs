using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
    public class MapToTemplateLineItemViewModelParam
    {
        /// <summary>
        /// RecurringOrderlineItem to Map
        /// </summary>
        public RecurringOrderLineItem RecurringOrderlineItem { get; set; }

        /// <summary>
        /// Culture Info for the ViewModel.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Image dictionnary for the lineitem
        /// </summary>
        public IDictionary<(string, string), ProductMainImage> ImageDictionnary { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }     
        
        /// <summary>
        /// The recurring schedule details base url
        /// </summary>
        public string RecurringScheduleUrl { get; set; }
    }
}
