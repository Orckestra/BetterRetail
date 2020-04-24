using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Orckestra.Composer.Cart.Tests.Factory
{
    [TestFixture]
    [TestOf(nameof(TaxViewModelFactory.CreateTaxViewModels))]
    public class TaxViewModelFactory_CreateViewModel
    {
        private readonly IViewModelMapper _mapper;
        private readonly CultureInfo _invariantCulture = CultureInfo.InvariantCulture;

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
        }

        [Test, Description("In case to pass null to list of taxes, method should not throw")]
        public void WHEN_null_taxes_SHOULD_not_throw()
        {
            var instance = Container.CreateInstance<TaxViewModelFactory>();
            Action act = () => instance.CreateTaxViewModels(null, _invariantCulture).ToList();
            act.ShouldNotThrow();
        }

        [Test, Description("In case to pass null to culture info, method should throw NullReferenceException")]
        public void WHEN_null_culture_info_SHOULD_throw_null_reference_exception()
        {
            var taxes = new List<Tax>()
            {
                FakeCartFactory.CreateTax(_invariantCulture, 1, "CODE01", "DISP01")
            };
            var instance = Container.CreateInstance<TaxViewModelFactory>();
            Action act = () => instance.CreateTaxViewModels(taxes, null).ToList();
            act.ShouldThrow<NullReferenceException>();
        }

        [Test, Description("In case to pass a tax with positive total, a model should be created for it")]
        public void WHEN_positive_tax_total_SHOULD_return_model()
        {
            var taxes = new List<Tax>()
            {
                FakeCartFactory.CreateTax(_invariantCulture, 1, "CODE01", "DISP01")
            };
            var instance = Container.CreateInstance<TaxViewModelFactory>();
            var models = instance.CreateTaxViewModels(taxes, _invariantCulture).ToList();
            models.Should().NotBeNull();
            models.FirstOrDefault().Should().NotBeNull();
        }


        [Test, Description("Total tax amount and percentage should not be modified in returned model")]
        [TestCase(1, 10)]
        [TestCase(0.01, 1)]
        [TestCase(10, 0.01)]
        [TestCase(int.MaxValue, 100)]
        public void WHEN_positive_tax_total_SHOULD_not_modify_total_amount_and_percentage(double? amount, double percentage)
        {
            var baseTax = FakeCartFactory.CreateTax(_invariantCulture, amount, "CODE01", "DISP01");
            baseTax.Percentage = percentage;
            List<Tax> taxes = new List<Tax>()
            {
                baseTax
            };

            var instance = Container.CreateInstance<TaxViewModelFactory>();
            var models = instance.CreateTaxViewModels(taxes, _invariantCulture).ToList();
            models.Count().Should().Be(1);

            var modelTax = models.First();
            modelTax.Percentage.Should().Be(percentage.ToString());
            modelTax.TaxTotal.Should().Be((decimal)amount);
        }


        [Test, Description("In case to pass a tax with zero, minus or null total value, do not create models for them")]
        [TestCase(0, -1, -0.1, ExpectedResult = 0)]
        [TestCase(null, 1, -1, ExpectedResult = 1)]
        [TestCase(1, 0, 0.1, ExpectedResult = 2)]
        [TestCase(1, 1, 1, ExpectedResult = 3)]
        [TestCase(0, 0, 0, ExpectedResult = 0)]
        [TestCase(0, 0, int.MinValue, ExpectedResult = 0)]
        public int WHEN_null_zero_negative_tax_total_SHOULD_not_return_models_for_them(double? amount1, double? amount2, double? amount3)
        {
            List<Tax> taxes = new List<Tax>()
            {
                FakeCartFactory.CreateTax(_invariantCulture, amount1, "CODE01", "DISP01"),
                FakeCartFactory.CreateTax(_invariantCulture, amount2, "CODE02", "DISP02"),
                FakeCartFactory.CreateTax(_invariantCulture, amount3, "CODE03", "DISP03")
            };
            var instance = Container.CreateInstance<TaxViewModelFactory>();
            var models = instance.CreateTaxViewModels(taxes, _invariantCulture).ToList();
            return models.Count();
        }

        [Test, Description("Tax display name should not be changed in models with invariant culture")]
        [TestCase("Value added tax", "VALUE ADDED TAX", "value added tax")]
        [TestCase("Taxe sur la valeur ajoutée", "TAXE SUR LA VALEUR AJOUTÉE", "taxe sur la valeur ajoutée")]
        public void WHEN_invariant_culture_SHOULD_not_modify_display_name(string locValue1, string locValue2, string locValue3)
        {
            List<Tax> taxes = new List<Tax>()
            {
                FakeCartFactory.CreateTax(_invariantCulture, 1, "CODE01", locValue1),
                FakeCartFactory.CreateTax(_invariantCulture, 1, "CODE02", locValue2),
                FakeCartFactory.CreateTax(_invariantCulture, 1, "CODE03", locValue3)
            };
            var instance = Container.CreateInstance<TaxViewModelFactory>();
            var models = instance.CreateTaxViewModels(taxes, _invariantCulture).ToList();

            for (int i = 0; i < models.Count; i++)
            {
                var currentTax = models[i];
                var taxReference = taxes[i];
                currentTax.DisplayName.Should().Be(taxReference.DisplayName.GetLocalizedValue(_invariantCulture.Name));
            }
        }
    }
}