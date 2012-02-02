using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Core;
using WatiN.Core;
namespace BugNetProxySample.Client.Pages.BugNET.Errors
{
	[MMDB.UITest.Core.UIClientPageAttribute ("BugNET.Errors.NotFound")]
	partial class NotFoundPageClient : MMDB.UITest.Core.BasePageClient
	{
		public NotFoundPageClient (Browser browser) : base (browser)
		{
		}
		protected override string ExpectedUrl {
			get {
				return "Errors/NotFound.aspx";
			}
		}
	}
}
