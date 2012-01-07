using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace ScrewturnWikiProxySample.Client.Pages.ScrewTurn.Wiki
{
	[MMDB.UITest.Core.UIClientPageAttribute ("ScrewTurn.Wiki.AdminMaster")]
	partial class AdminMasterPageClient : MMDB.UITest.Core.BaseMasterPageClient
	{
		public AdminMasterPageClient (Browser browser) : base (browser)
		{
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectAdminHome", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectAdminHome {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectAdminHome");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectGroups", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectGroups {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectGroups");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectAccounts", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectAccounts {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectAccounts");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectNamespaces", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectNamespaces {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectNamespaces");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectPages", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectPages {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectPages");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectCategories", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectCategories {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectCategories");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectSnippets", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectSnippets {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectSnippets");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectNavPaths", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectNavPaths {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectNavPaths");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectContent", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectContent {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectContent");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectPluginsConfiguration", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectPluginsConfiguration {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectPluginsConfiguration");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectConfig", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectConfig {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectConfig");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectTheme", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectTheme {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectTheme");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectAdminGlobalHome", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectAdminGlobalHome {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectAdminGlobalHome");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectGlobalConfig", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectGlobalConfig {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectGlobalConfig");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectPluginsManagement", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectPluginsManagement {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectPluginsManagement");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectImportExport", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectImportExport {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectImportExport");
			}
		}
		[MMDB.UITest.Core.UIClientPropertyAttribute ("lnkSelectLog", "System.Web.UI.WebControls.HyperLink")]
		public Link lnkSelectLog {
			get {
				return this.TryGetLink ("ctrl00_lnkSelectLog");
			}
		}
	}
}
