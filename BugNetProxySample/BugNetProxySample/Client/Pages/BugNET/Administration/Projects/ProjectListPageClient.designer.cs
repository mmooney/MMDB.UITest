using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Administration.Projects
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Projects.ProjectList")]
	partial class ProjectListPageClient : MMDB.UITest.Core.BasePageClient
	{
		public ProjectListPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Administration/Projects/ProjectList.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkNewProject", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkNewProject {
			get {
				return this.TryGetLink ("ctrl00_lnkNewProject");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkCloneProject", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkCloneProject {
			get {
				return this.TryGetLink ("ctrl00_lnkCloneProject");
			}
		}
	}
}
