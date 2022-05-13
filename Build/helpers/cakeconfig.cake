#addin "nuget:?package=Microsoft.Extensions.Configuration&version=3.1.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.Abstractions&version=3.1.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.FileExtensions&version=3.1.0"
#addin "nuget:?package=Microsoft.Extensions.FileProviders.Abstractions&version=3.1.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.Json&version=3.1.0"
#addin "nuget:?package=Microsoft.Extensions.Primitives&version=3.1.0"
#addin "nuget:?package=Microsoft.Extensions.FileProviders.Physical&version=3.1.0"
#addin "nuget:?package=System.Text.Json&version=4.7.0"
#addin "nuget:?package=Microsoft.Bcl.AsyncInterfaces&version=1.0.0"
#addin "nuget:?package=System.Threading.Tasks.Extensions&version=4.5.2"


#load "emitter.cake"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cake.Core;
using Cake.Core.Diagnostics;
using Microsoft.Extensions.Configuration;

public class CakeConfig : ICakeConfig
{
    private readonly ICakeContext _context;
    private readonly ConfigurationBuilder _builder = new ConfigurationBuilder();
    private string _rootPath;
    private readonly string _environment;
    
    public CakeConfig(ICakeContext context, string environment = null)
    {
        _context = context;
        _environment = environment;
    }

    public void AddFile(string filePath)
    {
        _builder.AddJsonFile(filePath, true, false);
    }

    public void SetRootPath(string path)
    {
        _rootPath = path;
    }

    public Dictionary<string, string> GetConfig()
    {
        var config = _builder.Build();
        var result = Flatten(config);
        result = SubstituteExpressions(result);
        result = SubstituteVariables(result);

        return result;
    }

    private Dictionary<string, string> Flatten(IConfigurationRoot config)
    {
        var rootProperties = config.GetChildren().Where(x => x.Key != "environments").ToList();

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in rootProperties)
            result[prop.Key] = prop.Value;


        var envProperties = config.GetChildren().Where(x => x.Key == "environments").Select(
          x => x.GetChildren().Where(d => d.Key == _environment).Select(d => d.GetChildren()).FirstOrDefault()
        ).FirstOrDefault();

        foreach (var prop in envProperties ?? Enumerable.Empty<IConfigurationSection>())
            result[prop.Key] = prop.Value;

        return result;
    }

    private Dictionary<string, string> SubstituteExpressions(Dictionary<string, string> config)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvPair in config)
        {
            var match = Regex.Match(kvPair.Value, "{{(?<expression>.+)}}");
            if (match.Success)
            {
                var expression = match.Groups["expression"].Value;

                try
                {
                    var value = Emitter.ExecuteScript(expression, _rootPath);
                    result[kvPair.Key] = value;
                }
                catch (AggregateException ex)
                {
                    _context?.Log.Error($"Expression can't be compiled");
                    _context?.Log.Error($"'{expression}'");
                    foreach (var exception in ex.InnerExceptions)
                    {
                        _context?.Log.Error($"{exception.Message}");    
                    }
                    
                    result[kvPair.Key] = kvPair.Value;
                }
            }
            else
            {
                result[kvPair.Key] = kvPair.Value;
            }
        }

        return result;
    }

    private Dictionary<string, string> SubstituteVariables(Dictionary<string, string> config)
    {
        var result = new Dictionary<string, string>(config, StringComparer.OrdinalIgnoreCase);
        foreach (var key in result.Keys.ToList())
        {
            bool finished = false;
            for (int i = 0; i < 10; i++)
            {
                var value = result[key];
                var match = Regex.Match(value, @"{(?<expression>.+)}");
                if (match.Success)
                {
                    var expression = match.Groups["expression"].Value;
                    if (result.TryGetValue(expression, out var substituteValue))
                    {
                        var newValue = value.Replace($"{{{expression}}}", substituteValue);
                        result[key] = newValue;
                    }
                    else
                    {
                        _context?.Log.Error($"Can't find substitution for {{{expression}}}");
                        finished = true;
                        break;
                    }
                }
                else
                {
                    finished = true;
                    break;
                }
            }

            if (!finished)
            {
                _context?.Log.Error($"Recursive substitutions detected in '{key}'");
            }
        }

        return result;
    }
};

public ICakeConfig CreateCakeConfig(string environment = null)
{
    return new CakeConfig(Context, environment);
}

public interface ICakeConfig
{
    void AddFile(string filePath);
    void SetRootPath(string path);
    Dictionary<string, string> GetConfig();
};

public static ICakeConfig UseFile(this ICakeConfig cakeConfig, string filePath)
{
    cakeConfig.AddFile(filePath);
    return cakeConfig;
}

public static ICakeConfig UseRootPath(this ICakeConfig cakeConfig, string path)
{
    cakeConfig.SetRootPath(path);
    return cakeConfig;
}


public static Dictionary<string, string> Build(this ICakeConfig cakeConfig)
{
    return cakeConfig.GetConfig();
}