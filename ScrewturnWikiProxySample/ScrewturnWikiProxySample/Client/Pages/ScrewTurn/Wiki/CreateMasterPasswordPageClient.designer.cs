using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.CreateMasterPassword")]
	partial class CreateMasterPasswordPageClient : MMDB.UITest.Core.BasePageClient
	{
		public CreateMasterPasswordPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "CreateMasterPassword.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkMainRedirect", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkMainRedirect {
			get {
				return this.TryGetLink ("ctrl00_lnkMainRedirect");
			}
		}
	}
}
