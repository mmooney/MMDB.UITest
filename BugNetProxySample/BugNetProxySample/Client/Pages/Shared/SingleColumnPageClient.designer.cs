using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Shared
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Shared.SingleColumn")]
	partial class SingleColumnPageClient : MMDB.UITest.Core.BaseMasterPageClient
	{
		public SingleColumnPageClient (Browser browser) : base (browser)
		{
		}
	}
}
