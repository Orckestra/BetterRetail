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
    [ImmutableTypeId("588779be-1861-47ba-9632-7998c6741242")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Recipe Meal Type")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    public interface IRecipeMealType : IData, IProcessControlled, ILocalizedControlled
    {
        [ImmutableFieldId("b460b8f5-e161-4e7f-a70b-f8d658febdd9")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("2de01eba-05bd-4771-a54d-8cada90d45ae")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }
    
        [ImmutableFieldId("30d27036-c02d-4d5d-96a4-5a3cf4195474")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(10)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        [TreeOrdering(1)]
        int Order { get; set; }
    }
}