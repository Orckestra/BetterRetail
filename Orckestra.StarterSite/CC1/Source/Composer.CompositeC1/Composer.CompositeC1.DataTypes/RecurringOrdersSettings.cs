using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("991d5a0b-053e-4d43-91a1-5a0b4d96a85d")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Recurring Orders Settings")]
    [LabelPropertyName("Enabled")]
    public interface RecurringOrdersSettings : IPageMetaData, ILocalizedControlled
    {

        [ImmutableFieldId("211704b4-1ff5-40e4-a332-c1875b358eb4")]
        [FormRenderingProfile(Label = "Recurring Schedule Page")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(1)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes = true)]
        Nullable<Guid> RecurringSchedulePageId { get; set; }

        [ImmutableFieldId("222704b4-1ff5-40e4-a332-c1875b358eb4")]
        [FormRenderingProfile(Label = "Recurring Schedule Details Page")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(1)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes = true)]
        Nullable<Guid> RecurringScheduleDetailsPageId { get; set; }

        [ImmutableFieldId("223704b4-1ff5-40e4-a332-c1875b358eb4")]
        [FormRenderingProfile(Label = "Recurring Cart Details Page")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(1)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes = true)]
        Nullable<Guid> RecurringCartDetailsPageId { get; set; }

        [ImmutableFieldId("224704b0-1ff5-40e4-a332-c1875b358eb3")]
        [FormRenderingProfile(Label = "Recurring Cart Details Page")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(1)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes = true)]
        Nullable<Guid> RecurringCartsPageId { get; set; }

        [ImmutableFieldId("a74604fd-6c8a-4994-90dd-0f8c433980e4")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
    "nstant.Boolean\"><f:param name=\"Constant\" value=\"False\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(0)]
        [DefaultFieldBoolValue(false)]
        bool Enabled { get; set; }
    }
}
