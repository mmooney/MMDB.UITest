using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;
using NUnit.Framework;

namespace MMDB.UITest.Core
{
	public abstract class BasePageClient : BaseContainerClient
	{
		public BasePageClient(Browser browser) : base(browser)
		{
			this.ValidateUrl();
		}

		public void ValidateUrl()
		{
			string absolutePath = this.Browser.Uri.AbsolutePath;
			Assert.AreEqual(this.ExpectedUrl.ToLower(), absolutePath.ToLower());
		}

		protected abstract string ExpectedUrl { get; }
	}
}
