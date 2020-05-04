using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Security;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Providers;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel.Customers;
using static Orckestra.Composer.Utils.ExpressionUtility;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class MembershipViewService_Register
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<ICustomerRepository>(MockBehavior.Strict));
            _container.Use(new Mock<ICustomerRepository>(MockBehavior.Strict));
            _container.Use(new Mock<ICartMergeProvider>());
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(CreateAccountViewModel).Assembly));
            _container.Use(MockMembershipProviderFactory.Create());
            _container.Use(MockMembershipFactory.Create(_container.Get<MembershipProvider>()));

            _container.GetMock<MembershipProvider>()
            .SetupGet(m => m.MinRequiredPasswordLength)
            .Returns(GetRandom.Int)
            .Verifiable();

            _container.GetMock<MembershipProvider>()
                .SetupGet(m => m.MinRequiredNonAlphanumericCharacters)
                .Returns(GetRandom.Int)
                .Verifiable("Regex must be based on this value");
        }

        [Test]
        public async Task WHEN_calling_with_valid_parameters_SHOULD_return_the_created_user()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.CreateUserAsync(It.IsNotNull<CreateUserParam>()))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await sut.RegisterAsync(new CreateUserParam
            {
                Email = expectedCustomer.Email,
                FirstName = expectedCustomer.FirstName,
                LastName = expectedCustomer.LastName,
                Username = expectedCustomer.Username,
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = CultureInfo.GetCultureInfo(expectedCustomer.Language),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
            result.FirstName.Should().BeEquivalentTo(expectedCustomer.FirstName);
            result.LastName.Should().BeEquivalentTo(expectedCustomer.LastName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_email_SHOULD_throw_ArgumentException(string email)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new CreateUserParam
            {
                Email = email,
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RegisterAsync(param));

            //Assert
            exception.Message.Should().Contain("Email");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_FirstName_SHOULD_throw_ArgumentException(string firstName)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = firstName,
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RegisterAsync(param));

            //Assert
            exception.Message.Should().Contain("FirstName");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_LastName_SHOULD_throw_ArgumentException(string lastName)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = lastName,
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RegisterAsync(param));

            //Assert
            exception.Message.Should().Contain("LastName");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public async Task WHEN_passing_empty_username_SHOULD_fallback_to_email_as_username(string username)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            var expectedEmail = GetRandom.Email();

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.CreateUserAsync(It.Is<CreateUserParam>(param => string.IsNullOrWhiteSpace(param.Username))))
                      .ReturnsAsync(new Customer
                      {
                          Email = expectedEmail,
                          Username = expectedEmail
                      });

            //Act
            var result = await sut.RegisterAsync(new CreateUserParam
            {
                Email = expectedEmail,
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = username,
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
            result.Username.Should().BeEquivalentTo(expectedEmail);
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
            var param = new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = password,
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RegisterAsync(param));

            //Assert
            exception.Message.Should().Contain("Password");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public async Task WHEN_passing_empty_PasswordQuestion_SHOULD_succeed(string passwordQuestion)
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.CreateUserAsync(It.Is<CreateUserParam>(param => string.IsNullOrWhiteSpace(param.PasswordQuestion))))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await sut.RegisterAsync(new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = passwordQuestion,
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public async Task WHEN_passing_empty_PasswordAnswer_SHOULD_succeed(string passwordAnswer)
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.CreateUserAsync(It.Is<CreateUserParam>(param => string.IsNullOrWhiteSpace(param.PasswordAnswer))))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await sut.RegisterAsync(new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = passwordAnswer,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
        }

        [Test]
        [TestCase(null)]
        public void WHEN_passing_empty_CultureInfo_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();
            var param = new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = cultureInfo,
                Scope = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RegisterAsync(param));

            //Assert
            exception.Message.Should().Contain("CultureInfo");
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
            var param = new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = scope
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RegisterAsync(param));

            //Assert
            exception.Message.Should().Contain("Scope");
        }

        [Test]
        public void WHEN_passing_null_param_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();
            sut.Membership = _container.Get<IMembershipProxy>();


            // Act
            Expression<Func<Task<CreateAccountViewModel>>> expression = () => sut.RegisterAsync(null);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }

        [Test]
        public void WHEN_creating_user_fails_SHOULD_not_attempt_to_merge_cart()
        {
            //Arrange
            var sut = _container.CreateInstance<MembershipViewService>();

            _container.GetMock<ICustomerRepository>()
                      .Setup(p => p.CreateUserAsync(It.IsAny<CreateUserParam>()))
                      .Throws(new ComposerException(GetRandom.String(3)));

            //Act and Assert
            Assert.ThrowsAsync<ComposerException>(() => sut.RegisterAsync(new CreateUserParam
            {
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                Username = GetRandom.Email(),
                Password = GetRandom.String(32),
                PasswordQuestion = GetRandom.String(32),
                PasswordAnswer = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32),
            }));

            _container.GetMock<ICartMergeProvider>().Verify(provider => provider.MergeCartAsync(It.IsAny<CartMergeParam>()), Times.Never);
        }
    }
}
