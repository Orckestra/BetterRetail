using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using Composite.Core.Collections.Generic;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ExceptionFilters
{
    /// <summary>
    /// This exception filter sets an internal server error code and a collection of errors to the content of the HTTP response message
    /// if and only if the unhandled exception is an AggregateException containing only inner exceptions of type ComposerException.
    /// </summary>
    public sealed class AggregatedComposerExceptionFilter : BaseExceptionFilter
    {
        public AggregatedComposerExceptionFilter(IComposerContext context, ILocalizationProvider localizationProvider) : base(context, localizationProvider)
        {
        }

        protected override List<ErrorViewModel> GetLocalizedErrors(HttpActionExecutedContext context)
        {
            var aggregateException = context.Exception as AggregateException;
            if (aggregateException == null)
            {
                return new List<ErrorViewModel>();
            }

            var innerExceptions = aggregateException.Flatten().InnerExceptions;
            if (!innerExceptions.All(e => e is ComposerException))
            {
                return new List<ErrorViewModel>();
            }

            var query = from exception in innerExceptions.OfType<ComposerException>()
                from error in exception.Errors
                select CreateErrorViewModel(error);

            return query.ToList();
        }

        private ErrorViewModel CreateErrorViewModel(ErrorViewModel error)
        {
            var localizedErrorMessage =
                LocalizationProvider.GetLocalizedErrorMessage(error.ErrorCode, Context.CultureInfo);
            var errorModel = new ErrorViewModel
            {
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.ErrorMessage,
                LocalizedErrorMessage = localizedErrorMessage,
            };
            errorModel.Bag.AddDictionary(error.Bag);
            return errorModel;
        }
    }
}