<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:variable name="ComposerSectionGroup" xml:space="preserve">
    <sectionGroup name="composer" type="System.Configuration.ConfigurationSectionGroup, System.Configuration">
      <section name="caching" type="Orckestra.Overture.Components.Caching.CacheConfiguration, Orckestra.Caching" />
    </sectionGroup>
  </xsl:variable>

  <xsl:variable name="ConfigBuildersSection" xml:space="preserve">
    <section name="configBuilders" type="System.Configuration.ConfigurationBuildersSection, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
             restartOnExternalChanges="false" requirePermission="false" />
  </xsl:variable>

  <xsl:variable name="ComposerSection" xml:space="preserve">
  <composer>
    <caching configSource="App_Config\Caching.config" />
  </composer>
  </xsl:variable>

  <xsl:variable name="ConfigBuilders" xml:space="preserve">
  <configBuilders>
    <builders>
      <add name="EnvAppSettings" mode="Greedy" prefix="AppSettings_"  stripPrefix="true"
             type="Orckestra.Composer.Configuration.EnvironmentVariablesKeyValueConfigurationBuilder,Orckestra.Composer" />
      <add name="EnvExpManSettings" mode="Greedy" prefix="ExperienceManagement_" stripPrefix="true"
             type="Orckestra.Composer.Configuration.EnvironmentVariablesKeyValueConfigurationBuilder,Orckestra.Composer" />
    </builders>
  </configBuilders>
  </xsl:variable>

  <xsl:variable name="AppSettings" xml:space="preserve">
<appSettings configBuilders="EnvAppSettings">
  <xsl:comment> Required for SqlSink </xsl:comment>
  <add key="Environment" value="dev" />
  <add key="Hostname" value="composer-c1-cm-dev.orckestra.local" />
  <add key="Variation" value="" />
  <add key="InstrumentationKey" value="" />
  <add key="HandelbarsCompiler" value="true" />
  <add key="Composer.DefaultScope" value="Canada" />
  <add key="CC1.DeploymentToken" value="***REMOVED***" />
  <add key="UrlAlias::UseCountEnabled" value="false" />
  <add key="AutoImageResizing.ImageWidthBreakpoints" value="280, 560, 1130"/>
  <add key="AutoImageResizing.MaxWidth" value="1600" />
  <add key="AutoImageResizing.ImageFormats" value="image/webp, image/jpeg"/>
  <xsl:comment> OWIN </xsl:comment>
  <add key="owin:AutomaticAppStartup" value="false" />
</appSettings>
</xsl:variable>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>


  <xsl:template match="configuration">
    <xsl:copy xml:space="preserve">
    <xsl:apply-templates select="@*" />
    <xsl:if test="not(configSections)">
  <configSections>
    <xsl:copy-of select="$ConfigBuildersSection"/>
		<xsl:copy-of select="$ComposerSectionGroup"/>
		<sectionGroup name="experienceManagement" type="System.Configuration.ConfigurationSectionGroup, System.Configuration">
			<section name="settings" type="System.Configuration.NameValueFileSectionHandler" />
		</sectionGroup>
  </configSections>
      <xsl:copy-of select="$ConfigBuilders"/>
      <xsl:comment>Composer configuration</xsl:comment>
      <xsl:copy-of select="$ComposerSection"/>
      <xsl:copy-of select="$AppSettings"/>
    </xsl:if>
    <xsl:apply-templates select="node()" />
		<xsl:if test="not(experienceManagement)" xml:space="preserve">
				<experienceManagement>
					<settings configSource="App_Config\ExperienceManagement.config" />
				</experienceManagement>
			</xsl:if>
  </xsl:copy>
  </xsl:template>

  <xsl:template match="configuration/configSections" xml:space="preserve">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
        <xsl:copy-of select="$ConfigBuildersSection"/>
        <xsl:copy-of select="$ComposerSectionGroup"/>
        <xsl:apply-templates select="node()" />
	      <xsl:if test="not(sectionGroup[@name='experienceManagement'])">
				  <sectionGroup name="experienceManagement" type="System.Configuration.ConfigurationSectionGroup, System.Configuration">
					  <section name="settings" type="System.Configuration.NameValueFileSectionHandler" />
				  </sectionGroup>
			  </xsl:if>
   </xsl:copy>
    <xsl:copy-of select="$ConfigBuilders"/>
    <xsl:comment>Composer configuration</xsl:comment>
    <xsl:copy-of select="$ComposerSection"/>
    <xsl:copy-of select="$AppSettings"/>

  </xsl:template>

  <xsl:template match="configuration/system.web" xml:space="preserve">
    <xsl:copy><xsl:apply-templates select="@*" />
    <httpCookies httpOnlyCookies="true" requireSSL="true" />
    <authentication mode="Forms">
      <forms loginUrl="login" name=".AUTH" cookieless="UseCookies" requireSSL="true" timeout="131760" />
    </authentication>
    <membership defaultProvider="composer">
      <providers>
        <add name="composer" type="Orckestra.Composer.Providers.Membership.OvertureMembershipProvider, Orckestra.Composer" />
      </providers>
    </membership>
    <httpModules>
      <add name="TelemetryCorrelationHttpModule" type="Microsoft.AspNet.TelemetryCorrelation.TelemetryCorrelationHttpModule, Microsoft.AspNet.TelemetryCorrelation" />
      <add name="ApplicationInsightsWebTracking" type="Microsoft.ApplicationInsights.Web.ApplicationInsightsHttpModule, Microsoft.AI.Web" />
    </httpModules>
<xsl:apply-templates select="node()" /></xsl:copy>
	</xsl:template>

  <!-- Setting output cache duration to 15 minutes -->
  <xsl:template match="configuration/system.web/caching/outputCacheSettings/outputCacheProfiles/add[@name='C1Page']" xml:space="preserve">
          <xsl:comment> 15 minutes Output cache </xsl:comment>
          <add name="C1Page" enabled="true" duration="900" varyByCustom="C1Page" varyByParam="*" location="Any" />
	</xsl:template>

  <xsl:template match="configuration/system.webServer/modules/add[@name='UrlRoutingModule']" xml:space="preserve">
        <add name="UrlRewriteModule" type="Orckestra.Composer.CompositeC1.UrlRewriteModule, Orckestra.Composer.CompositeC1" />
       <add name="AntiCookieTamperingModule" type="Orckestra.Composer.HttpModules.AntiCookieTamperingModule, Orckestra.Composer" />    
       <xsl:copy><xsl:apply-templates select="@* | node()"/></xsl:copy>
       <remove name="WebDAVModule" />
</xsl:template>

  <xsl:template match="configuration/system.webServer/handlers">
    <xsl:copy xml:space="preserve"><xsl:apply-templates select="@* | node()"/>
      <remove name="WebDAV" />
      <remove name="ExtensionlessUrlHandler-Integrated-4.0" />
      <remove name="OPTIONSVerbHandler" />
      <remove name="TRACEVerbHandler" />
      <add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="*" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />
</xsl:copy>
  </xsl:template>

  <xsl:template match="configuration/system.webServer">
    <xsl:copy xml:space="preserve"><xsl:apply-templates select="@* | node()"/>
    <httpProtocol>
      <customHeaders>
        <add name="X-UA-Compatible" value="IE=edge" />
        <add name="X-Frame-Options" value="SAMEORIGIN" />
      </customHeaders>
    </httpProtocol>
    <rewrite>
      <rules>
        <rule name="HTTP/S to HTTPS Redirect" enabled="true" stopProcessing="true">
          <match url="(.*)" />
          <conditions>
            <add input="{{HTTPS}}" pattern="off" />
          </conditions>
          <action type="Redirect" url="https://{{HTTP_HOST}}{{REQUEST_URI}}" redirectType="Found" />
        </rule>
      </rules>
      <outboundRules>
        <rule name="Add Strict-Transport-Security when HTTPS" enabled="true">
          <match serverVariable="RESPONSE_Strict_Transport_Security" pattern=".*" />
          <conditions>
            <add input="{{HTTPS}}" pattern="on" ignoreCase="true" />
          </conditions>
          <action type="Rewrite" value="max-age=31536000" />
        </rule>
      </outboundRules>
    </rewrite></xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/system.webServer/modules">
    <xsl:copy>
      <xsl:apply-templates select="@*" />

      <xsl:if test="not(add[@name='ContextPreservationHttpModule'])">
        <add name="ContextPreservationHttpModule" type="Orckestra.Composer.CompositeC1.ContextPreservationHttpModule, Orckestra.Composer.CompositeC1" />
      </xsl:if>

      <xsl:apply-templates select="node()" />

      <xsl:if test="not(add[@name='ComposerRequestInterceptor'])">
        <add name="ComposerRequestInterceptor" type="Orckestra.Composer.CompositeC1.RequestInterceptorHttpModule, Orckestra.Composer.CompositeC1" />
      </xsl:if>

      <xsl:if test="not(add[@name='TelemetryCorrelationHttpModule'])">
        <remove name="TelemetryCorrelationHttpModule" />
        <add name="TelemetryCorrelationHttpModule" type="Microsoft.AspNet.TelemetryCorrelation.TelemetryCorrelationHttpModule, Microsoft.AspNet.TelemetryCorrelation" preCondition="managedHandler" />
      </xsl:if>

      <xsl:if test="not(add[@name='ApplicationInsightsWebTracking'])">
        <remove name="ApplicationInsightsWebTracking" />
        <add name="ApplicationInsightsWebTracking" type="Microsoft.ApplicationInsights.Web.ApplicationInsightsHttpModule, Microsoft.AI.Web" preCondition="managedHandler" />
      </xsl:if>

    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>
