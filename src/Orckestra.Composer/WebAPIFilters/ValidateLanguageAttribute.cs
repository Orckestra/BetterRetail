using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.WebAPIFilters
{
    /// <summary>
    /// WebAPI handler making sure the requests addressed to WebAPI are localized.
    /// </summary>
    public class ValidateLanguageAttribute : ActionFilterAttribute
    {
        private readonly Lazy<ICultureService> _lazyCultureService; 

        /// <summary>
        /// Gets or sets a value indicating whether more than one instance of the indicated attribute can be specified for a single program element.
        /// </summary>
        /// <returns>
        /// true if more than one instance is allowed to be specified; otherwise, false. The default is false.
        /// </returns>
        public override bool AllowMultiple { get { return false; } }

        protected ICultureService CultureService { get { return _lazyCultureService.Value; } }

        public ValidateLanguageAttribute()
        {
            _lazyCultureService = new Lazy<ICultureService>(() => ComposerHost.Current.Resolve<ICultureService>());
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var acceptedLanguages = GetAcceptLanguageHeaders(actionContext.Request);
            var resolvedCulture = GetFirstMatchingCultureInfo(acceptedLanguages, CultureService.GetAllSupportedCultures());

            if (resolvedCulture == null)
            {
                throw new HttpResponseException(BuildBadRequestResponse());
            }

            SetThreadCulture(resolvedCulture);

            base.OnActionExecuting(actionContext);
        }

        private List<string> GetAcceptLanguageHeaders(HttpRequestMessage request)
        {
            var requestValues = request.Headers.AcceptLanguage;

            if (requestValues == null || !requestValues.Any())
            {
                return null;
            }

            return requestValues.OrderByDescending(r => r.Quality ?? 1)
                                .Select(r => r.Value)
                                .ToList();
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
                Content =
                    new StringContent(
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