using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Projects
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Projects.ProjectSummary")]
	partial class ProjectSummaryPageClient : MMDB.UITest.Core.BasePageClient
	{
		public ProjectSummaryPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Projects/ProjectSummary.aspx";
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkRSSIssuesByCategory", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkRSSIssuesByCategory {
			get {
				return this.TryGetLink ("ctrl00_lnkRSSIssuesByCategory");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkRSSIssuesByAssignee", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkRSSIssuesByAssignee {
			get {
				return this.TryGetLink ("ctrl00_lnkRSSIssuesByAssignee");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkRSSIssuesByStatus", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkRSSIssuesByStatus {
			get {
				return this.TryGetLink ("ctrl00_lnkRSSIssuesByStatus");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkRSSIssuesByMilestone", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkRSSIssuesByMilestone {
			get {
				return this.TryGetLink ("ctrl00_lnkRSSIssuesByMilestone");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkRSSIssuesByPriority", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkRSSIssuesByPriority {
			get {
				return this.TryGetLink ("ctrl00_lnkRSSIssuesByPriority");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkRSSIssuesByType", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkRSSIssuesByType {
			get {
				return this.TryGetLink ("ctrl00_lnkRSSIssuesByType");
			}
		}
	}
}
