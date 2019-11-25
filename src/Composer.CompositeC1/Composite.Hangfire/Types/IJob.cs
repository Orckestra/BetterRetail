using System;

using Composite.Data;

namespace Hangfire.CompositeC1.Types
{
    [ImmutableTypeId("9d87193f-6ae6-4bfb-9aec-fbc0506bd5c1")]
    [Title("Job")]
    [KeyPropertyName("Id")]
    public interface IJob : IExpirable
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("d3d38a38-a369-4b6d-ac71-5868ae76bcee")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [ImmutableFieldId("1bc9b011-5097-43f1-99ac-3cfcc12cd2f6")]
        Guid? StateId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 20, IsNullable = true)]
        [ImmutableFieldId("6ee6ea7c-1f44-4705-a4c7-6b242a070243")]
        string StateName { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("8f6f6813-fe1e-49c5-a6ac-d561eb6a6127")]
        string InvocationData { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("5da2bbc5-4093-47e3-b894-e2231e8b357f")]
        string Arguments { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [ImmutableFieldId("7acc6212-2d37-4041-859b-56121bc246ab")]
        DateTime CreatedAt { get; set; }
    }
}
