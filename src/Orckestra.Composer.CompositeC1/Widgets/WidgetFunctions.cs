﻿using Composite.Core;
using Orckestra.Composer.CompositeC1.Providers.MainMenu;
using System;
using System.IO;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Widgets
{
    public static class WidgetFunctions
    {
        const string THEMES_FOLDER = "UI.Package/Sass/Themes";
        public static string[] GetWebsiteThemeOptions()
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, THEMES_FOLDER);
            var dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Select(i => (new DirectoryInfo(i)).Name).ToArray();
            return dirs;
        }

        public static string[] GetMainMenuProviderNames()
        {
            return ServiceLocator.GetServices<IMainMenuItemsProvider>()?.Select(p => p.GetType().Name).ToArray();
        }
    }
}
