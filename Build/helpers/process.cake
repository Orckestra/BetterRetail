using System.Diagnostics;

public static Process StartHiddenProcess(this ICakeContext context, string path, params object[] parameters)
{
    var process = new Process();
    process.StartInfo.FileName = path;
    process.StartInfo.Arguments = string.Join(" ", parameters);
    process.StartInfo.CreateNoWindow = true;
    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.Start();
    return process;
}

public static void DisplayProcessOutput(this ICakeContext context, Process process)
{
    try
    {
        var output = process.StandardOutput.ReadToEnd();
        context.Information($"{process.ProcessName} output:");
        context.Information(output);
    }
    catch (Exception ex)
    {
        context.Warning($"Can't get output of process {process.ProcessName} error:");
        context.Warning(ex.ToString());
    }
}