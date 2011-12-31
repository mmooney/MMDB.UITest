using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class UIClientAttribute : Attribute
	{
		public string SourceClassName { get; set; }
		public string SourceClassNamespace { get; set; }

		public UIClientAttribute(string sourceClassName, string sourceClassNamespace)
		{
			this.SourceClassName = sourceClassName;
			this.SourceClassNamespace = sourceClassNamespace;
		}
	}
}
