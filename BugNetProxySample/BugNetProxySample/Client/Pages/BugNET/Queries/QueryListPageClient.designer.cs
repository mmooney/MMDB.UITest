using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Queries
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Queries.QueryList")]
	partial class QueryListPageClient : MMDB.UITest.Core.BasePageClient
	{
		public QueryListPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Queries/QueryList.aspx";
			}
		}
	}
}
