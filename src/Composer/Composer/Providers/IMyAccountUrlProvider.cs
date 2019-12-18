using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Provider for urls required by the MyAccount Concerns
    /// </summary>
    public interface IMyAccountUrlProvider
    {
        /// <summary>
        /// Url to the My Account page
        /// </summary>
        /// <returns>localized url</returns>
        string GetMyAccountUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the Terms and conditions page
        /// </summary>
        /// <returns>localized url</returns>
        string GetTermsAndConditionsUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the Create Account page
        /// </summary>
        /// <returns>localized url</returns>
        string GetCreateAccountUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the Login page
        /// </summary>
        /// <returns>localized url</returns>
        string GetLoginUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the Forgot password url page
        /// </summary>
        /// <returns>localized url</returns>
        string GetForgotPasswordUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the Reset password url page
        /// </summary>
        /// <returns>localized url</returns>
        string GetNewPasswordUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the Change password url page
        /// </summary>
        /// <returns>localized url</returns>
        string GetChangePasswordUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the My Addresses url page
        /// </summary>
        /// <returns>localized url</returns>
        string GetAddressListUrl(BaseUrlParameter param);

        /// <summary>
        /// Url to the Add new address url page
        /// </summary>
        /// <returns>localized url</returns>
        string GetAddAddressUrl(BaseUrlParameter param);

        /// <summary>
        /// Base Url to the Update url page
        /// </summary>
        /// <returns>localized url</returns>
        string GetUpdateAddressBaseUrl(BaseUrlParameter param);

    }
}
