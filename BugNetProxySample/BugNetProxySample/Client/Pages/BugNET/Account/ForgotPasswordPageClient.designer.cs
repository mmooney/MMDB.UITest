using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Account
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Account.ForgotPassword")]
	partial class ForgotPasswordPageClient : MMDB.UITest.Core.BasePageClient
	{
		public ForgotPasswordPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Account/ForgotPassword.aspx";
			}
		}
	}
}
