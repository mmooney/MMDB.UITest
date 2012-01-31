using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class ClassParser
	{
		public List<CSClass> ParseString(string data)
		{
			List<CSClass> returnValue = new List<CSClass>();
			var parser = new CSharpParser();
			var compilationUnit = parser.Parse(data, "FileName.cs");
			var namespaceNodeList = compilationUnit.Children.Where(i=>i is NamespaceDeclaration);
			foreach(NamespaceDeclaration namespaceNode in namespaceNodeList)
			{
				var typeDeclarationNodeList = namespaceNode.Children.Where(i=>i is TypeDeclaration);
				foreach(TypeDeclaration typeDeclarationNode in typeDeclarationNodeList)
				{
					CSClass classObject = CSClass.Parse(namespaceNode, typeDeclarationNode, "FileName.cs");
					returnValue.Add(classObject);
				}
			}
			return returnValue;
		}
	}
}
