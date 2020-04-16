using System;
using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class UpdatePaymentMethodViewModel : BaseViewModel
    {
        [Required]
        public Guid? PaymentId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string PaymentProviderName { get; set; }
        public string PaymentType { get; set; }

        [Required]
        public Guid? PaymentMethodId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public IEnumerable<string> Providers { get; set; }

        public UpdatePaymentMethodViewModel()
        {
            Providers = Enumerable.Empty<string>();
        }
    }
}
