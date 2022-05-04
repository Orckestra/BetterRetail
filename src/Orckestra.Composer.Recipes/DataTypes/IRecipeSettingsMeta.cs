using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using System;

namespace Orckestra.Composer.Recipes.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("4f16a00c-b156-45b1-86db-cdfca8ca2780")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Recipe Settings")]
    [LabelPropertyName("FavoritesPageId")]
    public interface IRecipeSettingsMeta: IPageMetaData, ILocalizedControlled
    {

        [ImmutableFieldId("3f282980-8ca9-4e6d-8582-ba5b5f90d280")]
        [FormRenderingProfile(Label = "My Recipe Favorites Page")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(0)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes = true)]
        Nullable<Guid> FavoritesPageId { get; set; }
    }
}
