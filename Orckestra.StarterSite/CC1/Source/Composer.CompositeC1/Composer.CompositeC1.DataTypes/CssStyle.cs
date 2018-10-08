using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("abdeea9b-df69-43b3-9a39-5343072dc6c0")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("CSS Display Style")]
    [LabelPropertyName("Label")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    public interface CssStyle : IData, IProcessControlled, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("a996c418-bd0c-45d8-b07c-ba1c34382754")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("38c42e99-d1be-4c21-aba1-4066eb03857e")]
        [FormRenderingProfile(Label = "Display Name", HelpText = "Displays the name of the CSS Display Style. The name must be short and descriptive. For example, use 'Header Mega menu  - Sales', 'Link - Red' etc.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string Label { get; set; }

        [ImmutableFieldId("7467af98-f51b-41e1-9eb6-18c34de535a3")]
        [FormRenderingProfile(Label = "Display Style Code", HelpText = "Specify the style code e.g. color: blue;", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.TextArea"" />")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [NotNullValidator]
        [FieldPosition(1)]
        [DefaultFieldStringValue("")]
        string CssCode { get; set; }
    }
}