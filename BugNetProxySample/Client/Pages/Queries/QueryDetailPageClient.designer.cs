using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Queries
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Queries.QueryDetail")]
	partial class QueryDetailPageClient : MMDB.UITest.Core.BasePageClient
	{
		public QueryDetailPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Projects/BugNet/src/BugNET_WAP/Queries/QueryDetail.aspx";
			}
		}
	}
}
