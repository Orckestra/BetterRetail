using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Orckestra.Composer.ViewModels;
using ServiceStack;
using ServiceStack.Validation;

namespace Orckestra.Composer.Exceptions
{
    public sealed class ComposerException : Exception
    {
        /// <summary>
        /// Gets the list of errors describing why Composer failed.
        /// </summary>
        public List<ErrorViewModel> Errors { get; private set; }

        public ComposerException(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
            {
                var errorMessage = "The error code cannot be null or whitespace";
                var argumentNullException = new ArgumentException(errorMessage);

                throw argumentNullException;
            }

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
            if (error == null)
            {
                var errorMessage = "error";
                var argumentNullException = new ArgumentNullException(errorMessage);

                throw argumentNullException;
            }

            Errors = new List<ErrorViewModel> { error };
        }

        public ComposerException(List<ErrorViewModel> errors)
        {
            if (errors == null)
            {
                var errorMessage = "errors";
                var argumentNullException = new ArgumentNullException(errorMessage);

                throw argumentNullException;
            }

            Errors = errors;
        }

        /// <summary>
        /// Throws this exception if the list of errors is not empty.
        /// </summary>
        public void ThrowIfAnyError()
        {
            if (Errors.Any())
            {
                throw this;
            }
        }

        public override string Message 
        {
            get
            {
                if (Errors.Count == 0)
                {
                    return "No errors";
                }

                return string.Join(Environment.NewLine, 
                    Errors.Select(e => $"Error code '{e.ErrorCode ?? "<undefined>"}': {e.ErrorMessage}"));
            }
        }
        
        internal static ComposerException Create(ValidationError source)
        {
            if (source == null)
            {
                var errorMessage = "source";
                var argumentNullException = new ArgumentNullException(errorMessage);

                throw argumentNullException;
            }

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

            var exception = new ComposerException(errors);

            return exception;
        }

        internal static ComposerException Create(WebServiceException source)
        {
            if (source == null)
            {
                var errorMessage = "source";
                var argumentNullException = new ArgumentNullException(errorMessage);

                throw argumentNullException;
            }

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

            var exception = new ComposerException(errors);

            return exception;
        }

        internal static ComposerException Create(WebException source)
        {
            if (source == null)
            {
                var errorMessage = "source";
                var argumentNullException = new ArgumentNullException(errorMessage);

                throw argumentNullException;
            }

            var errors = new List<ErrorViewModel>
            {
                new ErrorViewModel
                {
                    ErrorCode = "Web",
                    ErrorMessage = source.Message
                }
            };

            var exception = new ComposerException(errors);

            return exception;
        }
    }
}