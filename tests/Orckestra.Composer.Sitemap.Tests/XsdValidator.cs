using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace Orckestra.Composer.Sitemap.Tests
{
	internal static class XsdValidator
	{
		public static void ValidateXml(string xmlFilePath, string xsdResourceName, string targetNamespace)
		{
			XmlReaderSettings settings = new XmlReaderSettings();

			using (Stream xsdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xsdResourceName))
			{
				using (XmlReader xsdReader = XmlReader.Create(xsdStream))
				{	
					settings.Schemas.Add(targetNamespace, xsdReader);
					settings.ValidationType = ValidationType.Schema;
				}
			}

			using (StreamReader xmlStream = new StreamReader(xmlFilePath))
			{
				using (XmlReader xmlReader = XmlReader.Create(xmlStream, settings))
				{
					XmlDocument document = new XmlDocument();
					document.Load(xmlReader);
					ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
					document.Validate(eventHandler);
				}
			}
		}

		static void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Warning)
			{
				Assert.Warn(e.Message);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				Assert.Fail(e.Message);
			}
		}
	}
}