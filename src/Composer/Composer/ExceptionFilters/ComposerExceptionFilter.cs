using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ExceptionFilters
{
    /// <summary>
    /// This exception filter sets an internal server error code and a collection of errors to the content of the HTTP response message
    /// if and only if the unhandled exception is a ComposerException.
    /// </summary>
    public sealed class ComposerExceptionFilter : BaseExceptionFilter
    {
        public ComposerExceptionFilter(IComposerContext context, ILocalizationProvider localizationProvider) : base(context, localizationProvider)
        {
        }

        protected override List<ErrorViewModel> GetLocalizedErrors(HttpActionExecutedContext context)
        {
            var composerException = context.Exception as ComposerException;
            if (composerException == null)
            {
                return new List<ErrorViewModel>();
            }

            var query = from error in composerException.Errors
                        let localizedErrorMessage = LocalizationProvider.GetLocalizedErrorMessage(error.ErrorCode, Context.CultureInfo)
                        select new ErrorViewModel
                        {
                            ErrorCode = error.ErrorCode,
                            ErrorMessage = error.ErrorMessage,
                            LocalizedErrorMessage = localizedErrorMessage
                        };

            return query.ToList();
        }
    }
}