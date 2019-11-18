using Orckestra.Composer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    public class RecurringProductSummaryViewModel: BaseViewModel
    {
        /// <summary>
        /// The display name of the product
        /// </summary>
        public string DisplayName { get; set; }

        [Lookup(LookupType.Product, "Brand")]
        public string Brand { get; set; }
    }
}
