using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.ContentSearch.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("5f19a6dd-0f6f-4e6e-a83d-c5a0cef16a90")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Content Search Sort Option")]
    [LabelPropertyName("Title")]
    public interface ISortOption : IData, IProcessControlled, ILocalizedControlled
    {
        [ImmutableFieldId("2f0d8050-3c80-4629-9b1b-fd0afce7aec1")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("48842093-b386-483b-a177-4bdc2629144e")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }
        
        [ImmutableFieldId("751798d9-9211-461f-95e5-57ac03aa5b5c")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(1)]
        [DefaultFieldBoolValue(false)]
        bool IsReverted { get; set; }
        
        [ImmutableFieldId("32c06895-fd69-474b-bbf7-7a53aebf1ba7")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(2)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string FieldName { get; set; }
        
        [ImmutableFieldId("2a8544ce-2fcb-4d65-94a5-1d15792c1839")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(3)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        [TreeOrdering(1)]
        int Order { get; set; }
    }
}