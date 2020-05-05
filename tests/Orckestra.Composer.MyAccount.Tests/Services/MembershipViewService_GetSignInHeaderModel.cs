using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Providers;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Repositories;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel.Customers;
using System.Threading.Tasks;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class MembershipViewService_GetSignInHeaderModel
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(SignInHeaderViewModel).Assembly));
            _container.Use(new Mock<IMembershipProxy>(MockBehavior.Strict));
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var isAuthenticated = GetRandom.Boolean();
            var firstName = GetRandom.String(32);
            var lastName = GetRandom.String(32);
            var cultureInfo = TestingExtensions.GetRandomCulture();

            var composerContext = new Mock<IComposerContext>();
            composerContext
                .SetupGet(c => c.IsAuthenticated)
                .Returns(isAuthenticated);

            _container.Use(composerContext);

            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository
                .Setup(c => c.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(new Customer
                {
                    FirstName = firstName,
                    LastName = lastName
                });

            _container.Use(customerRepository);

            var expectedLoginUrl = _container.Get<IMyAccountUrlProvider>().GetLoginUrl(new BaseUrlParameter
            {
                CultureInfo = cultureInfo
            });

            var expectedMyAccountUrl = _container.Get<IMyAccountUrlProvider>().GetMyAccountUrl(new BaseUrlParameter
            {
                CultureInfo = cultureInfo
            });

            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            membershipViewService.Membership = _container.Get<IMembershipProxy>();

            var getSignInHeaderParam = new GetSignInHeaderParam
            {
                Scope = GetRandom.String(32),
                CustomerId = GetRandom.Guid(),
                CultureInfo = cultureInfo,
                EncryptedCustomerId = GetRandom.String(64),
                IsAuthenticated = isAuthenticated
            };

            //Act
            var viewModel = await membershipViewService.GetSignInHeaderModel(getSignInHeaderParam);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.IsLoggedIn.Should().Be(isAuthenticated);

            if (isAuthenticated)
            {
                viewModel.Url.Should().Be(expectedMyAccountUrl, "Because logged in user are invited to their account");
            }
            else
            {
                viewModel.Url.Should().Be(expectedLoginUrl, "Because logged out user are invited to log in");
            }

            viewModel.FirstName.Should().Be(firstName);
            viewModel.LastName.Should().Be(lastName);
        }

        [Test]
        public async Task WHEN_Customer_is_Null_SHOULD_create_view_model_with_empty_bag()
        {
            //Arrange
            var isAuthenticated = GetRandom.Boolean();
            var cultureInfo = TestingExtensions.GetRandomCulture();

            var expectedLoginUrl = _container.Get<IMyAccountUrlProvider>().GetLoginUrl(new BaseUrlParameter
            {
                CultureInfo = cultureInfo
            });
            
            var expectedMyAccountUrl = _container.Get<IMyAccountUrlProvider>().GetMyAccountUrl(new BaseUrlParameter
            {
                CultureInfo = cultureInfo
            });
            
            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            membershipViewService.Membership = _container.Get<IMembershipProxy>();

            var getSignInHeaderParam = new GetSignInHeaderParam
            {
                Scope = GetRandom.String(32),
                CustomerId = GetRandom.Guid(),
                CultureInfo = cultureInfo,
                EncryptedCustomerId = GetRandom.String(64),
                IsAuthenticated = isAuthenticated
            };

            //Act
            var viewModel = await membershipViewService.GetSignInHeaderModel(getSignInHeaderParam);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.IsLoggedIn.Should().Be(isAuthenticated);
            if (isAuthenticated)
            {
                viewModel.Url.Should().Be(expectedMyAccountUrl, "Because logged in user are invited to their account");
            }
            else
            {
                viewModel.Url.Should().Be(expectedLoginUrl, "Because logged out user are invited to log in");
            }
            viewModel.Bag.Should().BeEmpty("No PropertyBag to load from");
        }
    }
}
