using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class SourceWebControl
	{
		public string FieldName { get; set; }
		public string ClassName { get; set; }
		public string NamespaceName { get; set; }

		public string ClassFullName 
		{ 
			get
			{
				return this.NamespaceName + "." + this.ClassName;
			}
		}
	}
}
