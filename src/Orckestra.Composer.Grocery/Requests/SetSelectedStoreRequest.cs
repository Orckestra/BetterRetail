using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Grocery.Requests
{
	public class SetSelectedStoreRequest
	{
		[Required]
		[MinLength(1)]
		public string StoreId { get; set; }
	}
}