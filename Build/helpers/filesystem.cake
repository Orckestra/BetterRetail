public static void DeleteDirectories(this ICakeContext context, string pattern)
{
    var directories = context.GetDirectories(pattern);
    var deleteSettings = new DeleteDirectorySettings { Recursive = true, Force = true };
    foreach (var directory in directories)
    {
        if (context.DirectoryExists(directory))
            context.DeleteDirectory(directory, deleteSettings);
    }
}
