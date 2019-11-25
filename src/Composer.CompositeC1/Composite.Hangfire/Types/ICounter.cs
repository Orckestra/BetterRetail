using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace Hangfire.CompositeC1.Types
{
    [AutoUpdateble]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("b3503da3-006d-4965-82be-1553b31e2ed5")]
    [Title("Counter")]
    [KeyPropertyName("Id")]
    public interface ICounter : IExpirable
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("a8f49d83-95a7-4bb1-8a25-149f9ad410e3")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 100)]
        [ImmutableFieldId("6378638c-f345-466c-9899-fb85ba088550")]
        string Key { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [ImmutableFieldId("523cc108-de2f-4f4a-a207-7e0ad63695bf")]
        int Value { get; set; }
    }
}
