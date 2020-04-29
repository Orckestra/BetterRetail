using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Requests.Customers.Membership;
using static Orckestra.Composer.Utils.ExpressionUtility;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    class CustomerRepository_CreateUserAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOvertureClient>(MockBehavior.Strict));
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var expectedPassword = GetRandom.String(32);
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<CreateCustomerMembershipRequest>(
                    param => param.Email == expectedCustomer.Email &&
                    param.Username == expectedCustomer.Username &&
                    param.FirstName == expectedCustomer.FirstName &&
                    param.LastName == expectedCustomer.LastName &&
                    param.Password == expectedPassword)))
                .ReturnsAsync(expectedCustomer);

            //Act
            var result = await customerRepository.CreateUserAsync(
                new CreateUserParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Username = expectedCustomer.Username,
                    Email = expectedCustomer.Email,
                    FirstName = expectedCustomer.FirstName,
                    LastName = expectedCustomer.LastName,
                    Password = expectedPassword,
                    PasswordAnswer = GetRandom.String(70),
                    PasswordQuestion = expectedCustomer.PasswordQuestion,
                    Scope = GetRandom.String(32)
                }
            );

            //Assert
            result.Id.Should().Be(expectedCustomer.Id);
        }

        [Test]
        public async Task WHEN_creating_with_requiresapproval_SHOULD_succeed()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom(AccountStatus.RequiresApproval);
            var expectedPassword = GetRandom.String(32);
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<CreateCustomerMembershipRequest>(
                    param => param.Email == expectedCustomer.Email &&
                    param.Username == expectedCustomer.Username &&
                    param.FirstName == expectedCustomer.FirstName &&
                    param.LastName == expectedCustomer.LastName &&
                    param.Password == expectedPassword)))
                .ReturnsAsync(expectedCustomer);

            //Act
            var result = await customerRepository.CreateUserAsync(
                new CreateUserParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Username = expectedCustomer.Username,
                    Email = expectedCustomer.Email,
                    FirstName = expectedCustomer.FirstName,
                    LastName = expectedCustomer.LastName,
                    Password = expectedPassword,
                    PasswordAnswer = GetRandom.String(70),
                    PasswordQuestion = GetRandom.String(70),
                    Scope = GetRandom.String(32)
                }
            );

            //Assert
            result.Id.Should().Be(expectedCustomer.Id);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_email_SHOULD_throw_ArgumentException(string email)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.CreateUserAsync(param));

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
            var customerRepository = _container.CreateInstance<CustomerRepository>();
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.CreateUserAsync(param));

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
            var customerRepository = _container.CreateInstance<CustomerRepository>();
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.CreateUserAsync(param));

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
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            var expectedEmail = GetRandom.Email();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<CreateCustomerMembershipRequest>(param => string.IsNullOrWhiteSpace(param.Username))))
                .ReturnsAsync(new Customer
                {
                    Id = GetRandom.Guid(),
                    Email = expectedEmail,
                    Username = expectedEmail,
                    AccountStatus = AccountStatus.Active
                });

            //Act
            var result = await customerRepository.CreateUserAsync(new CreateUserParam
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
            result.Email.Should().BeEquivalentTo(expectedEmail);
            result.Username.Should().BeEquivalentTo(expectedEmail);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_passing_empty_Password_SHOULD_throw_ArgumentException(string password)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.CreateUserAsync(param));

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
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                      .Setup(p => p.SendAsync(It.Is<CreateCustomerMembershipRequest>(param => string.IsNullOrWhiteSpace(param.PasswordQuestion))))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await customerRepository.CreateUserAsync(new CreateUserParam
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
            result.Id.Should().Be(expectedCustomer.Id);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public async Task WHEN_passing_empty_PasswordAnswer_SHOULD_succeed(string passwordAnswer)
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                      .Setup(p => p.SendAsync(It.Is<CreateCustomerMembershipRequest>(param => string.IsNullOrWhiteSpace(param.PasswordAnswer))))
                      .ReturnsAsync(expectedCustomer);

            //Act
            var result = await customerRepository.CreateUserAsync(new CreateUserParam
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
            result.Id.Should().Be(expectedCustomer.Id);
        }

        [Test]
        [TestCase(null)]
        public void WHEN_passing_empty_CultureInfo_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.CreateUserAsync(param));

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
            var customerRepository = _container.CreateInstance<CustomerRepository>();
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.CreateUserAsync(param));

            //Assert
            exception.Message.Should().Contain("Scope");
        }

        [Test]
        public void WHEN_passing_null_param_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            // Act
            Expression<Func<Task<Customer>>> expression = () => customerRepository.CreateUserAsync(null);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }
    }
}
