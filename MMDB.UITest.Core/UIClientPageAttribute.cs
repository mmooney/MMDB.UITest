using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class UIClientPageAttribute : Attribute
	{
		public string SourceClassFullName { get; set; }
		//public IEnumerable<string> ExpectedUrlList { get; set; }

		public UIClientPageAttribute(string sourceClassFullName/*, IEnumerable<string> expectedUrlList=null*/)
		{
			this.SourceClassFullName = sourceClassFullName;
			//this.ExpectedUrlList = expectedUrlList;
		}
	}
}
