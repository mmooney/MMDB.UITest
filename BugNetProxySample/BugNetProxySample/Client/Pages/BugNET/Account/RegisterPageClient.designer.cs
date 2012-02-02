using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Account
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Account.Register")]
	partial class RegisterPageClient : MMDB.UITest.Core.BasePageClient
	{
		public RegisterPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Account/Register.aspx";
			}
		}
	}
}