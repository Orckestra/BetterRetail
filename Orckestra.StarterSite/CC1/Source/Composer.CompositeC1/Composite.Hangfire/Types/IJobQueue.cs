using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace Hangfire.CompositeC1.Types
{
    [AutoUpdateble]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("f776567f-b23a-416d-80f6-2aa723c328fb")]
    [Title("Job queue")]
    [KeyPropertyName("Id")]
    public interface IJobQueue : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("c1886cec-6dc0-4b48-9bcc-102b408fa1ba")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("eaa59ab3-9fa8-4584-b0e7-11adc0ec789e")]
        Guid JobId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 50)]
        [ImmutableFieldId("63e1d780-3747-4be3-a2ba-1e1c905da76c")]
        string Queue { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [ImmutableFieldId("bce85320-fc2d-449d-9e04-8c8a426a3e1b")]
        DateTime AddedAt { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime, IsNullable = true)]
        [ImmutableFieldId("60bae343-69b6-4538-a0ef-1852de98649e")]
        DateTime? FetchedAt { get; set; }
    }
}
