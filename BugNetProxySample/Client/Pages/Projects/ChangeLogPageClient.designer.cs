using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Projects
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Projects.ChangeLog")]
	partial class ChangeLogPageClient : MMDB.UITest.Core.BasePageClient
	{
		public ChangeLogPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Projects/BugNet/src/BugNET_WAP/Projects/ChangeLog.aspx";
			}
		}
	}
}
