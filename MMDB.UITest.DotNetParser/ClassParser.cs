using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class ClassParser
	{
		public CSClass ParseClassString(string data)
		{
			var tree = SyntaxTree.ParseCompilationUnit(data);
			var classNodes = tree.Root.DescendentNodes(i=>i.Kind == SyntaxKind.ClassDeclaration);
			throw new NotImplementedException();
		}
	}
}
