using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Administration.Users
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Users.EditUser")]
	partial class EditUserPageClient : MMDB.UITest.Core.BasePageClient
	{
		public EditUserPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Administration/Users/EditUser.aspx";
			}
		}
	}
}
