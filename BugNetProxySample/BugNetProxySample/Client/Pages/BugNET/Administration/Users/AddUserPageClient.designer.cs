using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Administration.Users
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Users.AddUser")]
	partial class AddUserPageClient : MMDB.UITest.Core.BasePageClient
	{
		public AddUserPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Administration/Users/AddUser.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("ReturnLink", "System.Web.UI.WebControls.HyperLink")]
		public Link ReturnLink {
			get {
				return this.TryGetLink ("ctrl00_ReturnLink");
			}
		}
	}
}
