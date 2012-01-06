using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public abstract class SourceContainer 
	{
		public List<SourceWebControl> Controls { get; set; }
		public string ClassFullName { get; set; }

		public SourceContainer()
		{
			this.Controls = new List<SourceWebControl>();
		}
	}
}
