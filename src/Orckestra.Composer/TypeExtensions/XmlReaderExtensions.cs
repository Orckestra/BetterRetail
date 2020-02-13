using System;
using System.Threading.Tasks;
using System.Xml;

namespace Orckestra.Composer.TypeExtensions
{
    public static class XmlReaderExtensions
    {
        /// <summary>
        /// Async version of XmlReader ReadToFollowing
        /// from msdn https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.aspx#xmlreader_async
        /// </summary>
        public static async Task<bool> ReadToFollowingAsync(this XmlReader reader, string localName, string namespaceUri)
        {
            if (reader == null) { throw new ArgumentNullException("reader"); }
            if (reader.NameTable == null) { throw new ArgumentNullException("reader.NameTable"); }
            if (string.IsNullOrEmpty(localName)) { throw new ArgumentException("localName is empty or null"); }
            if (namespaceUri == null) { throw new ArgumentNullException("namespaceUri"); }

            // atomize local name and namespace

            localName = reader.NameTable.Add(localName);
            namespaceUri = reader.NameTable.Add(namespaceUri);

            // find element with that name 
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.NodeType == XmlNodeType.Element && 
                   (localName == reader.LocalName) && 
                   (namespaceUri == reader.NamespaceURI))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Async version of XmlReader ReadToDescendant
        /// from msdn https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.aspx#xmlreader_async
        /// </summary>
        public static async Task<bool> ReadToDescendantAsync(this XmlReader reader, string localName, string namespaceUri)
        {
            if (reader == null) { throw new ArgumentNullException("reader"); }
            if (reader.NameTable == null) { throw new ArgumentNullException("reader.NameTable"); }
            if (string.IsNullOrEmpty(localName)) { throw new ArgumentException("localName is empty or null"); }
            if (namespaceUri == null) { throw new ArgumentNullException("namespaceUri"); }
            
            // save the element or root depth 
            var parentDepth = reader.Depth;

            if (reader.NodeType != XmlNodeType.Element)
            {
                // adjust the depth if we are on root node 
                if (reader.ReadState == ReadState.Initial)
                {
                    parentDepth--;
                }
                else
                {
                    return false;
                }
            }
            else if (reader.IsEmptyElement)
            {
                return false;
            }

            // atomize local name and namespace
            localName = reader.NameTable.Add(localName);
            namespaceUri = reader.NameTable.Add(namespaceUri);

            // find the descendant 
            while (await reader.ReadAsync().ConfigureAwait(false) && reader.Depth > parentDepth)
            {
                if (reader.NodeType == XmlNodeType.Element && 
                   (localName == reader.LocalName) && 
                   (namespaceUri == reader.NamespaceURI))
                {
                    return true;
                }
            }

            return false;
        }
    }
}