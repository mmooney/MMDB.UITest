using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace <invalid>
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Feed")]
	partial class FeedPageClient : MMDB.UITest.Core.BasePageClient
	{
		public FeedPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Projects/BugNet/src/BugNET_WAP/Feed.aspx";
			}
		}
	}
}
