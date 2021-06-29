<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PublicationSchedule.aspx.cs" Inherits="Orckestra_Versioning_VersionPublication_publicationschedule" %>
<%@ Import Namespace="Orckestra.Versioning.VersionPublication" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
<control:httpheaders runat="server" />
<head>
	<control:styleloader runat="server" />
	<control:scriptloader type="sub" runat="server" />
	<title><%= Request["title"] %></title>
	<link rel="stylesheet" type="text/css" href="PublicationSchedule.css.aspx" />
	<link href="https://fonts.googleapis.com/css?family=Open+Sans:600,400" rel="stylesheet" type="text/css" />
	<link href="PublicationSchedule.css.aspx" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="bindings/VersionPageBinding.js"></script>
	<script type="text/javascript" src="bindings/RowContainerBinding.js"></script>
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.2/jquery.min.js"></script>
</head>
<body>
	<form runat="server" class="updateform updatezone">
		<ui:page label="${Orckestra.Versioning.VersionPublication,PublicationSchedule.PageTitle}" image="${icon:page-list-unpublished-items}" binding="VersionPageBinding" id="page" page-id="<%= _pageId %>">
			<ui:toolbar id="toolbar">
				<ui:toolbarbody>
					<ui:toolbargroup>
						<ui:toolbarbutton image="${icon:previous}" oncommand="this.dispatchAction(PageBinding.ACTION_RESPONSE);" class="back"/>
					</ui:toolbargroup>
					<ui:toolbargroup class="title text-primary" >
						<%= Localization.PublicationSchedule_PageTitle %>
					</ui:toolbargroup>
				</ui:toolbarbody>
				<ui:toolbarbody align="right">
					<ui:toolbargroup class="btns-group">
						<aspui:ToolbarButton runat="server" ID="PastButton" Text="${Orckestra.Versioning.VersionPublication,PublicationSchedule.Past.Label}" OnClick="PastButton_Click"></aspui:ToolbarButton>
						<aspui:ToolbarButton runat="server" ID="ActiveButton" Text="${Orckestra.Versioning.VersionPublication,PublicationSchedule.Active.Label}" OnClick="ActiveButton_Click"></aspui:ToolbarButton>
					</ui:toolbargroup>
				</ui:toolbarbody>
			</ui:toolbar>
			<ui:pagebody>


				<ui:scrollbox id="scrollbox">
					<% if(!Versions.Visible) { %>
					<div class="center">
						<svg id="logo" xmlns="http://www.w3.org/2000/svg" version="1.1" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:svgjs="http://svgjs.com/svgjs" width="144" height="144"><defs id="SvgjsDefs1001"></defs><path id="SvgjsPath1007" d="M638 398C638 358.2355 670.2355 326 710 326C749.7645 326 782 358.2355 782 398C782 437.7645 749.7645 470 710 470C670.2355 470 638 437.7645 638 398Z " fill="#dfdfdf" fill-opacity="1" transform="matrix(1,0,0,1,-638,-326)"></path><path id="SvgjsPath1008" d="M718.85 417L674.9 417C673.3 417 672 418.3 672 419.9L672 435.41999999999996C672 437.02 673.3 438.31999999999994 674.9 438.31999999999994L718.85 438.31999999999994C720.45 438.31999999999994 721.75 437.0199999999999 721.75 435.41999999999996L721.75 419.9C721.75 418.29999999999995 720.45 417 718.85 417 " fill="#f1f1f1" fill-opacity="1" transform="matrix(1,0,0,1,-638,-326)"></path><path id="SvgjsPath1009" d="M719.42 389L703.9 389C702.3 389 701 390.3 701 391.9L701 407.41999999999996C701 409.02 702.3 410.31999999999994 703.9 410.31999999999994L719.42 410.31999999999994C721.02 410.31999999999994 722.3199999999999 409.0199999999999 722.3199999999999 407.41999999999996L722.3199999999999 391.9C722.3199999999999 390.29999999999995 721.02 389 719.42 389 " fill="#e7e7e7" fill-opacity="1" transform="matrix(1,0,0,1,-638,-326)"></path><path id="SvgjsPath1010" d="M711.74 360L674.9 360C673.3 360 672 361.3 672 362.9L672 406.84999999999997C672 408.45 673.3 409.74999999999994 674.9 409.74999999999994L690.42 409.74999999999994C692.02 409.74999999999994 693.3199999999999 408.44999999999993 693.3199999999999 406.84999999999997L693.3199999999999 381.31999999999994L711.7399999999999 381.31999999999994C713.3399999999999 381.31999999999994 714.6399999999999 380.0199999999999 714.6399999999999 378.41999999999996L714.6399999999999 362.9C714.6399999999999 361.29999999999995 713.3399999999999 360 711.7399999999999 360 " fill="#f1f1f1" fill-opacity="1" transform="matrix(1,0,0,1,-638,-326)"></path><path id="SvgjsPath1011" d="M747.53 360L724.9 360C723.3 360 722 361.3 722 362.9L722 378.41999999999996C722 380.02 723.3 381.31999999999994 724.9 381.31999999999994L729.11 381.31999999999994L729.11 435.2799999999999C729.11 436.87999999999994 730.4 438.1699999999999 732 438.1699999999999L747.53 438.1699999999999C749.13 438.1699999999999 750.43 436.8799999999999 750.43 435.2799999999999L750.43 362.8999999999999C750.43 361.2999999999999 749.13 359.99999999999994 747.53 359.99999999999994 " fill="#fdfdfd" fill-opacity="1" transform="matrix(1,0,0,1,-638,-326)"></path></svg>
						<% if(PublicationStatus == "Active") { %>
							<h1><%= Localization.PublicationSchedule_NoActiveVersionsText %></h1>
							<aspui:ToolbarButton runat="server" ID="AddVersionButton" ImageUrl="${icon:add}"  OnClick="AddVersionButton_Click" Text="${Orckestra.Versioning.VersionPublication,EditPageToolbar.AddNewVersionLabel}" Visible="true"></aspui:ToolbarButton>
						<% } else { %>
							<h1><%= Localization.PublicationSchedule_NoPastVersionsText %></h1>
						<% } %>
					</div>
					<% } %>
					<table class="table">
						<asp:Repeater ID="Versions" runat="server" 
							OnItemCommand="rptUserData_ItemCommand" OnItemDataBound="rptUserData_ItemDataBound"
							ItemType="VersionViewModel">
							<HeaderTemplate>
								<thead>
									<tr class="head">
										<th></th>
										<th>
											<asp:LinkButton ID="LinkButton0" runat="server" CommandName="Version" CssClass="hrefclass"><%# Localization.PublicationSchedule_Columns_Version %></asp:LinkButton>
										</th>
										<th>
											<asp:LinkButton ID="LinkButton1" runat="server" CommandName="Status" CssClass="hrefclass"><%# Localization.PublicationSchedule_Columns_Status %></asp:LinkButton>
										</th>
										<th>
											<asp:LinkButton ID="LinkButton2" runat="server" CommandName="DateCreated" CssClass="hrefclass"><%# Localization.PublicationSchedule_Columns_DateCreated %></asp:LinkButton>
										</th>
										<th>
											<asp:LinkButton ID="LinkButton3" runat="server" CommandName="DateModified" CssClass="hrefclass"><%# Localization.PublicationSchedule_Columns_DateModified %></asp:LinkButton>
										</th>
										<th>
											<asp:LinkButton ID="LinkButton4" runat="server" CommandName="Author" CssClass="hrefclass"><%# Localization.PublicationSchedule_Columns_Author %></asp:LinkButton>
										</th>
										<th>
											<asp:LinkButton ID="LinkButton5" runat="server" CommandName="PublishDate" CssClass="hrefclass"><%# Localization.PublicationSchedule_Columns_PublishDate %></asp:LinkButton>
											<ui:fieldhelp ><%# Localization.PublicationSchedule_Columns_PublishDate_Help %></ui:fieldhelp>
										</th>
										<th>
											<asp:LinkButton ID="LinkButton6" runat="server" CommandName="UnpublishDate" CssClass="hrefclass"><%# Localization.PublicationSchedule_Columns_UnpublishDate %></asp:LinkButton>
											<ui:fieldhelp ><%# Localization.PublicationSchedule_Columns_UnpublishDate_Help %></ui:fieldhelp>
										</th>
										<th></th>
										<th></th>
										<th></th>
									</tr>
								</thead>
								<tbody id="tbody" binding="RowContainerBinding">
							</HeaderTemplate>
							<ItemTemplate>
								<tr live="<%# Item.Live.ToString().ToLower() %>" 
									class="<%# Item.Live ? " text-primary" : "" %>">
									<td>
										<ui:labelbox image="page" class="icon" />
									</td>
									<td>
										<%#Server.HtmlEncode( Item.Version )%>
									</td>
									<td>
										<%#Server.HtmlEncode( Item.Status )%>
									</td>
									<td>
										<%#Server.HtmlEncode( Item.DateCreated )%>
									</td>
									<td>
										<%#Server.HtmlEncode( Item.DateModified )%>
									</td>
									<td>
										<%#Server.HtmlEncode( Item.Author )%>
									</td>
									<td class="date">
										<%#Server.HtmlEncode( Item.PublishDate )%>
									</td>
									<td class="date">
										<%#Server.HtmlEncode( Item.UnpublishDate )%>
									</td>
									<td>
										<aspui:ToolbarButton runat="server" ID="EditButton" ImageUrl="${icon:edit}" 
											CommandArgument='<%# Item.VersionId.ToString()%>' OnClick="EditButton_Click" 
											client_class="hidden"
											Visible="<%# Item.ShowEditButton %>">
										</aspui:ToolbarButton>
									</td>
									<td>
										<aspui:ToolbarButton runat="server" ID="DeleteButton" ImageUrl="${icon:delete}" 
											CommandArgument='<%# Item.VersionId.ToString()%>' OnClick="DeleteButton_Click" 
											client_class="hidden"
											Visible="<%# Item.ShowDeleteButton %>">
										</aspui:ToolbarButton>
									</td>
									<td></td>
								</tr>
							</ItemTemplate>
							<FooterTemplate>
                                </tbody>

                            </FooterTemplate>
						</asp:Repeater>
					</table>
				</ui:scrollbox>

			</ui:pagebody>
		</ui:page>

	</form>

</body>

</html>
