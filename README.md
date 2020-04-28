# Orckestra Reference Application Starter Site
[![Build Status](https://orckestra001.visualstudio.com/OrckestraCommerce/_apis/build/status/Product%20extension%20-%20RefApp?branchName=master)](https://orckestra001.visualstudio.com/OrckestraCommerce/_build/latest?definitionId=68&branchName=master)
## Table of Contents
- [Getting started](#getting-started)
- [Prerequisites](#prerequisites)
- [Certificate](#certificate)
- [Build](#build)
- [Configuring](#configuring)
- [Deploy](#deploy)
- [Development](#development)
- [Debug](#debug)
- [Related projects](#related-projects)
- [FAQ](#faq)

## Gettings started
Get the latest source code from the [dev](https://github.com/Orckestra/ReferenceApplication/tree/dev) branch:
- using the git command `$git clone https://github.com/Orckestra/ReferenceApplication.git`
- with a direct download [by the following link](https://github.com/Orckestra/ReferenceApplication/archive/dev.zip)

## Prerequisites
A development environment  has to include:
- [Node.js with NPM](https://nodejs.org/en/download/releases/)
	- Recommended to use Node.js with the version 10.10 or higher and NPM with the version 6.4.1 or higher. If NPM already installed, run in a command line the command `npm -v` to check the current version
	- Be sure, that NPM registry uses default [https://registry.npmjs.org](https://registry.npmjs.org) URL
		- To check current NPM registry URL run in a command line the command `npm config get registry`
		- To set default NPM registry URL run in a command line the command
		`npm config set registry https://registry.npmjs.org/`
- [IIS](https://www.iis.net/) with [URL Rewrite Module](https://www.iis.net/downloads/microsoft/url-rewrite)
- For debugging purposes recommended to use [Visual Studio 2019](https://visualstudio.microsoft.com/) 
	
## Certificate
To deploy a website and to use it via https protocol you have to install **.develop.orckestra.local** certificate.

## Build
To provide a full release build with passing unit tests and providing additional release operations run in Powershell the script by the path **{solution_dir_path}\build\build.ps1**.
In general, the full build process includes the following steps:
- Restoring NuGet Packages;
- Compiling solution;
- Restoring NMP packages;
- Compiling typescripts;
- Running solution unit-tests;
- Running typescripts unit-tests;
- Creating artifacts;
- Creating a package

It is also available to run a specific (separate) task of a build if to pass `-Target` param and to specify a task name. For example, to run again all tests execute in Powershell `{solution_dir_path}\build\build.ps1 -Target Tests` command. In the table below is the list of build tasks.
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
- to run in PowerShell a command:
`{solution_dir_path}\build\build.ps1 -Target Compile-Typescripts`
- to use [Orckestra.Web.Typescript](https://github.com/Orckestra/CMS-Packages/tree/master/Orckestra.Web.Typescript) package, so any typescripts changes can be automatically recompiled into javascript using defined in this package settings.

### SASS
The Reference Application uses SASS by default. For dynamic Saas compilation into CSS designed [Orckestra.Web.Css.Sass](https://github.com/Orckestra/CMS-Packages/tree/master/Composite.Web.Css.Sass) package.

## Configuring
### Reference application
Before deploy, you have to configure the settings of a deploying website. Configurations stored in **{solution_dir_path}\build\configuration** folder:

- **parameters.json** - default configuration file, contains a template of settings structure, but can be used for configuring itself. Not Git ignored.
- **parameters.local.json** - this file to be used for local deploy purposes and has a higher priority than parameters.json. Create  it manually based on parameters.json. Git ignored.

In the same solution you can have a set of settings to use them for different environments and different websites on demand. To have the ability to use a variety of configurations, you have to add a pre-pointed keyword in a file name after the word "parameters" as below:
- parameters.{keyword}.json
- parameters.{keyword}.local.json

For example:
- parameters.**int2**.json
- parameters.**int2**.local.json

Such configurations with keywords to be used and have the highest priority only if to deploy with passing `-env=int2` param and argument, [more about deploying here](#deploy).

The typical configuration file has the following params:
  - `ocs-cm-hostName` - environment hostname
  - `ocsAuthToken` - environment authorization token
  - `adminName` - admin name of a deploying website
  - `adminPassword` - admin password, no less than 6 symbols
  - `adminEmail` - admin email
  - `C1Url` - URL to download C1 CMS since the Reference Application based on it and needs during the deploy process. If the URL includes **orckestra.local** hostname, be sure Orckestra VPN connection during a deploying is active 
  - `baseCulture` - a culture to be used for a deploying website.
  
  Other settings usually not have to be changed but still can be.

Settings for specific environment can be set in any json file as {"environments": {"int": { "<paramName>": "<paramValue>" }}}, But this json file should be included in specific deploy (see rules above). This will allow to set setting param using Azure Pipelines [JSON variable substitution](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/transforms-variable-substitution?view=azure-devops&tabs=Classic#json-variable-substitution).
  
### Additional packages
During a deploy can be installed a set of additional packages. Additional packages to be installed defined in the **{solution_dir_path}\build\configuration\SetupDescription.xml** file. 

## Deploy
To deploy the Reference Application run in Administrator mode the Powershell script by the path `{solution_dir_path}\build\install.ps1`. 
In general, the full deploy process includes the following steps:
- Downloading C1 CMS;
- Creating and installing C1 CMS website in IIS;
- Creating https binding of the configured domain to the localhost;
- Installing the Reference Application itself and additional packages to C1 CMS

If you need to deploy using configuration from a specific file, use `-env={keyword}` param and argument. So if you want to use the configuration from the **parameters.int2.json** file, then run in Powershell the deploying command `{solution_dir}\build\install.ps1 -env=int2`. The configuration file parameters.int2.json have the highest priority in this case. 

The file **{solution_dir_path}\build\configuration\SetupDescription.xml** includes packages to be installed during deploy. These packages installing from the **develop** branch by default. To install packages from a specific Experience Management branch, use during a deploy`-branch={branch_name}` param and argument. For example, `{solution_dir}\build\install.ps1 -branch=develop`.

The file **{solution_dir_path}\build\configuration\SetupDescriptionSecondary.xml** includes packages to be installed after. 

After successfully deploying the Reference Application configured and ready to use in IIS.


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
- Open the **Web.config** file in the root of a deployed website, locate to the **configuration/system.web/compilation** path, set up `debug` attribute to `true`
- Open solution project file **{solution_dir_path}\Orckestra.ReferenceApplication.sln** with Visual Studio in Administrator mode
- In Visual Studio select **Debug - Attach to process** and select a **w3wp** process of a needed website

### Cake files
Cake uses to build and deploy the Reference Application. 
To debug a specific cake file:
- Restore NuGet packages: run in Powershell a command `{solution_dir_path}\build\build.ps1 -Target Restore-NuGet-Packages`
- Open the target .cake file with Visual Studio in Administrator mode, set up a breakpoint in it
- In Powershell run cake executable with passing a path to a cake file and a debug param:
`{solution_dir_path}\build\tools\Cake\cake.exe {relative_path_to_a_cake_file} -debug`

For example, to debug **install.cake** file run a command:
`{solution_dir_path}\build\tools\Cake\cake.exe ..\build\install.cake -debug`. After execution a console window opens with a target process ID.
- Open Visual Studio in Administrator mode, then open a menu **Debug-Attach** and select process ID from the console before. After selection a debugging starts.

### Typescripts
To debug typescripts, they should be compiled into javascript with a source map. As mentioned before, there are 2 default ways to compile typescripts:
- during a build process. In this case, open the **{solution_dir_path}\build\tsconfigs\orckestra.json** file and make sure that the `sourceMap` param value is true
- using [Orckestra.Web.Typescripts](https://github.com/Orckestra/CMS-Packages/tree/master/Orckestra.Web.Typescript) package. In this case, see this package documentation to locate to the compilation config file and to set up `sourceMap` value to true. By default, it is true.

### Typescripts unit tests
To debug typescripts unit tests:
- Make sure you have installed [Chrome Internet browser](https://www.google.com/chrome) 
- Run in Powershell a command:
`{solution_dir_path}\build\build.ps1 -Target Karma-Tests`. After execution, a Chrome window opens
- To run or re-run unit tests just update opened page in Chrome
- To debug typescripts unit tests click on the **Debug** button in the website content area, then in the new opened tab press **F12** key and then locate to the **Sources** tab where in the tree expand the **"base"** folder. Select the needed typescript unit test and set up a breakpoint.

To lint check typescripts 
- Make shure you have installed [TSLint] (https://www.npmjs.com/package/tslint)
- Run in Powershell a command: `{solution_dir_path}\build\build.ps1 -Target Tslint-Tests`.
- Run in Powershell a command: `{solution_dir_path}\build\build.ps1 -Target Tslint-Fix`. for fixes linting errors for select rules (this may overwrite linted files) 

## Related projects
Reference Application is dependent on [C1 CMS Foundation](https://github.com/Orckestra/C1-CMS-Foundation) and can use [C1 CMS packages](https://github.com/Orckestra/CMS-Packages)

## FAQ
Q: Cannot execute PowerShell scripts because of PowerShell execution policy {TODO: set here specific error}

A: In a Powershell console in Administrator mode run a command:
`Set-ExecutionPolicy unrestricted -scope CurrentUser`


{TODO: fill this section with known issues}
