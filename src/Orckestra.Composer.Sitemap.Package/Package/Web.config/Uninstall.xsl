<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/composer/sitemap[@configSource='App_Config\Sitemap.config']" />
	<xsl:template match="/configuration/configSections/sectionGroup[@name='composer']/section[@type='Orckestra.Composer.Sitemap.Config.SitemapConfiguration, Orckestra.Composer.Sitemap']" />
	<xsl:template match="/configuration/system.webServer/handlers/add[@type='Orckestra.Composer.Sitemap.HttpHandlers.SitemapHttpHandler, Orckestra.Composer.Sitemap']" />
	<xsl:template match="/configuration/system.web/httpHandlers/add[@type='Orckestra.Composer.Sitemap.HttpHandlers.SitemapHttpHandler, Orckestra.Composer.Sitemap']" />
</xsl:stylesheet>