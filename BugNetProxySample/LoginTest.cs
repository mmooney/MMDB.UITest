﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WatiN.Core;

namespace BugNetProxySample
{
	[TestFixture]
	public class LoginTest 
	{
		[Test]
		public void TestLogin()
		{
			using(var browser = new IE("http://localhost:59847/Default.aspx"))
			{
			}
		}
	}
}
