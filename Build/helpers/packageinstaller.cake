#tool "nuget:?package=Composite.PackageInstaller&version=0.1.4"
using Cake.Core;
using Cake.Core.Annotations;

[CakeMethodAlias]
public PackageInstaller GetPackageInstaller(string websiteDir) =>
     new PackageInstaller(Context, websiteDir);


public class PackageInstaller : IDisposable
{
    private readonly string[] _installerFiles = { "Composite.PackageInstaller.exe", "Composite.PackageInstaller.exe.config", "CommandLine.dll" };
    private string _websiteDir;
    private ICakeContext _context;

    public PackageInstaller(ICakeContext context, string websiteDir)
    {
        _context = context;
        _websiteDir = websiteDir;
        var rootDir = _context.MakeAbsolute(DirectoryPath.FromString("..")).FullPath;

         _context.Information("Inject Package Installer to Website");
        foreach (var installerFile in this._installerFiles) {
            var file = _context.GetFiles($"{rootDir}/build/tools/Composite.PackageInstaller*/**/{installerFile}").Last();
            _context.CopyFile(file, $"{_websiteDir}/Bin/{file.GetFilename()}");
        }

        CopyRuntimeConfigSection();
    }

    public void InstallPackagesFromConfig(string culture, string configFile, string branch = null) {
        _context.StartProcess($"{_websiteDir}/Bin/Composite.PackageInstaller.exe", new ProcessSettings {
            Arguments = $"-l \"{culture}\" --configFile \"{configFile}\"" + (branch == null? "" : $" --branch \"{branch}\"")
        });
    }

    public void InstallPackage(string culture, string packageFile) {
        _context.StartProcess($"{_websiteDir}/Bin/Composite.PackageInstaller.exe", new ProcessSettings {
            Arguments = $"-l \"{culture}\" --packageFile \"{packageFile}\""
        });
    }

    private void CopyRuntimeConfigSection() {
        var webConfigFile = $"{_websiteDir}/Web.config";
        var installerConfigFile = $"{_websiteDir}/Bin/Composite.PackageInstaller.exe.config";

        var documentWeb = XDocument.Load(webConfigFile);
        var documentInstaller = XDocument.Load(installerConfigFile);
        var runtimeWebConfig = documentWeb.XPathSelectElement("/configuration/runtime");
        var runtimeInstaller = documentInstaller.XPathSelectElement("/configuration/runtime");

        runtimeInstaller?.ReplaceWith(runtimeWebConfig);

        documentInstaller.Save(installerConfigFile);
    }

    public void Dispose()
    {
        _context.Information("Removing Package Installer from Website");
        foreach (var installerFile in this._installerFiles) {
            _context.DeleteFile($"{_websiteDir}/Bin/{installerFile}");
        }
    }
}