# Orckestra Reference Application Starter Site

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

#### Deploy 
* Go to ./Installer folder
* Run PS as Administrator `.\Invoke-EnvironmentDeployment.ps1 dev full-install` to deploy Reference Application starter site


#### Deploy notes
 * The Deploy creates website in IIS, downloads the specified C1 CMS version from GITHUB, initializes the **Bare Bone** starter site and installs **Reference Application** packages as AutoInstall packages.
 * The C1 CMS version configured in parameters file *~\Installer\configs\generic\Parameters.xml*, parameter name `<param name="cms-c1-version" value="6.5" />` 
 * The additional C1 CMS packages, which can be installed on website can be configured in parameters file *~\Installer\configs\generic\Parameters.xml*, parameter name `<param name="cms-c1-custom-packages" value="Orckestra.Search.KeywordRedirect,Orckestra.Search.LuceneNET" />`
 * All C1 CMS packages are downloaded from C1 CMS packages server

