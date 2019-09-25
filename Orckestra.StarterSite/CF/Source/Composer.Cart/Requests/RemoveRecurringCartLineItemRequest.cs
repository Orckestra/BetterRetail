using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Cart.Requests
{
    public class RemoveRecurringCartLineItemRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string LineItemId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CartName { get; set; }
    }
}
