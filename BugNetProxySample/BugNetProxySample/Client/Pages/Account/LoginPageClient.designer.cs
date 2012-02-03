using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Account
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Account.Login")]
	partial class LoginPageClient : MMDB.UITest.Core.BasePageClient
	{
		public LoginPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Users/admin/Dropbox/Code/BugNet/src/BugNET_WAP/Account/Login.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("CreateUserLink", "System.Web.UI.WebControls.HyperLink")]
		public Link CreateUserLink {
			get {
				return this.TryGetLink ("ctrl00_CreateUserLink");
			}
		}
	}
}
