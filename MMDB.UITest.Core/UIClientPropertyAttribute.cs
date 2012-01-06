using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Core
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class UIClientPropertyAttribute : Attribute
	{
		public string SourceFieldName { get; set; }
		public string SourceFieldTypeFullName { get; set; }
		
		public UIClientPropertyAttribute(string sourceFieldName, string sourceFieldTypeFullName)
		{
			this.SourceFieldName = sourceFieldName;
			this.SourceFieldTypeFullName = sourceFieldTypeFullName;
		}
	}
}
