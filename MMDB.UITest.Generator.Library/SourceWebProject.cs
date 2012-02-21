using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class SourceWebProject
	{
		public List<SourceMasterPage> MasterPageList { get; set; }
		public List<SourceWebPage> WebPageList { get; set; }
		public List<SourceUserControl> UserControlList { get; set; }
		public string RootNamespace { get; set; }

		public SourceWebProject()
		{
			this.MasterPageList = new List<SourceMasterPage>();
			this.WebPageList = new List<SourceWebPage>();
			this.UserControlList = new List<SourceUserControl>();
		}
	}
}
