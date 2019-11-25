using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Requests
{
	public class StoresInventoryRequest
    {
		[Required]
		[MinLength(1)]
		public string Sku { get; set; }
        public Coordinate SearchPoint { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
