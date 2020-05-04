using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Security;
using Autofac.Integration.Mvc;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Customers.Membership;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Customers.Membership;
using ServiceStack;
using ServiceStack.Validation;

namespace Orckestra.Composer.Providers.Membership
{
    public class OvertureMembershipProvider : MembershipProvider
    {
        private IOvertureClient _client;
        private MembershipConfiguration _configuration;
        private Regex _matchDomainUserRegex;

        public override bool EnablePasswordRetrieval
        {
            get { return _configuration.EnablePasswordRetrieval; }
        }

        public override bool EnablePasswordReset
        {
            get { return _configuration.EnablePasswordReset; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _configuration.RequiresQuestionAndAnswer; }
        }

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _configuration.MaxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _configuration.PasswordAttemptWindow; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return _configuration.RequiresUniqueEmail; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                if (!Enum.TryParse(_configuration.PasswordStrategy.ToString(), out MembershipPasswordFormat passwordFormat))
                {
                    throw new InvalidOperationException(
                        "Unable to parse MembershipPasswordFormat from Overture's MembershipPasswordStrategy.");
                }

                return passwordFormat;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _configuration.MinRequiredPasswordLength; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _configuration.MinRequiredNonAlphanumericCharacters; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _configuration.PasswordStrengthRegularExpression; }
        }

        public virtual string GetCurrentScope()
        {
            var scopeProvider = (IScopeProvider)AutofacDependencyResolver.Current.GetService(typeof(IScopeProvider));
            return scopeProvider?.DefaultScope;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            InitOvertureSettings(ref name, config);

            // Initialize the abstract base class.
            base.Initialize(name, config);
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name could be user's email depending on configuration.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">The new user is always approved no matter the value of this parameter.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user. Generated if null.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer,
                                                  bool isApproved, object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            if (TryGetDomainUser(username, out string domainUsername))
            {
                // Username shouldn't contain domain
                status = MembershipCreateStatus.UserRejected;
                return null;
            }

            var args = new ValidatePasswordEventArgs(domainUsername, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            Guid userId = Guid.Empty;
            if (providerUserKey != null)
            {
                if (!(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }

                userId = (Guid)providerUserKey;
            }

            var request = new CreateCustomerMembershipRequest
            {
                Id = userId,
                Username = username,
                Email = email,
                FirstName = OvertureMembershipConfiguration.DefaultFirstName, // Required in Overture
                LastName = OvertureMembershipConfiguration.DefaultLastName,   // Required in Overture
                Password = password,
                PasswordQuestion = passwordQuestion,
                PasswordAnswer = passwordAnswer,
                ScopeId = GetCurrentScope(),
            };

            try
            {
                var createdCustomer = _client.Send(request);

                status = MembershipCreateStatus.Success;

                return ConvertToMembershipUser(createdCustomer);
            }
            catch (ValidationError ex)
            {
                switch (ex.ErrorCode)
                {
                    case "InvalidPassword":
                    case "InvalidOperationException":
                        status = MembershipCreateStatus.InvalidPassword;
                        return null;
                    case "PasswordQuestionNoSet":
                        status = MembershipCreateStatus.InvalidQuestion;
                        return null;
                    case "UserNameAlreadyUsed":
                        status = MembershipCreateStatus.DuplicateUserName;
                        return null;
                    default:
                        status = MembershipCreateStatus.UserRejected;
                        return null;
                }
            }
            catch (WebException ex)
            {
                throw new MembershipCreateUserException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new MembershipCreateUserException(ex.ErrorMessage, ex);
            }
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">NotSupported:The new password answer for the specified user.</param>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">username</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Configuration.Provider.ProviderException"></exception>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }

            try
            {
                if (!InternalValidateUser(username, password)) { return false; }

                var customer = GetCustomerByUsername(username);

                if (customer == null) { return false; }

                var updateRequest = new UpdateCustomerRequest(customer)
                {
                    PasswordQuestion = newPasswordQuestion,
                    ScopeId = GetCurrentScope()
                };

                var updatedCustomer = _client.Send(updateRequest);

                return updatedCustomer.PasswordQuestion == newPasswordQuestion;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException(string.Format("Password retrieval isn't supporter by {0}", Name));
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null) { throw args.FailureInformation; }

                throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
            }

            var request = new ChangePasswordRequest
            {
                UserName = username,
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            try
            {
                var response = _client.Send(request);
                return response.Success;
            }
            catch (ValidationError)
            {
                return false;
            }
            catch (WebException ex)
            {
                throw new MembershipPasswordException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new MembershipPasswordException(ex.ErrorMessage, ex);
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset) { throw new NotSupportedException("Password reset is not enabled."); }

            if (answer == null && RequiresQuestionAndAnswer)
            {
                //UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword = System.Web.Security.Membership.GeneratePassword(MinRequiredPasswordLength, MinRequiredNonAlphanumericCharacters);

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null) { throw args.FailureInformation; }
                throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
            }

            var request = new ResetPasswordRequest
            {
                Username = username,
                Password = newPassword,
                PasswordAnswer = answer
            };

            try
            {
                var response = _client.Send(request);

                if (!response.Success)
                {
                    throw new MembershipPasswordException("An error occured during password resetting process.");
                }

                return newPassword;
            }
            catch (ValidationError ex)
            {
                throw new MembershipPasswordException(ex.ErrorMessage, ex);
            }
            catch (WebException ex)
            {
                throw new MembershipPasswordException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new MembershipPasswordException(ex.ErrorMessage, ex);
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }

            var request = new UpdateCustomerRequest(ConvertToCustomer(user)) { ScopeId = GetCurrentScope() };

            try
            {
                var customer = _client.Send(request);

                user.Email = customer.Email;
                user.IsApproved = customer.AccountStatus != AccountStatus.RequiresApproval;
                user.LastActivityDate = customer.LastActivityDate;
                user.LastLoginDate = customer.LastLoginDate;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            return TryGetDomainUser(username, out _) &&
                   InternalLoginUser(username, password);
        }

        public override bool UnlockUser(string username)
        {
            return UpdateUserAccountStatus(username, AccountStatus.Active);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null) { throw new ArgumentNullException(nameof(providerUserKey)); }

            if (!(providerUserKey is Guid)) { throw new ArgumentException("Provider user key must be a valid guid."); }

            var request = new GetCustomerRequest
            {
                CustomerId = (Guid)providerUserKey,
                ScopeId = GetCurrentScope()
            };

            try
            {
                var customer = _client.Send(request);

                return ConvertToMembershipUser(customer);
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                var customer = GetCustomerByUsername(username);

                return ConvertToMembershipUser(customer);
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            var currentScope = GetCurrentScope();
            var request = new FindCustomersRequest
            {
                SearchTerms = email,
                FilteringScopes = currentScope,
                ScopeId = currentScope,
                Query = new Query
                {
                    IncludeTotalCount = true,
                    Sortings = new List<QuerySorting>
                                {
                                    new QuerySorting { PropertyName = "AccountStatus", Direction = SortDirection.Ascending},
                                    new QuerySorting { PropertyName = "LastActivityDate", Direction = SortDirection.Descending}
                                }
                }
            };

            try
            {
                var result = _client.Send(request);

                return result.TotalCount == 0
                    ? null
                    : result.Results.First().Username;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return UpdateUserAccountStatus(username, AccountStatus.Inactive);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var currentScope = GetCurrentScope();
            var request = new FindCustomersRequest
            {
                SearchTerms = null,
                FilteringScopes = currentScope,
                ScopeId = currentScope,
                Query = new Query
                {
                    IncludeTotalCount = true,
                    MaximumItems = pageSize,
                    StartingIndex = pageIndex
                }
            };

            try
            {
                var membershipUsers = new MembershipUserCollection();
                var result = _client.Send(request);

                totalRecords = result.TotalCount;

                result.Results.Select(ConvertToMembershipUser)
                    .ToList().ForEach(membershipUsers.Add);

                return membershipUsers;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = DateTime.Now.Subtract(onlineSpan);

            var currentScope = GetCurrentScope();
            var request = new FindCustomersRequest
            {
                SearchTerms = null,
                FilteringScopes = currentScope,
                ScopeId = currentScope,
                Query = new Query
                {
                    IncludeTotalCount = true,
                    Filter = new FilterGroup
                    {
                        Filters = new List<Filter>
                                        {
                                            new Filter { Operator = Operator.GreaterThan, Member = "LastActivityDate", Value = compareTime }
                                        }
                    }
                }
            };

            try
            {
                var result = _client.Send(request);

                return result.TotalCount;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            var currentScope = GetCurrentScope();
            var request = new FindCustomersRequest
            {
                SearchTerms = usernameToMatch,
                FilteringScopes = currentScope,
                ScopeId = currentScope,
                Query = new Query
                {
                    IncludeTotalCount = true,
                    StartingIndex = pageIndex,
                    MaximumItems = pageSize
                }
            };

            try
            {
                var membershipUsers = new MembershipUserCollection();
                var result = _client.Send(request);

                totalRecords = result.TotalCount;

                result.Results.Select(ConvertToMembershipUser)
                    .ToList().ForEach(membershipUsers.Add);

                return membershipUsers;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            var currentScope = GetCurrentScope();
            var request = new FindCustomersRequest
            {
                SearchTerms = emailToMatch,
                FilteringScopes = currentScope,
                ScopeId = currentScope,
                Query = new Query
                {
                    IncludeTotalCount = true,
                    StartingIndex = pageIndex,
                    MaximumItems = pageSize
                }
            };

            try
            {
                var membershipUsers = new MembershipUserCollection();
                var result = _client.Send(request);

                totalRecords = result.TotalCount;

                result.Results.Select(ConvertToMembershipUser)
                    .ToList().ForEach(membershipUsers.Add);

                return membershipUsers;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        private Customer GetCustomerByUsername(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }

            try
            {
                var getRequest = new GetCustomerByUsernameRequest { Username = username, ScopeId = GetCurrentScope() };

                return _client.Send(getRequest);
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        private MembershipUser ConvertToMembershipUser(Customer customer)
        {
            if (customer == null) { return null; }

            bool isLockedOut = customer.AccountStatus == AccountStatus.Inactive;
            bool isApproved = customer.AccountStatus != AccountStatus.RequiresApproval;

            return new MembershipUser(Name, customer.Username, customer.Id, customer.Email,
                                      customer.PasswordQuestion, null, isApproved, isLockedOut,
                                      customer.Created, customer.LastLoginDate,
                                      customer.LastActivityDate,
                                      customer.LastPasswordChanged, default);
        }

        private static Customer ConvertToCustomer(MembershipUser user)
        {
            if (user == null) { return null; }
            if (user.ProviderUserKey != null && !(user.ProviderUserKey is Guid))
            {
                throw new ArgumentException("Provider user key must be a valid guid.");
            }

            var customerId = user.ProviderUserKey as Guid? ?? Guid.Empty;
            var accountStatus = user.IsLockedOut ? AccountStatus.Inactive : AccountStatus.Active;
            accountStatus = user.IsApproved ? accountStatus : AccountStatus.RequiresApproval;

            return new Customer
            {
                Id = customerId,
                Username = user.UserName,
                Email = user.Email,
                FirstName = OvertureMembershipConfiguration.DefaultFirstName, // Required in Overture
                LastName = OvertureMembershipConfiguration.DefaultLastName,   // Required in Overture
                AccountStatus = accountStatus,
                PasswordQuestion = user.PasswordQuestion,
                Created = user.CreationDate,
                LastLoginDate = user.LastLoginDate,
                LastActivityDate = user.LastActivityDate,
                LastPasswordChanged = user.LastPasswordChangedDate,
                CustomerType = CustomerType.Registered
            };
        }

        private bool UpdateUserAccountStatus(string username, AccountStatus newStatus)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }

            try
            {
                var customer = GetCustomerByUsername(username);

                if (customer == null) { throw new InvalidOperationException(string.Format("This customer with username {0} doesn't exist.", username)); }

                var updateRequest = new UpdateCustomerRequest(customer)
                {
                    AccountStatus = newStatus,
                    ScopeId = GetCurrentScope()
                };

                var updatedCustomer = _client.Send(updateRequest);

                return updatedCustomer.AccountStatus == newStatus;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        private bool InternalValidateUser(string domainUsername, string password)
        {
            var request = new ValidateUserRequest
            {
                UserName = domainUsername,
                Password = password,
                ScopeId = GetCurrentScope()
            };

            try
            {
                var response = _client.Send(request);

                return response.Success;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        private bool InternalLoginUser(string domainUsername, string password)
        {
            var request = new LoginRequest
            {
                UserName = domainUsername,
                Password = password,
                ScopeId = GetCurrentScope()
            };

            try
            {
                var response = _client.Send(request);

                return response.Success;
            }
            catch (WebException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            catch (WebServiceException ex)
            {
                throw new ProviderException(ex.ErrorMessage, ex);
            }
        }

        private bool TryGetDomainUser(string fullyQualifiedUsername, out string username)
        {
            username = null;

            if (fullyQualifiedUsername == null) { return false; }

            if (UsersNameStartsWithDomain(fullyQualifiedUsername))
            {
                var match = _matchDomainUserRegex.Match(fullyQualifiedUsername);

                username = match.Groups["username"]?.Value;
                return match.Success;
            }

            username = fullyQualifiedUsername;
            return true;
        }

        private static bool UsersNameStartsWithDomain(string fullyQualifiedUsername)
        {
            var pattern = string.Concat(OvertureMembershipConfiguration.DefaultMembershipDomain, "\\");
            var startsWithDomain = fullyQualifiedUsername.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase);

            return startsWithDomain;
        }

        private void InitOvertureSettings(ref string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name)) { name = "overture"; }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Overture Membership provider");
            }

            _matchDomainUserRegex = new Regex(string.Format(@"^{0}\\(?<username>.+)$", OvertureMembershipConfiguration.DefaultMembershipDomain), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            _client = ComposerHost.Current.Resolve<IOvertureClient>();

            GetMembershipConfigurationFromOverture();
        }

        private void GetMembershipConfigurationFromOverture()
        {
            if (_configuration != null) { return; }

            try
            {
                _configuration = _client.Send(new GetMembershipConfigurationRequest());
            }
            catch (Exception ex)
            {
                if (ex is WebException || ex is WebServiceException)
                {
                    throw new ConfigurationErrorsException("Unable to retrieve membership configuration.", ex);
                }

                throw;
            }
        }
    }

}