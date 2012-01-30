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
		public string NamespaceName { get; set; }
		public string ClassName { get; set; }
		public string ClassFullName 
		{
			get 
			{
				return DotNetParserHelper.BuildFullName(this.NamespaceName, this.ClassName);
			}
			set 
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.ClassName = className;
				this.NamespaceName = namespaceName;
			}
		}
		public EnumProtectionLevel ProtectionLevel { get; set; }
		public List<string> FilePathList { get; set; }
		public List<CSField> FieldList { get; set; }
		public List<CSProperty> PropertyList { get; set; }
		public List<CSAttribute> AttributeList { get; set; }
		public List<string> DependentUponFilePathList { get; set; }


		public CSClass()
		{
			this.FieldList = new List<CSField>();
			this.PropertyList = new List<CSProperty>();
			this.FilePathList = new List<string>();
			this.AttributeList = new List<CSAttribute>();
			this.DependentUponFilePathList = new List<string>();
		}

		public static CSClass Parse(NamespaceDeclaration namespaceNode, TypeDeclaration typeDefinitionNode, string filePath)
		{
			CSClass classObject = new CSClass
			{
				ClassFullName = namespaceNode.FullName + "." + typeDefinitionNode.Name
			};
			classObject.Parse(typeDefinitionNode, filePath);
			return classObject;	
		}

		public void Parse(TypeDeclaration typeDefinitionNode, string filePath)
		{
			var fieldList = typeDefinitionNode.Children.Where(i=>i is FieldDeclaration);
			foreach(FieldDeclaration fieldNode in fieldList)
			{
				var fieldObjectList = CSField.Parse(fieldNode);
				this.FieldList.AddRange(fieldObjectList);
			}
			var propertyList = typeDefinitionNode.Children.Where(i=>i is PropertyDeclaration);
			foreach(PropertyDeclaration propertyNode in propertyList)
			{
				var propertyObject = CSProperty.Parse(propertyNode);
				this.PropertyList.Add(propertyObject);
			}
			var attributeSectionList = typeDefinitionNode.Children.Where(i => i is AttributeSection);
			foreach (AttributeSection attributeSectionNode in attributeSectionList)
			{
				foreach (var attributeNode in attributeSectionNode.Attributes)
				{
					var attribute = CSAttribute.Parse(attributeNode);
					this.AttributeList.Add(attribute);
				}
			}
			if (!this.FilePathList.Contains(filePath, StringComparer.CurrentCultureIgnoreCase))
			{
				this.FilePathList.Add(filePath);
			}
		}

	}
}
