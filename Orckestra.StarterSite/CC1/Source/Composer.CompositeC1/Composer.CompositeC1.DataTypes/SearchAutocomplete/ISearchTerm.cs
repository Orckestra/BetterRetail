using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes.SearchAutocomplete
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("52033fe8-5963-449c-bf31-bdc449cb0494")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("ISearchTerm")]
    [LabelPropertyName("Value")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [SearchableType]
    public interface ISearchTerm : IData, IProcessControlled, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("a57882f4-3c12-4504-a656-b9985f5b8cb4")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("e0907c1b-51fe-4dd8-9925-7ff26b29c8d6")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Value { get; set; }
        
        [ImmutableFieldId("8794480d-a694-46dd-b78d-c86db6f19ead")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(1)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes=true, NullReferenceValue="{00000000-0000-0000-0000-000000000000}")]
        [GroupByPriority(1)]
        Guid WebsiteId { get; set; }
    }
}