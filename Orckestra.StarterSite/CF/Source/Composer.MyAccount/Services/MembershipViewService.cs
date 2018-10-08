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
            if (myAccountUrlProvider == null) { throw new ArgumentNullException("myAccountUrlProvider"); }
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }
            if (customerRepository == null) { throw new ArgumentNullException("customerRepository"); }
            if (cartMergeProvider == null) { throw new ArgumentNullException("cartMergeProvider"); }

            Membership = new StaticMembershipProxy();

            MyAccountUrlProvider = myAccountUrlProvider;
            ViewModelMapper = viewModelMapper;
            CustomerRepository = customerRepository;
            CartMergeProvider = cartMergeProvider;
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
        /// <param name="createUserParam">Service call params <see cref="CreateUserParam"/></param>
        /// <returns>
        /// The Created in Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<CreateAccountViewModel> RegisterAsync(CreateUserParam createUserParam)
        {
            if (createUserParam == null) { throw new ArgumentNullException("createUserParam"); }
            if (createUserParam.CultureInfo == null) { throw new ArgumentException("createUserParam.CultureInfo"); }
            if (string.IsNullOrWhiteSpace(createUserParam.Password)) { throw new ArgumentException("createUserParam.Password"); }
            if (string.IsNullOrWhiteSpace(createUserParam.FirstName)) { throw new ArgumentException("createUserParam.FirstName"); }
            if (string.IsNullOrWhiteSpace(createUserParam.LastName)) { throw new ArgumentException("createUserParam.LastName"); }
            if (string.IsNullOrWhiteSpace(createUserParam.Email)) { throw new ArgumentException("createUserParam.Email"); }
            if (string.IsNullOrWhiteSpace(createUserParam.Scope)) { throw new ArgumentException("createUserParam.Scope"); }

            var termsAndConditionsUrl = MyAccountUrlProvider.GetTermsAndConditionsUrl(new GetMyAccountUrlParam { CultureInfo = createUserParam.CultureInfo });

            var customer = await CustomerRepository.CreateUserAsync(new CreateUserParam
            {
                CustomerId = Guid.Empty,
                Email = createUserParam.Email,
                FirstName = createUserParam.FirstName,
                LastName = createUserParam.LastName,
                Username = createUserParam.Username,
                Password = createUserParam.Password,
                PasswordQuestion = createUserParam.PasswordQuestion,
                PasswordAnswer = createUserParam.PasswordAnswer,
                CultureInfo = createUserParam.CultureInfo,
                Scope = createUserParam.Scope

            }).ConfigureAwait(false);

            return GetCreateAccountViewModel(new GetCreateAccountViewModelParam
            {
                ReturnUrl = createUserParam.ReturnUrl,
                Status = customer.AccountStatus == AccountStatus.RequiresApproval ? MyAccountStatus.RequiresApproval : MyAccountStatus.Success,
                CultureInfo = createUserParam.CultureInfo,
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
        /// <param name="loginParam">Service call params <see cref="LoginParam"/></param>
        /// <returns>
        /// The logged in Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<LoginViewModel> LoginAsync(LoginParam loginParam)
        {
            if (loginParam == null) { throw new ArgumentNullException("loginParam"); }
            if (loginParam.CultureInfo == null) { throw new ArgumentException("loginParam.CultureInfo"); }
            if (string.IsNullOrWhiteSpace(loginParam.Password)) { throw new ArgumentException("loginParam.Password"); }
            if (string.IsNullOrWhiteSpace(loginParam.Username)) { throw new ArgumentException("loginParam.Username"); }
            if (string.IsNullOrWhiteSpace(loginParam.Scope)) { throw new ArgumentException("loginParam.Scope"); }

            var response = new CustomerAndStatus
            {
                Status = MyAccountStatus.Failed
            };

            var createAccountUrl = MyAccountUrlProvider.GetCreateAccountUrl(new GetMyAccountUrlParam { CultureInfo = loginParam.CultureInfo, ReturnUrl = loginParam.ReturnUrl });
            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new GetMyAccountUrlParam { CultureInfo = loginParam.CultureInfo, ReturnUrl = loginParam.ReturnUrl });
            var loginUrl = MyAccountUrlProvider.GetLoginUrl(new GetMyAccountUrlParam { CultureInfo = loginParam.CultureInfo, ReturnUrl = loginParam.ReturnUrl });
            var userName = GenerateUserName(loginParam.Username);

            var loginResponse = Membership.LoginUser(userName, loginParam.Password);

            if (loginResponse)
            {
                var user = Membership.GetUser(userName, true);
                if (user != null && user.ProviderUserKey is Guid && !Guid.Empty.Equals(user.ProviderUserKey) && !user.IsLockedOut)
                {
                    var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
                    {
                        CultureInfo = loginParam.CultureInfo,
                        Scope = loginParam.Scope,
                        CustomerId = (Guid)user.ProviderUserKey
                    }).ConfigureAwait(false);


                    var cartMergeParam = new CartMergeParam
                    {
                        Scope = loginParam.Scope,
                        GuestCustomerId = loginParam.GuestCustomerId,
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
                ReturnUrl = loginParam.ReturnUrl,
                Status = response.Status,
                Username = userName,
                CultureInfo = loginParam.CultureInfo,
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
            if (param == null) { throw new ArgumentNullException("loginParam"); }
            if (param.CultureInfo == null) { throw new ArgumentException("loginParam.CultureInfo"); }

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
            var myAccountUrl = MyAccountUrlProvider.GetMyAccountUrl(new GetMyAccountUrlParam
            {
                CultureInfo = param.CultureInfo
            });

            var loginUrl = MyAccountUrlProvider.GetLoginUrl(new GetMyAccountUrlParam
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
        /// <param name="forgotPasswordParam">Service call params <see cref="ForgotPasswordParam"/></param>
        /// <returns>
        /// The Customer who received the instructions and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<ForgotPasswordViewModel> ForgotPasswordAsync(ForgotPasswordParam forgotPasswordParam)
        {
            if (forgotPasswordParam == null) { throw new ArgumentNullException("forgotPasswordParam"); }
            if (string.IsNullOrWhiteSpace(forgotPasswordParam.Email)) { throw new ArgumentException("forgotPasswordParam.Email"); }
            if (string.IsNullOrWhiteSpace(forgotPasswordParam.Scope)) { throw new ArgumentException("forgotPasswordParam.Scope"); }
            if (forgotPasswordParam.CultureInfo == null) throw new ArgumentException("forgotPasswordParam.CultureInfo");

            try
            {
                var param = new SendResetPasswordInstructionsParam
                {
                    Email = forgotPasswordParam.Email,
                    Scope = forgotPasswordParam.Scope
                };
                await CustomerRepository.SendResetPasswordInstructionsAsync(param).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // To avoid divulging information, the default implementation of ForgotPasswordAsync always succeed;
                // It ignore all errors and return a success
            }

            return GetForgotPasswordViewModelAsync(new GetForgotPasswordViewModelParam
            {
                Status = MyAccountStatus.Success,
                CultureInfo = forgotPasswordParam.CultureInfo,
                EmailSentTo = forgotPasswordParam.Email,
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
        /// <param name="resetPasswordParam">Service call params <see cref="ResetPasswordParam"/></param>
        /// <returns>
        /// The updated Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<ResetPasswordViewModel> ResetPasswordAsync(ResetPasswordParam resetPasswordParam)
        {
            if (resetPasswordParam == null) { throw new ArgumentNullException("resetPasswordParam"); }
            if (string.IsNullOrWhiteSpace(resetPasswordParam.Ticket)) { throw new ArgumentException("resetPasswordParam.Ticket"); }
            if (resetPasswordParam.CultureInfo == null) { throw new ArgumentException("resetPasswordParam.CultureInfo"); }
            if (string.IsNullOrWhiteSpace(resetPasswordParam.Scope)) { throw new ArgumentException("resetPasswordParam.Scope"); }
            if (string.IsNullOrWhiteSpace(resetPasswordParam.NewPassword)) { throw new ArgumentException("resetPasswordParam.NewPassword"); }
            if (string.IsNullOrWhiteSpace(resetPasswordParam.PasswordAnswer) && MembershipProvider.RequiresQuestionAndAnswer)
            {
                throw new ArgumentException("resetPasswordParam.PasswordAnswer");
            }

            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new GetMyAccountUrlParam { CultureInfo = resetPasswordParam.CultureInfo });

            var customer = await CustomerRepository.GetCustomerByTicketAsync(resetPasswordParam.Ticket).ConfigureAwait(false);

            await CustomerRepository.ResetPasswordAsync(
                customer.Username,
                resetPasswordParam.NewPassword,
                resetPasswordParam.PasswordAnswer).ConfigureAwait(false);

            return GetResetPasswordViewModel(new GetResetPasswordViewModelParam
            {
                ReturnUrl = resetPasswordParam.ReturnUrl,
                Status = MyAccountStatus.Success,
                Customer = customer,
                CultureInfo = resetPasswordParam.CultureInfo,
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
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo"); }

            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new GetMyAccountUrlParam { CultureInfo = param.CultureInfo });

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
        /// <param name="changePasswordParam">Service call params <see cref="ChangePasswordParam"/></param>
        /// <returns>
        ///  The updated Customer and a status representing a possible cause of errors
        /// </returns>
        public virtual async Task<ChangePasswordViewModel> ChangePasswordAsync(ChangePasswordParam changePasswordParam)
        {
            if (changePasswordParam == null) { throw new ArgumentNullException("changePasswordParam"); }
            if (changePasswordParam.CultureInfo == null) { throw new ArgumentException("changePasswordParam.CultureInfo"); }
            if (changePasswordParam.CustomerId == Guid.Empty) { throw new ArgumentException("changePasswordParam.CustomerId"); }
            if (string.IsNullOrWhiteSpace(changePasswordParam.Scope)) { throw new ArgumentException("changePasswordParam.Scope"); }
            if (string.IsNullOrWhiteSpace(changePasswordParam.NewPassword)) { throw new ArgumentException("changePasswordParam.NewPassword"); }
            if (string.IsNullOrWhiteSpace(changePasswordParam.OldPassword)) { throw new ArgumentException("changePasswordParam.OldPassword"); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = changePasswordParam.CustomerId,
                CultureInfo = changePasswordParam.CultureInfo,
                Scope = changePasswordParam.Scope
            }).ConfigureAwait(false);

            if (customer == null)
            {
                return GetChangePasswordViewModel(new GetChangePasswordViewModelParam
                {
                    Status = MyAccountStatus.Failed,
                    ReturnUrl = changePasswordParam.ReturnUrl,
                    CultureInfo = changePasswordParam.CultureInfo
                });
            }

            await CustomerRepository.ChangePasswordAsync(
                customer.Username,
                changePasswordParam.OldPassword,
                changePasswordParam.NewPassword
            );

            return GetChangePasswordViewModel(new GetChangePasswordViewModelParam
            {
                Status = MyAccountStatus.Success,
                Customer = customer,
                ReturnUrl = changePasswordParam.ReturnUrl,
                CultureInfo = changePasswordParam.CultureInfo
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
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("changePasswordParam.Scope"); }

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
        /// Get the view Model to display a Change Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetChangePasswordViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Change Password Form
        /// </returns>
        protected virtual ChangePasswordViewModel GetChangePasswordViewModel(GetChangePasswordViewModelParam param)
        {
            var viewModel = param.Customer != null ? ViewModelMapper.MapTo<ChangePasswordViewModel>(param.Customer, param.CultureInfo)
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
        private Regex CreatePasswordRegexPattern()
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
            if (string.IsNullOrEmpty(getCustomerByTicketParam.Ticket))
            {
                return null;
            }

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
