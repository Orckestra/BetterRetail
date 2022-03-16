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
    [ImmutableTypeId("07199f68-192b-4b2e-84ef-49e68e453200")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Recipe Difficulty")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    public interface IDifficulty : IData, ILocalizedControlled, IProcessControlled
    {
        [ImmutableFieldId("e1e9efa9-a460-4a77-be2f-ec09192f86a8")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("396977a1-b403-450e-b5c7-9359a4b93e06")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }
        
        [ImmutableFieldId("a0d27036-c02d-4d5d-96a4-5a3cf419547c")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(1)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        [TreeOrdering(1)]
        int Order { get; set; }
    }
}