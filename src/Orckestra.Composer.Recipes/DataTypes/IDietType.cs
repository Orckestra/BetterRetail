using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.Recipes.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("1122354c-9b49-408f-9c9d-6ebfc27ed685")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Recipe Diet Type")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    public interface IDietType : IData, ILocalizedControlled, IProcessControlled
    {
        [ImmutableFieldId("67c77a88-d17c-4a8f-b052-0652dc016be6")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("1cf394f0-5957-4cd9-b759-bf925401b9a6")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        [SearchableField(false, true, true)]
        string Title { get; set; }

        [ImmutableFieldId("454868e2-6e14-49f4-ba2d-1217beae01a4")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(1)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        [TreeOrdering(1)]
        int Order { get; set; }
    }
}