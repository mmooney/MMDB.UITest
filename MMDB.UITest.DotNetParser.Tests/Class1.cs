using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MMDB.UITest.DotNetParser.Tests
{
	[TestFixture]
	public class ProjectTests
	{
		[Test]
		public void ParseProject()
		{
			Assert.Fail();
		}

		[Test]
		public void ParseClass()
		{
			CSClassParser parser = new CSClassParser();
			string classObject = "
			CSClass classObject = parser.ParseClassString();
		}
	}
}
