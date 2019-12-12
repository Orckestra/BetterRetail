using System;
using System.IO;
using System.Runtime.InteropServices;

[DllImport( "kernel32.dll" )]
public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

public enum SymbolicLink
{
    File = 0,
    Directory = 1,
};

[DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

public void ReplaceDirWithSymbolicLink(string destination, string source)
{
    if (System.IO.Directory.Exists(destination))
        System.IO.Directory.Delete(destination, true);
    
    Context.Information($"Creating link '{source}'->'{destination}'");
    var res = CreateSymbolicLink(destination, source, SymbolicLink.Directory);
    if (!res)
    {
        throw new Exception($"Failed to create sym link '{source}'->'{destination}'");
    }
}

public void ReplaceFilesWithSymbolicLinks(string destination, string source)
{
    var files = System.IO.Directory.GetFiles(source, "*", SearchOption.AllDirectories);
    foreach (var file in files)
    {
        var relativePath = GetRelativePath(source, file);
        var descFilePath = System.IO.Path.Combine(destination, relativePath);
        ReplaceFileWithSymbolicLink(descFilePath, file);
    }
}

public string GetRelativePath(string relativeTo, string path)
{
    return System.IO.Path.GetFullPath(path).Replace(System.IO.Path.GetFullPath(relativeTo), "").TrimStart(new [] { '\\', '/' });
}

public void ReplaceFileWithSymbolicLink(string destination, string source)
{
    if (System.IO.File.Exists(destination))
        System.IO.File.Delete(destination);

    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(destination)))
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination));

    Context.Information($"Creating link '{source}'->'{destination}'");
    var res = CreateHardLink(destination, source, IntPtr.Zero);
    if (!res)
    {
        throw new Exception($"Failed to create sym link '{source}'->'{destination}'");
    }
}