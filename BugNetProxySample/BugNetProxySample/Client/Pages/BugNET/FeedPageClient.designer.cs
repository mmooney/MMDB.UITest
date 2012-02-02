using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Feed")]
	partial class FeedPageClient : MMDB.UITest.Core.BasePageClient
	{
		public FeedPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Feed.aspx";
			}
		}
	}
}
