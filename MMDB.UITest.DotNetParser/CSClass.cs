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
		public string Namespace { get; set; }
		public string ClassName { get; set; }
		public EnumProtectionLevel ProtectionLevel { get; set; }
		public List<string> FilePathList { get; set; }
		public List<CSField> FieldList { get; set; }
		public List<CSAttribute> AttributeList { get; set; }

		public CSClass()
		{
			this.FieldList = new List<CSField>();
			this.FilePathList = new List<string>();
			this.AttributeList = new List<CSAttribute>();
		}

		public static CSClass Parse(NamespaceDeclaration namespaceNode, TypeDeclaration typeDefinitionNode, string filePath)
		{
			CSClass classObject = new CSClass
			{
				Namespace = namespaceNode.FullName,
				ClassName = typeDefinitionNode.Name
			};
			classObject.Parse(typeDefinitionNode, filePath);
			return classObject;	
		}

		public void Parse(TypeDeclaration typeDefinitionNode, string filePath)
		{
			foreach (var node in typeDefinitionNode.Children)
			{
				if (node is FieldDeclaration)
				{
					var fieldNode = (FieldDeclaration)node;
					CSField fieldObject = CSField.Parse(fieldNode);
					this.FieldList.Add(fieldObject);
				}
			}
			if(!this.FilePathList.Contains(filePath, StringComparer.CurrentCultureIgnoreCase))
			{
				this.FilePathList.Add(filePath);
			}
		}
	}
}
