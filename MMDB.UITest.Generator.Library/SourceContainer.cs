using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;

namespace MMDB.UITest.Generator.Library
{
	public abstract class SourceContainer 
	{
		public abstract EnumSourceObjectType SourceObjectType { get; }
		public List<SourceWebControl> Controls { get; set; }
		public string ClassName { get; set; }
		public string NamespaceName { get; set; }
		public string ClassFullName
		{
			get 
			{
				return DotNetParserHelper.BuildFullName(this.NamespaceName, this.ClassName);
			}
			set 
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.ClassName = className;
				this.NamespaceName = namespaceName;
			}
		}

		public SourceContainer()
		{
			this.Controls = new List<SourceWebControl>();
		}
	}
}
