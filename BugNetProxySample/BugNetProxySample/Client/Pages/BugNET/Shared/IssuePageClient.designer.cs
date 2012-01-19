using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Shared
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Shared.Issue")]
	partial class IssuePageClient : MMDB.UITest.Core.BaseMasterPageClient
	{
		public IssuePageClient (Browser browser) : base (browser)
		{
		}
	}
}
