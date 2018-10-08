<%@ Page Language="C#" AutoEventWireup="false" %>
<%@ Import Namespace="Hangfire" %>
<%@ Import Namespace="Hangfire.Storage" %>

<!DOCTYPE html>
<script runat="server">
    protected void OnStatusButton_Click(object sender, EventArgs e)
    {
        using (var connection = JobStorage.Current.GetConnection())
        {
            var builder = new StringBuilder();

            JobStorage.Current.GetMonitoringApi().SucceededJobs(0, 10).ForEach(kvp =>
                builder.AppendFormat("Type: {0}, Last Execution: {1}, Duration: {2}<br/>", kvp.Value.Job.Type, kvp.Value.SucceededAt.GetValueOrDefault(), kvp.Value.TotalDuration.GetValueOrDefault())
            );

            Status.Text = builder.ToString();
        }   
    }

    protected void OnLaunchButton_Click(object sender, EventArgs e)
    {
        RecurringJob.Trigger("sitemap-generation");
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sitemap Generation Status</title>
</head>
<body>
    <form runat="server">
        <div>
            <h1>Sitemap Job Status</h1>
            <p>
                <asp:Label runat="server" ID="Status" />
            </p>
            <p>
                <asp:Button runat="server" ID="StatusButton" OnClick="OnStatusButton_Click" Text="Status" />
                <asp:Button runat="server" ID="LaunchButton" OnClick="OnLaunchButton_Click" Text="Launch" />
            </p>
        </div>
    </form>
</body>
</html>
