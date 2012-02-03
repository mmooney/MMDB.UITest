using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Administration.Projects
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Projects.CloneProject")]
	partial class CloneProjectPageClient : MMDB.UITest.Core.BasePageClient
	{
		public CloneProjectPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Users/admin/Dropbox/Code/BugNet/src/BugNET_WAP/Administration/Projects/CloneProject.aspx";
			}
		}
	}
}
