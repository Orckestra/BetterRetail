using System;
using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("591d5a4b-053e-4d43-91a1-5a0b4d96a85d")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Google Settings")]
    [LabelPropertyName("MapsApiKey")]
    public interface GoogleSettings : IPageMetaData, ILocalizedControlled
    {
        [ImmutableFieldId("2b4bb15b-3989-481e-b268-264c61a8a8c3")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        [FormRenderingProfile(Label = "GTM Container Id")]
        [FieldPosition(0)]
        string GTMContainerId { get; set; }

        [ImmutableFieldId("3b5bb15b-3989-481e-b268-264c61a8a8c3")]
        [FormRenderingProfile(Label = "Google Maps API Key")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        [FieldPosition(0)]
        string MapsApiKey { get; set; }

        [ImmutableFieldId("2ecbc013-189b-426b-ba6a-4de637f6e89a")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"11\" /></f:function>")]
        [FormRenderingProfile(Label = "Google Maps ZoomLevel")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(1)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        Nullable<int> MapsZoomLevel { get; set; }

        [ImmutableFieldId("e12b49aa-9e4d-489c-aec9-7e3cf13d6f6a")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"30\" /></f:function>")]
        [FormRenderingProfile(Label = "Google Maps Marker Padding")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(2)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        Nullable<int> MapsMarkerPadding { get; set; }
        [ImmutableFieldId("42c33c58-0e2e-42ab-bbd0-f673f60d7c10")]
        [FormRenderingProfile(Label = "Length measure unit")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.String\"><f:param name=\"Constant\" value=\"km\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [FieldPosition(3)]
        string LengthMeasureUnit { get; set; }

        [ImmutableFieldId("32aced52-d65b-4cf3-a13b-5275e218ab5d")]
        [FormRenderingProfile(HelpText = "Set up a distance, in which stores will be available for a customer")]
        [StoreFieldType(PhysicalStoreFieldType.Decimal, 28, 2, IsNullable = true)]
        [FieldPosition(4)]
        [DecimalPrecisionValidator(28, 2)]
        [DefaultFieldDecimalValue(0)]
        Nullable<decimal> StoresAvailabilityDistance { get; set; }
    }
}
