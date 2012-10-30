using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.Errors
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Errors.AccessDenied")]
	partial class AccessDeniedPageClient : MMDB.UITest.Core.BasePageClient
	{
		public AccessDeniedPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "C:/Projects/BugNet/src/BugNET_WAP/Errors/AccessDenied.aspx";
			}
		}
	}
}