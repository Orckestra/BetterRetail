using System.Collections.Generic;
using System.Linq;

/// <summary>Finds the native TypeScript compiler restored from the Microsoft.TypeScript.MSBuild package.</summary>
public FilePath FindTsc(string rootDir)
{
    return GetFiles($"{rootDir}/build/tools/Microsoft.TypeScript.MSBuild.*/tools/runtimes/win32-x64/tsc.exe").Last();
}

/// <summary>Runs the native compiler and returns its output. Fails the build on a non-zero exit code.</summary>
public List<string> RunTsc(string rootDir, string arguments)
{
    var settings = new ProcessSettings { Arguments = arguments, RedirectStandardOutput = true };
    int exitCode = StartProcess(FindTsc(rootDir), settings, out IEnumerable<string> output);
    var lines = output.ToList();
    if (exitCode != 0)
        throw new Exception($"Typescript compilation ended with code {exitCode}. " + string.Join("\n", lines));
    return lines;
}

/// <summary>
/// Compiles a project to individual files, then concatenates the emitted scripts into one bundle.
/// The compiler's own file order is used, which keeps base classes ahead of derived ones.
/// </summary>
public void BundleTypeScripts(string rootDir, string configPath, string sourceRoot, string outDir, string bundlePath)
{
    RunTsc(rootDir, $"--project {configPath}");
    string root = MakeAbsolute(Directory(sourceRoot)).FullPath;
    var scripts = RunTsc(rootDir, $"--project {configPath} --listFilesOnly")
        .Where(f => f.EndsWith(".ts") && !f.EndsWith(".d.ts"))
        .Select(f => System.IO.Path.ChangeExtension(System.IO.Path.GetRelativePath(root, f), ".js"))
        .Select(f => System.IO.Path.Combine(MakeAbsolute(Directory(outDir)).FullPath, f))
        .ToList();
    System.IO.File.WriteAllText(bundlePath, string.Join("\n", scripts.Select(System.IO.File.ReadAllText)));
    Information($"Bundled {scripts.Count} scripts into {bundlePath}.");
}
