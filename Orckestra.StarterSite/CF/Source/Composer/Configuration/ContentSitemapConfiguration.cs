using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Configuration
{
    public class ContentSitemapConfiguration : ConfigurationElement
    {
        internal const string ConfigurationName = "contentSitemap";

        const string PageIdsToExcludeKey = "pageIdsToExclude";
        [ConfigurationProperty(PageIdsToExcludeKey, IsRequired = true)]
        [ConfigurationCollection(typeof(ContentSitemapPageIdsToExcludeCollection))]
        public ContentSitemapPageIdsToExcludeCollection PageIdsToExclude
        {
            get { return (ContentSitemapPageIdsToExcludeCollection)this[PageIdsToExcludeKey]; }            
        }

        const string TypesToExcludeKey = "typesToExclude";
        [ConfigurationProperty(TypesToExcludeKey, IsRequired = false)]
        [ConfigurationCollection(typeof(ContentSitemapPageIdsToExcludeCollection))]
        public ContentSitemapTypesToExcludeCollection TypesToExclude
        {
            get { return (ContentSitemapTypesToExcludeCollection)this[TypesToExcludeKey]; }
        }

    }

    // Based on the MSDN documentation
    // See examples sections
    // Source: https://msdn.microsoft.com/en-us/library/system.configuration.configurationelementcollection%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
    public class ContentSitemapPageIdsToExcludeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ContentSitemapPageToExcludeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ContentSitemapPageToExcludeElement)element).PageId;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap; 
            }
        }
    }

    public class ContentSitemapPageToExcludeElement : ConfigurationElement
    {
        const string pageIdKey = "id";
        [ConfigurationProperty(pageIdKey, IsRequired = true, IsKey = true)]
        public string PageId
        {
            get { return (string)this[pageIdKey]; }
            set { this[pageIdKey] = value; }
        }
    }

    public class ContentSitemapTypesToExcludeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ContentSitemapTypeToExcludeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ContentSitemapTypeToExcludeElement)element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
    }

    public class ContentSitemapTypeToExcludeElement : ConfigurationElement
    {
        const string typeKey = "name";
        [ConfigurationProperty(typeKey, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[typeKey]; }
            set { this[typeKey] = value; }
        }
    }
}
