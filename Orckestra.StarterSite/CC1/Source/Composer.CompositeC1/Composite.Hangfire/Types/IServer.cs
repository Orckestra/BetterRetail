using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace Hangfire.CompositeC1.Types
{
    [AutoUpdateble]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("4fe31504-dd78-4c60-bbf0-a68e08aa0cde")]
    [Title("Server")]
    [KeyPropertyName("Id")]
    public interface IServer : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 100)]
        [ImmutableFieldId("4bad37ac-7d41-46a3-9fc3-2ae51d64b3c0")]
        string Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("0ae23520-2cdf-4f68-8cc6-5f9d3f227833")]
        string Data { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [ImmutableFieldId("1f4b2606-d160-48de-9155-4c36e29a23e6")]
        DateTime LastHeartbeat { get; set; }
    }
}
