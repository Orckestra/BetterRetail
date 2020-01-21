using System.Linq;
using System.Threading;
using Orckestra.Composer;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

// ReSharper disable once CheckNamespace
namespace Composite.AspNet.Razor
{
    public static class WebPagesHtmlHelperExtensions
    {
        /// <summary>
        /// Localizes the strings from Razor using specified key.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Localize(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string category, string key)
        {
            var localizationProvider = ComposerHost.Current.Resolve<ILocalizationProvider>();

            return localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category = category,
                    Key = key,
                    CultureInfo = Thread.CurrentThread.CurrentCulture
                }
            );
        }

        /// <summary>
        /// Localizes the strings from Razor using specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Localized(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string category, string key, params object[] args)
        {
            var formatter = Localize(htmlHelper, category, key);

            //avoiding the "[error]" when using it in a C1 function
            if (formatter == null)
            {
                return $"[{string.Join(string.Empty, args.Select(d => $"{d}, "))}{category}/{key}]";
            }

            return string.Format(formatter, args);
        }

    }
}
