using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Requests
{
	public class StoresRequest
	{
        public Bounds MapBounds { get; set; }

        public Coordinate SearchPoint { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
