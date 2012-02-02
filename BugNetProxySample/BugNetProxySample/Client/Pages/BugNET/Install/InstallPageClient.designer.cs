using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Install
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Install.Install")]
	partial class InstallPageClient : MMDB.UITest.Core.BasePageClient
	{
		public InstallPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Install/Install.aspx";
			}
		}
	}
}
