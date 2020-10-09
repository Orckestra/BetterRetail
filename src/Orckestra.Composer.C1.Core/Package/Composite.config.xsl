<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.Configuration.Plugins.GlobalSettingsProviderConfiguration/GlobalSettingsProviderPlugins/add/@omitAspNetWebFormsSupport">
    <xsl:attribute name="omitAspNetWebFormsSupport">true</xsl:attribute>
  </xsl:template>
</xsl:stylesheet>