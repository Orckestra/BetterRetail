<%@ Page Language="C#" AutoEventWireup="false" %>

<%@ Import Namespace="System.Threading.Tasks" %>
<%@ Import Namespace="Composite.Core" %>
<%@ Import Namespace="Orckestra.Composer.CompositeC1.Sitemap" %>
<%@ Import Namespace="System" %>

<!DOCTYPE html>
<script runat="server">
    protected void OnLaunchButton_Click(object sender, EventArgs e)
    {

        var sitemapResponse = GenerateSitemaps();
        if (sitemapResponse.SitemapList.Any())
        {
            hostnameList.DataSource = sitemapResponse.SitemapList;
            hostnameList.DataBind();
            Success.Visible = true;
        }
        else
        {
            Success.Visible = false;
        }


        errorList.DataSource = sitemapResponse.ErrorList;
        errorList.DataBind();

        ResultPanel.Update();
    }

    protected SitemapResponse GenerateSitemaps()
    {
        var sitemapGenerators = ServiceLocator.GetService<IMultiSitemapGenerator>();
        return sitemapGenerators.GenerateSitemaps();
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sitemap Generation Status</title>
</head>
<body>
    <form runat="server">
        <asp:ScriptManager runat="server" />
        <div>

            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <h1>Sitemap</h1>
                    <p>
                        <asp:Label runat="server" ID="Status" />
                    </p>
                    <p>
                        <asp:Button runat="server" ID="LaunchButton" OnClick="OnLaunchButton_Click" Text="Launch" />
                    </p>
                    <p>
                        <asp:UpdateProgress runat="server">
                            <ProgressTemplate>Please wait...</ProgressTemplate>
                        </asp:UpdateProgress>
                    </p>

                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel Style="visibility: hidden" ID="ResultPanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Repeater runat="server" ID="errorList">
                        <ItemTemplate>
                            <p style="color: red"><%# Container.DataItem %></p>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:PlaceHolder ID="Success" runat="server">
                        <p>Sitemap was generated</p>
                        <ul>
                            <asp:Repeater runat="server" ID="hostnameList">
                                <ItemTemplate>
                                    <li><a target="_blank" href="<%# Container.DataItem %>/sitemap.xml"><%# Container.DataItem %>/sitemap.xml</a></li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>
    </form>
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(setupButtonDisablers);

            function setupButtonDisablers() {
                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(onSubmit);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(onEndRequest);
                Sys.Application.remove_load(setupButtonDisablers);
            }

            function onSubmit() {
                disableButton();
                hideResultPanel();
            }

            function onEndRequest(e, args) {
                showResultPanel();
                enableButton();
            }

            function enableButton() {
                let button = $get('<%= LaunchButton.ClientID %>');
                button.disabled = false;
            }

            function disableButton() {
                let button = $get('<%= LaunchButton.ClientID %>');
                button.disabled = true;
            }

            function showResultPanel() {
                let resultPanel = $get('<%= ResultPanel.ClientID %>');
                resultPanel.style.visibility = 'visible';
            }

            function hideResultPanel() {
                let resultPanel = $get('<%= ResultPanel.ClientID %>');
                resultPanel.style.visibility = 'hidden';
            }
        })();
    </script>
</body>
</html>
