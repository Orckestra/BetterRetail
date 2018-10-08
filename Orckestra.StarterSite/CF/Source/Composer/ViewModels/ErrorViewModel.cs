namespace Orckestra.Composer.ViewModels
{
    public sealed class ErrorViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the code associated with this error. The code may be used to retrieve a localized resource for this error.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets a message that describes the current error. This message is aimed at developpers and should be used for debugging purposes.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets a localized message that describes the current error.
        /// </summary>
        public string LocalizedErrorMessage { get; set; }
    }
}