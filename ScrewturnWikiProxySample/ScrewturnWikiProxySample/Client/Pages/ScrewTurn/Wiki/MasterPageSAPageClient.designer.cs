using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	partial class MasterPageSAPageClient : MMDB.UITest.Core.BasePageClient
	{
		public MasterPageSAPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "MasterPageSA.Master";
			}
		}
	}
}
