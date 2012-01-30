using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using System.IO;

namespace MMDB.UITest.DotNetParser
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void ParseBasicClass()
		{
			ClassParser parser = new ClassParser();
			StringBuilder sb = new StringBuilder();

			string data = 
			@"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using NUnit.Framework;
				using Moq;
				using System.IO;

				namespace Test.Namesapce
				{
					[TestFixture]
					public class TestClass
					{
						[Test]
						public void ParseBasicClass()
						{
						}
					}
				}
			";

			CSClass classObject = parser.ParseClassString(data);
			Assert.AreEqual("TestClass",classObject.ClassName);
			Assert.AreEqual("Test.Namespace",classObject.NamespaceName);
			Assert.AreEqual("Test.Namespace.TestClass", classObject.ClassFullName);
		}
	}
}
