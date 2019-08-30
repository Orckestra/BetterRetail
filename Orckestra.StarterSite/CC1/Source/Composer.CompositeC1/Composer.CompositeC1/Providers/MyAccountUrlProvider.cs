using System;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class MyAccountUrlProvider : IMyAccountUrlProvider
    {
        protected IPageService PageService { get; private set; }

        public MyAccountUrlProvider(IPageService pageService)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }
            
            PageService = pageService;
        }

        /// <summary>
        /// Url to the My Account page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetMyAccountUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().MyAccountPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Terms and conditions page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetTermsAndConditionsUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().TermsAndConditionsPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Create Account page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetCreateAccountUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().CreateAccountPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Login page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetLoginUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().LoginPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Forgot Password page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetForgotPasswordUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().ForgotPasswordPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Reset Password page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetNewPasswordUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().ResetPasswordPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Change Password page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetChangePasswordUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().ChangePasswordPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the My Addresses url page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetAddressListUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().AddressListPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Add new address url page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetAddAddressUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().AddAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetUpdateAddressBaseUrl(GetMyAccountUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(SiteConfiguration.GetPagesConfiguration().UpdateAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }
    }
}
