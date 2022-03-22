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
    [ImmutableTypeId("bba89fbe-8485-4b84-a8ce-9a3bd16be313")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Recipe Ingredients List")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    [SearchableType]
    public interface IIngredientsList : IData, IProcessControlled, ILocalizedControlled
    {
        [ImmutableFieldId("0f2df7ef-9718-4547-b565-b36988fca751")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("f539ca55-585a-4642-8e73-5bf35a9bac6c")]
        [FormRenderingProfile(Label = "Title")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }

        [ImmutableFieldId("85fd7c6d-78bd-446d-84eb-6170d4c4d79e")]
        [FormRenderingProfile(Label = "Hide Title")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(0)]
        [DefaultFieldBoolValue(false)]
        bool HideTitle { get; set; }

        [ImmutableFieldId("7f565799-d112-4b3c-b702-76a85422b3f8")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(1)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(IRecipe), nameof(IRecipe.Id), AllowCascadeDeletes=true, NullReferenceValue="{00000000-0000-0000-0000-000000000000}")]
        Guid Recipe { get; set; }
        
        [ImmutableFieldId("5fae8812-e782-4a83-8b0d-6f6182fc3b75")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(2)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        [TreeOrdering(1)]
        int Order { get; set; }
    }
}