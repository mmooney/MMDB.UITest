using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace <invalid>
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.SearchResults")]
	partial class SearchResultsPageClient : MMDB.UITest.Core.BasePageClient
	{
		public SearchResultsPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Projects/BugNet/src/BugNET_WAP/Issues/IssueSearch.aspx";
			}
		}
	}
}
