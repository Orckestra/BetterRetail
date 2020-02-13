using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Providers.Checkout;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CheckoutNavigationViewServiceGetCheckoutNavigationViewModel
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CheckoutNavigationViewService>();

            var viewModel = service.GetCheckoutNavigationViewModel(new GetCheckoutNavigationParam
            {
                CurrentStep = 2,
                StepUrls = new Dictionary<int, CheckoutStepPageInfo>
                {
                   {1, new CheckoutStepPageInfo{ Url = GetRandom.String(32), IsDisplayedInHeader = true, Title = GetRandom.String(32)}},
                   {2, new CheckoutStepPageInfo{ Url = GetRandom.String(32), IsDisplayedInHeader = true, Title = GetRandom.String(32)}},
                   {3, new CheckoutStepPageInfo{ Url = GetRandom.String(32), IsDisplayedInHeader = true, Title = GetRandom.String(32)}}
                }
            });

            viewModel.Steps.Should().HaveCount(3);
            var step1 = viewModel.Steps.FirstOrDefault(x => x.StepNumber == 1);
            step1.IsActive.ShouldBeEquivalentTo(false);
            step1.IsEnable.ShouldBeEquivalentTo(true);

            var step2 = viewModel.Steps.FirstOrDefault(x => x.StepNumber == 2);
            step2.IsActive.ShouldBeEquivalentTo(true);
            step2.IsEnable.ShouldBeEquivalentTo(false);

            var step3 = viewModel.Steps.FirstOrDefault(x => x.StepNumber == 3);
            step3.IsActive.ShouldBeEquivalentTo(false);
            step3.IsEnable.ShouldBeEquivalentTo(false);
        }
    }
}
