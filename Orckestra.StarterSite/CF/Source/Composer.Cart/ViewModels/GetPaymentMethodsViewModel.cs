using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class GetPaymentMethodsViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public IEnumerable<string> Providers { get; set; }

        public GetPaymentMethodsViewModel()
        {
            Providers = Enumerable.Empty<string>();
        }
    }
}
