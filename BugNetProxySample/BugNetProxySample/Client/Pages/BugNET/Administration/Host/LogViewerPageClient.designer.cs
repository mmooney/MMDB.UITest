using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Administration.Host
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Host.LogViewer")]
	partial class LogViewerPageClient : MMDB.UITest.Core.BasePageClient
	{
		public LogViewerPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Administration/Host/LogViewer.aspx";
			}
		}
	}
}
