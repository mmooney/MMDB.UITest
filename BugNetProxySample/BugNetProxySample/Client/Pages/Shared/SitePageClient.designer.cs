using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Shared
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Shared.Site")]
	partial class SitePageClient : MMDB.UITest.Core.BaseMasterPageClient
	{
		public SitePageClient (Browser browser) : base (browser)
		{
		}
	}
}
