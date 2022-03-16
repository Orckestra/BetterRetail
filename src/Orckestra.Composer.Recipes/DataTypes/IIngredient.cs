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
    [ImmutableTypeId("b64f2dcc-7e0b-4ea8-97ff-5ddc461029dd")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Ingredient")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    public interface IIngredient : IData, IProcessControlled, ILocalizedControlled
    {
        [ImmutableFieldId("bb89a40c-ce57-4f42-8ba8-0c546c65722b")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("c7176b9d-3b2e-4b40-bfc1-e254e5adbd69")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }
        
        [ImmutableFieldId("914028c5-6e0d-4862-bddd-632b3fdb59c4")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable=true)]
        [FieldPosition(1)]
        [NullStringLengthValidator(0, 64)]
        string Keyword { get; set; }
        
        [ImmutableFieldId("46c34164-80f3-47fe-9f87-dc73527c9069")]
        [FormRenderingProfile(Label = "Product SKU")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable=true)]
        [FieldPosition(2)]
        [NullStringLengthValidator(0, 64)]
        string SKU { get; set; }
        
        [ImmutableFieldId("15b2a5ae-a38c-4446-89b1-a08898f59a30")]
        [FormRenderingProfile(Label = "Ingredients List")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(3)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(IIngredientsList), nameof(IIngredientsList.Id), AllowCascadeDeletes=true, NullReferenceValue="{00000000-0000-0000-0000-000000000000}")]
        Guid IngredientsList { get; set; }
        
        [ImmutableFieldId("590a599e-5a01-4aef-b4d8-66de618cd5ee")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable=true)]
        [FieldPosition(4)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        [TreeOrdering(1)]
        Nullable<int> Order { get; set; }
    }
}