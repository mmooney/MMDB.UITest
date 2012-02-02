using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.SvnBrowse
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.SvnBrowse.SubversionBrowser")]
	partial class SubversionBrowserPageClient : MMDB.UITest.Core.BasePageClient
	{
		public SubversionBrowserPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "SvnBrowse/SubversionBrowser.aspx";
			}
		}
	}
}
