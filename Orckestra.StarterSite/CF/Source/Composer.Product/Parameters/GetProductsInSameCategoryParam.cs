using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Product.Parameters
{
    public class GetProductsInSameCategoryParam
    {
        public string CategoryId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string Scope { get; set; }
        public List<string> InventoryLocationIds { get; set; }
        public int MaxItems { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        /// <summary>
        /// Purpose is to be excluded from same categories results
        /// </summary>
        public string CurrentProductId { get; set; }
    }
}
