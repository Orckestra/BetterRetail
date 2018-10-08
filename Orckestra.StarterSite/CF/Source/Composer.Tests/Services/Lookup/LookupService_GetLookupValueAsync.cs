using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Tests.Services.Lookup
{
    // ReSharper disable once InconsistentNaming
    public class LookupService_GetLookupValueAsync : BaseTest
    {
        private ILookupService _lookupService;
        [SetUp]
        public void Setup()
        {
            var repository = new Mock<ILookupRepository>();
            repository.Setup(m => m.GetLookupAsync("Size")).ReturnsAsync(CreateLooup());
            Container.GetMock<ILookupRepositoryFactory>()
                    .Setup(m => m.CreateLookupRepository(It.IsAny<LookupType>()))
                    .Returns(repository.Object);
            _lookupService = Container.CreateInstance<LookupService>();
        }
        [Test]
        public async Task WHEN_Lookup_Value_Exists_SHOULD_Return_Value()
        {
            var param = new GetLookupDisplayNameParam
            {
                LookupType = LookupType.Customer,
                LookupName = "Size",
                Value = "xlarge",
                CultureInfo = new CultureInfo("en-CA")
            };
            var lookup = await _lookupService.GetLookupDisplayNameAsync(param);

            lookup.Should().Be("X-Large");
        }

        [Test]
        public async Task WHEN_Lookup_Value_Does_Not_Exist_SHOULD_Return_Empty_String()
        {
            var param = new GetLookupDisplayNameParam
            {
                LookupType = LookupType.Customer,
                LookupName = "Size",
                Value = "potato",
                CultureInfo = new CultureInfo("en-CA")
            };
            var lookup = await _lookupService.GetLookupDisplayNameAsync(param);

            lookup.Should().Be(string.Empty);
        }

        [Test]
        public async Task WHEN_Lookup_Has_Multiple_Values_SHOULD_Return_Comma_Space_Delimited()
        {
            var param = new GetLookupDisplayNameParam
            {
                LookupType = LookupType.Customer,
                LookupName = "Size",
                Value = "small|medium",
                CultureInfo = new CultureInfo("fr-CA")
            };

            var lookup =
                await
                    _lookupService.GetLookupDisplayNameAsync(param);

            lookup.Should().Be("Petit, Moyen");
        }

        [Test]
        public void WHEN_LookupName_Does_Not_Exist_SHOULD_Throw_InvalidOperationException()
        {
            var repository = Container.GetMock<ILookupRepository>();
            repository.Setup(m => m.GetLookupAsync(It.IsAny<string>())).ReturnsAsync(null);
            Container.GetMock<ILookupRepositoryFactory>()
                .Setup(m => m.CreateLookupRepository(It.IsAny<LookupType>()))
                .Returns(repository.Object);


            var param = new GetLookupDisplayNameParam
            {
                LookupType = LookupType.Customer,
                LookupName = "Size",
                Value = "small|medium",
                CultureInfo = new CultureInfo("fr-CA")
            };

            _lookupService.Awaiting(async ls => await ls.GetLookupDisplayNameAsync(param)).ShouldThrow<InvalidOperationException>();

        }

        private Overture.ServiceModel.Metadata.Lookup CreateLooup()
        {
            return new Overture.ServiceModel.Metadata.Lookup
            {
                LookupName = "Size",
                Values = new List<LookupValue>
                {
                    new LookupValue
                    {
                        Value = "xlarge",
                        DisplayName = new LocalizedString {{"en-CA", "X-Large"}, {"fr-CA", "T-Grand"}}
                    },
                    new LookupValue
                    {
                        Value = "small",
                        DisplayName = new LocalizedString{{"en-CA", "Small"}, {"fr-CA", "Petit"}}
                    },
                    new LookupValue
                    {
                        Value = "medium",
                        DisplayName = new LocalizedString{{"en-CA", "Medium"}, {"fr-CA", "Moyen"}}
                    }
                }
            };
        }
    }
}
