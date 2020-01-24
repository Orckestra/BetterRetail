using Jurassic;
using Orckestra.Composer.HandlebarsCompiler.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using NUglify;

namespace Orckestra.Composer.HandlebarsCompiler.Services
{
    public class HandlebarsCompileService: IHandlebarsCompileService
    {
        private readonly string fileCompileMask = "*.hbs";

        private string precompileFileHeader;
        private string precompileFileTemplate;

        private ScriptEngine InitScriptEngine(string handlebarsLibraryPath)
        {
            var scriptEngine = new ScriptEngine();
            scriptEngine.ExecuteFile(handlebarsLibraryPath);
            scriptEngine.Execute(@"var precompile = Handlebars.precompile;");

            return scriptEngine;
        }

        public virtual void PrecompileHandlebarsTemplate(string compiledFile, string[] handlebarFiles)
        {
            var libraryPath = HostingEnvironment.MapPath(HandlebarsCompileConfig.HandlebarsLibraryPath);
            var scriptEngine = InitScriptEngine(libraryPath);
            GenerateCompileTemplate(HandlebarsCompileConfig.Namespace);

            try
            {
                var compiledString = precompileFileHeader;
                foreach (var file in handlebarFiles)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    var template = File.ReadAllText(file);
                    var precompileTemplate = scriptEngine.CallGlobalFunction("precompile", template).ToString();

                    compiledString += string.Format(precompileFileTemplate, name, precompileTemplate);
                }
                var minifiedString = Uglify.Js(compiledString).Code;
             
                if (!string.IsNullOrEmpty(minifiedString))
                using (var sw = new StreamWriter(compiledFile))
                {
                    sw.Write(minifiedString);
                }
            } catch (IOException)
            {

            }
        }

        public virtual void PrecompileHandlebarsTemplate(string compiledFile, string templatesPath)
        {
            var files = Directory.GetFiles(templatesPath, fileCompileMask);
            PrecompileHandlebarsTemplate(compiledFile, files);
        }

        private void GenerateCompileTemplate(string namespaceValue)
        {
            var namespaceArray = namespaceValue.Split('.').Select(x => $@"[""{x}""]");

            precompileFileHeader = Enumerable.Range(1, namespaceArray.Count()).Aggregate("", (accumulate, index) => {
                var namespaceString = string.Join("", namespaceArray.Take(index));
                return $"{accumulate} this{namespaceString} = this{namespaceString} || {{}}; ";
            });

            precompileFileTemplate = $@"this{string.Join("", namespaceArray)}[""{{0}}""] = Handlebars.template({{1}});";
        }
    }
}
