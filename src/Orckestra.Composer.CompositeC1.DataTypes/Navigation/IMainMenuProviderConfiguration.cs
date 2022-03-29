using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes.Navigation
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("34e87f08-3017-4ff6-b4df-cabe4ca23622")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("External Main Menu Items Provider Configuration")]
    [LabelPropertyName("Provider")]
    [Caching(CachingType.Full)]
    public interface IMainMenuProviderConfiguration : IData, IPageDataFolder, IPageRelatedData
    {
        [ImmutableFieldId("3cb8d836-9d2d-41d8-984a-abec4349356a")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("e50b9c2d-9a19-49c2-ad5c-19ada3ac9acd")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Composer.CompositeC1.Widgets.WidgetFunctions.GetMainMenuProviderNames"" /></f:param></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Provider { get; set; }

        [ImmutableFieldId("d18a7e45-2a98-47c7-9611-abee742d9b29")]
        [FormRenderingProfile(Label = "Parent Page ", HelpText = "If present, then will be used as parent navigation item")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(1)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes = true)]
        Nullable<Guid> ParentPage { get; set; }

        [ImmutableFieldId("8b7f1074-4495-4d40-adf8-da08a43757fe")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
    "nstant.Integer\"><f:param name=\"Constant\" value=\"1\" /></f:function>")]
        [FormRenderingProfile(Label = "Display Order", HelpText = "Specifies the position of this menu item in the menu.")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(2)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        int Order { get; set; }
    }
}
