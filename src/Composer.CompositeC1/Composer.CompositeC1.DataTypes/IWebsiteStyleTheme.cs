using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("991d5a4b-053e-4d43-91a1-5a0b4d96a84d")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Website Style Theme")]
    [LabelPropertyName("Theme")]
    public interface IWebsiteStyleTheme : IPageMetaData, ILocalizedControlled
    {
        [ImmutableFieldId("2b4bb15b-3989-481e-b268-264c61a8a8c3")]
        [FormRenderingProfile(Label = "Style Theme", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.Selector"">
        <f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options"">
            <f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Composer.CompositeC1.Widgets.WidgetFunctions.GetWebsiteThemeOptions"" />
        </f:param>
        <f:param name=""Required"" value=""False"" />
        </f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        [FieldPosition(0)]
        string Theme { get; set; }
    }
}
