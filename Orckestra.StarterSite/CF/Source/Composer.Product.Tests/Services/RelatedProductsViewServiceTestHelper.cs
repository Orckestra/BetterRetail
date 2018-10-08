using System.Collections.Generic;
using Orckestra.Composer.Product.Services;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Tests.Services
{
    public class RelatedProductsViewServiceTestHelper
    {
        public static Overture.ServiceModel.Products.Product Product = new Overture.ServiceModel.Products.Product
        {
            Relationships = new List<Relationship>
                {
                    new Relationship
                    {
                        EntityId = "111",
                        MerchandiseType = MerchandiseType.CrossSell,
                        RelationshipType = RelationshipType.Product,
                        SequenceNumber = 3
                    },
                    new Relationship
                    {
                        EntityId = "222",
                        VariantProductId = "333",
                        MerchandiseType = MerchandiseType.ReplacementPart,
                        RelationshipType = RelationshipType.Product,
                        SequenceNumber = 1
                    },
                    new Relationship
                    {
                        EntityId = "444",
                        VariantProductId = "555",
                        RelationshipType = RelationshipType.Variant,
                        MerchandiseType = MerchandiseType.CrossSell,
                        SequenceNumber = 1
                    },
                    new Relationship
                    {
                        EntityId = "666",
                        RelationshipType = RelationshipType.Category,
                        SequenceNumber = 5
                    },
                    new Relationship
                    {
                        EntityId = "777",
                        RelationshipType = RelationshipType.Product,
                        MerchandiseType = MerchandiseType.CrossSell,
                        SequenceNumber  = 2
                    },
                    new Relationship
                    {
                        EntityId = "888",
                        RelationshipType = RelationshipType.Product,
                        MerchandiseType = MerchandiseType.CrossSell,
                        SequenceNumber  = 4
                    },
                    new Relationship
                    {
                        EntityId = "999",
                        RelationshipType = RelationshipType.Product,
                        MerchandiseType = MerchandiseType.CrossSell,
                        SequenceNumber  = 5
                    },
                    new Relationship
                    {
                        EntityId = "10",
                        RelationshipType = RelationshipType.Product,
                        MerchandiseType = MerchandiseType.CrossSell,
                        SequenceNumber  = 6
                    }

                },
                DisplayName = new LocalizedString(new Dictionary<string, string> {{"en-CA", "Test"}}),
                Id = "1",
                Variants = new List<Variant>()
                {
                    new Variant
                    {
                        Id = "1"
                    },
                    new Variant
                    {
                        Id = "2"
                    }
                }
        };

    }
}
