using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.Requests
{
    public class UpdateRecurringCartLineItemViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string LineItemId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be greater or equal to 0.")]
        public double? Quantity { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CartName { get; set; }

        public string RecurringOrderFrequencyName { get; set; }
        public string RecurringOrderProgramName { get; set; }
    }
}
