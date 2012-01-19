using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Account
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Account.Verify")]
	partial class VerifyPageClient : MMDB.UITest.Core.BasePageClient
	{
		public VerifyPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Account/Verify.aspx";
			}
		}
	}
}
