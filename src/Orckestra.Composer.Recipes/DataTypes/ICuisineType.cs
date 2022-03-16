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
    [ImmutableTypeId("cabfe8e4-d9ee-465b-b2ef-e1f58f394a08")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Recipe Cuisine Type")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    public interface ICuisineType : IData, ILocalizedControlled, IProcessControlled
    {
        [ImmutableFieldId("453b1050-2221-4b86-948b-9ab6ce195b0e")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("1725dff2-7024-4687-aeeb-f699904f23eb")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        [TreeOrdering(1)]
        string Title { get; set; }
        
        [ImmutableFieldId("668d06a0-7c43-4647-b71e-a667d5623159")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable=true)]
        [FieldPosition(1)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        Nullable<int> Order { get; set; }
    }
}