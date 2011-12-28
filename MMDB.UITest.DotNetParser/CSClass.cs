using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class CSClass
	{
		public string Namespace { get; set; }
		public string ClassName { get; set; }
		public EnumProtectionLevel ProtectionLevel { get; set; }

		//public List<int> Properties;
		public List<CSField> FieldList { get; set; }
		//public List<string> Functions;

		public CSClass()
		{
			this.FieldList = new List<CSField>();
		}

		public static CSClass Parse(NamespaceDeclaration namespaceNode, TypeDeclaration typeDefinitionNode)
		{
			CSClass classObject = new CSClass
			{
				Namespace = namespaceNode.FullName,
				ClassName = typeDefinitionNode.Name
			};
			foreach (var node in typeDefinitionNode.Children)
			{
				if (node is FieldDeclaration)
				{
					var fieldNode = (FieldDeclaration)node;
					CSField fieldObject = CSField.Parse(fieldNode);
					classObject.FieldList.Add(fieldObject);
				}
			}
			return classObject;	
		}
	}
}
