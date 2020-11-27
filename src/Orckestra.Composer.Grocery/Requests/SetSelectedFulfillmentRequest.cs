using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Grocery.Requests
{
	public class SetSelectedFulfillmentRequest
    {
        [Required]
        [MinLength(1)]
        public string FulfillmentMethodType { get; set; }

        [Required]
        [MinLength(1)]
        public string StoreId { get; set; }

    }
}