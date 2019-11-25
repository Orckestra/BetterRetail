using System;

using Composite.Data;

namespace Hangfire.CompositeC1.Types
{
    [ImmutableTypeId("1e1e1c0b-384d-4599-baff-4414e91106bb")]
    [Title("Hash")]
    [KeyPropertyName("Id")]
    public interface IHash : IExpirable
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("2438d8d6-e51e-4ecc-867e-5031a29ecc0b")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 100)]
        [ImmutableFieldId("30357274-48a2-4572-bcf8-efd6644d87a5")]
        string Key { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 100)]
        [ImmutableFieldId("e1931edf-ecb1-41a3-a605-7e562b67aea2")]
        string Field { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("9f4e9096-5231-4592-9c17-db5f0fbb8434")]
        string Value { get; set; }
    }
}
