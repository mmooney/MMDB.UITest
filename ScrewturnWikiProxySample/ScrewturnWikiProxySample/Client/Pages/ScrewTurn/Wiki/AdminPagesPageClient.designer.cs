using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.AdminPages")]
	partial class AdminPagesPageClient : MMDB.UITest.Core.BasePageClient
	{
		public AdminPagesPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "AdminPages.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkDiff", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkDiff {
			get {
				return this.TryGetLink ("ctrl00_lnkDiff");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkEdit", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkEdit {
			get {
				return this.TryGetLink ("ctrl00_lnkEdit");
			}
		}
	}
}
