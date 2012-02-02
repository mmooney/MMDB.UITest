using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Issues
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Issues.IssueList")]
	partial class IssueListPageClient : MMDB.UITest.Core.BasePageClient
	{
		public IssueListPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Issues/IssueList.aspx";
			}
		}
	}
}
