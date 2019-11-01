using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ExceptionFilters
{
    public abstract class BaseExceptionFilter : IAutofacExceptionFilter
    {
        protected IComposerRequestContext Context { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        protected BaseExceptionFilter(IComposerRequestContext context, ILocalizationProvider localizationProvider)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (localizationProvider == null)
            {
                throw new ArgumentNullException("localizationProvider");
            }

            Context = context;
            LocalizationProvider = localizationProvider;
        }

        public void OnException(HttpActionExecutedContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            List<ErrorViewModel> errors = GetLocalizedErrors(context);
            if (errors.Count == 0)
            {
                return;
            }

            var value = new { Errors = errors };

            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = "Composer Exception",
                Content = new ObjectContent(value.GetType(), value, new JsonMediaTypeFormatter())
            };
        }

        protected abstract List<ErrorViewModel> GetLocalizedErrors(HttpActionExecutedContext context);
    }
}