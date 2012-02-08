using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class CSProjectFile
	{
		public List<CSClass> ClassList { get; set; }
		public string RootNamespace { get; set; }
		public List<ClassFileDependency> ClassFileDependencyList { get; set; }

		public CSProjectFile()
		{
			this.ClassList = new List<CSClass>();
			this.ClassFileDependencyList = new List<ClassFileDependency>();
		}

	}
}
