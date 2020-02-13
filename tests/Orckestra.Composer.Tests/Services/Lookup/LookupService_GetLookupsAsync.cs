using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services.Lookup;

namespace Orckestra.Composer.Tests.Services.Lookup
{
    // ReSharper disable once InconsistentNaming
    public class ProductLookupService_GetLookupsAsync : BaseTest
    {
        [Test]
        public async Task WHEN_GetLookupsAsync_Called_SHOULD_Call_Repository()
        {
            // Arrange
            var repoMock = Container.GetMock<ILookupRepository>();
            var repoFactoryMock = Container.GetMock<ILookupRepositoryFactory>();
            repoFactoryMock.Setup(m => m.CreateLookupRepository(It.IsAny<LookupType>())).Returns(repoMock.Object);
            var serviceMock = Container.CreateInstance<LookupService>();
            
            //serviceMock.CallBase = true;
            // Act
            await serviceMock.GetLookupsAsync(LookupType.Product);

            // Assert
            repoMock.Verify(m => m.GetLookupsAsync());
        }
    }
}
