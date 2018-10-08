using System.Collections.Generic;
using Moq;
using Orckestra.Composer.Country;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Tests.Mock
{
    internal static class CountryRepositoryFactory
    {
         internal static Mock<ICountryRepository> Create()
        {
            var country = new Overture.ServiceModel.Country();

            var countryRepository = new Mock<ICountryRepository>(MockBehavior.Strict);

            countryRepository.Setup(repo => repo.RetrieveCountry(It.IsNotNull<RetrieveCountryParam>()))
                          .ReturnsAsync(country)
                          .Verifiable();


            var regions = new List<Region>
            {
                new Region()
            };
            countryRepository.Setup(repo => repo.RetrieveRegions(It.IsNotNull<RetrieveCountryParam>()))
                          .ReturnsAsync(regions)
                          .Verifiable();


            return countryRepository;
        }

        internal static Mock<ICountryRepository> CreateWithNullValues()
        {
            var country = new Overture.ServiceModel.Country
            {
                IsoCode = null,
                Name = null,
                PropertyBag = null,
                Id = null,
                IsSupported = false,
                PhoneRegex = null,
                PostalCodeRegex = null,
                Regions = null,
                SortOrder = -1
            };

            var countryRepository = new Mock<ICountryRepository>(MockBehavior.Strict);

            countryRepository.Setup(repo => repo.RetrieveCountry(It.IsNotNull<RetrieveCountryParam>()))
                          .ReturnsAsync(country)
                          .Verifiable();

            return countryRepository;
        }
    }
}
