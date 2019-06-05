# Orckestra Reference Application Starter Site

[![Build Status](https://orckestra001.visualstudio.com/OrckestraCommerce/_apis/build/status/Product%20extension%20-%20RefApp?branchName=master)](https://orckestra001.visualstudio.com/OrckestraCommerce/_build/latest?definitionId=68&branchName=master)

### Domains
**DEV** - http://composer-c1-cm-dev.develop.orckestra.cloud


### Local Build and Deploy

#### Getting Started
* Get latest source code from branch 'master'

`$git clone https://github.com/Orckestra/ReferenceApplication.git`

#### Prerequesites
* IIS with URL Rewrite Module
* NPM version 5.4.2 or 5.6.0 (npm -v to see version) - Must install node.js version 6.14.4 or 8.11.3 (this versions was tested to complete the Build)
* If you have npm installed, make sure that npm registry use default url *https://registry.npmjs.org/*
- run `npm config get registry` to check
- run `npm config set registry https://registry.npmjs.org/` to fix 

#### Build Projects
* Go to *./Build folder*
* Run PS as Administrator `.\Build.ps1 -t all` to build Reference Application projects

#### Deploy Parameters
* Configure specific deploy parameters for your DEV enviroment
* Go to *./Installer/configs/specific/DEV* folder
* Create you own file *Parameters.Dev.xml*  and specify deploy parameters. See example below

```<?xml version="1.0" encoding="utf-8"?>
<parameters>
  <param name="environment_suffix" value="dev"/>
 
  <param name="composer_apppool_username" value="NA"/>
  <param name="composer_apppool_password" value="NA"/>
  <param name="composer_apppool_identitytype" value="ApplicationPoolIdentity"/>
  
  <param name="machineKey-validationKey" value="***REMOVED***" />
  <param name="machineKey-decryptionKey" value="***REMOVED***" />
  
  <param name="ocs-cm-hostName" value="ENTER_VALUE_HEREd"/>
  <param name="ocs-cd-hostName" value="ENTER_VALUE_HERE"/>
  <param name="ocsAuthToken" value="ENTER_VALUE_HERE"/>
	
  <param name="gtm-containerid" value="ENTER_VALUE_HERE"/>
</parameters>
```

#### Deploy 
* Go to ./Installer folder
* Run PS as Administrator `.\Invoke-EnvironmentDeployment.ps1 dev full-install` to deploy Reference Application Starter Site


#### Deploy notes
 * The Deploy creates website in IIS, downloads the specified C1 CMS version from GITHUB, initializes the **Bare Bone** starter site and installs **Reference Application** packages as AutoInstall packages.
 * The C1 CMS version configured in parameters file *~\Installer\configs\generic\Parameters.xml*, parameter name `<param name="cms-c1-version" value="6.5" />` 
 * The additional C1 CMS packages, which can be installed on website can be configured in parameters file *~\Installer\configs\generic\Parameters.xml*, parameter name `<param name="cms-c1-custom-packages" value="Orckestra.Search.KeywordRedirect,Orckestra.Search.LuceneNET" />`
 * All C1 CMS packages are downloaded from C1 CMS packages server

#### Deploy local changes 
* Go to .\Orckestra.StarterSite\cf\Source
* Build local dll with VisualStudio
* Run PS as Administrator 'gulp devPackage' to deploy composer Dlls and front end files
