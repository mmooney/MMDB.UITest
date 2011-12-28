using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class SourceMasterPage : SourceWebPage
	{
		public List<string> ContentHolderIDs { get; set; }

		public SourceMasterPage()
		{
			this.ContentHolderIDs = new List<string>();
		}
	}
}
