using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace Hangfire.CompositeC1.Types
{
    [AutoUpdateble]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("62f270b3-f9c3-435d-a5a5-4bb7a4a20a45")]
    [Title("Job parameter")]
    [KeyPropertyName("Id")]
    public interface IJobParameter : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("db169caa-8e71-4839-9f3b-eb2177483375")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("e5ea950d-d660-44be-8d2c-20c665da4861")]
        [ForeignKey(typeof(IJob), "Id", AllowCascadeDeletes = true)]
        Guid JobId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 40)]
        [ImmutableFieldId("b6701502-d295-4e51-8ea5-599dceb18daf")]
        string Name { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("4fb390af-8486-485a-a85e-48038edbee8d")]
        string Value { get; set; }
    }
}
