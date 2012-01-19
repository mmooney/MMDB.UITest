using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.AdminHome")]
	partial class AdminHomePageClient : MMDB.UITest.Core.BasePageClient
	{
		public AdminHomePageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "AdminHome.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkPages", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkPages {
			get {
				return this.TryGetLink ("ctrl00_lnkPages");
			}
		}
	}
}
