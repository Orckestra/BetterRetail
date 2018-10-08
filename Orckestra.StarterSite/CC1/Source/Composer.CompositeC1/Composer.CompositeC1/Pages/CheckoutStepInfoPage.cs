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
    [ImmutableTypeId("554f7290-fca4-4390-a4cb-044cd0096149")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("CheckoutStepInfoPage")]
    [LabelPropertyName("CurrentStep")]
    public interface CheckoutStepInfoPage : IData, ILocalizedControlled, IProcessControlled, IPageData, IPageMetaData, IPublishControlled
    {
        [ImmutableFieldId("3ecf7088-f609-46cb-a82c-2aba4476fd7f")]
        [FormRenderingProfile(Label = "Current Step")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(0)]
        [DefaultFieldIntValue(-1)]
        int CurrentStep { get; set; }

        [ImmutableFieldId("f5c1cf46-3cd7-4c54-b445-fabfe3b38f3b")]
        [FormRenderingProfile(Label = "Is Displayed In Header")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(1)]
        [DefaultFieldBoolValue(false)]
        bool IsDisplayedInHeader { get; set; }
    }
}
