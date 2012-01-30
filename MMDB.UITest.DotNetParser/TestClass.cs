using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using System.IO;

namespace MMDB.UITest.DotNetParser
{
	public class TestClass
	{
		public static string Test()
		{
			ClassParser parser = new ClassParser();
			StringBuilder sb = new StringBuilder();

			string data =
			@"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using NUnit.Framework;
				using Moq;
				using System.IO;

				namespace Test.Namesapce
				{
					[TestFixture]
					public class TestClass
					{
						[Test]
						public void ParseBasicClass()
						{
						}
					}
				}
			";

			SyntaxTree tree = SyntaxTree.ParseCompilationUnit(data);
			var classNodeList = tree.Root.DescendentNodes().Where(i => i.Kind == SyntaxKind.ClassDeclaration);
			foreach(var classNode in classNodeList)
			{
				
			}
			return string.Empty;
		}
	}
}
