<%@ Page Language="C#" Debug="true" Async="true" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="Composite.C1Console.Security" %>
<%@ Import Namespace="Composite.Core.Configuration" %>
<%@ Import Namespace="Composite.Data" %>
<%@ Import Namespace="Composite.Data.Types" %>
<%@ Import Namespace="Orckestra.Composer" %>
<%@ Import Namespace="Orckestra.Composer.CompositeC1.Services" %>
<script runat="server" language="c#">

    protected void Page_Load(object sender, EventArgs e)
    {
        Handle();
    }

    private void Handle()
    {
        Response.Clear();
        Response.ClearHeaders();
        Response.ContentType = "text/plain";

        if (!IsSecuredRequest())
        {
            Response.StatusCode = 403;
            OutPut("Unauthorized access");
            Response.Flush();
            Response.End();
            
            return;
        }

        OutPut("Logging in...");
        EnsureLoggedInUser();
        SystemSetupFacade.SetupIsRunning = true;

        OutPut("Starting category sync");
        
        try {
            
            var categoryService = ComposerHost.Current.Resolve<ICategoryBrowsingService>();
            categoryService.Clear();    // We do a Clear in case Categories found their way into the content package...
            categoryService.Sync();
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            
            OutPut("--------------------------------------------------------------------------------");
            OutPut("[ERROR]");
                OutPut("Exception caught ! : " + ex);
                OutPut(ex.Message);
                OutPut(ex.StackTrace);
            OutPut("--------------------------------------------------------------------------------");
        }
        SystemSetupFacade.SetupIsRunning = false;

        OutPut("Categories were synced");
        Response.Flush();
        Response.End();
    }
    
    private void OutPut(object @object)
    {
        if(@object == null) return;

        Response.Write(@object + "\r\n");
    }     

    private bool IsSecuredRequest()
    {
        OutPut("Request secured by token");
        string authHeader = Request.Headers["X-Auth"] ?? string.Empty;
        string token = ConfigurationManager.AppSettings["CC1.DeploymentToken"] ?? string.Empty;
        
        var isEqual = string.Equals(authHeader, token, StringComparison.InvariantCultureIgnoreCase);
        return isEqual;
    }

    private static void EnsureLoggedInUser()
    {
        if (UserValidationFacade.IsLoggedIn())
        {
            return;
        }
    
        string userName;
    
        using (var c = new DataConnection())
        {
            userName = c.Get<IUser>().Select(u => u.Username).FirstOrDefault();
        }
    
        if (userName != null)
        {
            var userValidationFacade = typeof(UserValidationFacade);
            var methodInfo = userValidationFacade.GetMethod("PersistUsernameInSessionDataProvider", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    
            if (methodInfo != null)
            {
                methodInfo.Invoke(null, new object[] { userName });
            }
        }
    }
</script>