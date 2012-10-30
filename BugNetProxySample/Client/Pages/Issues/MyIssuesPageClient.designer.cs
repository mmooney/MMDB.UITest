using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Issues
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Issues.MyIssues")]
	partial class MyIssuesPageClient : MMDB.UITest.Core.BasePageClient
	{
		public MyIssuesPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Projects/BugNet/src/BugNET_WAP/Issues/MyIssues.aspx";
			}
		}
	}
}
