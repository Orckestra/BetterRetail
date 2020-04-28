using System;
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

        public static IHtmlString HelpBubble(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string category, string key, params object[] args)
        {
            var helpText = Localize(category, key);

            if (string.IsNullOrEmpty(helpText))
            {
                return new HtmlString("");
            }

            if (args.Length > 0)
            {
                helpText = Localized(category, key, args);
            }

            return new HtmlString(string.Format(@"data-toggle=""popover""
            data-container=""body""
            data-trigger=""focus""
            data-content=""&lt;div class='multiline-message'&gt;&lt;span class='multiline-message-icon  fa  fa-comment-o  fa-lg'&gt;&lt;/span&gt;{0}&lt;/div&gt;""", helpText));
        }

        public static IHtmlString ParsleyMessage(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string category, string key, string dataParsleyKey)
        {
            var message = Localize(category, key);
            if (!string.IsNullOrEmpty(message))
            {
                message = string.Format("data-parsley-{0}=\"{1}\"", dataParsleyKey, message);
                return new HtmlString(HttpUtility.HtmlDecode(message));
            }

            return new HtmlString("");
        }
        /// <summary>
        /// Localizes the strings from Razor using specified key.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Localize(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string category, string key)
        {
            return Localize(category, key);
        }

        /// <summary>
        /// Localizes the strings from MVC using specified key.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Localize(this System.Web.Mvc.HtmlHelper htmlHelper, string category, string key)
        {
            return Localize(category, key);
        }

        public static string Localized(this System.Web.Mvc.HtmlHelper htmlHelper, string category, string key, params object[] args)
        {
            return Localized(category, key, args);
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
            return Localized(category, key, args);
        }

        private static string Format(string formatter, string category, string key, params object[] args)
        {
            //avoiding the "[error]" when using it in a C1 function
            if (formatter == null)
            {
                return $"[{string.Join(string.Empty, args.Select(d => $"{d}, "))}{category}/{key}]";
            }

            return string.Format(formatter, args);
        }

        public static string Localize(string category, string key)
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

        public static string Localized(string category, string key, params object[] args)
        {
            var formatter = Localize(category, key);

            return Format(formatter, category, key, args);
        }


        public static IHtmlString LazyFunction(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string name, string className = null, string loaderClassName = null)
        {
            return LazyFunction(htmlHelper, name, new { }, className, loaderClassName);
        }

        public static IHtmlString LazyFunction(this System.Web.WebPages.Html.HtmlHelper htmlHelper, string name, object parameters, string className = null, string loaderClassName = null)
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
                    new XAttribute("class", className ?? "text-center  text-muted  js-loading"),
                    new XAttribute("data-oc-controller", "General.Lazy"),
                    new XAttribute("data-request", protectedFunctionCall),
                    new XElement("span",
                        new XAttribute("class", loaderClassName ?? "fa  fa-spin  fa-circle-o-notch  fa-2x"))
                    ).ToString());

        }

    }
}
