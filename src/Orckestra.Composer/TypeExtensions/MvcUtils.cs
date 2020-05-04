using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Fasterflect;

namespace Orckestra.Composer.TypeExtensions
{
    public static class MvcUtils
    {
        private const string ControllerNamePattern = "^(\\w+)Controller$";
        private static readonly Regex ControllerNameRegex;

        static MvcUtils()
        {
            ControllerNameRegex = new Regex(ControllerNamePattern);
        }

        /// <summary>
        /// Extracts the controller types in the specified assembly.
        /// </summary>
        /// <param name="assemblyToCrawl">Assembly to crawl for Controllers.</param>
        /// <returns>Enumeration of controllers discovered. If none found, will return an empty enumeration.</returns>
        public static IEnumerable<Type> GetControllerTypes(_Assembly assemblyToCrawl)
        {
            if (assemblyToCrawl == null) { throw new ArgumentNullException(nameof(assemblyToCrawl)); }

            var controllerTypes =
                assemblyToCrawl.GetTypes().Where(t => t.Inherits<Controller>() && t.IsPublic && !t.IsAbstract && ControllerNameRegex.IsMatch(t.Name));

            return controllerTypes;
        }

        /// <summary>
        /// Gets the action infos in the specified controller type.
        /// </summary>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetActionInfos(Type controllerType)
        {
            if (controllerType == null) { throw new ArgumentNullException(nameof(controllerType)); }

            var actionInfos = controllerType
                .GetMethods(Flags.InstancePublic)
                .Where(m => m.HasAttribute<RouteAttribute>() && !m.HasAttribute<NonActionAttribute>());

            return actionInfos;
        }

        public static string ExtractControllerName(string typeName)
        {
            var match = ControllerNameRegex.Match(typeName);
            var controllerName = match.Groups[1].Value;
            return string.IsNullOrWhiteSpace(controllerName) ? typeName : controllerName;
        }
    }
}
