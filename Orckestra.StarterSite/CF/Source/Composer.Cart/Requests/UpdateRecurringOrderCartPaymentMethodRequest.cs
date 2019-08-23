using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Requests
{
    public class UpdateRecurringOrderCartPaymentMethodRequest
    {
        [Required]
        public Guid? PaymentId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string PaymentProviderName { get; set; }

        [Required]
        public Guid? PaymentMethodId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public IEnumerable<string> Providers { get; set; }

        public UpdateRecurringOrderCartPaymentMethodRequest()
        {
            Providers = Enumerable.Empty<string>();
        }
    }
}
