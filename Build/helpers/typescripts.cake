using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public void CompileTypeScripts(string rootDir, string projectFilePathArg, int timeOutSeconds)
{
    string compilerPath = GetFiles($"{rootDir}/build/tools/Microsoft.TypeScript.Compiler.*/tools/tsc.exe").Last().FullPath;
    Process tscProcess = Process.Start(new ProcessStartInfo()
    {
        FileName = compilerPath,
        Arguments = projectFilePathArg,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        WorkingDirectory = $"{rootDir}/Build",
        CreateNoWindow = true
    });
            
    List<string> output = new List<string>();
    while (tscProcess.StandardOutput.Peek() > -1)
    {
        string line = tscProcess.StandardOutput.ReadLine();
        output.Add(line);
    }
    while (tscProcess.StandardError.Peek() > -1)
    {
        string line = tscProcess.StandardError.ReadLine();
        output.Add(line);
    }

    bool tscProcessResult = tscProcess.WaitForExit(30 * 1000);
    if (!tscProcessResult)
    {
        tscProcess.Kill();
        throw new TimeoutException($"Typescript compilation timeout expired.");
    }

    if (tscProcess.ExitCode != 0)
    {
        string detalisation = output.Any() ? "Additional info: " + string.Join(",", output) : "No additional info";
        throw new Exception($"Typescript compilation ended with code {tscProcess.ExitCode}." + detalisation);
    }
    Information($"Successfull compiled typescripts with arguments {projectFilePathArg}.");
}