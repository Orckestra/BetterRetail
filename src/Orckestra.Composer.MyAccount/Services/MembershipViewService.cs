using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Providers;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Membership;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Customers;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.MyAccount.Services
{
    /// <summary>
    /// Service for building ViewModel relative to Users
    /// </summary>
    public class MembershipViewService : IMembershipViewService
    {
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ICustomerRepository CustomerRepository { get; private set; }
        protected ICartMergeProvider CartMergeProvider { get; private set; }

        /// <summary>
        /// For Unit test purposes
        /// </summary>
        internal IMembershipProxy Membership { private get; set; }

        public MembershipViewService(
            IMyAccountUrlProvider myAccountUrlProvider,
            IViewModelMapper viewModelMapper,
            ICustomerRepository customerRepository,
            ICartMergeProvider cartMergeProvider)
        {
            Membership = new StaticMembershipProxy();

            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            CartMergeProvider = cartMergeProvider ?? throw new ArgumentNullException(nameof(cartMergeProvider));
        }

        /// <summary>
        /// Gets the configured OvertureMembershipProvider, or the default one if non available.
        /// </summary>
        private MembershipProvider MembershipProvider
        {
            get { return Membership.Providers.OfType<OvertureMembershipProvider>().FirstOrDefault() ?? Membership.Provider; }
        }

        /// <summary>
        /// Create a customer account.
        /// </summary>
        /// <param name="param">Service call params <see cref="CreateUserParam"/></param>
        /// <returns>
        /// The Created in Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<CreateAccountViewModel> RegisterAsync(CreateUserParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) {throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Password)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Password)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.FirstName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.FirstName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.LastName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.LastName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Email)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Email)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var termsAndConditionsUrl = MyAccountUrlProvider.GetTermsAndConditionsUrl(new BaseUrlParameter { CultureInfo = param.CultureInfo });

            var customer = await CustomerRepository.CreateUserAsync(new CreateUserParam
            {
                CustomerId = Guid.Empty,
                Email = param.Email,
                FirstName = param.FirstName,
                LastName = param.LastName,
                Username = param.Username,
                Password = param.Password,
                PasswordQuestion = param.PasswordQuestion,
                PasswordAnswer = param.PasswordAnswer,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope

            }).ConfigureAwait(false);

            return GetCreateAccountViewModel(new GetCreateAccountViewModelParam
            {
                ReturnUrl = param.ReturnUrl,
                Status = customer.AccountStatus == AccountStatus.RequiresApproval ? MyAccountStatus.RequiresApproval : MyAccountStatus.Success,
                CultureInfo = param.CultureInfo,
                Customer = customer,
                TermsAndConditionsUrl = termsAndConditionsUrl
            });
        }

        /// <summary>
        /// Get the view Model to display a Create Account Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetCreateAccountViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Create Account Form
        /// </returns>
        public virtual CreateAccountViewModel GetCreateAccountViewModel(GetCreateAccountViewModelParam param)
        {
            var viewModel = ViewModelMapper.MapTo<CreateAccountViewModel>(param.Customer, param.CultureInfo) ?? new CreateAccountViewModel();

            viewModel.MinRequiredPasswordLength = MembershipProvider.MinRequiredPasswordLength;
            viewModel.MinRequiredNonAlphanumericCharacters = MembershipProvider.MinRequiredNonAlphanumericCharacters;
            viewModel.PasswordRegexPattern = CreatePasswordRegexPattern().ToString();
            viewModel.TermsAndConditionsUrl = param.TermsAndConditionsUrl;
            viewModel.Status = param.Status?.ToString("G") ?? string.Empty;
            viewModel.ReturnUrl = param.ReturnUrl;
            viewModel.IsSuccess = param.Status == MyAccountStatus.Success;
            viewModel.Username = param.Customer != null ? param.Customer.Username : string.Empty;
            viewModel.CustomerId = param.Customer?.Id ?? Guid.Empty;
            viewModel.Created = param.Customer?.Created ?? DateTime.MinValue;

            return viewModel;
        }

        /// <summary>
        /// Logs in a customer.
        /// </summary>
        /// <param name="param">Service call params <see cref="LoginParam"/></param>
        /// <returns>
        /// The logged in Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<LoginViewModel> LoginAsync(LoginParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Password)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Password)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Username)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Username)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var response = new CustomerAndStatus
            {
                Status = MyAccountStatus.Failed
            };

            var createAccountUrl = MyAccountUrlProvider.GetCreateAccountUrl(new BaseUrlParameter { CultureInfo = param.CultureInfo, ReturnUrl = param.ReturnUrl });
            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new BaseUrlParameter { CultureInfo = param.CultureInfo, ReturnUrl = param.ReturnUrl });
            var loginUrl = MyAccountUrlProvider.GetLoginUrl(new BaseUrlParameter { CultureInfo = param.CultureInfo, ReturnUrl = param.ReturnUrl });
            var userName = GenerateUserName(param.Username);

            var loginResponse = Membership.LoginUser(userName, param.Password);

            if (loginResponse)
            {
                var user = Membership.GetUser(userName, true);
                if (user != null && user.ProviderUserKey is Guid && !Guid.Empty.Equals(user.ProviderUserKey) && !user.IsLockedOut)
                {
                    var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
                    {
                        CultureInfo = param.CultureInfo,
                        Scope = param.Scope,
                        CustomerId = (Guid)user.ProviderUserKey
                    }).ConfigureAwait(false);

                    var cartMergeParam = new CartMergeParam
                    {
                        Scope = param.Scope,
                        GuestCustomerId = param.GuestCustomerId,
                        LoggedCustomerId = customer.Id
                    };
                    await CartMergeProvider.MergeCartAsync(cartMergeParam).ConfigureAwait(false);

                    response.Customer = customer;
                    response.Status = MyAccountStatus.Success;
                    if (response.Customer.AccountStatus == AccountStatus.RequiresApproval)
                    {
                        response.Status = MyAccountStatus.RequiresApproval;
                    }
                }
            }

            return GetLoginViewModel(new GetLoginViewModelParam
            {
                ReturnUrl = param.ReturnUrl,
                Status = response.Status,
                Username = userName,
                CultureInfo = param.CultureInfo,
                Customer = response.Customer,
                LoginUrl = loginUrl,
                CreateAccountUrl = createAccountUrl,
                ForgotPasswordUrl = forgotPasswordUrl
            });
        }

        /// <summary>
        /// Gets the LoginViewModel for a specified customer.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual LoginViewModel GetLoginViewModel(GetLoginViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var viewModel = ViewModelMapper.MapTo<LoginViewModel>(param.Customer, param.CultureInfo) ?? new LoginViewModel();

            viewModel.LoginUrl = param.LoginUrl;
            viewModel.CreateAccountUrl = param.CreateAccountUrl;
            viewModel.ForgotPasswordUrl = param.ForgotPasswordUrl;
            viewModel.Status = param.Status.HasValue ? param.Status.Value.ToString("G") : string.Empty;
            viewModel.ReturnUrl = param.ReturnUrl;
            viewModel.IsSuccess = param.Status == MyAccountStatus.Success;
            viewModel.Username = param.Username;
            viewModel.CustomerId = param.Customer != null ? param.Customer.Id : Guid.Empty;

            return viewModel;
        }

        /// <summary>
        /// Get the view Model to display the Sign In Header
        /// </summary>
        /// <returns>
        /// The view model to display the Sign In Header
        /// </returns>
        public virtual async Task<SignInHeaderViewModel> GetSignInHeaderModel(GetSignInHeaderParam param)
        {
            var myAccountUrl = MyAccountUrlProvider.GetMyAccountUrl(new BaseUrlParameter
            {
                CultureInfo = param.CultureInfo
            });

            var loginUrl = MyAccountUrlProvider.GetLoginUrl(new BaseUrlParameter
            {
                CultureInfo = param.CultureInfo
            });

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var viewModel = ViewModelMapper.MapTo<SignInHeaderViewModel>(customer, param.CultureInfo) ?? new SignInHeaderViewModel();

            viewModel.IsLoggedIn = param.IsAuthenticated;

            viewModel.EncryptedCustomerId = param.EncryptedCustomerId;

            viewModel.Url = viewModel.IsLoggedIn ? myAccountUrl : loginUrl;

            return viewModel;
        }

        /// <summary>
        /// Send instructions to the customer on how to reset it's password.
        /// 
        /// Note: To avoid divulging information, the default implementation of ForgotPasswordAsync always succeed;
        /// It ignore all errors and return a success without the Customer information
        /// 
        /// </summary>
        /// <param name="param">Service call params <see cref="ForgotPasswordParam"/></param>
        /// <returns>
        /// The Customer who received the instructions and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<ForgotPasswordViewModel> ForgotPasswordAsync(ForgotPasswordParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Email)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Email)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param));

            try
            {
                var resetParam = new SendResetPasswordInstructionsParam
                {
                    Email = param.Email,
                    Scope = param.Scope
                };
                await CustomerRepository.SendResetPasswordInstructionsAsync(resetParam).ConfigureAwait(false);
            }
            //TODO: process exception
            catch (Exception)
            {
                // To avoid divulging information, the default implementation of ForgotPasswordAsync always succeed;
                // It ignore all errors and return a success
            }

            return GetForgotPasswordViewModelAsync(new GetForgotPasswordViewModelParam
            {
                Status = MyAccountStatus.Success,
                CultureInfo = param.CultureInfo,
                EmailSentTo = param.Email,
            });
        }

        /// <summary>
        /// Get the view Model to display a Forgot Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetForgotPasswordViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Forgot Password Form
        /// </returns>
        protected virtual ForgotPasswordViewModel GetForgotPasswordViewModelAsync(GetForgotPasswordViewModelParam param)
        {
            return new ForgotPasswordViewModel
            {
                Status = param.Status.HasValue ? param.Status.Value.ToString("G") : string.Empty,
                EmailSentTo = param.EmailSentTo
            };
        }

        /// <summary>
        /// Sets the new password for the user idenfied by the secure Ticket.
        /// </summary>
        /// <param name="param">Service call params <see cref="ResetPasswordParam"/></param>
        /// <returns>
        /// The updated Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<ResetPasswordViewModel> ResetPasswordAsync(ResetPasswordParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Ticket)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Ticket)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.NewPassword)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.NewPassword)), nameof(param)); }

            if (string.IsNullOrWhiteSpace(param.PasswordAnswer) && MembershipProvider.RequiresQuestionAndAnswer)
            {
                throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.PasswordAnswer)), nameof(param));
            }

            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new BaseUrlParameter { CultureInfo = param.CultureInfo });

            var customer = await CustomerRepository.GetCustomerByTicketAsync(param.Ticket).ConfigureAwait(false);

            await CustomerRepository.ResetPasswordAsync(
                customer.Username,
                param.Scope,
                param.NewPassword,
                param.PasswordAnswer).ConfigureAwait(false);

            return GetResetPasswordViewModel(new GetResetPasswordViewModelParam
            {
                ReturnUrl = param.ReturnUrl,
                Status = MyAccountStatus.Success,
                Customer = customer,
                CultureInfo = param.CultureInfo,
                ForgotPasswordUrl = forgotPasswordUrl
            });
        }

        /// <summary>
        /// Get the view Model to display a Reset Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetResetPasswordViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Reset Password Form
        /// </returns>
        protected virtual ResetPasswordViewModel GetResetPasswordViewModel(GetResetPasswordViewModelParam param)
        {
            var viewModel = ViewModelMapper.MapTo<ResetPasswordViewModel>(param.Customer, param.CultureInfo) ?? new ResetPasswordViewModel();

            viewModel.Status = param.Status.HasValue ? param.Status.Value.ToString("G") : string.Empty;
            viewModel.MinRequiredPasswordLength = MembershipProvider.MinRequiredPasswordLength;
            viewModel.MinRequiredNonAlphanumericCharacters = MembershipProvider.MinRequiredNonAlphanumericCharacters;
            viewModel.PasswordRegexPattern = CreatePasswordRegexPattern().ToString();
            viewModel.ForgotPasswordUrl = param.ForgotPasswordUrl;
            viewModel.ReturnUrl = param.ReturnUrl;

            if (param.Customer == null)
            {
                viewModel.Status = MyAccountStatus.InvalidTicket.ToString("G");
            }

            return viewModel;
        }

        /// <summary>
        /// Get the view Model to display a Reset Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetResetPasswordByTicketViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Reset Password Form
        /// </returns>
        public virtual async Task<ResetPasswordViewModel> GetCustomerByTicketResetPasswordViewModelAsync(GetResetPasswordByTicketViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new BaseUrlParameter { CultureInfo = param.CultureInfo });

            var customer = await GetCustomerByTicketAsync(new GetCustomerByTicketParam
            {
                Ticket = param.Ticket

            }).ConfigureAwait(false);

            return GetResetPasswordViewModel(new GetResetPasswordViewModelParam
            {
                Customer = customer,
                CultureInfo = param.CultureInfo,
                ForgotPasswordUrl = forgotPasswordUrl
            });
        }

        /// <summary>
        /// Sets the new password for a customer.
        /// </summary>
        /// <param name="param">Service call params <see cref="ChangePasswordParam"/></param>
        /// <returns>
        ///  The updated Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<ChangePasswordViewModel> ChangePasswordAsync(ChangePasswordParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.NewPassword)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.NewPassword)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.OldPassword)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OldPassword)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope
            }).ConfigureAwait(false);

            if (customer == null)
            {
                return GetChangePasswordViewModel(new GetChangePasswordViewModelParam
                {
                    Status = MyAccountStatus.Failed,
                    ReturnUrl = param.ReturnUrl,
                    CultureInfo = param.CultureInfo
                });
            }

            await CustomerRepository.ChangePasswordAsync(
                customer.Username,
                param.Scope,
                param.OldPassword,
                param.NewPassword
            );

            return GetChangePasswordViewModel(new GetChangePasswordViewModelParam
            {
                Status = MyAccountStatus.Success,
                Customer = customer,
                ReturnUrl = param.ReturnUrl,
                CultureInfo = param.CultureInfo
            });
        }

        /// <summary>
        /// Get the view Model to display a Change Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetCustomerChangePasswordViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Change Password Form
        /// </returns>
        public virtual async Task<ChangePasswordViewModel> GetChangePasswordViewModelAsync(GetCustomerChangePasswordViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId
            }).ConfigureAwait(false);

            return GetChangePasswordViewModel(new GetChangePasswordViewModelParam
            {
                Customer = customer,
                CultureInfo = param.CultureInfo,
            });
        }

        /// <summary>
        /// Return true if the user exist
        /// </summary>
        /// <param name="param">Builder params <see cref="GetCustomerByEmailParam"/></param>
        /// <returns>
        /// The view model is user exist
        /// </returns>
        public virtual async Task<IsUserExistViewModel> GetIsUserExistViewModelAsync(GetCustomerByEmailParam getCustomerByEmailParam)
        {
            var customerQueryResult = await CustomerRepository.GetCustomerByEmailAsync(getCustomerByEmailParam).ConfigureAwait(false);

            return new IsUserExistViewModel
            {
                IsExist = customerQueryResult.Results.Any(customer => customer.Email == getCustomerByEmailParam.Email)
            };
        }

        /// <summary>
        /// Get the view Model to display a Change Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetChangePasswordViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Change Password Form
        /// </returns>
        protected virtual ChangePasswordViewModel GetChangePasswordViewModel(GetChangePasswordViewModelParam param)
        {
            var viewModel = param.Customer != null 
                ? ViewModelMapper.MapTo<ChangePasswordViewModel>(param.Customer, param.CultureInfo)
                : new ChangePasswordViewModel();

            viewModel.Status = param.Status.HasValue ? param.Status.Value.ToString("G") : string.Empty;
            viewModel.MinRequiredPasswordLength = MembershipProvider.MinRequiredPasswordLength;
            viewModel.MinRequiredNonAlphanumericCharacters = MembershipProvider.MinRequiredNonAlphanumericCharacters;
            viewModel.PasswordRegexPattern = CreatePasswordRegexPattern().ToString();
            viewModel.ReturnUrl = param.ReturnUrl;

            return viewModel;
        }

        /// <summary>
        /// Create a javascript ready regex to help on client side validation of password
        /// </summary>
        /// <returns></returns>
        protected virtual Regex CreatePasswordRegexPattern()
        {
            return new Regex(@"(.*(?:[\!\@\#\$\%\^\&\*\(\)_\-\+\=\[\{\]\}\;\:\>\|\.\/\?]).*){" + MembershipProvider.MinRequiredNonAlphanumericCharacters + "}");            
        }

        /// <summary>
        /// Resolve the Customer identified by the ticket
        /// </summary>
        /// <param name="getCustomerByTicketParam">Service call params <see cref="GetCustomerByTicketParam"/></param>
        /// <returns>
        /// The customer found; or null if the ticket is invalid in any way
        /// </returns>
        protected virtual async Task<Customer> GetCustomerByTicketAsync(GetCustomerByTicketParam getCustomerByTicketParam)
        {
            if (string.IsNullOrEmpty(getCustomerByTicketParam.Ticket)) { return null; }

            try
            {
                return await CustomerRepository.GetCustomerByTicketAsync(getCustomerByTicketParam.Ticket).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // To avoid divulging information on the failure cause, the default implementation of GetCustomerByTickeAsync;
                // either succeed or fail with no other information.

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalUsername"></param>
        /// <returns></returns>
        protected virtual string GenerateUserName(string originalUsername)
        {
            return originalUsername;
        }
    }
}