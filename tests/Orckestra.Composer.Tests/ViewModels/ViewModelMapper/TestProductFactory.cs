using System;
using System.Collections.Generic;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    public static class TestProductFactory
    {
        public static TestProduct CreateValid()
        {
            return new TestProduct
            {
                Name = new LocalizedString(new Dictionary<string, string> { { "en-CA", "Chair" }, { "fr-CA", "Chaise" } }),
                Brand = "AmericanTire",
                Price = 9.99,
                Height = 120,
                Size = "xlarge",
                DateCreated = new DateTime(2015, 05, 14),
                PropertyBag = new PropertyBag(new Dictionary<string, object>
                {
                    { "Colour", "Brown" }, 
                    { "Description", new LocalizedString(new Dictionary<string, string> { { "en-CA", "An exquisite chair." }}) },
                    { "CustomCategory", new TestCategory { Id = 2, Title = "Custom" }},
                    { "CustomProperty", 5 },
                    {"MappedCustomProperty", "I'm a mapped field!"}
                }),
                Category = new TestCategory
                {
                    Id = 42,
                    Title = "Sports",
                    ParentCategory = new TestCategory
                    {
                        Id = 1,
                        Title = "Fitness"
                    },
                    PropertyBag = new PropertyBag(new Dictionary<string, object>
                    {
                        { "CustomName", "Custom sports" }
                    })
                },
                ChildProducts = new[]
                {
                    new TestProduct
                    {
                        Brand = "FloorMart"
                    },
                    new TestProduct
                    {
                        Brand = "GenericCompany"
                    }
                },
                Tags = new List<string>(new[]
                {
                    "Outdoor", "Indoor"
                })
            };
        }
    }
}
