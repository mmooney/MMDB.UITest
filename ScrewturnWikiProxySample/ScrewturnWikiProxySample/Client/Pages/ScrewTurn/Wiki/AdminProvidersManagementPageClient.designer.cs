using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.AdminProvidersManagement")]
	partial class AdminProvidersManagementPageClient : MMDB.UITest.Core.BasePageClient
	{
		public AdminProvidersManagementPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "AdminProvidersManagement.aspx";
			}
		}
	}
}