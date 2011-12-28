//
// ParameterCompletionTests.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeCompletion
{
	[TestFixture()]
	public class ParameterCompletionTests : TestBase
	{
		class TestFactory : IParameterCompletionDataFactory
		{
			ITypeResolveContext ctx;
			
			public TestFactory (ITypeResolveContext ctx)
			{
				this.ctx = ctx;
			}
			
			class Provider : IParameterDataProvider
			{
				public IEnumerable<IMethod> Data { get; set; }
				#region IParameterDataProvider implementation
				public string GetMethodMarkup (int overload, string[] parameterMarkup, int currentParameter)
				{
					return "";
				}

				public string GetParameterMarkup (int overload, int paramIndex)
				{
					return "";
				}

				public int GetParameterCount (int overload)
				{
					var method = Data.ElementAt (overload);
					return method.Parameters.Count;
				}

				public int OverloadCount {
					get {
						return Data.Count ();
					}
				}
				#endregion
			}
			
			class IndexerProvider : IParameterDataProvider
			{
				public IEnumerable<IProperty> Data { get; set; }
				#region IParameterDataProvider implementation
				public string GetMethodMarkup (int overload, string[] parameterMarkup, int currentParameter)
				{
					return "";
				}

				public string GetParameterMarkup (int overload, int paramIndex)
				{
					return "";
				}

				public int GetParameterCount (int overload)
				{
					var method = Data.ElementAt (overload);
					return method.Parameters.Count;
				}

				public int OverloadCount {
					get {
						return Data.Count ();
					}
				}
				#endregion
			}
			
			#region IParameterCompletionDataFactory implementation
			public IParameterDataProvider CreateConstructorProvider (ICSharpCode.NRefactory.TypeSystem.IType type)
			{
				
				return new Provider () {
					Data = type.GetConstructors (ctx, m => m.Accessibility == Accessibility.Public)
				};
			}

			public IParameterDataProvider CreateMethodDataProvider (ICSharpCode.NRefactory.CSharp.Resolver.MethodGroupResolveResult par1)
			{
				return new Provider () {
					Data = par1.Methods
				};
			}

			public IParameterDataProvider CreateMethodDataProvider (ICSharpCode.NRefactory.TypeSystem.IMethod method)
			{
				return new Provider () {
					Data = new [] { method }
				};
			}

			public IParameterDataProvider CreateDelegateDataProvider (ICSharpCode.NRefactory.TypeSystem.IType type)
			{
				return new Provider () {
					Data = new [] { type.GetDelegateInvokeMethod () }
				};
			}
			
			public IParameterDataProvider CreateIndexerParameterDataProvider (IType type, AstNode resolvedNode)
			{
				return new IndexerProvider () {
					Data = type.GetProperties (ctx, p => p.IsIndexer)
				};
			}
			#endregion
		}
		
		internal static IParameterDataProvider CreateProvider (string text)
		{
			string parsedText;
			string editorText;
			int cursorPosition = text.IndexOf ('$');
			int endPos = text.IndexOf ('$', cursorPosition + 1);
			if (endPos == -1)
				parsedText = editorText = text.Substring (0, cursorPosition) + text.Substring (cursorPosition + 1);
			else {
				parsedText = text.Substring (0, cursorPosition) + new string (' ', endPos - cursorPosition) + text.Substring (endPos + 1);
				editorText = text.Substring (0, cursorPosition) + text.Substring (cursorPosition + 1, endPos - cursorPosition - 1) + text.Substring (endPos + 1);
				cursorPosition = endPos - 1; 
			}
			var doc = new ReadOnlyDocument (editorText);
			
			
			var pctx = new SimpleProjectContent ();
			
			var compilationUnit = new CSharpParser ().Parse (parsedText);
			
			var parsedFile = new TypeSystemConvertVisitor (pctx, "program.cs").Convert (compilationUnit);
			pctx.UpdateProjectContent (null, parsedFile);

			var ctx = new CompositeTypeResolveContext ( new [] { pctx, CecilLoaderTests.Mscorlib, CecilLoaderTests.SystemCore });
			
			var engine = new CSharpParameterCompletionEngine (doc, new TestFactory (ctx));
			engine.ctx = ctx;
			engine.CSharpParsedFile = parsedFile;
			engine.ProjectContent = pctx;
			engine.Unit = compilationUnit;
			
			return engine.GetParameterDataProvider (cursorPosition, doc.GetCharAt (cursorPosition - 1));
		}
		
		/// <summary>
		/// Bug 427448 - Code Completion: completion of constructor parameters not working
		/// </summary>
		[Test()]
		public void TestBug427448 ()
		{
			IParameterDataProvider provider = CreateProvider (
@"class Test
{
	public Test (int a)
	{
	}
	
	public Test (string b)
	{
	}
	protected Test ()
	{
	}
	Test (double d, float m)
	{
	}
}

class AClass
{
	void A()
	{
		$Test t = new Test ($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (2, provider.OverloadCount);
		}

		/// <summary>
		/// Bug 432437 - No completion when invoking delegates
		/// </summary>
		[Test()]
		public void TestBug432437 ()
		{
			IParameterDataProvider provider = CreateProvider (
@"public delegate void MyDel (int value);

class Test
{
	MyDel d;

	void A()
	{
		$d ($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}

		/// <summary>
		/// Bug 432658 - Incorrect completion when calling an extension method from inside another extension method
		/// </summary>
		[Test()]
		public void TestBug432658 ()
		{
			IParameterDataProvider provider = CreateProvider (
@"static class Extensions
{
	public static void Ext1 (this int start)
	{
	}
	public static void Ext2 (this int end)
	{
		$Ext1($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount, "There should be one overload");
			Assert.AreEqual (1, provider.GetParameterCount(0), "Parameter 'start' should exist");
		}

		/// <summary>
		/// Bug 432727 - No completion if no constructor
		/// </summary>
		[Test()]
		public void TestBug432727 ()
		{
			IParameterDataProvider provider = CreateProvider (
@"class A
{
	void Method ()
	{
		$A aTest = new A ($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}

		/// <summary>
		/// Bug 434705 - No autocomplete offered if not assigning result of 'new' to a variable
		/// </summary>
		[Test()]
		public void TestBug434705 ()
		{
			IParameterDataProvider provider = CreateProvider (
@"class Test
{
	public Test (int a)
	{
	}
}

class AClass
{
	Test A()
	{
		$return new Test ($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}
		
		/// <summary>
		/// Bug 434705 - No autocomplete offered if not assigning result of 'new' to a variable
		/// </summary>
		[Test()]
		public void TestBug434705B ()
		{
			IParameterDataProvider provider = CreateProvider (
@"
class Test<T>
{
	public Test (T t)
	{
	}
}
class TestClass
{
	void TestMethod ()
	{
		$Test<int> l = new Test<int> ($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}
	
		
		/// <summary>
		/// Bug 434701 - No autocomplete in attributes
		/// </summary>
		[Test()]
		public void TestBug434701 ()
		{
			IParameterDataProvider provider = CreateProvider (
@"class TestAttribute : System.Attribute
{
	public Test (int a)
	{
	}
}

$[Test ($
class AClass
{
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}
		
		/// <summary>
		/// Bug 447985 - Exception display tip is inaccurate for derived (custom) exceptions
		/// </summary>
		[Test()]
		public void TestBug447985 ()
		{
			IParameterDataProvider provider = CreateProvider (
@"
namespace System {
	public class Exception
	{
		public Exception () {}
	}
}

class MyException : System.Exception
{
	public MyException (int test)
	{}
}

class AClass
{
	public void Test ()
	{
		$throw new MyException($
	}

}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
			Assert.AreEqual (1, provider.GetParameterCount(0), "Parameter 'test' should exist");
		}
		
		
		/// <summary>
		/// Bug 1760 - [New Resolver] Parameter tooltip not shown for indexers 
		/// </summary>
		[Test()]
		public void Test1760 ()
		{
			var provider = CreateProvider (
@"
class TestClass
{
	public static void Main (string[] args)
	{
		$args[$
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}
		
		[Test()]
		public void TestSecondIndexerParameter ()
		{
			var provider = CreateProvider (
@"
class TestClass
{
	public int this[int i, int j] { get { return 0; } } 
	public void Test ()
	{
		$this[1,$
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}
		
		[Test()]
		public void TestSecondMethodParameter ()
		{
			var provider = CreateProvider (
@"
class TestClass
{
	public int TestMe (int i, int j) { return 0; } 
	public void Test ()
	{
		$TestMe (1,$
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}
		
		
		/// Bug 599 - Regression: No intellisense over Func delegate
		[Test()]
		public void TestBug599 ()
		{
			var provider = CreateProvider (
@"using System;
using System.Core;

class TestClass
{
	void A (Func<int, int> f)
	{
		$f ($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}
		
		[Test()]
		public void TestConstructor ()
		{
			IParameterDataProvider provider = CreateProvider (
@"class Foo { public Foo (int a) {} }

class A
{
	void Method ()
	{
		$Bar = new Foo ($
	}
}");
			Assert.IsNotNull (provider, "provider was not created.");
			Assert.AreEqual (1, provider.OverloadCount);
		}

	
	}
}