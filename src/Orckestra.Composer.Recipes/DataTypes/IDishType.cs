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
    [ImmutableTypeId("a26a194d-b246-4933-b99a-1262f426078b")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Recipe Dish Type")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    public interface IDishType : IData, ILocalizedControlled, IProcessControlled
    {
        [ImmutableFieldId("f6b4fb6b-9478-439e-baff-45735c96bd85")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("cb3feb51-f8ef-4799-ac70-e733bcaaa588")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }
        
        [ImmutableFieldId("a54868e2-6e13-49f5-ba2d-1217beae01ac")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(1)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        [TreeOrdering(1)]
        int Order { get; set; }
    }
}