using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.MvcFilters
{
    /// <summary>
    /// WebAPI handler making sure the requests addressed to WebAPI are localized.
    /// </summary>
    public class ValidateLanguageAttribute : ActionFilterAttribute
    {
        private readonly Lazy<ICultureService> _lazyCultureService; 

        protected ICultureService CultureService { get { return _lazyCultureService.Value; } }

        public ValidateLanguageAttribute()
        {
            _lazyCultureService = new Lazy<ICultureService>(() => ComposerHost.Current.Resolve<ICultureService>());
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var acceptedLanguages = GetAcceptLanguageHeaders(filterContext.HttpContext.Request);
            var resolvedCulture = GetFirstMatchingCultureInfo(acceptedLanguages, CultureService.GetAllSupportedCultures());

            if (resolvedCulture == null) { throw new HttpResponseException(BuildBadRequestResponse()); }

            SetThreadCulture(resolvedCulture);

            base.OnActionExecuting(filterContext);
        }

        private List<string> GetAcceptLanguageHeaders(HttpRequestBase request)
        {
            var requestValues = request.UserLanguages;

            if (requestValues == null || !requestValues.Any()) { return null; }

            return requestValues.Select(GetQuantifiedValue)
                                .OrderByDescending(t => t.Item1)
                                .Select(t => t.Item2)
                                .ToList();
        }

        /// <summary>
        /// Split on ; quantified User Language values
        /// </summary>
        /// <param name="couple"></param>
        /// <returns></returns>
        private Tuple<decimal, string> GetQuantifiedValue(string couple)
        {
            decimal quantity = 1.0m;
            string value = string.Empty;

            var parts = (couple ?? string.Empty).Split(';');
            if (parts.Length > 0)
            {
                value = parts[0];
            }
            if (parts.Length > 1)
            {
                decimal.TryParse(parts[1], out quantity);
            }

            return Tuple.Create(quantity, value);
        }

        private CultureInfo GetFirstMatchingCultureInfo(List<string> acceptedLanguages, CultureInfo[] supportedCultures)
        {
            if (acceptedLanguages is null || !acceptedLanguages.Any() || supportedCultures is null || !supportedCultures.Any()) { return null; }

            foreach (var supportedCulture in supportedCultures)
            {
                foreach (var acceptLanguage in acceptedLanguages)
                {
                    if (acceptLanguage.Equals(supportedCulture.Name, StringComparison.OrdinalIgnoreCase)
                        || acceptLanguage.Equals(supportedCulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
                    {
                        return supportedCulture;
                    }
                }
            }
            return null;
        }

        private HttpResponseMessage BuildBadRequestResponse()
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(
                    "The HTTP Header 'Accept-Language' was not set or the value was not resolved into a supported culture.")
            };
        }

        private void SetThreadCulture(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}