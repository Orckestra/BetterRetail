using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.MvcFilters;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    [ValidateReturnUrl]
    public abstract class MembershipBaseController : Controller
    {
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IMembershipViewService MembershipViewService { get; private set; }
        protected ICustomerSettings CustomerSettings { get; private set; }

        protected MembershipBaseController(
            IMyAccountUrlProvider myAccountUrlProvider,
            IComposerContext composerContext,
            IMembershipViewService membershipViewService,
            ICustomerSettings customerSettings)
        {
            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            MembershipViewService = membershipViewService ?? throw new ArgumentNullException(nameof(membershipViewService));
            CustomerSettings = customerSettings ?? throw new ArgumentNullException(nameof(customerSettings));
        }

        [AllowAnonymous]
        [MustBeAnonymous(MustBeAnonymousAttribute.MyAccountDestination)]
        public virtual ActionResult ReturningCustomerBlade()
        {
            var returnUrl = GetReturnUrlToPreserve();

            var loginUrl = MyAccountUrlProvider.GetLoginUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = returnUrl
            });

            var createAccountUrl = MyAccountUrlProvider.GetCreateAccountUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = returnUrl
            });

            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = returnUrl
            });

            var loginViewModel = MembershipViewService.GetLoginViewModel(new GetLoginViewModelParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = returnUrl,
                CreateAccountUrl = createAccountUrl,
                ForgotPasswordUrl = forgotPasswordUrl,
                LoginUrl = loginUrl
            });
            var profileSettings = CustomerSettings.GetProfileSettingsAsync().Result;
            loginViewModel.UseEmailAsUsername = profileSettings.UseEmailAsUsername;

            return View("ReturningCustomerBlade", loginViewModel);
        }

        [AllowAnonymous]
        [MustBeAnonymous(MustBeAnonymousAttribute.MyAccountDestination)]
        public virtual ActionResult NewCustomerBlade()
        {
            var returnUrl = GetReturnUrlToPreserve();

            var loginUrl = MyAccountUrlProvider.GetLoginUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = returnUrl
            });

            var createAccountUrl = MyAccountUrlProvider.GetCreateAccountUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = returnUrl
            });

            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = returnUrl
            });

            var loginViewModel = MembershipViewService.GetLoginViewModel(new GetLoginViewModelParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                CreateAccountUrl = createAccountUrl,
                ForgotPasswordUrl = forgotPasswordUrl,
                LoginUrl = loginUrl,
                ReturnUrl = returnUrl
            });

            return View("NewCustomerBlade", loginViewModel);
        }

        [AllowAnonymous]
        [MustBeAnonymous(MustBeAnonymousAttribute.MyAccountDestination)]
        public virtual ActionResult CreateAccountBlade()
        {
            var termsAndConditionsUrl = MyAccountUrlProvider.GetTermsAndConditionsUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            });

            var createAccountViewModel =
                MembershipViewService.GetCreateAccountViewModel(new GetCreateAccountViewModelParam
                {
                    ReturnUrl = GetReturnUrlToPreserve(),
                    TermsAndConditionsUrl = termsAndConditionsUrl
                });
            var profileSettings = CustomerSettings.GetProfileSettingsAsync().Result;
            createAccountViewModel.UseEmailAsUsername = profileSettings.UseEmailAsUsername;

            return View("CreateAccountBlade", createAccountViewModel);
        }

        [AllowAnonymous]
        [MustBeAnonymous(MustBeAnonymousAttribute.MyAccountDestination)]
        public virtual ActionResult ForgotPasswordBlade()
        {
            return View("ForgotPasswordBlade", new ForgotPasswordViewModel());
        }

        [AllowAnonymous]
        [MustBeAnonymous(MustBeAnonymousAttribute.MyAccountDestination)]
        public virtual ActionResult NewPasswordBlade()
        {
            var ticket = Request.QueryString["ticket"] ?? string.Empty;

            var resetPasswordViewModel = MembershipViewService.GetCustomerByTicketResetPasswordViewModelAsync(new GetResetPasswordByTicketViewModelParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                Ticket = ticket
            }).Result;

            return View("NewPasswordBlade", resetPasswordViewModel);
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult ChangePasswordBlade()
        {
            var changePasswordViewModel = MembershipViewService.GetChangePasswordViewModelAsync(new GetCustomerChangePasswordViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId
            }).Result;

            return View("ChangePasswordBlade", changePasswordViewModel);
        }

        /// <summary>
        /// Find the ReturnUrl to preserve when swapping from login to creating.
        /// Or Empty if none provided
        /// </summary>
        /// <returns>ReturnUrl to preserve, or String.Empty if none found</returns>
        protected virtual string GetReturnUrlToPreserve()
        {
            return Request.QueryString["ReturnUrl"] ?? string.Empty;
        }
    }
}