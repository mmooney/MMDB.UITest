using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace MMDB.UITest.Core
{
	public abstract class BaseMasterPageClient : BaseContainerClient
	{
		public BaseMasterPageClient(Browser browser) : base(browser)
		{
		}
	}
}
