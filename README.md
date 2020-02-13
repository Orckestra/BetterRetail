# Orckestra Reference Application Starter Site

[![Build Status](https://orckestra001.visualstudio.com/OrckestraCommerce/_apis/build/status/Product%20extension%20-%20RefApp?branchName=master)](https://orckestra001.visualstudio.com/OrckestraCommerce/_build/latest?definitionId=68&branchName=master)

### Domains
**DEV** - http://composer-c1-{FOLDER-NAME}.develop.orckestra.cloud


### Dev Build and Deploy

#### Getting Started
* Get the latest source code from branch 'dev'

`$git clone https://github.com/Orckestra/ReferenceApplication.git`

#### Prerequisites
* Visual Studio 2019
* .NET Core 3.1 or higher. Download from https://dotnet.microsoft.com/download/dotnet-core/3.1
* IIS with URL Rewrite Module
* NPM version 6.4.1, 6.9.1 (npm -v to see version) - Must install node.js version 8.12 or 10 (this versions was tested to complete the Build)
* If you have npm installed, make sure that npm registry use default url *https://registry.npmjs.org/*
- run `npm config get registry` to check
- run `npm config set registry https://registry.npmjs.org/` to fix 
* Make sure you have installed *.develop.orckestra.local" 

#### Build Projects
* Run a Powershell Console as an Administrator `.\build\build.ps1 -t dev` to build Reference Application projects (Debug Build, without Unit Test).
 
**NOTE**: Before completing a new feature make sure to run the `.\build\build.ps1' without dev parameter to validate all Unit Tests.


#### Deploy Parameters
* Configure specific deploy parameters for your DEV environment
* Create your own file *ref.app.parameters.json* at the same level as your source code folder. Example: 
`{
  "ocs-cm-hostName": "",
  "ocsAuthToken": "",
  "adminName": "admin",
  "adminPassword": "123456"
}`

#### Deploy 
- Run PS as Administrator `.\build\install.ps1` to install Reference Application projects
- You can configure parameters for different enviroments. Create file *ref.app.parameters.{enviroment}.json*
- You can specify enviroment by adding parameter `-env={enviroment}`. Example create file  *ref.app.parameters.int2.json*  and run`.\build\install.ps1 -env=int2` 

#### Deploy Notes
 * The Deploy creates website in IIS, downloads the specified C1 CMS version from GITHUB, initializes the RefApp Starter Site.
 * The C1 CMS location configured in parameters file *~\Build\configuration\parameters.json*, parameter name `C1Url` 
 * The additional C1 CMS packages, which can be installed on website can be configured in parameters file *~\Build\configuration\SetupDescription.xml*
 * The setup description contains Experience Management packages from the *develop" branch by default. It is posible to intall packages from specific Experience Management bracnh, just run Install with parameter `-branch=master`


#### How to Debug
* Open Orckestra.ReferenceApplication.sln in Visual Studio as Administrator
* Locate **Orckestra.Composer.Website** project, right click and select "Make as StartUp Project"
* Press F5
* Yoa are Done!

**NOTE**: If debug doesn't work make sure CurrentUser has execution policy
Run in PS next commend `Set-ExecutionPolicy unrestricted -scope CurrentUser` 

 
#### Build Frontend
TODO

#### Build SASS
TODO
