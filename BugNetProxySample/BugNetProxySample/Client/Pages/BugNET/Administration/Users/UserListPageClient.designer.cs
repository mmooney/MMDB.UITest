using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Administration.Users
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Users.UserList")]
	partial class UserListPageClient : MMDB.UITest.Core.BasePageClient
	{
		public UserListPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Administration/Users/UserList.aspx";
			}
		}
	}
}
