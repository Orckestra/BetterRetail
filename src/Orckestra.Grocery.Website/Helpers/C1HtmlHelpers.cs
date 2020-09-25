using System.Web;
using System.Web.WebPages.Html;
using Orckestra.Composer.CompositeC1.Utils;

namespace Orckestra.Composer.Grocery.Website.Helpers
{
    public class C1HtmlHelpers
    {
        private readonly HtmlHelper _helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="C1HtmlHelper"/> class.
        /// </summary>
        /// <param name="helper">The helper.</param>
        public C1HtmlHelpers(HtmlHelper helper)
        {
            _helper = helper;
        }
        

        public IHtmlString ToAbsolute(string url)
        {
            var fullUrl = UrlUtils.ToAbsolute(url);

            return _helper.Raw(fullUrl.ToString());
        }
    }
}