using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Shared
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Shared.TwoColumn")]
	partial class TwoColumnPageClient : MMDB.UITest.Core.BaseMasterPageClient
	{
		public TwoColumnPageClient (Browser browser) : base (browser)
		{
		}
	}
}
