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
				return "C:/Projects/BugNet/src/BugNET_WAP/Administration/Admin.aspx";
			}
		}
	}
}
