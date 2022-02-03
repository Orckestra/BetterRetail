using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Requests
{
    public class CancelOrderRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string OrderNumber { get; set; }
    }
}
