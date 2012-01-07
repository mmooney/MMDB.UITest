using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.MasterPageClean")]
	partial class MasterPageCleanPageClient : MMDB.UITest.Core.BaseMasterPageClient
	{
		public MasterPageCleanPageClient (Browser browser) : base (browser)
		{
		}
	}
}
