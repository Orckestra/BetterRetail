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
	[ImmutableTypeId("fc3c417b-b1bd-4df2-8f6a-fb695da38549")]
	[KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
	[Title("ComposerPage")]
	[LabelPropertyName("Title")]
	public interface ComposerPage : ILocalizedControlled, IPageMetaData
	{
		[ImmutableFieldId("877a129b-283a-487a-a718-3c6bf9c18d75")]
		[StoreFieldType(PhysicalStoreFieldType.String, 512)]
		[FieldPosition(0)]
		[DefaultFieldStringValue("")]
		string Title { get; set; }
	}
}
