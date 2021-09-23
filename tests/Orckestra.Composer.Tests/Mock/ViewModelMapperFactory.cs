using System.Globalization;
using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Country;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Tests.Mock
{
    internal static class ViewModelMapperFactory
    {
        internal static Mock<IViewModelMapper> Create()
        {
            var dummyCountry = new Composer.Country.CountryViewModel
            {
                IsoCode = GetRandom.String(32),
                CountryName = GetRandom.String(32),
                PostalCodeRegex = GetRandom.String(32),
                IsSupported = GetRandom.Boolean(),
            };

            var viewModelMapper = new Mock<IViewModelMapper>(MockBehavior.Strict);

            viewModelMapper.Setup(
                mapper =>
                    mapper.MapTo<Composer.Country.CountryViewModel>(
                        It.IsNotNull<Overture.ServiceModel.Country>(),
                        It.IsNotNull<CultureInfo>(), "CAD"))
                .Returns(dummyCountry);

            var dummyRegions = new RegionViewModel();
            viewModelMapper.Setup(
              mapper =>
                  mapper.MapTo<RegionViewModel>(
                      It.IsNotNull<Region>(),
                      It.IsNotNull<CultureInfo>(), "CAD"))
              .Returns(dummyRegions);

            return viewModelMapper;
        }

        internal static Mock<IViewModelMapper> CreateViewNullValues()
        {
            var dummyCountry = new Composer.Country.CountryViewModel
            {
                IsoCode = null,
                CountryName = null,
                PostalCodeRegex = null,
                IsSupported = false,
            };

            var viewModelMapper = new Mock<IViewModelMapper>(MockBehavior.Strict);


            viewModelMapper.Setup(
                mapper =>
                    mapper.MapTo<Composer.Country.CountryViewModel>(
                        It.IsNotNull<Overture.ServiceModel.Country>(),
                        It.IsNotNull<CultureInfo>(), "CAD"))
                .Returns(dummyCountry);
          
            return viewModelMapper;
        }
    }
}
