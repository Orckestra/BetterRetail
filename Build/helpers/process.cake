using System.Diagnostics;

public Process StartHiddenProcess(string path, params object[] parameters)
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

public void DisplayProcessOutput(Process process)
{
    try
    {
        var output = process.StandardOutput.ReadToEnd();
        Context.Information($"{process.ProcessName} output:");
        Context.Information(output);
    }
    catch (Exception ex)
    {
        Context.Warning($"Can't get output of process {process.ProcessName} error:");
        Context.Warning(ex.ToString());
    }
}