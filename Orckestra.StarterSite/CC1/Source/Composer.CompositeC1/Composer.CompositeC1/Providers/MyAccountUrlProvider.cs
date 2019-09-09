using System;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class MyAccountUrlProvider : IMyAccountUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public MyAccountUrlProvider(IPageService pageService, IComposerContext composerContext)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }
            
            PageService = pageService;
            ComposerContext = composerContext;
        }

        /// <summary>
        /// Url to the My Account page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetMyAccountUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.MyAccountPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Terms and conditions page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetTermsAndConditionsUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.TermsAndConditionsPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Create Account page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetCreateAccountUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.CreateAccountPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Login page
        /// </summary>
        /// <returns>localized url</returns>
        public virtual string GetLoginUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.LoginPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Forgot Password page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetForgotPasswordUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.ForgotPasswordPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Reset Password page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetNewPasswordUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.ResetPasswordPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Change Password page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetChangePasswordUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.ChangePasswordPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the My Addresses url page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetAddressListUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.AddressListPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        /// <summary>
        /// Url to the Add new address url page
        /// </summary>
        /// <returns>localized url</returns>
        public string GetAddAddressUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.AddAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetUpdateAddressBaseUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.UpdateAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }
    }
}
