using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Security;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Providers;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Customers;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;

using System.Linq.Expressions;
using Orckestra.Composer.MyAccount.ViewModels;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class MembershipViewService_Login
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<ICustomerRepository>(MockBehavior.Strict));
            _container.Use(new Mock<IMembershipProxy>(MockBehavior.Strict));
        }

        [Test]
        public async Task WHEN_requesting_to_log_in_valid_user_response_SHOULD_be_success()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.LoginUser(It.IsNotNull<string>(), It.IsNotNull<string>()))
                      .Returns(true);

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.GetUser(It.IsNotNull<string>(), It.IsNotNull<bool>()))
                      .Returns(MockMembershipUser(
                          GetRandom.Email(),
                          expectedCustomer.Id,
                          GetRandom.Email(),
                          GetRandom.String(32),
                          GetRandom.String(32),
                          true,
                          false,
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime()
                      ));

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(param => param.CustomerId == expectedCustomer.Id)))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await sut.LoginAsync(new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32),
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
        }

        [Test]
        public async Task WHEN_requesting_to_log_in_valid_user_response_SHOULD_have_correct_user_id()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.LoginUser(It.IsNotNull<string>(), It.IsNotNull<string>()))
                      .Returns(true);

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.GetUser(It.IsNotNull<string>(), It.IsNotNull<bool>()))
                      .Returns(MockMembershipUser(
                          GetRandom.Email(),
                          expectedCustomer.Id,
                          GetRandom.Email(),
                          GetRandom.String(32),
                          GetRandom.String(32),
                          true,
                          false,
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime()
                      ));

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(param => param.CustomerId == expectedCustomer.Id)))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await sut.LoginAsync(new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
        }

        [Test]
        public async Task WHEN_requesting_to_log_in_valid_user_response_but_customer_id_is_null_THEN_response_SHOULD_be_unsuccessfull()
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.LoginUser(It.IsNotNull<string>(), It.IsNotNull<string>()))
                      .Returns(true);

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.GetUser(It.IsNotNull<string>(), It.IsNotNull<bool>()))
                      .Returns(MockMembershipUser(
                          GetRandom.Email(),
                          Guid.Empty,
                          GetRandom.Email(),
                          GetRandom.String(32),
                          GetRandom.String(32),
                          GetRandom.Boolean(),
                          GetRandom.Boolean(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime()
                      ));

            //Act
            var result = await sut.LoginAsync(new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Failed.ToString());
        }

        [Test]
        public async Task WHEN_requesting_to_log_in_valid_user_response_but_user_is_null_THEN_response_SHOULD_be_unsuccessfull()
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.LoginUser(It.IsNotNull<string>(), It.IsNotNull<string>()))
                      .Returns(true);

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.GetUser(It.IsNotNull<string>(), It.IsNotNull<bool>()))
                      .Returns<MembershipUser>(null);

            //Act
            var result = await sut.LoginAsync(new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Failed.ToString());
        }

        [Test]
        public async Task WHEN_requesting_to_log_in_valid_user_response_but_status_is_Inactive_THEN_response_SHOULD_be_unsuccessfull()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.LoginUser(It.IsNotNull<string>(), It.IsNotNull<string>()))
                      .Returns(true);

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.GetUser(It.IsNotNull<string>(), It.IsNotNull<bool>()))
                      .Returns(MockMembershipUser(
                          GetRandom.Email(),
                          expectedCustomer.Id,
                          GetRandom.Email(),
                          GetRandom.String(32),
                          GetRandom.String(32),
                          GetRandom.Boolean(),
                          true,
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime()
                      ));

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(param => param.CustomerId == expectedCustomer.Id)))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await sut.LoginAsync(new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Failed.ToString());
        }

        [Test]
        public async Task WHEN_requesting_to_log_in_valid_user_response_but_status_is_RequiresApproval_THEN_response_SHOULD_be_unsuccessfull()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom(AccountStatus.RequiresApproval);
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.LoginUser(It.IsNotNull<string>(), It.IsNotNull<string>()))
                      .Returns(true);

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.GetUser(It.IsNotNull<string>(), It.IsNotNull<bool>()))
                      .Returns(MockMembershipUser(
                          GetRandom.Email(),
                          expectedCustomer.Id,
                          GetRandom.Email(),
                          GetRandom.String(32),
                          GetRandom.String(32),
                          false,
                          false,
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime(),
                          GetRandom.DateTime()
                      ));

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(param => param.CustomerId == expectedCustomer.Id)))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await sut.LoginAsync(new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.RequiresApproval.ToString());
        }

        [Test]
        public async Task WHEN_requesting_to_log_in_invalid_user_response_SHOULD_be_false()
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<IMembershipProxy>()
                      .Setup(p => p.LoginUser(It.IsNotNull<string>(), It.IsNotNull<string>()))
                      .Returns(false);

            //Act
            var result = await sut.LoginAsync(new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Failed.ToString());
        }

        [Test]
        public void WHEN_passing_null_request_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            // Act
            Expression<Func<Task<LoginViewModel>>> expression = () => sut.LoginAsync(null);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }

        [Test]
        [TestCase(null)]
        public void WHEN_passing_empty_CultureInfo_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new LoginParam()
            {
                CultureInfo = cultureInfo,
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.LoginAsync(param));

            //Assert
            exception.Message.Should().Contain("CultureInfo");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_Password_SHOULD_throw_ArgumentException(string password)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = password,
                Username = GetRandom.Email(),
                Scope = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<LoginViewModel>>> expression = () => sut.LoginAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Password)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_Username_SHOULD_throw_ArgumentException(string username)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = username,
                Scope = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<LoginViewModel>>> expression = () => sut.LoginAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Username)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_Scope_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new LoginParam()
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Password = GetRandom.String(15),
                Username = GetRandom.Email(),
                Scope = scope
            };

            // Act
            Expression<Func<Task<LoginViewModel>>> expression = () => sut.LoginAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        private static MembershipUser MockMembershipUser(string name, Guid providerUserKey, string email, string passwordQuestion,
            string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate,
            DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
        {
            var membershipUser = new Mock<MembershipUser>(MockBehavior.Strict);

            membershipUser.SetupGet(m => m.UserName).Returns(name);
            membershipUser.SetupGet(m => m.ProviderUserKey).Returns(providerUserKey);
            membershipUser.SetupGet(m => m.Email).Returns(email);
            membershipUser.SetupGet(m => m.PasswordQuestion).Returns(passwordQuestion);
            membershipUser.SetupGet(m => m.Comment).Returns(comment);
            membershipUser.SetupGet(m => m.IsApproved).Returns(isApproved);
            membershipUser.SetupGet(m => m.IsLockedOut).Returns(isLockedOut);
            membershipUser.SetupGet(m => m.CreationDate).Returns(creationDate);
            membershipUser.SetupGet(m => m.LastLoginDate).Returns(lastLoginDate);
            membershipUser.SetupGet(m => m.LastActivityDate).Returns(lastActivityDate);
            membershipUser.SetupGet(m => m.LastPasswordChangedDate).Returns(lastPasswordChangedDate);
            membershipUser.SetupGet(m => m.LastLockoutDate).Returns(lastLockoutDate);

            return membershipUser.Object;
        }
    }
}
