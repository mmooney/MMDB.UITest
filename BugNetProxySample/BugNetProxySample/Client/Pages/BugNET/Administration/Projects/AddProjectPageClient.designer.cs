using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Administration.Projects
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Projects.AddProject")]
	partial class AddProjectPageClient : MMDB.UITest.Core.BasePageClient
	{
		public AddProjectPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Administration/Projects/AddProject.aspx";
			}
		}
	}
}
