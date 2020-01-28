using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using Composite.Core;
using Orckestra.Composer;
using Orckestra.Composer.CompositeC1.Providers;
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


        public static IHtmlString LazyFunction(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string name)
        {
            return LazyFunction(htmlHelper, name, new { });
        }

        public static IHtmlString LazyFunction(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string name, object parameters)
        {
            //TODO: add function that collect all current razor function parameters and pass to lazy. ?
            //TODO: render directly in preview mode
            var lazyPartialProvider = ServiceLocator.GetService<ILazyFunctionCallDataProvider>();
            Debug.Assert(lazyPartialProvider != null, nameof(lazyPartialProvider) + " != null");

            var protectedFunctionCall = lazyPartialProvider.ProtectFunctionCall(name,
                Functions.ObjectToDictionary(parameters)
                    .Where(d => d.Value != null)
                    .ToDictionary(d => d.Key, d => d.Value.ToString()));

            return new HtmlString(
                new XElement("div",
                    new XAttribute("data-oc-controller", "General.Lazy"),
                    new XAttribute("data-request", protectedFunctionCall)).ToString());

        }

    }
}
