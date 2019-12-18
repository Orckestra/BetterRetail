#addin "nuget:?package=Microsoft.CodeAnalysis.CSharp&version=3.3.1"
#addin "nuget:?package=Microsoft.CodeAnalysis.Common&version=3.3.1"
#addin "nuget:?package=System.Collections.Immutable&version=1.5.0"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

public class Emitter
{
    public static string ExecuteScript(string script, string rootPath)
    {
        var code = $@"
namespace Script
{{
    public static class Main
    {{
        public static object Execute(string rootPath)
        {{
            return {script};
        }}
    }};
}}";
        var refPaths = new [] {
            typeof(object).GetTypeInfo().Assembly.Location,
            typeof(System.IO.Path).GetTypeInfo().Assembly.Location,
            typeof(System.IO.Directory).GetTypeInfo().Assembly.Location,
            typeof(System.IO.DirectoryInfo).GetTypeInfo().Assembly.Location,
            typeof(IEnumerable<>).GetTypeInfo().Assembly.Location,
            typeof(Enumerable).GetTypeInfo().Assembly.Location
        };
        var references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).Distinct().ToArray();
        
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        string assemblyName = System.IO.Path.GetRandomFileName();
        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using (var ms = new MemoryStream())
        {
            var emitResult = compilation.Emit(ms);
            if (!emitResult.Success)
            {
                var failures = emitResult.Diagnostics.Where(diagnostic => 
                    diagnostic.IsWarningAsError || 
                    diagnostic.Severity == DiagnosticSeverity.Error);

                throw new AggregateException("Failed to compile", failures.Select(x => new Exception(x.GetMessage())));
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());
                var type= assembly.GetType("Script.Main");
                var method = type.GetMember("Execute").First() as MethodInfo;
                var result = method.Invoke(null, new object[] {rootPath});
                return result?.ToString();
            }
        }
    }
};
