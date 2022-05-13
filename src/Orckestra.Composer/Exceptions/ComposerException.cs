using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Orckestra.Composer.ViewModels;
using ServiceStack;
using ServiceStack.Validation;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Exceptions
{
    public sealed class ComposerException : Exception
    {
        /// <summary>
        /// Gets the list of errors describing why Composer failed.
        /// </summary>
        public List<ErrorViewModel> Errors { get; }

        public ComposerException(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(errorCode)); }

            Errors = new List<ErrorViewModel>
            {
                new ErrorViewModel
                {
                    ErrorCode = errorCode
                }
            };
        }

        public ComposerException(ErrorViewModel error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            Errors = new List<ErrorViewModel> { error };
        }

        public ComposerException(List<ErrorViewModel> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Throws this exception if the list of errors is not empty.
        /// </summary>
        public void ThrowIfAnyError()
        {
            if (Errors.Any()) { throw this; }
        }

        public override string Message
        {
            get
            {
                return Errors.Count == 0
                    ? "No errors"
                    : string.Join(Environment.NewLine,
                    Errors.Select(e => $"Error code '{e.ErrorCode ?? "<undefined>"}': {e.ErrorMessage}"));
            }
        }

        internal static ComposerException Create(ValidationError source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var errors = source.Violations.Select(violation => new ErrorViewModel
            {
                ErrorCode = violation.ErrorCode,
                ErrorMessage = violation.ErrorMessage

            }).ToList();

            if (errors.All(e => e.ErrorCode != source.ErrorCode))
            {
                errors.Add(new ErrorViewModel
                {
                    ErrorCode = source.ErrorCode,
                    ErrorMessage = source.ErrorMessage
                });
            }

            return new ComposerException(errors);
        }

        internal static ComposerException Create(WebServiceException source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var errors = source.GetFieldErrors().Select(error => new ErrorViewModel
            {
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.Message

            }).ToList();

            if (errors.All(e => e.ErrorCode != source.ErrorCode))
            {
                errors.Add(new ErrorViewModel
                {
                    ErrorCode = source.ErrorCode,
                    ErrorMessage = source.ErrorMessage
                });
            }

            return new ComposerException(errors);
        }

        internal static ComposerException Create(WebException source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var errors = new List<ErrorViewModel>
            {
                new ErrorViewModel
                {
                    ErrorCode = "Web",
                    ErrorMessage = source.Message
                }
            };

            return new ComposerException(errors);
        }
    }
}