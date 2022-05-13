# Orckestra.Composer.C1.PreConfiguration

The package configures categories and the main menu.

There are two options OCS categories became root categories or Create one root category for OCS categories.  Configured in install.xml.

 ## Root OCS categories as root categories
 Installer should be without parameters
```xml
<mi:Add installerType="Orckestra.Composer.CompositeC1.Installers.PreConfigurationInstaller, Orckestra.Composer.C1.PreConfiguration" uninstallerType="Orckestra.Composer.CompositeC1.Installers.PreConfigurationUninstaller, Orckestra.Composer.C1.PreConfiguration" >
</mi:Add>
```

## Root OCS categories grouped by the custom main menu

Installer should have parameter with labels
```xml
<mi:Add installerType="Orckestra.Composer.CompositeC1.Installers.PreConfigurationInstaller, Orckestra.Composer.C1.PreConfiguration" uninstallerType="Orckestra.Composer.CompositeC1.Installers.PreConfigurationUninstaller, Orckestra.Composer.C1.PreConfiguration" >
  <MainMenuDisplayNames>
    <add locale="en-CA">Grocery</add>
    <add locale="fr-CA">L'épicerie</add>
  </MainMenuDisplayNames>
</mi:Add>
```

