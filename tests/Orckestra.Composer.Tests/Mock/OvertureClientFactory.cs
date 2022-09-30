using System.Collections.Generic;
using Moq;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests;

namespace Orckestra.Composer.Tests.Mock
{
    internal static class OvertureClientFactory
    {
        internal static Mock<IComposerOvertureClient> Create()
        {
            var dummyCountry = new Overture.ServiceModel.Country();

            var overtureClient = new Mock<IComposerOvertureClient>(MockBehavior.Strict);

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetCountryRequest>()))
                          .ReturnsAsync(dummyCountry)
                          .Verifiable();

            var dummyRegions = new List<Region>();

             overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetRegionsRequest>()))
                          .ReturnsAsync(dummyRegions)
                          .Verifiable();

            return overtureClient;
        }

        internal static Mock<IComposerOvertureClient> CreateWithNullValues()
        {
            var dummyCountry = new Overture.ServiceModel.Country
            {
                IsoCode = null,
                Id = null,
                Name = null,
                PropertyBag = null,
                IsSupported = false,
                PhoneRegex = null,
                PostalCodeRegex = null,
                Regions = null,
                SortOrder = -1
            };

            var overtureClient = new Mock<IComposerOvertureClient>(MockBehavior.Strict);

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetCountryRequest>()))
                          .ReturnsAsync(dummyCountry)
                          .Verifiable();

            return overtureClient;
        }
    }
}