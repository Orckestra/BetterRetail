using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class CustomerViewServiceUpdateAccountAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(UpdateAccountViewModel).Assembly));
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>())).ReturnsAsync(customer);
            mockedCustomerRepository.Setup(r => r.UpdateUserAsync(It.IsAny<UpdateUserParam>())).ReturnsAsync(customer);

            _container.Use(mockedCustomerRepository);

            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var viewModel = await customerViewService.UpdateAccountAsync(new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = GetRandom.String(32),
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                PreferredLanguage = GetRandom.String(4),
            });

            //Assert
            viewModel.Should().NotBeNull();
            viewModel.FirstName.Should().Be(customer.FirstName);
        }

        [Test]
        public async Task WHEN_customer_not_exist_SHOULD_return_null()
        {
            //Arrange
            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(null);

            _container.Use(mockedCustomerRepository);

            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var viewModel = await customerViewService.UpdateAccountAsync(new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = GetRandom.String(32),
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                PreferredLanguage = GetRandom.String(4),
            });

            //Assert
            viewModel.Should().BeNull();
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerViewService.UpdateAccountAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new UpdateAccountParam
             {
                 ReturnUrl = GetRandom.String(32),
                 Scope = GetRandom.String(32),
                 CustomerId = Guid.NewGuid(),
                 CultureInfo = cultureInfo,
                 Email = GetRandom.String(32),
                 FirstName = GetRandom.String(32),
                 LastName = GetRandom.String(32),
                 PreferredLanguage = GetRandom.String(4),
             };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.UpdateAccountAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_Scope_is_Empty_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = GetRandom.String(32),
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                PreferredLanguage = GetRandom.String(4),
                Scope = scope
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.UpdateAccountAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
        }


        [Test]
        public void WHEN_Customer_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = GetRandom.String(32),
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                PreferredLanguage = GetRandom.String(4),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.UpdateAccountAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Customer");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_Lastname_is_Empty_SHOULD_throw_ArgumentException(string lastname)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = GetRandom.String(32),
                FirstName = GetRandom.String(32),
                LastName = lastname,
                PreferredLanguage = GetRandom.String(4),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.UpdateAccountAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("LastName");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_Email_is_Empty_SHOULD_throw_ArgumentException(string email)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = email,
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                PreferredLanguage = GetRandom.String(4),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.UpdateAccountAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Email");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_Firstname_is_Empty_SHOULD_throw_ArgumentException(string firstName)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = GetRandom.String(32),
                FirstName = firstName,
                LastName = GetRandom.String(32),
                PreferredLanguage = GetRandom.String(4),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.UpdateAccountAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("FirstName");
        }
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_PreferredLanguage_is_Empty_SHOULD_throw_ArgumentException(string preferredLanguage)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new UpdateAccountParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Email = GetRandom.String(32),
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                PreferredLanguage = preferredLanguage,
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.UpdateAccountAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("PreferredLanguage");
        }
    }
}
