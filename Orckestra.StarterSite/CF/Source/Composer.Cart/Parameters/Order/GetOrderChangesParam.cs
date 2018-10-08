using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetOrderChangesParam
    {
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the Number of the order to retrieve.
        /// </summary>
        /// <value>
        /// The Order Number.
        /// </value>
        public string OrderNumber { get; set; }
    }
}
