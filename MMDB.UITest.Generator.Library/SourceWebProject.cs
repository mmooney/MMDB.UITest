using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class SourceWebProject
	{
		public List<SourceMasterPage> MasterPageList { get; set; }
		public List<SourceWebControl> WebPageList { get; set; }

		public SourceWebProject()
		{
			this.MasterPageList = new List<SourceMasterPage>();
			this.WebPageList = new List<SourceWebControl>();
		}
	}
}
