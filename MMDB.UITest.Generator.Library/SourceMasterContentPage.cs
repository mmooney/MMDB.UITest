using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class SourceMasterContentPage : SourceWebPage
	{
		public SourceMasterPage MasterPage { get; set; }
		public Dictionary<string, SourceContainer> ContentHolders { get; set; }
	}
}
