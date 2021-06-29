<%@ Control Language="C#" Inherits="Orckestra.Versioning.VersionPublication.FormControls.AddNewPageVersionFormControlBase, Orckestra.Versioning.VersionPublication"  %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Composite.Core.ResourceSystem" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>
<%@ Import Namespace="Composite.Core.WebClient.UiControlLib" %>
<%@ Register Src="~/Composite/controls/FormsControls/FormUiControlTemplates/Selectors/ComboBox.ascx" TagPrefix="uc1" TagName="ComboBox" %>
<%@ Register Src="~/Composite/controls/FormsControls/FormUiControlTemplates/DateTimeSelectors/DateSelector.ascx" TagPrefix="uc1" TagName="DateSelector" %>

<script type="text/javascript" src="/Composite/InstalledPackages/controls/FormsControls\Orckestra.Versioning.VersionPublication/Bindings/VersionInputSelectorBinding.js" ></script>

<script runat="server">

	public override void BindStateToControlProperties()
	{
		VersionComboBox.Options = this.Options;
		VersionComboBox.BindStateToControlProperties();
		this.VersionName = VersionComboBox.SelectedObjects.Cast<string>().SingleOrDefault();

		PublishDateSelector.BindStateToProperties();
		this.PublishTime = PublishDateSelector.Date;

		if (!PublishDateSelector.IsValid)
		{
			ShowFieldError(PublishDateSelector, PublishDateSelector.ValidationError);
		}

		UnpublishDateSelector.BindStateToProperties();
		this.UnpublishTime = UnpublishDateSelector.Date;

		if (!UnpublishDateSelector.IsValid)
		{
			ShowFieldError(UnpublishDateSelector, UnpublishDateSelector.ValidationError);
		}

		this.DatesAreFormattedCorrectly = PublishDateSelector.IsValid && UnpublishDateSelector.IsValid;
	}


	protected override void OnLoad(EventArgs e)
	{
		if (this.SelectedIndexChangedEventHandler != null)
		{
			VersionComboBox.SelectedIndexChangedEventHandler += SelectedIndexChangedEventHandler;
		}

		base.OnLoad(e);
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		PublishDateSelector.ReadOnly = this.Options.Contains(VersionName);
		UnpublishDateSelector.ReadOnly = this.Options.Contains(VersionName);
	}


	private void ShowFieldError(Control control, string message)
	{
		MessagesPlaceHolder.Controls.Add(new FieldMessage(control.UniqueID, message));
	}

	public override void InitializeViewState()
	{
		var combobox = VersionComboBox as SelectorTemplateUserControlBase;
		combobox.Options = this.Options;
		combobox.SelectedObjects = new List<object> { this.VersionName };
		combobox.InitializeViewState();

		PublishDateSelector.Date = PublishTime;
		PublishDateSelector.InitializeViewState();

		UnpublishDateSelector.Date = UnpublishTime;
		UnpublishDateSelector.InitializeViewState();
	}

	private static string GetString(string key)
	{
		return StringResourceSystemFacade.GetString("Orckestra.Versioning.VersionPublication", key);
	}

</script>
<style type="text/css">
    .field-left,
    .field-right {
        clear: none;
        float: left;
        width: 48%;
    }

    .field-right {
        float: right;
    }
</style>

<div style="display:none">
	<asp:PlaceHolder ID="MessagesPlaceHolder" runat="server"></asp:PlaceHolder>
</div>

<ui:field id="versionselector">
	<ui:fielddesc><%= GetString("AddNewPageVersion.VersionSelector.Label") %></ui:fielddesc>
	<ui:fielddata>
		<uc1:ComboBox runat="server" ID="VersionComboBox" OptionsKeyField="" OptionsLabelField="" Required="True" BindingType="BindToObject" client_binding="VersionInputSelectorBinding" />
	</ui:fielddata>
</ui:field>

<ui:field fieldrel="versionselector" class="field-left">
	<ui:fielddesc><%= GetString("AddNewPageVersion.PublishTime.Label") %></ui:fielddesc>
	<ui:fielddata>
		<uc1:DateSelector runat="server" ID="PublishDateSelector" showHours="true" Required="True"/>
	</ui:fielddata>
</ui:field>

<ui:field fieldrel="versionselector" class="field-right">
	<ui:fielddesc><%= GetString("AddNewPageVersion.UnpublishTime.Label") %></ui:fielddesc>
	<ui:fieldhelp><%= GetString("AddNewPageVersion.VersionSelector.Help") %></ui:fieldhelp>
	<ui:fielddata>
		<uc1:DateSelector runat="server" ID="UnpublishDateSelector" showHours="true" />
	</ui:fielddata>
</ui:field>






