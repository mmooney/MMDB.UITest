using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public abstract class SourceWebControl
	{
		public string FieldName { get; set; }
		public string TypeName { get; set; }
		public string TypeNamespace { get; set; }
	}
}
