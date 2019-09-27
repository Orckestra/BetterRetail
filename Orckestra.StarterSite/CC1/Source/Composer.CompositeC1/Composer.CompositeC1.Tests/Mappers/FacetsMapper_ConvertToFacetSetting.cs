using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using Orckestra.Composer.CompositeC1.Mappers;
using Orckestra.Composer.CompositeC1.Tests.Services.FacetConfigurationContext;
using Orckestra.Composer.Search.Facets;

// ReSharper disable InconsistentNaming

namespace Orckestra.Composer.CompositeC1.Tests.Mappers
{
    [TestFixture]
    public class FacetsMapper_ConvertToFacetSetting : FacetTestsBase
    {
        [Test]
        public void WHEN_facet_is_null_SHOULD_throw_error()
        {
            // arrange
            IFacet facet = null;

            // act & assert
            Assert.Throws<ArgumentNullException>(() => FacetsMapper.ConvertToFacetSetting(facet));
        }

        [Test]
        public void SHOULD_convert_field()
        {
            // arrange
            var facet = new FacetImpl
            {
                Id = GetRandom.Guid(),
                FieldName = GetRandom.String(10),
                FacetType = "SingleSelect",
                IsDisplayed = true,
                StartValue = GetRandom.NumericString(2),
                EndValue = GetRandom.NumericString(2),
                GapSize = GetRandom.NumericString(2),
                MaxCollapsedValueCount = GetRandom.Int(),
                MaxExpendedValueCount = GetRandom.Int(),
                SortWeight = GetRandom.Decimal(),
            };

            // act
            var result = FacetsMapper.ConvertToFacetSetting(facet);

            // assert
            result.Should().NotBeNull();
            result.FieldName.Should().Be(facet.FieldName);
            result.FacetType.Should().Be(FacetType.SingleSelect);
            result.FacetValueType.Should().Be(typeof(string));
            result.IsDisplayed.Should().Be(facet.IsDisplayed);
            result.StartValue.Should().Be(facet.StartValue);
            result.EndValue.Should().Be(facet.EndValue);
            result.GapSize.Should().Be(facet.GapSize);
            result.MaxCollapsedValueCount.Should().Be(facet.MaxCollapsedValueCount);
            result.MaxExpendedValueCount.Should().Be(facet.MaxExpendedValueCount);
            result.SortWeight.Should().Be((double)facet.SortWeight);
        }


        [Test]
        public void WHEN_facet_is_singleSelect_SHOULD_convert()
        {
            // arrange
            var facet = CreateFacet();

            // act
            var result = FacetsMapper.ConvertToFacetSetting(facet);

            // assert
            result.FacetType.Should().Be(FacetType.SingleSelect);
            result.FacetValueType.Should().Be(typeof(string));
        }

        [Test]
        public void WHEN_facet_is_multiSelect_SHOULD_convert()
        {
            // arrange
            var facet = CreateFacet();
            facet.FacetType = "MultiSelect";

            // act
            var result = FacetsMapper.ConvertToFacetSetting(facet);

            // assert
            result.FacetType.Should().Be(FacetType.MultiSelect);
            result.FacetValueType.Should().Be(typeof(string));
        }

        [Test]
        public void WHEN_facet_is_range_SHOULD_convert()
        {
            // arrange
            var facet = CreateFacet();
            facet.FacetType = "Range";

            // act
            var result = FacetsMapper.ConvertToFacetSetting(facet);

            // assert
            result.FacetType.Should().Be(FacetType.Range);
            result.FacetValueType.Should().Be(typeof(decimal));
        }


        [Test]
        public void WHEN_dependsOn_present_SHOULD_convert()
        {
            // arrange
            var dependsOn = CreateFacet();
            var facet = CreateFacet(dependsOn);

            // act
            var result = FacetsMapper.ConvertToFacetSetting(facet, new List<IFacet> { dependsOn });

            // assert
            result.DependsOn.Should().NotBeNull();
            result.DependsOn.Should().HaveCount(1);
            result.DependsOn[0].Should().Be(dependsOn.FieldName);
        }

        [Test]
        public void WHEN_promotedValues_present_SHOULD_convert()
        {
            // arrange
            var promoted = CreatePromotedFacet();
            var facet = CreateFacet();
            facet.PromotedValuesItems.Add(promoted);

            // act
            var result = FacetsMapper.ConvertToFacetSetting(facet, null, new List<IPromotedFacetValueSetting> { promoted });

            // assert
            result.PromotedValues.Should().NotBeNull();
            result.PromotedValues.Should().HaveCount(1);
            var promotedResult = result.PromotedValues[0];
            promotedResult.Title.Should().Be(promoted.Title);
            promotedResult.SortWeight.Should().Be((double)promoted.SortWeight);
        }

    };
}
