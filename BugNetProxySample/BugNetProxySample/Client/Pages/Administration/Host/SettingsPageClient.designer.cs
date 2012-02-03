using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Administration.Host
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Host.Settings")]
	partial class SettingsPageClient : MMDB.UITest.Core.BasePageClient
	{
		public SettingsPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Users/admin/Dropbox/Code/BugNet/src/BugNET_WAP/Administration/Host/Settings.aspx";
			}
		}
	}
}
