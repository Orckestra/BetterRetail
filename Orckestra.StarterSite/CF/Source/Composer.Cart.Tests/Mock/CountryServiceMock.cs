using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Country;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class CountryServiceMock
    {
        internal static Mock<ICountryService> Create()
        {
            return Create(GetRandom.String(32));
        }

        internal static Mock<ICountryService> Create(string postalCodeRegex)
        {
            var countryServiceMock = new Mock<ICountryService>();

            countryServiceMock
                .Setup(c => c.RetrieveCountryAsync(It.IsAny<RetrieveCountryParam>()))
                .ReturnsAsync(new Country.CountryViewModel()
                {
                    PostalCodeRegex = postalCodeRegex
                });

            return countryServiceMock;
        }
    }
}
