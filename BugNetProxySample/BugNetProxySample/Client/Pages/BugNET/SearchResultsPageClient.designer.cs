using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.SearchResults")]
	partial class SearchResultsPageClient : MMDB.UITest.Core.BasePageClient
	{
		public SearchResultsPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Issues/IssueSearch.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("srchOptions", "System.Web.UI.WebControls.HyperLink")]
		public Link srchOptions {
			get {
				return this.TryGetLink ("ctrl00_srchOptions");
			}
		}
	}
}
