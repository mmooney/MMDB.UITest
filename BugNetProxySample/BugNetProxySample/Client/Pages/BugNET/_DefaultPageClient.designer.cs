using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET._Default")]
	partial class _DefaultPageClient : MMDB.UITest.Core.BasePageClient
	{
		public _DefaultPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Default.aspx";
			}
		}
	}
}