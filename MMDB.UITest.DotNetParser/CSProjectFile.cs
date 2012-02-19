using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using ICSharpCode.NRefactory.CSharp;
using MMDB.UITest.DotNetParser.WebForms;

namespace MMDB.UITest.DotNetParser
{
	public class CSProjectFile
	{
		public List<CSClass> ClassList { get; set; }
		public string RootNamespace { get; set; }
		public List<ClassFileDependency> ClassFileDependencyList { get; set; }
		public List<WebFormContainer> WebFormContainers { get; set; }

		public CSProjectFile()
		{
			this.ClassList = new List<CSClass>();
			this.ClassFileDependencyList = new List<ClassFileDependency>();
			this.WebFormContainers = new List<WebFormContainer>();
		}

	}
}
