using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Factory
{
    public class TaxViewModelFactory_CreateViewModel
    {
        private readonly IViewModelMapper _mapper;
        private List<Tax> _taxes;

        public AutoMocker Container { get; set; }

        public TaxViewModelFactory_CreateViewModel()
        {
            _mapper = ViewModelMapperFactory.CreateFake(typeof(CartViewModelFactory).Assembly);
        }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
            Container.Use(_mapper);
            Container.Use(LocalizationProviderFactory.Create());

            _taxes = new List<Tax>();
        }

        [Test]
        public void WHEN_cart_has_taxes_SHOULD_map_first_shipment_taxes()
        {
            //Arrange
            var culture = CultureInfo.InvariantCulture;

            var taxes = new List<Tax>()
            {
                FakeCartFactory.CreateTax(_taxes, culture),
                FakeCartFactory.CreateTax(_taxes, culture),
                FakeCartFactory.CreateTax(_taxes, culture)
            };

            var sut = Container.CreateInstance<TaxViewModelFactory>();

            //Act
            var vm = sut.CreateTaxViewModels(taxes, culture);

            //Assert
            vm.Should().NotBeNull();
            vm.Should().HaveSameCount(_taxes, "only taxes from first shipment are considered");

            for (int i = 0; i < vm.ToList().Count; i++)
            {
                var tax = vm.ToList()[i];
                var taxReference = _taxes[i];

                tax.Should().NotBeNull();
                tax.DisplayName.Should().Be(taxReference.DisplayName.GetLocalizedValue(culture.Name));
                tax.TaxTotal.Should().HaveValue("TaxTotal was defined in tax");
            }
        }
    }
}
