using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;
using System.IO;

namespace MMDB.UITest.DotNetParser
{
	public class CSClass
	{
		public string ClassName { get; set; }
		public string NamespaceName { get; set; }
		public string ClassFullName 
		{
			get 
			{
				return DotNetParserHelper.BuildFullName(this.NamespaceName, this.ClassName);
			}
		}
		public EnumProtectionLevel ProtectionLevel { get; set; }
		public List<string> FilePathList { get; set; }
		public List<CSField> FieldList { get; set; }
		public List<CSProperty> PropertyList { get; set; }
		public List<CSAttribute> AttributeList { get; set; }


		public CSClass()
		{
			this.FieldList = new List<CSField>();
			this.PropertyList = new List<CSProperty>();
			this.FilePathList = new List<string>();
			this.AttributeList = new List<CSAttribute>();
		}


		public void Merge(CSClass newClass)
		{
			if(newClass.ClassFullName != this.ClassFullName)
			{
				throw new ArgumentException(string.Format("Class names do not match, expected \"{0}\", found \"{1}\"", this.ClassFullName, newClass.ClassFullName));
			}
			this.AttributeList.AddRange(newClass.AttributeList);
			this.PropertyList.AddRange(newClass.PropertyList);
			this.FieldList.AddRange(newClass.FieldList);
		}
	}
}
