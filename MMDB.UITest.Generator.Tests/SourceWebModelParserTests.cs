using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.UITest.Generator.Library;
using MMDB.UITest.DotNetParser;

namespace MMDB.UITest.Generator.Tests
{
	[TestFixture]
	class SourceWebModelParserTests
	{
		[Test]
		public void BasicTest()
		{
			SourceWebModelParser parser = new SourceWebModelParser();
			CSClass classObject = new CSClass();
			SourceWebPage page = parser.TryLoad("C:\\Test\\Test.csproj", classObject);
		}
	}
}
