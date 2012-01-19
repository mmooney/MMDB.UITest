using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;
using NUnit.Framework;

namespace Screwturn.Wiki.Proxy.Sample
{
	[TestFixture]
	public class DemoTests
	{
		[Test]
		public void Test1()
		{
			using(var browser = new IE("http://localhost:16206/"))
			{
			}
		}
	}
}
