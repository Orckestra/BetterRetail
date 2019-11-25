using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace Hangfire.CompositeC1.Types
{
    [AutoUpdateble]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("dc94194d-80ce-4a24-aa7f-79e05fbe232f")]
    [Title("State")]
    [KeyPropertyName("Id")]
    public interface IState : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("747d5859-afc8-404e-962e-cea4d8e3973f")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("736950f2-1331-4a0a-8dc7-7c2a827e0595")]
        [ForeignKey(typeof(IJob), "Id", AllowCascadeDeletes = true)]
        Guid JobId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 20)]
        [ImmutableFieldId("3746075b-e55b-4ba7-92cf-15fa0ed27556")]
        string Name { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 100, IsNullable = true)]
        [ImmutableFieldId("227d918c-61da-4512-bc64-b9a5d0cdd313")]
        string Reason { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [ImmutableFieldId("c6a08055-0f5a-4c30-af15-2a7826c30e63")]
        DateTime CreatedAt { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("a13daa29-b59f-445a-9a5a-1197f70658f6")]
        string Data { get; set; }
    }
}
