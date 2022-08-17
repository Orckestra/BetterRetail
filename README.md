# Orckestra Reference Application Starter Site
[![Build Status](https://dev.azure.com/orckestra001/OrckestraCommerce/_apis/build/status/Orckestra.BetterRetail?api-version=6.0-preview.1&branchName=dev)](https://orckestra001.visualstudio.com/OrckestraCommerce/_build/latest?definitionId=297&branchName=dev)
## Table of Contents
- [Getting started](#getting-started)
- [Prerequisites](#prerequisites)
- [Certificate](#certificate)
- [Build](#build)
- [Configuring](#configuring)
- [Renaming](#renaming-the-web-site-project)
- [Deploy](#deploy)
- [Development](#development)
- [Debug](#debug)
- [Troubleshooting with logs](#Troubleshooting-with-logs)
- [Related projects](#related-projects)
- [FAQ](#faq)

## Getting started
Get the latest source code from the [dev](https://github.com/Orckestra/BetterRetail/tree/dev) branch 
- using the git command `$git clone https://github.com/Orckestra/BetterRetail.git`
- with a direct download [here](https://github.com/Orckestra/BetterRetail/archive/dev.zip)

## Prerequisites
A development environment has to include:
- [Node.js with NPM](https://nodejs.org/en/download/releases/)
	- We recommend using Node.js with the version 10.10 or higher and NPM with the version 6.4.1 or higher. If NPM is already installed, run the command `npm -v` in the command line to check the current version.
	- Be sure that the NPM registry uses the URL [https://registry.npmjs.org](https://registry.npmjs.org) as a default
		- To check the current NPM registry URL run in a command line the command `npm config get registry`
		- To set the default NPM registry URL run in a command line the command
		`npm config set registry https://registry.npmjs.org/`
- [IIS](https://www.iis.net/) with [URL Rewrite Module](https://www.iis.net/downloads/microsoft/url-rewrite)
- For debugging purposes, we recommend using [Visual Studio 2019](https://visualstudio.microsoft.com/) 
	
## Certificate
To deploy a website and use it via https protocol you must install the **.develop.orckestra.local** certificate.

## Build
To provide a full release build with passing unit tests and additional release operations, run the script in Powershell by the path **{solution_dir_path}\build\build.ps1**.
In general, the full build process includes the following steps:
- Restoring NuGet Packages;
- Compiling solution;
- Restoring NMP packages;
- Compiling typescripts;
- Running solution unit-tests;
- Running typescripts unit-tests;
- Creating artifacts;
- Creating a package

It is also possible to run a specific (separate) task of a build if you pass `-Target` param and specify a task name. For example, to run all tests again, execute in Powershell this `{solution_dir_path}\build\build.ps1 -Target Tests` command. In the table below is the list of build tasks:
| Task name | Description |
|--|--|
| All| Default task, includes full build process with unit tests and additional release operations |
|Dev|Restores NuGet packages, compiles solution, restores NPM packages, compiles typescripts and creates artifacts|
|Restore-NuGet-Packages| To restore NuGet packages|
|Compile-Solution| To compile solution source code|
|Restore-NPM-Packages|To restore NPM packages|
|Compile-Typescripts|To compile typescripts into javascript|
|NUnit-Tests|To run NUnit unit tests to check projects source code. Solution source code will be recompiled|
|Karma-Tests|To run Karma unit tests to check typescripts. Test related typescripts will be recompiled|
|Karma-Debug|To debug Karma unit tests. Be sure you have installed Google Chrome|
|Tests|To run all tests (both NUnit and Karma)
|Artifacts|To copy artifacts to the end destination|
|Package|To create a NuGet package|

### Typescripts
Typescripts compile into javascript during the build process, but for development purposes (fast to recompile typescripts changes) there are 2 possible ways to (re)compile typescripts into javascript:
- run, in PowerShell, the command:
`{solution_dir_path}\build\build.ps1 -Target Compile-Typescripts`
- to use the [Orckestra.Web.Typescript](https://github.com/Orckestra/CMS-Packages/tree/master/Orckestra.Web.Typescript) package, so any typescript changes can be automatically recompiled into javascript as defined in this package's settings.

### SASS
The Reference Application uses SASS by default. For dynamic SASS compilation into CSS [Orckestra.Web.Css.Sass](https://github.com/Orckestra/CMS-Packages/tree/master/Composite.Web.Css.Sass) package.

### Restore-NPM-Packages
If you are not authorized for the azure artifacts feed, run the command and put your username or provided username and token (for password prompt). 
```
nuget restore
```

## Configuring
### Reference application
Before deploying, you have to configure the settings of the deployed website. Configurations are stored in the **{solution_dir_path}\build\configuration** folder:

- **parameters.json** - default configuration file, contains a template of settings structure but can be used for configuring itself. Not Git ignored.
- **parameters.local.json** - this file is to be used for local deploy purposes and has a higher priority than parameters.json. Create it manually based on parameters.json. Git ignored.

In the same solution you can have a set of settings to use for different environments and different websites on demand. To have the ability to use a variety of configurations, you must add a pre-pointed keyword in a file name after the word "parameters" as demonstrated below:
- parameters.{keyword}.json
- parameters.{keyword}.local.json

For example:
- parameters.**int2**.json
- parameters.**int2**.local.json

Such configurations with keywords are to be used and have the highest priority only if deployed by passing the `-env=int2` param and argument, [more about deploying here](#deploy).

The typical configuration file has the following params:
  - `ocs-cm-hostName` - environment hostname
  - `ocsAuthToken` - environment authorization token
  - `adminName` - admin name of a deploying website
  - `adminPassword` - admin password, no less than 6 symbols
  - `adminEmail` - admin email
  - `C1Url` - URL to download C1 CMS since the Reference Application based on it and requires it during the deploy process. If the URL includes the **orckestra.local** hostname, be sure that the Orckestra VPN connection during a deployment is active.
  - `baseCulture` - a culture to be used for a deploying website
  
  Other settings usually do not have to be changed but still can be.

Settings for specific environments can be set in any json file as {"environments": {"int": { "<paramName>": "<paramValue>" }}}, but this json file should be included in specific deployments (see rules above). This will allow you to set the setting param using Azure Pipelines [JSON variable substitution](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/transforms-variable-substitution?view=azure-devops&tabs=Classic#json-variable-substitution).
  
### Additional packages
During a deployment a set of additional packages can be installed. Additional packages to be installed must be defined in the **{solution_dir_path}\build\configuration\SetupDescription.xml** file. 

## Renaming the web site project
In order to make it easier for developers to have a custom name for the web application project we have supplied an easy to user renamer.ps1 script. If for example, you would like to have you website called MySuperStore.Website, simply run <em>./renamer.ps1 MySuperstore.Website</em>.

## Deploy
To deploy the Reference Application, run in Administrator mode the Powershell script from the path `{solution_dir_path}\build\install.ps1`. 
In general, the full deployment process includes the following steps:
- Downloading C1 CMS;
- Creating and installing C1 CMS website in IIS;
- Creating https binding of the configured domain to the localhost;
- Installing the Reference Application itself and additional packages to C1 CMS

If you need to deploy using a configuration from a specific file, use the `-env={keyword}` param and argument. For example, if you want to use the configuration from the **parameters.int2.json** file, then run in Powershell the deploying command `{solution_dir}\build\install.ps1 -env=int2`. The configuration file parameters.int2.json has the highest priority in this case. 

The file **{solution_dir_path}\build\configuration\SetupDescription.xml** includes packages to be installed during the deploy.
The file contains *C1 CMS* packages, *Experience Management* packages and *Reference Application* packages, created during the Build process. 

*Experience Management* packages are installed from the **develop** branch by default. To install *Experience Management* packages from a specific branch, set `"em-branch" : "{branch_name}"` in parameters.

For *C1 CMS* package next link is used by default: *http://package.composite.net/Download.ashx?package=Orckestra.Versioning.VersionPublication&amp;c1version=$(version)* which will download **latest** package version suitable for current *C1 CMS* version. To download a specific package version, it is required to use the following link format: *https://package.composite.net:443/packages/{package-GUID}-ver-{package-Version}.zip*. 

Example:

`<package id="d665fbe2-3ca1-4c3a-b25b-79e4760e0c16" url="https://package.composite.net:443/packages/d665fbe2-3ca1-4c3a-b25b-79e4760e0c16-ver-2.0.5.zip"/>`

**NOTE**: 
For client solutions we recommend to use specific package versions, to avoid automatic updates of packages. 

The file **{solution_dir_path}\build\configuration\SetupDescriptionSecondary.xml** includes packages to be installed after. 

After successfully deploying the Reference Application it is configured and ready to use in IIS.


## Development
### Making changes to the dev branch
To make a change to the dev branch create a pull request
### Branches naming convention
Branches have to be named in the following way: {type_in_plural}/{task_number}-{description}.

For example:
- bugs/10100-header-image-size
- features/10101-tree-selector

## Debug
### Reference Application solution
- Open the **Web.config** file in the root of a deployed website, navigate to the **configuration/system.web/compilation** path, and set up `debug` attribute to `true`
- Open the solution project file **{solution_dir_path}\Orckestra.ReferenceApplication.sln** with Visual Studio in Administrator mode
- In Visual Studio select **Debug - Attach to process** and select a **w3wp** process of the intended website

### Cake files
Cake files are used to build and deploy the Reference Application. 
To debug a specific cake file:
- Restore NuGet packages: run in Powershell the command `{solution_dir_path}\build\build.ps1 -Target Restore-NuGet-Packages`
- Open the target .cake file with Visual Studio in Administrator mode and set up a breakpoint in it
- In Powershell run the cake executable by passing a path to a cake file and the debug param:
`{solution_dir_path}\build\tools\Cake\cake.exe {relative_path_to_a_cake_file} -debug`

For example, to debug **install.cake** file run a command:
`{solution_dir_path}\build\tools\Cake\cake.exe ..\build\install.cake -debug`. After execution a console window will open with a target process ID.
- Open Visual Studio in Administrator mode, then open a menu **Debug-Attach** and select the process ID from the previous console. After selection debugging starts.

### Typescripts
To debug typescripts, they should be compiled into javascript with a source map. As mentioned before, there are two default ways to compile typescripts:
- During a build process. In this case, open the **{solution_dir_path}\build\tsconfigs\orckestra.json** file and make sure that the `sourceMap` param value is true
- Using [Orckestra.Web.Typescripts](https://github.com/Orckestra/CMS-Packages/tree/master/Orckestra.Web.Typescript) package. In this case, see that this package documentation is located in the compilation config file and the `sourceMap` value is set to true. By default, it is true.

### Typescripts unit tests
To debug typescript unit tests:
- Make sure you have installed [Chrome Internet browser](https://www.google.com/chrome) 
- Run this command in Powershell:
`{solution_dir_path}\build\build.ps1 -Target Karma-Tests`. After execution, a Chrome window opens
- To run or re-run unit tests just update opened page in Chrome
- To debug typescript unit tests click on the **Debug** button in the website content area, then in the new opened tab press the **F12** key and navigate to the **Sources** tab where, in the tree, expand the **"base"** folder. Select the needed typescript unit test and set up a breakpoint.

To lint check typescripts 
- Make shure you have installed [TSLint] (https://www.npmjs.com/package/tslint)
- Run this command in Powershell: `{solution_dir_path}\build\build.ps1 -Target Tslint-Tests`.
- Run this command in Powershell: `{solution_dir_path}\build\build.ps1 -Target Tslint-Fix`. for fixes linting errors for select rules (this may overwrite linted files) 
	
## Troubleshooting with logs

There are a few tools available to view and analyze executions logs of the website.

### Windows Event Viewer

When a website is deployed on an a physical or virtual Windows machine, Windows Event Viewer will show the following types of errors:
- Website startup exceptions
- Unhandled exceptions that led to a website shutdown or a restart


### C1 CMS Logs

To reach the [C1 CMS logs](https://docs.c1.orckestra.com/Configuration/Logging). Make sure that the C1 CMS Console is available and you have administrator rights, use the following steps:
- Go to the admin section of the web site: *https://{website hostname}/Composite/top.aspx*
- Login with your username and password
- On the left side panel, click on the `System` icon <img src="https://user-images.githubusercontent.com/57723696/147662749-9933346c-bb25-49cd-9595-feccb7e19fbf.png" style="width:20px;"/>
- Click on the `Server Log` menu

If you have access to the website's files, the log files are located under **"/App_Data/Composite/LogFiles"** folder. 

Note: when the website is hosted as an Azure App Service, **App Service Editor** can be used to access the website's files.

### App Insights

To get detailed execution logs and usage statistics, you can connect the RefApp to Azure App Insights. General information about AppInsights you can read [here](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview).

#### Connecting the RefApp to Azure App Insights

If the website is deployed as an Azure App Service, you can turn on AppInsights directly inside the Azure portal.

For other types of deployments - you will need to use the AppInsights Instrumentation key. The steps are:

1. Go to the [Azure Portal](https://portal.azure.com/), reach out to your certain AppInsights service, and check the key on the main Overview Page of such service.
![image](https://user-images.githubusercontent.com/57723696/147671292-1940a612-b3a8-49a7-a1a1-cb196e9d3bd1.png)

2. After you have the AppInsights Instrumentation Key (GUID value), you have to set it up for RefApp. You can choose any of the possible options:
- to specify it in the `web.config` of the deployed website. Go to the RefApp deployment folder, open the web.config file, open the path **configuration/appSettings** and for the key `APPINSIGHTS_INSTRUMENTATIONKEY` set up the GUID of the Instrumentation key.
- to specify it for the environment if it is expected to use this AppInsights Instrumentation key all the time. To do this on the developer station, run the `cmd` with administrator rights, and execute the command `rundll32.exe sysdm.cpl,EditEnvironmentVariables`. The window with current environment variables will open. In this window, add a new system variable with the name `AppSettings_APPINSIGHTS_INSTRUMENTATIONKEY` and a value of the Instrumentation key. 

  ![image](https://user-images.githubusercontent.com/57723696/147671460-7469ef57-48a7-49d1-b487-1c6de95e7052.png).

  After this, go back to the `cmd` window and run the command `iisreset` to affect the changes. The IIS service will be restarted.
	
3. At this point, the App Insights should already be connected to the website. Visit the website via browser, and within a few minutes the website related operations should appear in App Insights.

#### RefApp-specific logs on App Insights

In addition to the default website request logging that App Insights provides the RefApp has a few customizations:

- Exceptions in C1 functions are logged in "failures" section of in App Insights

  On the `Dependencies` tab the information about C1 functions' executions will appear. 
In the `Failures` section, on `Dependencies` tab, it will be possible to see the failed C1 Functions 
![image](https://user-images.githubusercontent.com/57723696/147672007-69a6a6e2-5f4a-4caa-abbd-6e82099d6d3d.png)
If you look into samples and open some, it is possible to see a very detailed log with information, where the function failed, what exception appeared, and the details of this exception.
![image](https://user-images.githubusercontent.com/57723696/147672113-79f80d94-b66f-48ce-a052-9eff8b25d8ad.png)

- To improve the readability of the statistics, the HTTP calls to API controls have their "operation name" in App Insights changed. The overridden operation name has format "`WFE{Variation} {ControllerName}.{MethodName}`". For example, if a controller named `ControllerA` is called to use the API method named `MethodX` on the RefApp website with the CM variation, the operation name will be displayed as "`WFECM ControllerA.MethodX`".

	
  In the `Performance` section of the AppInsights service, on the `Operations` tab, it will be possible to see the operations with the new formatting. 
Also, it is possible to filter by the `WFE` role to see only the RefApp logs. ![image](https://user-images.githubusercontent.com/57723696/147671714-5374c65b-a03d-49b9-9444-27e9aebdf57e.png) 

## Related projects
The Reference Application is dependent on [C1 CMS Foundation](https://github.com/Orckestra/C1-CMS-Foundation) and can use [C1 CMS packages](https://github.com/Orckestra/CMS-Packages)

## FAQ
Q: Cannot execute PowerShell scripts because of PowerShell execution policy {TODO: set here specific error}

A: In a Powershell console in Administrator mode run the command:
`Set-ExecutionPolicy unrestricted -scope CurrentUser`
	

Q: After the deployment is completed, on the main website page, the product categories have not appeared
	
A: The deployment process installing the C1 package `Orckestra.Composer.C1.PreConfiguration`. During this package installation the product categories are creating. If you provide a deployment in a place with existing files, the categories might not be re-created because the package is already installed. There are 2 options how to handle this.
A1. You can re-create the categories. To do this, remove the `Orckestra.Composer.C1.PreConfiguration` package in the following way:
- Go to the admin section of the web site: *https://{website hostname}/Composite/top.aspx*
- Login with your username and password
- On the left side panel, click on the `System` icon <img src="https://user-images.githubusercontent.com/57723696/147662749-9933346c-bb25-49cd-9595-feccb7e19fbf.png" style="width:20px;"/>
- Find the `Packages` menu
- In this menu, go to the Installed Packages - Local Packages branch and reach the `Orckestra.Composer.C1.PreConfiguration` package
- Right click on this package and select the `Package Info` menu.
- In the opened window, click on the `Uninstall` button.
	
After the uninstallation, the website will be automatically restarted, and it will automatically re-install the `Orckestra.Composer.C1.PreConfiguration` package. During its installation the categories will appear.
	
A2. To avoid this issue at all, if in the pipeline is not cleaning up the target forder, add to the deployment pipeline a step to remove the specific `Website\App_Data\Composite\Packages` folder. Then the issue with categories will not appear.
	
{TODO: fill this section with known issues}
