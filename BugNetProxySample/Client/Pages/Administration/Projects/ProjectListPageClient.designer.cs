using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Administration.Projects
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Administration.Projects.ProjectList")]
	partial class ProjectListPageClient : MMDB.UITest.Core.BasePageClient
	{
		public ProjectListPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Projects/BugNet/src/BugNET_WAP/Administration/Projects/ProjectList.aspx";
			}
		}
	}
}
