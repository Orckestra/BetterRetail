using System.Globalization;

namespace Orckestra.Composer.Search.Services
{
	public class GetChildCategoryParam
	{
		public CultureInfo CultureInfo { get; set; }

		public string CategoryId { get; set; }

		public string Scope { get; set; }

		public string BaseUrl { get; set; }
	}
}
