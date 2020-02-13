using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Store.Requests
{
	public class StoreRequest
	{
		[Required]
		[MinLength(1)]
		public string StoreNumber { get; set; }
	}
}
