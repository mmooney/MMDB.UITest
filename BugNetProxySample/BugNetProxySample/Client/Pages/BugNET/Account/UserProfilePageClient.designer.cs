using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Account
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Account.UserProfile")]
	partial class UserProfilePageClient : MMDB.UITest.Core.BasePageClient
	{
		public UserProfilePageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Account/UserProfile.aspx";
			}
		}
	}
}
