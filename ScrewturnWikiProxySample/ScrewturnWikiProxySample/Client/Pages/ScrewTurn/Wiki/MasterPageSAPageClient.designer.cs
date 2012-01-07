using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.MasterPageSA")]
	partial class MasterPageSAPageClient : MMDB.UITest.Core.BaseMasterPageClient
	{
		public MasterPageSAPageClient (Browser browser) : base (browser)
		{
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkPreviousPage", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkPreviousPage {
			get {
				return this.TryGetLink ("ctrl00_lnkPreviousPage");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkMainPage", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkMainPage {
			get {
				return this.TryGetLink ("ctrl00_lnkMainPage");
			}
		}
	}
}
