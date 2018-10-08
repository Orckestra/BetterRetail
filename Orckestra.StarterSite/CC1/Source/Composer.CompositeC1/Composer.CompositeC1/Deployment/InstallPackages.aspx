<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.IO.Compression" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="Composite" %>
<%@ Import Namespace="Composite.C1Console.Events" %>
<%@ Import Namespace="Composite.C1Console.Security" %>
<%@ Import Namespace="Composite.Core.Configuration" %>
<%@ Import Namespace="Composite.Core.IO" %>
<%@ Import Namespace="Composite.Core.PackageSystem" %>
<%@ Import Namespace="Composite.Data" %>
<%@ Import Namespace="Composite.Data.Types" %>
<%@ Import Namespace="ICSharpCode.SharpZipLib.Zip" %>
<%@ Language=C# %>
<html xmlns="http://www.w3.org/1999/xhtml">
    <script runat="server" language="c#">
            protected void Page_Load(object sender, EventArgs e)
            {
            Response.Clear();
            Response.ClearHeaders();
			Response.AddHeader("Content-Type", "text/plain");
            
            if (!IsSecuredRequest())
            {
               Response.StatusCode = 403;
               Response.Write("Unauthorized access");
               Response.Flush();
               Response.End();
            
               return;
            }
            
            
            EnsureLoggedInUser();
            
                bool shouldInstallCodePackage = Request.Params["InstallCodePackage"]=="true";
                bool shouldInstallContentPackage = Request.Params["InstallContentPackage"]=="true";
                string packageToInstallAndResetSite = Request.Params["InstallPackageAndResetSite"];

			    string serverPath = Server.MapPath(@"~/");
                OutPut("Path for packages :  " + serverPath);
                IEnumerable<string> allPackageFiles = Directory.EnumerateFiles(serverPath, "*.zip");
                IEnumerable<string> packageFiles = null;
                if (string.IsNullOrWhiteSpace(packageToInstallAndResetSite))
                {
                    packageFiles =
                        from fullPathName in allPackageFiles
                        let filename = Path.GetFileName(fullPathName)
                        where (shouldInstallCodePackage && (filename.StartsWith("0") || filename.StartsWith("1")) || (shouldInstallContentPackage && filename.StartsWith("2")))
                        select fullPathName;
                }
                else
                {
                    packageFiles = from fullPathName in allPackageFiles
                        let filename = Path.GetFileName(fullPathName)
                        where (String.Equals(filename, packageToInstallAndResetSite.Replace("%20"," "), StringComparison.InvariantCultureIgnoreCase))
                        select fullPathName;
                }
				
				SystemSetupFacade.SetupIsRunning = true;

                foreach (string package in packageFiles)
                {
                    OutPut("Installing " + package + "...");

                    bool alreadyInstalled = PackageManager.IsInstalled(GetPackageId(package));
                    if (!alreadyInstalled)
                    {
                        Stream zipFileStream = new FileStream(package, FileMode.Open);
                        var installProcess = PackageManager.Install(zipFileStream, true);
                        try
                        {
                            var validationResults = installProcess.Validate();
                            foreach (var r in validationResults)
                            {
                                OutPut(r.ValidationResult + ": " + r.Message);
                                OutPut(r.Exception);
                                OutPut(r.InnerResult);
                            }

                            if (!validationResults.Any(v => v.ValidationResult == PackageFragmentValidationResultType.Fatal))
                            {
                                var installationResults = installProcess.Install();
                                foreach (var r in installationResults)
                                {
                                    OutPut(r.ValidationResult + ": " + r.Message);
                                    OutPut(r.Exception);
                                    OutPut(r.InnerResult);
                                }

                                if (installProcess.FlushOnCompletion)
                                {
                                    OutPut("Flushing on completion ...");
                                    GlobalEventSystemFacade.FlushTheSystem();
                                }
                            }
                            
                            zipFileStream.Close();
                            zipFileStream.Dispose();

                        }
                        catch (Exception ex)
                        {
                        OutPut("--------------------------------------------------------------------------------");
                        OutPut("[ERROR]");
                            OutPut("Exception caught ! : " + ex);
                            OutPut(ex.Message);
                            OutPut(ex.StackTrace);
                        OutPut("--------------------------------------------------------------------------------");
                        }
                    }
                }
                SystemSetupFacade.SetupIsRunning = false;

                OutPut("Done.");
            
            Response.Flush();
            Response.End();
            }

        private void OutPut(object @object)
            {
                if(@object == null) return;

            Response.Write(@object.ToString()+"\n");
            }            
            
        private static Guid GetPackageId(string filename)
        {
            Guid packageId = Guid.Empty;

            using (var zip = new ZipFile(filename))
            {
                var entry = zip.GetEntry("install.xml");
                if (entry != null)
                {
                    using (var stream = zip.GetInputStream(entry))
                    {
                        var result = new byte[entry.Size];
                        stream.Read(result, 0, result.Length);
                        var xmlContentAsText = Encoding.Default.GetString(result);
                        var xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(xmlContentAsText);
                        var packageInformationElements = xmlDocument.GetElementsByTagName("mi:PackageInformation");
                        if (packageInformationElements.Count > 0)
                        {
                            var element = packageInformationElements[0];
                            if (element.Attributes != null)
                            {
                                var idAttribute = element.Attributes["id"];
                                if (idAttribute != null)
                                {
                                    packageId = new Guid(idAttribute.Value);
                                }
                            }
                        }
                    }
                }
            }
            return packageId;
        }            
        
        private bool IsSecuredRequest()
        {
            Response.Write("Request secured by token\r\n");
           string authHeader = Request.Headers["X-Auth"] ?? String.Empty;
           string token = ConfigurationManager.AppSettings["CC1.DeploymentToken"] ?? String.Empty;
        
           var isEqual = String.Equals(authHeader, token, StringComparison.InvariantCultureIgnoreCase);
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
</html>
