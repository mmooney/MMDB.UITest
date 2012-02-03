using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Administration
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Admin")]
	partial class AdminPageClient : MMDB.UITest.Core.BasePageClient
	{
		public AdminPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Users/admin/Dropbox/Code/BugNet/src/BugNET_WAP/Administration/Admin.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkProjects", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkProjects {
			get {
				return this.TryGetLink ("ctrl00_lnkProjects");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkUserAccounts", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkUserAccounts {
			get {
				return this.TryGetLink ("ctrl00_lnkUserAccounts");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkConfiguration", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkConfiguration {
			get {
				return this.TryGetLink ("ctrl00_lnkConfiguration");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkLogViewer", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkLogViewer {
			get {
				return this.TryGetLink ("ctrl00_lnkLogViewer");
			}
		}
	}
}
