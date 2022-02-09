<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" indent="yes"/>
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.Configuration.Plugins.GlobalSettingsProviderConfiguration/GlobalSettingsProviderPlugins/add/@omitAspNetWebFormsSupport">
    <xsl:attribute name="omitAspNetWebFormsSupport">true</xsl:attribute>
  </xsl:template>

  <xsl:template match="configuration/loggingConfiguration/listeners">
    <xsl:copy>
        <xsl:apply-templates select="@* | node()" />
      <xsl:if test="not(add[@name='AppInsightsListener'])">
        <add name="AppInsightsListener" listenerDataType="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.CustomTraceListenerData, Microsoft.Practices.EnterpriseLibrary.Logging, Version=3.0.0.0" traceOutputOptions="None" type="Orckestra.Composer.Grocery.Website.App_Insights.AppInsightsListener, Orckestra.Composer.Grocery.Website" initializeData="" formatter="Text Formatter" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

    <xsl:template match="configuration/loggingConfiguration/specialSources/allEvents/listeners">
    <xsl:copy>
    <xsl:apply-templates select="@* | node()" />
      <xsl:if test="not(add[@name='AppInsightsListener'])">
        <add name="AppInsightsListener" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>