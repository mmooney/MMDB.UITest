using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Errors
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Errors.Error")]
	partial class ErrorPageClient : MMDB.UITest.Core.BasePageClient
	{
		public ErrorPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Users/admin/Dropbox/Code/BugNet/src/BugNET_WAP/Errors/Error.aspx";
			}
		}
	}
}
