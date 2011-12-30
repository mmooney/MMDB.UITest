using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace MMDB.UITest.Core
{
	public class BaseUserControlClient : BaseContainerClient
	{
		public string ControlPrefix { get; set; }

		public BaseUserControlClient(string controlPrefix, Browser browser) : base(browser)
		{
			this.ControlPrefix = controlPrefix;
		}

		public override string FormatControlID(string controlID)
		{
			return this.ControlPrefix + controlID;
		}
	}
}
