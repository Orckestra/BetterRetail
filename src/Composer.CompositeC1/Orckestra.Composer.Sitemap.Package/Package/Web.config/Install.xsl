<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/composer">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(sitemap[@configSource='App_Config\Sitemap.config'])=0">
				<sitemap configSource="App_Config\Sitemap.config" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/configSections/sectionGroup[@name='composer']">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(section[@type='Orckestra.Composer.Sitemap.Config.SitemapConfiguration, Orckestra.Composer.Sitemap'])=0">
				<section name="sitemap" type="Orckestra.Composer.Sitemap.Config.SitemapConfiguration, Orckestra.Composer.Sitemap" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.webServer/handlers">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@type='Orckestra.Composer.Sitemap.HttpHandlers.SitemapHttpHandler, Orckestra.Composer.Sitemap'])=0">
				<remove name="SiteMap" />
				<add name="SiteMap" verb="GET" path="sitemap.xml" type="Orckestra.Composer.Sitemap.HttpHandlers.SitemapHttpHandler, Orckestra.Composer.Sitemap" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.web/httpHandlers">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@type='Orckestra.Composer.Sitemap.HttpHandlers.SitemapHttpHandler, Orckestra.Composer.Sitemap'])=0">
				<add name="SiteMap" verb="GET" path="sitemap.xml" type="Orckestra.Composer.Sitemap.HttpHandlers.SitemapHttpHandler, Orckestra.Composer.Sitemap" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>