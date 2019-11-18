<%@ Page Language="C#" AutoEventWireup="false" %>
<%@ Import Namespace="System.Threading.Tasks" %>
<%@ Import Namespace="Hangfire" %>
<%@ Import Namespace="Hangfire.Storage" %>
<%@ Import Namespace="Composite.Core" %>
<%@ Import Namespace="Orckestra.Composer.CompositeC1.Sitemap" %>

<!DOCTYPE html>
<script runat="server">
    protected void OnLaunchButton_Click(object sender, EventArgs e)
    {
        GenerateSitemaps();
    }

    protected void GenerateSitemaps()
    {
        var sitemapGenerators = ServiceLocator.GetService<IMultiSitemapGenerator>();
        sitemapGenerators.GenerateSitemaps();
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
        </div>
    </form>
<script type="text/javascript">
    (function () {
        Sys.Application.add_load(setupButtonDisablers);

        function setupButtonDisablers() {
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(onPageLoad);
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(onSubmit);
            Sys.Application.remove_load(setupButtonDisablers);
        }

        function onPageLoad() {
            findAndEnable('<%= LaunchButton.ClientID %>');
        }

        function onSubmit() {
            findAndDisable('<%= LaunchButton.ClientID %>');
        }

        function findAndDisable(id) {
            findAndSetDisabledProperty(id, true);
        }

        function findAndEnable(id) {
            findAndSetDisabledProperty(id, false);
        }

        function findAndSetDisabledProperty(id, value) {
            var control = $get(id);
            if (control) {
                control.disabled = value;
            }
        }
    })();
</script>
</body>
</html>
