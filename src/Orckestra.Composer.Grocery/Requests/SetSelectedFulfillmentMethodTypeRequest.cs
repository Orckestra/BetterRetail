using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Grocery.Requests
{
	public class SetSelectedFulfillmentMethodTypeRequest
    {
        [Required]
        [MinLength(1)]
        public string FulfillmentMethodType { get; set; }
    }
}