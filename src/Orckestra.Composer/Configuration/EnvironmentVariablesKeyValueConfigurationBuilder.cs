using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.Configuration.ConfigurationBuilders;

namespace Orckestra.Composer.Configuration
{
    /// <summary>
    /// Allows configuration values to be loaded form environment variables.
    /// </summary>
    public class EnvironmentVariablesKeyValueConfigurationBuilder: KeyValueConfigBuilder
    {
        public override XmlNode ProcessRawXml(XmlNode rawXml)
        {
            if (rawXml.Name != "appSettings" 
                && (Mode == KeyValueMode.Strict || Mode == KeyValueMode.Greedy))
            {
                bool stripPrefix = (bool) typeof(KeyValueConfigBuilder)
                    .GetProperty("StripPrefix", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .GetValue(this);

                var existingNodes = rawXml.ChildNodes
                    .OfType<XmlElement>()
                    .Where(node => node.Attributes["key"] != null && node.Attributes["value"] != null)
                    .ToDictionary(_ => _.Attributes["key"].Value);

                foreach (var kvp in GetAllValues(KeyPrefix))
                {
                    var targetKey = kvp.Key;
                    if (!string.IsNullOrEmpty(KeyPrefix) && stripPrefix)
                    {
                        targetKey = targetKey.Substring(KeyPrefix.Length);
                    }

                    existingNodes.TryGetValue(targetKey, out var existingNode);

                    if (existingNode != null)
                    {
                        existingNode.Attributes["value"].Value = kvp.Value;
                    }
                    else if (Mode == KeyValueMode.Greedy)
                    {
                        var newNode = rawXml.OwnerDocument.CreateElement("add");
                        var keyAttr = rawXml.OwnerDocument.CreateAttribute("key");
                        var valueAttr = rawXml.OwnerDocument.CreateAttribute("value");
                        keyAttr.Value = targetKey;
                        valueAttr.Value = kvp.Value;

                        newNode.Attributes.Append(keyAttr);
                        newNode.Attributes.Append(valueAttr);

                        rawXml.AppendChild(newNode);
                    }
                }
            }

            return base.ProcessRawXml(rawXml);
        }

        public override string GetValue(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public override ICollection<KeyValuePair<string, string>> GetAllValues(string prefix)
        {
            var allValues = new List<KeyValuePair<string, string>>();

            var variables = Environment.GetEnvironmentVariables();
            foreach (var key in variables.Keys)
            {
                var value = variables[key];
                if (key is string keyString && value is string stringValue)
                {
                    if (!string.IsNullOrEmpty(prefix) && !keyString.StartsWith(prefix))
                    {
                        continue;
                    }

                    allValues.Add(new KeyValuePair<string, string>(keyString, stringValue));
                }
            }

            return allValues;
        }
    }
}
