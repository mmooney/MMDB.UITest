using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using System.IO;

namespace MMDB.UITest.DotNetParser
{
	public class ClassParser 
	{
		public virtual List<CSClass> ParseFile(string filePath, IEnumerable<CSClass> existingClassList = null, IEnumerable<string> dependentUponFileList = null)
		{
			if(!File.Exists(filePath))
			{
				throw new Exception("Class file does not exist: " + filePath);
			}
			string data = File.ReadAllText(filePath);
			return ParseString(data, filePath, existingClassList, dependentUponFileList);
		}

		public virtual List<CSClass> ParseString(string data, string filePath, IEnumerable<CSClass> existingClassList = null, IEnumerable<string> dependentUponFileList = null)
		{
			string fileName = Path.GetFileName(filePath);
			List<CSClass> returnValue = new List<CSClass>(existingClassList ?? new CSClass[]{} );
			var parser = new CSharpParser();
			var compilationUnit = parser.Parse(data, fileName);
			var namespaceNodeList = compilationUnit.Children.Where(i=>i is NamespaceDeclaration);
			foreach(NamespaceDeclaration namespaceNode in namespaceNodeList)
			{
				var typeDeclarationNodeList = namespaceNode.Children.Where(i=>i is TypeDeclaration);
				foreach(TypeDeclaration typeDeclarationNode in typeDeclarationNodeList)
				{
					var classObject = returnValue.SingleOrDefault(i=>i.ClassName == typeDeclarationNode.Name && i.NamespaceName == namespaceNode.FullName);
					if(classObject == null)
					{
						classObject = new CSClass
						{
							NamespaceName = namespaceNode.FullName,
							ClassName = typeDeclarationNode.Name
						};
						returnValue.Add(classObject);
					}
					ClassParser.BuildClass(classObject, typeDeclarationNode, fileName);

					if(dependentUponFileList != null)
					{
						foreach(string dependentUponFilePath in dependentUponFileList)
						{
							if(!string.IsNullOrEmpty(dependentUponFilePath))
							{
								string relativeDependentUponFilePath = Path.Combine(Path.GetDirectoryName(filePath), dependentUponFilePath);
								if (!classObject.DependentUponFilePathList.Contains(relativeDependentUponFilePath, StringComparer.CurrentCultureIgnoreCase))
								{
									classObject.DependentUponFilePathList.Add(relativeDependentUponFilePath);
								}
							}
						}
					}
				}
			}
			return returnValue;
		}

		internal static void BuildClass(CSClass classObject, TypeDeclaration typeDefinitionNode, string filePath)
		{
			var fieldList = typeDefinitionNode.Children.Where(i => i is FieldDeclaration);
			if ((typeDefinitionNode.Modifiers & Modifiers.Public) == Modifiers.Public)
			{
				classObject.ProtectionLevel = EnumProtectionLevel.Public;
			}
			else if ((typeDefinitionNode.Modifiers & Modifiers.Private) == Modifiers.Private)
			{
				classObject.ProtectionLevel = EnumProtectionLevel.Private;
			}
			else if ((typeDefinitionNode.Modifiers & Modifiers.Protected) == Modifiers.Protected)
			{
				classObject.ProtectionLevel = EnumProtectionLevel.Protected;
			}
			else if ((typeDefinitionNode.Modifiers & Modifiers.Internal) == Modifiers.Internal)
			{
				classObject.ProtectionLevel = EnumProtectionLevel.Internal;
			}
			foreach (FieldDeclaration fieldNode in fieldList)
			{
				var fieldObjectList = CSField.Parse(fieldNode);
				classObject.FieldList.AddRange(fieldObjectList);
			}
			var propertyList = typeDefinitionNode.Children.Where(i => i is PropertyDeclaration);
			foreach (PropertyDeclaration propertyNode in propertyList)
			{
				var propertyObject = CSProperty.Parse(propertyNode);
				classObject.PropertyList.Add(propertyObject);
			}
			var attributeSectionList = typeDefinitionNode.Children.Where(i => i is AttributeSection);
			foreach (AttributeSection attributeSectionNode in attributeSectionList)
			{
				foreach (var attributeNode in attributeSectionNode.Attributes)
				{
					var attribute = CSAttribute.Parse(attributeNode);
					classObject.AttributeList.Add(attribute);
				}
			}
			if (!classObject.FilePathList.Contains(filePath, StringComparer.CurrentCultureIgnoreCase))
			{
				classObject.FilePathList.Add(filePath);
			}
		}
	}
}
