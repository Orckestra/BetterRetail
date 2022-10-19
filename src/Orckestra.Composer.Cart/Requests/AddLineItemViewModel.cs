using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Cart.Requests
{
    public sealed class AddLineItemViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string ProductId { get; set; }

        public string VariantId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be greater or equal to 0.")]
        public double? Quantity  { get; set; }

        public string RecurringOrderFrequencyName { get; set; }
        public string RecurringOrderProgramName { get; set; }

        public PropertyBag PropertyBag { get; set; }
    }
}
