using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.AllPages")]
	partial class AllPagesPageClient : MMDB.UITest.Core.BasePageClient
	{
		public AllPagesPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "AllPages.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkCategories", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkCategories {
			get {
				return this.TryGetLink ("ctrl00_lnkCategories");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSearch", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSearch {
			get {
				return this.TryGetLink ("ctrl00_lnkSearch");
			}
		}
	}
}
