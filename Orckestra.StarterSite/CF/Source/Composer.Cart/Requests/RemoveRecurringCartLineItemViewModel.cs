using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.Requests
{
    public class RemoveRecurringCartLineItemViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string LineItemId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CartName { get; set; }
    }
}
