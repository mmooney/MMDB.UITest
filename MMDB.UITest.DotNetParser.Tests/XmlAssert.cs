using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace MMDB.UITest.DotNetParser.Tests
{
	public static class XmlAssert
	{
		public static void AreEqual(XElement expected, XElement actual)
		{
			Assert.AreEqual(
				Normalize(expected).ToString(),
				Normalize(actual).ToString());
		}

		private static XElement Normalize(XElement element)
		{
			if (element.HasElements)
			{
				return new XElement(
					element.Name,
					element.Attributes()
						   .OrderBy(a => a.Name.ToString()),
					element.Elements()
						   .OrderBy(a => a.Name.ToString())
						   .Select(e => Normalize(e)));
			}

			if (element.IsEmpty)
			{
				return new XElement(
					element.Name,
					element.Attributes()
						   .OrderBy(a => a.Name.ToString()));
			}

			return new XElement(element.Name, element.Attributes()
				.OrderBy(a => a.Name.ToString()), element.Value);
		}
	}
}
