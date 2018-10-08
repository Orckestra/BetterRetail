<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.IO.Compression" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Composite" %>
<%@ Import Namespace="Composite.C1Console.Events" %>
<%@ Import Namespace="Composite.C1Console.Security" %>
<%@ Import Namespace="Composite.Core.Configuration" %>
<%@ Import Namespace="Composite.Core.IO" %>
<%@ Import Namespace="Composite.Core.Localization" %>
<%@ Import Namespace="Composite.Core.PackageSystem" %>
<%@ Import Namespace="Composite.Data" %>
<%@ Import Namespace="Composite.Data.Types" %>
<%@ Language=C# %>
<html xmlns="http://www.w3.org/1999/xhtml">
    <script runat="server" language="c#">
		protected void Page_Load(object sender, EventArgs e)
		{


			var culture = Request["culture"];

			if (!string.IsNullOrWhiteSpace(culture))
			{
				var cultureInfo = new CultureInfo(culture);

				if (!LocalizationFacade.IsLocaleInstalled(cultureInfo))
				{
					LocalizationFacade.AddLocale(culture, culture, true);
				}
				else
				{
					OutPut(culture + " exists.");
				}
			}


			OutPut("Done.");

			Response.Flush();
			Response.End();
		}

		private void OutPut(object @object)
		{
			if (@object == null) return;

			Response.Write(@object.ToString() + "\n");
		}


	</script>
</html>
