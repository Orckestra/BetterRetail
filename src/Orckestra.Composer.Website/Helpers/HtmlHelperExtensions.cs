using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace Orckestra.Composer.Website.Helpers
{
    /// <summary>
    /// Add C1 specific extension methods for Razor functions
    /// </summary>
	public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Exposes C1 specific functionality
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
		public static C1HtmlHelpers RefApp(this HtmlHelper helper)
        {
            return new C1HtmlHelpers(helper);
        }
    }
}