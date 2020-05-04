using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Factory
{
    class DiscountViewModelFactory_CreateViewModel
    {
        private readonly IViewModelMapper _mapper;
        private List<Reward> _rewards;

        public AutoMocker Container { get; set; }

        public DiscountViewModelFactory_CreateViewModel()
        {
            _mapper = ViewModelMapperFactory.CreateFake(typeof(RewardViewModel).Assembly);
        }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
            Container.Use(_mapper);
            Container.Use(LocalizationProviderFactory.Create());

            _rewards = new List<Reward>();
        }

        [Test]
        public void WHEN_cart_has_discounts_SHOULD_map_first_shipment_distinct_non_lineitem_discounts()
        {
            var randomReward = FakeCartFactory.CreateRandomReward(_rewards, RewardLevel.FulfillmentMethod);

            //Arrange
            var rewards = new List<Reward>()
            {
                randomReward,
                FakeCartFactory.CreateRandomReward(_rewards, RewardLevel.FulfillmentMethod),
                FakeCartFactory.CreateRandomReward(null, RewardLevel.LineItem),
                FakeCartFactory.CreateRandomReward(_rewards, RewardLevel.FulfillmentMethod),
                randomReward
            };

            var sut = Container.CreateInstance<RewardViewModelFactory>();

            //Act
            var vm = sut.CreateViewModel(rewards, CultureInfo.InvariantCulture, RewardLevel.FulfillmentMethod, RewardLevel.Shipment);

            //Assert
            vm.Should().NotBeNullOrEmpty();
            vm.Should().HaveSameCount(_rewards);
            var collection = vm.ToList();

            for (int i = 0; i < collection.Count; i++)
            {
                var d = collection[i];

                d.Should().NotBeNull();
                d.Description.Should().Be(_rewards[i].Description);
            }
        }

        [Test]
        public void WHEN_no_discounts_SHOULD_return_empty_discounts()
        {
            var sut = Container.CreateInstance<RewardViewModelFactory>();

            //Act
            var vm = sut.CreateViewModel(new List<Reward>(), CultureInfo.InvariantCulture, RewardLevel.FulfillmentMethod, RewardLevel.Shipment);

            //Assert
            vm.Should().BeEmpty();
        }
    }
}
