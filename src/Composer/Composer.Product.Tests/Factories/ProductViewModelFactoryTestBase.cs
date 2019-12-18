using System.Collections.Generic;
using Moq;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Product.Tests.Factories
{
    public abstract class ProductViewModelFactoryTestBase
    {
        protected static Mock<ILookupService> CreateLookupService()
        {
            
            var lookupService = new Mock<ILookupService>();
            var lookups = BuildLookups();

            lookupService.Setup(
                repo => repo.GetLookupsAsync(It.IsAny<LookupType>()))
                .ReturnsAsync(lookups)
                .Verifiable();
            return lookupService;
        }

        private static List<Lookup> BuildLookups()
        {
            var lookups = new List<Lookup>();

            lookups.Add(new Lookup
            {
                LookupName = "Lookup1Lookup",
                IsActive = true,
                Description = "Some description",
                Values = {
                     new LookupValue {
                        Value = "LookupValue1",
                        SortOrder = 380,
                        DisplayName = new LocalizedString(new Dictionary<string,string> {
                            {"en-US", "Look Up 1 EN"},
                            {"en-CA", "Look Up 1 EN"},
                            {"fr-CA", "Look Up 1 FR"}
                        })
                     },
                     new LookupValue {
                        Value = "LookupValue2",
                        SortOrder = 130,
                        DisplayName = new LocalizedString(new Dictionary<string,string> {
                            {"en-US", "Look Up 2 EN"},
                            {"en-CA", "Look Up 2 EN"},
                            {"fr-CA", "Look Up 2 FR"}
                        })
                     },
                     new LookupValue {
                        Value = "LookupValue3",
                        SortOrder = 100,
                        DisplayName = new LocalizedString(new Dictionary<string,string> {
                            {"en-US", "Look Up 3 EN"},
                            {"en-CA", "Look Up 3 EN"},
                            {"fr-CA", "Look Up 3 FR"}
                        })
                     }
                }
            });


            lookups.Add(new Lookup
            {
                LookupName = "SizeLookup",
                IsActive = true,
                Description = "Some description",
                Values = {
                    new LookupValue {
                        Value = "Small",
                        LookupId = "Small",
                        SortOrder = 1,
                        DisplayName = new LocalizedString(new Dictionary<string,string> {
                            {"en-US", "Small"},
                            {"en-CA", "Small"},
                            {"fr-CA", "Petit"}
                        })
                    },
                    new LookupValue {
                        Value = "Medium",
                        LookupId = "Medium",
                        SortOrder = 2,
                        DisplayName = new LocalizedString(new Dictionary<string,string> {
                            {"en-US", "Medium"},
                            {"en-CA", "Medium"},
                            {"fr-CA", "Medium"}
                        })
                    },
                }
            });

            return lookups;
        }
    }
}