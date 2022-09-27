using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("8a5e3541-17cd-4cca-a67e-cd23a931594b")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Variant Color")]
    [LabelPropertyName("LookupValue")]
    [InternalUrl("VariantColor")]
    [Caching(CachingType.Full)]
    public interface IVariantColorConfiguration : IData
    {
        [ImmutableFieldId("620358ee-702c-459c-9a47-e397fbdf7bb1")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("2f192350-dba2-40a1-a81e-1d398fc54c00")]
        [FormRenderingProfile(Label = "Lookup Value", HelpText = "Specify the value which equals Color lookup value you want to configure")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable = true)]
        [FieldPosition(0)]
        [NullStringLengthValidator(0, 64)]
        string LookupValue { get; set; }

        [ImmutableFieldId("741ea996-eb77-48c3-a4c3-e47dec9903bd")]
        [FormRenderingProfile(HelpText = "CSS valid color format, ex: white, #fff, rgba(0,0,0,255)")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable = true)]
        [FieldPosition(1)]
        [NullStringLengthValidator(0, 64)]
        string Color { get; set; }

        [ImmutableFieldId("3b308c07-a1ed-4197-a4a0-556f7c06211a")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable = true)]
        [FieldPosition(2)]
        [NullStringLengthValidator(0, 64)]
        string Image { get; set; }
    }
}