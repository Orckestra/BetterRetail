using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.HandlebarsCompiler.Config
{
    public class HandlebarsCompileConfig
    {
        public const string Namespace = "Orckestra.Composer.Templates";
        public const string HandlebarsLibraryPath = "~/UI.Package/JavaScript/handlebars-v4.7.6.js";
        public const string TemplatesPath = "~/UI.Package/Templates/";
        public const string CompiledFilePath = "~/UI.Package/JavaScript/composer-templates.min.js";
        public static bool IsEnabled => ConfigurationManager.AppSettings["HandelbarsCompiler"] == "true";
    }
}
