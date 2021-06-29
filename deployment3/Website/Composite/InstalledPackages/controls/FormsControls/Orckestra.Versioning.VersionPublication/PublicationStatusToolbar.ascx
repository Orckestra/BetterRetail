<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PublicationStatusToolbar.ascx.cs" Inherits="Composite_Plugins_Forms_WebChannel_UiControlFactories_ToolbarButtonTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Core.ResourceSystem" %>
<script runat="server">
	private static string GetString(string key)
	{
		return StringResourceSystemFacade.GetString("Orckestra.Versioning.VersionPublication", key);
	}
</script>
<script type="text/javascript" src="/Composite/InstalledPackages/controls/FormsControls/Orckestra.Versioning.VersionPublication/Bindings/PublicationScheduleToobarButton.js" ></script>
<ui:toolbarbutton id="publicationschedule" label="<%= GetString("PublicationSchedule.PageTitle") %>" class="btn-secondary" binding="PublicationScheduleToobarButton"
    url="${root}/InstalledPackages/content/views/Orckestra.Versioning.VersionPublication/PublicationSchedule.aspx?PageId=<%# PageIdStatusString %>&amp;PageVersionId=<%# PageVersionId %>"
    save-and-publsih-label="<%= GetSaveAndPublishTitle() %>"/>
