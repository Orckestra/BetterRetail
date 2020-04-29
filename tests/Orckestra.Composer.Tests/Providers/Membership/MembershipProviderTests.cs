using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Net;
using System.Reflection;
using System.Web.Security;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Membership;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Customers.Membership;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Customers.Membership;
using ServiceStack;
using ServiceStack.Validation;

namespace Orckestra.Composer.Tests.Providers.Membership
{
    [TestFixture]
    public class MembershipProviderTests
    {
        private Mock<IOvertureClient> _mockClient;
        private Mock<IScopeProvider> _mockScopeProvider;
        private OvertureMembershipProvider _sut;
        private MembershipConfiguration _membershipConfig;
        private ComposerHostMoq _composerHostMoq;
        private AutofacDependencyResolverMoq _autofacDependencyResolverMoq;

        [SetUp]
        public void Setup()
        {
            _mockClient = new Mock<IOvertureClient>(MockBehavior.Strict);
            _mockScopeProvider = new Mock<IScopeProvider>();
            _mockScopeProvider.SetupGet(sp => sp.DefaultScope).Returns(FakeData.ScopeId);
            _composerHostMoq = new ComposerHostMoq();
            _composerHostMoq.AutoMock.Provide(_mockClient.Object);
            _autofacDependencyResolverMoq = new AutofacDependencyResolverMoq();
            _autofacDependencyResolverMoq.AutoMock.Provide(_mockScopeProvider.Object);

            _sut = new OvertureMembershipProvider();

            _membershipConfig = new MembershipConfiguration()
            {
                EnablePasswordReset = true,
                EnablePasswordRetrieval = false,
                MaxInvalidPasswordAttempts = 0,
                MinRequiredNonAlphanumericCharacters = 1,
                MinRequiredPasswordLength = 6,
                PasswordAttemptWindow = 0,
                PasswordStrategy = MembershipPasswordStrategy.Hashed,
                PasswordStrengthRegularExpression = "",
                RequiresQuestionAndAnswer = false,
                RequiresUniqueEmail = true
            };

            _mockClient.Setup(x => x.Send(It.IsAny<GetMembershipConfigurationRequest>()))
                       .Returns(_membershipConfig);

            var config = new NameValueCollection { { "scope", FakeData.ScopeId } };
            _sut.Initialize("overture", config);

            InjectProvider(System.Web.Security.Membership.Providers, _sut);

        }

        [TearDown]
        public void TearDown()
        {
            _composerHostMoq.Dispose();
            _autofacDependencyResolverMoq.Dispose();
        }

        public void LoginUser_should_returns_true_with_valid_password()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<LoginRequest>()))
                       .Returns(new LoginResponse { Success = true });

            bool result = _sut.ValidateUser(FakeData.ValidFullUsername, FakeData.ValidPassword);

            _mockClient.Verify(
                x => x.Send(It.Is<ValidateUserRequest>(req => req.UserName == FakeData.ValidUsername && req.Password == FakeData.ValidPassword)));

            result.Should().BeTrue();
        }

        public void LoginUser_should_returns_false_with_invalid_password()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<LoginRequest>()))
                       .Returns(new LoginResponse { Success = false });

            bool result = _sut.ValidateUser(FakeData.InvalidFullUsername, FakeData.InvalidPassword);

            _mockClient.Verify(
                x => x.Send(It.Is<ValidateUserRequest>(req => req.UserName == FakeData.InvalidUsername && req.Password == FakeData.InvalidPassword)));

            result.Should().BeFalse();
        }

        [Test]
        public void LoginUser_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<LoginRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.ValidateUser(FakeData.ValidFullUsername, FakeData.ValidPassword))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void GetUser_should_returns_customer_when_exists()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(new Customer { Username = FakeData.ValidUsername, Email = FakeData.ValidEmail });

            var user = _sut.GetUser(FakeData.ValidUsername, false);

            _mockClient.Verify(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()));

            user.Should().NotBeNull();
            user.UserName.Should().Be(FakeData.ValidUsername);
            user.Email.Should().Be(FakeData.ValidEmail);
        }

        [Test]
        public void GetUser_should_returns_null_when_user_not_exist()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns((Customer)null);

            var user = _sut.GetUser(@"overture\dummyUser", userIsOnline: false);

            _mockClient.Verify(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()));

            user.Should().BeNull();
        }

        [Test]
        [Ignore("Currently last activity date seems not updated in Overture")]
        public void GetUser_should_update_lastactivity_when_is_online()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                        .Returns((Customer)null);
        
            var lastActivity = DateTime.Now;
            var user = _sut.GetUser(@"overture\user", userIsOnline: true);
        
            user.Should().NotBeNull();
            user.LastActivityDate.Should().BeAfter(lastActivity.AddMilliseconds(200));
        }

        [Test]
        public void GetUser_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.GetUser(FakeData.ValidUsername, false))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void GetUser_userKey_returns_user()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerRequest>()))
                       .Returns(new Customer { Username = FakeData.ValidUsername, Email = FakeData.ValidEmail });

            var providerUserKey = Guid.Parse("D17DDE10-BC8E-E2F2-736A-253BBBAAA4C5") as object;

            var user = _sut.GetUser(providerUserKey, userIsOnline: false);

            _mockClient.Verify(x => x.Send(It.IsAny<GetCustomerRequest>()));


            user.Should().NotBeNull();
            user.UserName.Should().Be(FakeData.ValidUsername);
            user.Email.Should().Be(FakeData.ValidEmail);
        }

        [Test]
        public void GetUser_userKey_returns_null_when_user_not_exists()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerRequest>()))
                       .Returns((Customer)null);

            var providerUserKey = Guid.Parse("148E3304-EA90-43A1-B2E6-CE589775F1C2") as object;

            var user = _sut.GetUser(providerUserKey, userIsOnline: false);

            _mockClient.Verify(x => x.Send(It.IsAny<GetCustomerRequest>()));

            user.Should().BeNull();
        }

        [Test]
        public void GetUser_userKey_should_throw_when_not_valid_guid()
        {
            _sut.Invoking(x => x.GetUser((object)null, userIsOnline: false)).ShouldThrow<ArgumentNullException>();
            _sut.Invoking(x => x.GetUser((object)"userKey", userIsOnline: false)).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void GetUser_userKey_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerRequest>()))
                .Throws(new WebServiceException("User not found.")
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                });

            var providerUserKey = Guid.Parse("148E3304-EA90-43A1-B2E6-CE589775F1C2") as object;

            _sut.Invoking(x => x.GetUser(providerUserKey, false))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        [Ignore("Found the test with this attribute, filling in required reason.")]
        //todo: fix this one
        public void CreateUser_returns_null_and_status_when_duplicate_name()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<CreateCustomerMembershipRequest>()))
                       .Throws(new ValidationError("UserNameAlreadyUsed", "Username already exists."));

            var user = _sut.CreateUser(@"duplicateUsername", FakeData.ValidPassword, FakeData.ValidEmail, null, null, false, null, out MembershipCreateStatus status);

            status.Should().Be(MembershipCreateStatus.DuplicateUserName);
            user.Should().BeNull();
        }

        [Test]
        [Ignore("No duplicate email status in overture")]
        public void CreateUser_returns_null_and_status_when_duplicate_email()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<CreateCustomerMembershipRequest>()))
                        .Throws(new ValidationError("UserNameAlreadyUsed", "Username already exists."));

            var user = _sut.CreateUser(FakeData.ValidUsername, FakeData.ValidPassword, "duplicateEmail", null, null, false, null, out MembershipCreateStatus status);

            Assert.AreEqual(MembershipCreateStatus.DuplicateEmail, status);
            Assert.IsNull(user);
        }

        [Test]
        [Ignore("Found the test with this attribute, filling in required reason.")]
        //todo: fix this one
        public void CreateUser_returns_null_and_status_when_password_not_met_condition()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<CreateCustomerMembershipRequest>()))
                       .Throws(new ValidationError("InvalidOperationException", "The customer membership creation failed: InvalidPassword."));

            var user = _sut.CreateUser(FakeData.ValidUsername, FakeData.ValidPassword, FakeData.ValidEmail, null, null, false, null, out MembershipCreateStatus status);

            status.Should().Be(MembershipCreateStatus.InvalidPassword);
            user.Should().BeNull();
        }

        [Test]
        [Ignore("Found the test with this attribute, filling in required reason.")]
        //todo: fix this one
        public void CreateUser_returns_user_and_success_status()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                AccountStatus = AccountStatus.Active,
                LastLoginDate = DateTime.Now,
                LastActivityDate = DateTime.Now
            };

            _mockClient.Setup(x => x.Send(It.IsAny<CreateCustomerMembershipRequest>()))
                       .Returns(customer);

            var user = _sut.CreateUser(FakeData.ValidUsername, FakeData.ValidPassword, FakeData.ValidEmail, null, null, false, null, out MembershipCreateStatus status);

            status.Should().Be(MembershipCreateStatus.Success);
            user.Should().NotBeNull();
        }

        [Test]
        [Ignore("Found the test with this attribute, filling in required reason.")]
        //todo: fix this one
        public void CreateUser_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<CreateCustomerMembershipRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.CreateUser(FakeData.ValidUsername, FakeData.ValidPassword, FakeData.ValidEmail, null, null, false, null, out MembershipCreateStatus status))
                .ShouldThrow<MembershipCreateUserException>();
        }

        [Test]
        public void UpdateUser_changes_values_and_persist()
        {
            const string newEmail = "newEmail@test.com";

            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                AccountStatus = AccountStatus.Active,
                LastLoginDate = DateTime.Now,
                LastActivityDate = DateTime.Now
            };

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            var updatedCustomer = new Customer()
            {
                Email = newEmail,
                AccountStatus = AccountStatus.Active,
                LastLoginDate = DateTime.Now,
                LastActivityDate = DateTime.Now
            };

            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Returns(updatedCustomer);

            var membershipUser = _sut.GetUser(FakeData.ValidUsername, false);

            _sut.UpdateUser(membershipUser);

            membershipUser.Should().NotBeNull();
            membershipUser.Email.Should().Be(newEmail);
            membershipUser.LastActivityDate.Should().Be(updatedCustomer.LastActivityDate);
            membershipUser.LastLoginDate.Should().Be(updatedCustomer.LastLoginDate);
        }

        [Test]
        public void UpdateUser_should_rethrow_overture_exception()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                AccountStatus = AccountStatus.Active,
                LastLoginDate = DateTime.Now,
                LastActivityDate = DateTime.Now
            };

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Throws<WebServiceException>();

            var membershipUser = _sut.GetUser(FakeData.ValidUsername, false);

            _sut.Invoking(x => x.UpdateUser(membershipUser))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void UnlockUser_should_reactivate_user()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                AccountStatus = AccountStatus.Inactive
            };

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            customer.AccountStatus = AccountStatus.Active;
            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Returns(customer);

            var result = _sut.UnlockUser("user");

            result.Should().Be(true);
        }

        [Test]
        public void UnlockUser_should_rethrow_overture_exception()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                AccountStatus = AccountStatus.Inactive
            };

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.UnlockUser(FakeData.ValidUsername))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void DeleteUser_deletes_should_deactivate_user()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                AccountStatus = AccountStatus.Active
            };

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            customer.AccountStatus = AccountStatus.Inactive;
            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Returns(customer);

            var result = _sut.DeleteUser("user", true);

            result.Should().Be(true);
        }

        [Test]
        public void DeleteUser_should_rethrow_overture_exception()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                AccountStatus = AccountStatus.Active
            };

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.DeleteUser(FakeData.ValidUsername, true))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void GetAllUsers_should_return_users()
        {
            var queryResult = new CustomerQueryResult()
            {
                Results = new List<Customer> { 
                    new Customer() { Username = "user1", Email = "user1@test.com" },
                    new Customer() { Username = "user2", Email = "user2@test.com" },
                    new Customer() { Username = "user3", Email = "user3@test.com" }
                },

                TotalCount = 3
            };

            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Returns(queryResult);

            var users = _sut.GetAllUsers(0, 10, out int totalRecord);

            var isEquivalent = GetRequestValidator(null, 0, 10);
            _mockClient.Verify(x => x.Send(It.Is<FindCustomersRequest>(req => isEquivalent(req))));

            totalRecord.Should().Be(3);
            users.Should().HaveCount(3);
            users.Should().NotContainNulls();
        }

        [Test]
        public void GetAllUsers_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.GetAllUsers(0, 10, out int totalRecords))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void GetNumberOfUsersOnline_should_return_online_count()
        {
            var queryResult = new CustomerQueryResult()
            {
                Results = new List<Customer> { 
                    new Customer() { Username = "user1", Email = "user1@test.com" },
                    new Customer() { Username = "user2", Email = "user2@test.com" },
                    new Customer() { Username = "user3", Email = "user3@test.com" }
                },

                TotalCount = 3
            };

            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Returns(queryResult);

            var userOnline = _sut.GetNumberOfUsersOnline();

            var isEquivalent = GetRequestValidator(null, 0, null);
            _mockClient.Verify(x => x.Send(It.Is<FindCustomersRequest>(req => isEquivalent(req))));

            userOnline.Should().Be(3);
        }

        [Test]
        public void GetNumberOfUsersOnline_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.GetNumberOfUsersOnline())
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void ResetPassword_should_works()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ResetPasswordRequest>()))
                       .Returns(new ResetPasswordResponse() { Success = true });

            var newPassword = _sut.ResetPassword(FakeData.ValidUsername, "GoodPasswordQuestionAnswer");

            newPassword.Should().NotBeNull();
        }

        [Test]
        public void ResetPassword_throws_not_supported_when_enablePasswordReset_is_false()
        {
            _membershipConfig.EnablePasswordReset = false;
            _mockClient.Setup(x => x.Send(It.IsAny<GetMembershipConfigurationRequest>()))
                       .Returns(_membershipConfig);

            _sut.Invoking(x => x.ResetPassword(FakeData.ValidUsername, "answer"))
                .ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void ResetPassword_throws_when_user_does_not_exist()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ResetPasswordRequest>()))
                          .Throws(new ValidationError("CustomerMembershipDoesNotExist", "This customer membership does not exist in the system."));

            _sut.Invoking(x => x.ResetPassword(FakeData.InvalidUsername, "answer"))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Test]
        public void ResetPassword_throws_when_answer_is_wrong()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ResetPasswordRequest>()))
                       .Throws(new ValidationError("InvalidAnswer", "Bad answer."));

            _sut.Invoking(x => x.ResetPassword(FakeData.ValidUsername, "invalidAnswer"))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Test]
        public void ResetPassword_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ResetPasswordRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.ResetPassword(FakeData.ValidUsername, "answer"))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Test]
        public void GetPassword_throws_NotSupportedException()
        {
            _sut.Invoking(x => x.GetPassword(FakeData.ValidUsername, "answer"))
                .ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void GetUserNameByEmail_should_returns_username()
        {
            var queryResult = new CustomerQueryResult()
            {
                Results = new List<Customer> { new Customer() { Username = FakeData.ValidUsername, Email = FakeData.ValidEmail } },
                TotalCount = 1
            };

            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Returns(queryResult);

            var username = _sut.GetUserNameByEmail(FakeData.ValidEmail);

            var isEquivalent = GetRequestValidator(FakeData.ValidEmail, 0, null);
            _mockClient.Verify(x => x.Send(It.Is<FindCustomersRequest>(req => isEquivalent(req))));

            username.Should().NotBeNull();
            username.Should().Be(FakeData.ValidUsername);
        }

        [Test]
        public void GetUserNameByEmail_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.GetUserNameByEmail(FakeData.ValidEmail))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void ChangePassword_returns_false_when_user_does_not_exist()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ChangePasswordRequest>()))
                       .Throws(new ValidationError("InvalidPassword", "The specified Password is invalid."));

            var changed = _sut.ChangePassword(FakeData.InvalidUsername, FakeData.ValidPassword, "newPassword");

            changed.Should().BeFalse();
        }

        [Test]
        public void ChangePassword_returns_false_when_old_pass_is_wrong()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ChangePasswordRequest>()))
                       .Throws(new ValidationError("PasswordDoesNotMatch", "The specified password does not match the password stored in the system."));

            var changed = _sut.ChangePassword(FakeData.ValidPassword, FakeData.InvalidPassword, "newPassword");

            changed.Should().BeFalse();
        }

        [Test]
        public void ChangePassword_returns_true_when_username_and_old_pass_are_correct()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ChangePasswordRequest>()))
                       .Returns(new ChangePasswordResponse() { Success = true });

            var changed = _sut.ChangePassword(FakeData.ValidPassword, FakeData.ValidPassword, "newPassword");

            changed.Should().BeTrue();
        }

        [Test]
        public void ChangePassword_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Throws<WebServiceException>();

            _sut.Invoking(x => x.GetUserNameByEmail(FakeData.ValidEmail))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void ChangePasswordQuestionAndAnswer_returns_false_when_user_does_not_exist()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ValidateUserRequest>()))
                       .Returns(new ValidateUserResponse() { Success = true });

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns((Customer)null);

            var changed = _sut.ChangePasswordQuestionAndAnswer(FakeData.InvalidUsername, FakeData.ValidPassword, "newPasswordQuestion", "NotSupported");

            changed.Should().BeFalse();
        }

        [Test]
        public void ChangePasswordQuestionAndAnswer_returns_false_when__pass_is_wrong()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<ValidateUserRequest>()))
                       .Returns(new ValidateUserResponse() { Success = false });

            var changed = _sut.ChangePasswordQuestionAndAnswer(FakeData.ValidUsername, FakeData.InvalidPassword, "newPasswordQuestion", "NotSupported");

            changed.Should().BeFalse();
        }

        [Test]
        public void ChangePasswordQuestionAndAnswer_returns_true_when_success()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                PasswordQuestion = "oldPasswordQuestion"
            };

            _mockClient.Setup(x => x.Send(It.IsAny<ValidateUserRequest>()))
                       .Returns(new ValidateUserResponse() { Success = true });

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            var updatedCustomer = new Customer()
            {
                PasswordQuestion = "newPasswordQuestion",
                AccountStatus = AccountStatus.Active,
                LastLoginDate = DateTime.Now,
                LastActivityDate = DateTime.Now
            };
            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Returns(updatedCustomer);

            var changed = _sut.ChangePasswordQuestionAndAnswer(FakeData.ValidUsername, FakeData.ValidPassword, "newPasswordQuestion", "NotSupported");

            changed.Should().BeTrue();
        }

        [Test]
        public void ChangePasswordQuestionAnswer_should_rethrow_overture_exception()
        {
            var customer = new Customer()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Username = FakeData.ValidUsername,
                Email = FakeData.ValidEmail,
                PasswordQuestion = "oldPasswordQuestion"
            };

            _mockClient.Setup(x => x.Send(It.IsAny<ValidateUserRequest>()))
                       .Returns(new ValidateUserResponse() { Success = true });

            _mockClient.Setup(x => x.Send(It.IsAny<GetCustomerByUsernameRequest>()))
                       .Returns(customer);

            _mockClient.Setup(x => x.Send(It.IsAny<UpdateCustomerRequest>()))
                       .Throws<ProviderException>();

            _sut.Invoking(x => x.ChangePasswordQuestionAndAnswer(FakeData.ValidUsername, FakeData.ValidPassword, "newPasswordQuestion", "NotSupported"))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void FindUsersByName_should_returns_users()
        {
            var queryResult = new CustomerQueryResult()
            {
                Results = new List<Customer> { new Customer() { Username = FakeData.ValidUsername, Email = FakeData.ValidEmail } },
                TotalCount = 1
            };

            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Returns(queryResult);

            var users = _sut.FindUsersByName(FakeData.ValidUsername, 0, 10, out int totalRecord);

            var isEquivalent = GetRequestValidator(FakeData.ValidUsername, 0, 10);
            _mockClient.Verify(x => x.Send(It.Is<FindCustomersRequest>(req => isEquivalent(req))));

            totalRecord.Should().Be(1);
            users.Should().HaveCount(1);
            users["user"].Should().NotBeNull();
            users["user"].As<MembershipUser>().UserName.Should().Be(FakeData.ValidUsername);
            users["user"].As<MembershipUser>().Email.Should().Be(FakeData.ValidEmail);
        }

        [Test]
        public void FindUsersByName_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Throws<ProviderException>();

            _sut.Invoking(x => x.FindUsersByName(FakeData.ValidUsername, 0, 10, out int totalRecord))
                .ShouldThrow<ProviderException>();
        }

        [Test]
        public void FindUsersByEmail_should_returns_matching_users()
        {
            var queryResult = new CustomerQueryResult()
            {
                Results = new List<Customer> { new Customer() { Username = FakeData.ValidUsername, Email = FakeData.ValidEmail } },
                TotalCount = 1
            };

            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Returns(queryResult);

            var users = _sut.FindUsersByEmail(FakeData.ValidEmail, 0, 10, out int totalRecord);

            var isEquivalent = GetRequestValidator(FakeData.ValidEmail, 0, 10);
            _mockClient.Verify(x => x.Send(It.Is<FindCustomersRequest>(req => isEquivalent(req))));

            totalRecord.Should().Be(1);
            users.Should().HaveCount(1);
            users["user"].Should().NotBeNull();
            users["user"].As<MembershipUser>().UserName.Should().Be(FakeData.ValidUsername);
            users["user"].As<MembershipUser>().Email.Should().Be(FakeData.ValidEmail);
        }

        [Test]
        public void FindUsersByEmail_should_rethrow_overture_exception()
        {
            _mockClient.Setup(x => x.Send(It.IsAny<FindCustomersRequest>()))
                       .Throws<ProviderException>();

            _sut.Invoking(x => x.FindUsersByEmail(FakeData.ValidEmail, 0, 10, out int totalRecord))
                .ShouldThrow<ProviderException>();
        }

        private Func<FindCustomersRequest, bool> GetRequestValidator(string searchTerm, int pageIndex, int? pageSize)
        {
            return x => x.SearchTerms == searchTerm &&
                        x.FilteringScopes == FakeData.ScopeId &&
                        x.ScopeId == FakeData.ScopeId &&
                        x.Query.IncludeTotalCount == true &&
                        x.Query.StartingIndex == pageIndex &&
                        x.Query.MaximumItems == pageSize;
        }

        private static void InjectProvider(IEnumerable collection, ProviderBase provider)
        {
            typeof(ProviderCollection).GetField("_ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
                                       .SetValue(collection, false);

            var hash = (Hashtable)typeof(ProviderCollection).GetField("_Hashtable", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(collection);

            hash[provider.Name] = provider;
        }

        private static class FakeData
        {
            public const string ScopeId = "Canada";

            public const string ValidFullUsername = @"overture\user";
            public const string ValidUsername = "user";
            public const string ValidEmail = "user@test.com";
            public const string ValidPassword = "validPassword";

            public const string InvalidFullUsername = @"overture\invalidUser";
            public const string InvalidUsername = "invalidUser";
            public const string InvalidEmail = "invalidUser@test.com";
            public const string InvalidPassword = "invalidPassword";
        }
    }
}