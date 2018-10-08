using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;

namespace Orckestra.Composer.CompositeC1.Pages
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("8072298d-e1ef-4228-8d0d-ab57f9d2a0d3")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("ComposerImage")]
    [LabelPropertyName("Image")]
    public interface ComposerImage : IData, ILocalizedControlled, IProcessControlled
    {
        [ImmutableFieldId("1894b585-a2be-4871-9875-a08c767b38d3")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(-1)]
        Guid Id { get; set; }

        [ImmutableFieldId("18d5738b-7b3a-4484-9788-4dd604d6a084")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.ImageSelector""><f:param name=""Required"" value=""True"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 2048)]
        [FieldPosition(0)]
        [DefaultFieldStringValue("")]
        [ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes = true, NullReferenceValue = null)]
        string Image { get; set; }

        [ImmutableFieldId("66affd32-0a69-44d8-8ef6-33f019c868f3")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        [FieldPosition(0)]
        string Alt { get; set; }
    }
}
