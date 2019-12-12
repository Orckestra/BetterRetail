public void DeleteDirectories(string pattern)
{
    var directories = Context.GetDirectories(pattern);
    var deleteSettings = new DeleteDirectorySettings { Recursive = true, Force = true };
    foreach (var directory in directories)
    {
        if (Context.DirectoryExists(directory))
            Context.DeleteDirectory(directory, deleteSettings);
    }
}
