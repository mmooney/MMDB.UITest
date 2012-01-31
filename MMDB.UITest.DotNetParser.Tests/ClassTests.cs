using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MMDB.UITest.DotNetParser.Tests
{
	[TestFixture]
	public class ClassTests
	{
		[Test]
		public void TestBasicClass()
		{
			string data = 
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual("Test.Namespace", classObject.NamespaceName);
			Assert.AreEqual("TestClass", classObject.ClassName);
			Assert.AreEqual("Test.Namespace.TestClass", classObject.ClassFullName);
		}		

		public void TestClassProtectionLevel()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class PublicClass
					{
					}

					private class PrivateClass
					{
					}

					internal class InternalClass
					{
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(3, classList.Count);

			Assert.AreEqual("PublicClass", classList[0].ClassName);
			Assert.AreEqual(EnumProtectionLevel.Public, classList[0].ProtectionLevel);

			Assert.AreEqual("PrivateClass", classList[1].ClassName);
			Assert.AreEqual(EnumProtectionLevel.Private, classList[1].ProtectionLevel);

			Assert.AreEqual("InternalClass", classList[1].ClassName);
			Assert.AreEqual(EnumProtectionLevel.Internal, classList[1].ProtectionLevel);
		}

		[Test]
		public void TestTwoProperties()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
						public string Property1 { get; set; }
						public int Property2 { get; set; }
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(2, classObject.PropertyList.Count);

			Assert.AreEqual("Property1", classObject.PropertyList[0].PropertyName);
			Assert.AreEqual("string", classObject.PropertyList[0].TypeName);
			Assert.AreEqual("string", classObject.PropertyList[0].TypeFullName);

			Assert.AreEqual("Property2", classObject.PropertyList[1].PropertyName);
			Assert.AreEqual("int", classObject.PropertyList[1].TypeName);
			Assert.AreEqual("int", classObject.PropertyList[1].TypeFullName);
		}

		[Test]
		public void TestPropertyTypes()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
						public Test123 Property1 { get; set; }
						public Test.Test.Test1 Property2 { get; set; }
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(2, classObject.PropertyList.Count);
			
			Assert.AreEqual("Property1", classObject.PropertyList[0].PropertyName);
			Assert.AreEqual("Test123", classObject.PropertyList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.PropertyList[0].TypeNamespace);
			Assert.AreEqual("Test123", classObject.PropertyList[0].TypeFullName);

			Assert.AreEqual("Property2", classObject.PropertyList[1].PropertyName);
			Assert.AreEqual("Test1", classObject.PropertyList[1].TypeName);
			Assert.AreEqual("Test.Test", classObject.PropertyList[1].TypeNamespace);
			Assert.AreEqual("Test.Test.Test1", classObject.PropertyList[1].TypeFullName);
		}

		[Test]
		public void TestSingleClassAttribute()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					[TestAttribute1]
					public class TestClass
					{
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(1, classObject.AttributeList.Count);
			Assert.AreEqual("TestAttribute1", classObject.AttributeList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.AttributeList[0].TypeNamespace);
			Assert.AreEqual("TestAttribute1", classObject.AttributeList[0].TypeFullName);
		}

		[Test]
		public void TestTwoClassAttributes()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					[TestAttribute1]
					[Test1.Test2.TestAttribute2]
					public class TestClass
					{
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(2, classObject.AttributeList.Count);

			Assert.AreEqual("TestAttribute1", classObject.AttributeList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.AttributeList[0].TypeNamespace);
			Assert.AreEqual("TestAttribute1", classObject.AttributeList[0].TypeFullName);

			Assert.AreEqual("TestAttribute2", classObject.AttributeList[1].TypeName);
			Assert.AreEqual("Test1.Test2", classObject.AttributeList[1].TypeNamespace);
			Assert.AreEqual("Test1.Test2.TestAttribute2", classObject.AttributeList[1].TypeFullName);
		}

		[Test]
		public void TestCompoundClassAttributes()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					[TestAttribute1, Test1.Test2.TestAttribute2]
					[TestAttribute3]
					public class TestClass
					{
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(3, classObject.AttributeList.Count);

			Assert.AreEqual("TestAttribute1", classObject.AttributeList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.AttributeList[0].TypeNamespace);
			Assert.AreEqual("TestAttribute1", classObject.AttributeList[0].TypeFullName);

			Assert.AreEqual("TestAttribute2", classObject.AttributeList[1].TypeName);
			Assert.AreEqual("Test1.Test2", classObject.AttributeList[1].TypeNamespace);
			Assert.AreEqual("Test1.Test2.TestAttribute2", classObject.AttributeList[1].TypeFullName);

			Assert.AreEqual("TestAttribute3", classObject.AttributeList[2].TypeName);
			Assert.IsNullOrEmpty(classObject.AttributeList[2].TypeNamespace);
			Assert.AreEqual("TestAttribute3", classObject.AttributeList[2].TypeFullName);
		}

		[Test]
		public void TestClassAttributeParameters()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					[TestAttribute1]
					[TestAttribute2(123, 234)]
					[TestAttribute3(123, ""abc"")]
					[TestAttribute4(Parameter1=123,Parameter2=234)]
					[TestAttribute5(123, Parameter2=234)]
					[TestAttribute6(TestSomething=TestSomthing.Else)]
					public class TestClass
					{
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString());
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(6, classObject.AttributeList.Count);

			Assert.AreEqual("TestAttribute1", classObject.AttributeList[0].TypeFullName);
			Assert.IsEmpty(classObject.AttributeList[0].ArgumentList);

			Assert.AreEqual("TestAttribute2", classObject.AttributeList[1].TypeFullName);
			Assert.AreEqual(2, classObject.AttributeList[1].ArgumentList.Count);
			Assert.IsNullOrEmpty(classObject.AttributeList[1].ArgumentList[0].ArgumentName);
			Assert.AreEqual(123, classObject.AttributeList[1].ArgumentList[0].ArguementValue);
			Assert.IsNullOrEmpty(classObject.AttributeList[1].ArgumentList[1].ArgumentName);
			Assert.AreEqual(234, classObject.AttributeList[1].ArgumentList[1].ArguementValue);
			Assert.AreEqual(123, classObject.AttributeList[1].GetAttributeParameter(0, "Something", true));
			Assert.AreEqual(234, classObject.AttributeList[1].GetAttributeParameter(1, "Something", true));
			Assert.Throws(typeof(ArgumentNullException), delegate { classObject.AttributeList[1].GetAttributeParameter(0, null, true );});
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[1].GetAttributeParameter(-1, "Something", true);});
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[1].GetAttributeParameter(2, "Something", true); });


			Assert.AreEqual("TestAttribute3", classObject.AttributeList[2].TypeFullName);
			Assert.IsNullOrEmpty(classObject.AttributeList[2].ArgumentList[0].ArgumentName);
			Assert.AreEqual(123, classObject.AttributeList[2].ArgumentList[0].ArguementValue);
			Assert.IsNullOrEmpty(classObject.AttributeList[2].ArgumentList[1].ArgumentName);
			Assert.AreEqual("abc", classObject.AttributeList[2].ArgumentList[1].ArguementValue);
			Assert.AreEqual(123, classObject.AttributeList[2].GetAttributeParameter(0, "Something", true));
			Assert.AreEqual("abc", classObject.AttributeList[2].GetAttributeParameter(1, "Something", true));
			Assert.Throws(typeof(ArgumentNullException), delegate { classObject.AttributeList[2].GetAttributeParameter(0, null, true); });
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[2].GetAttributeParameter(-1, "Something", true); });
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[2].GetAttributeParameter(2, "Something", true); });

			Assert.AreEqual("TestAttribute4", classObject.AttributeList[3].TypeFullName);
			Assert.AreEqual("Parameter1", classObject.AttributeList[3].ArgumentList[0].ArgumentName);
			Assert.AreEqual(123, classObject.AttributeList[3].ArgumentList[0].ArguementValue);
			Assert.AreEqual("Parameter2", classObject.AttributeList[3].ArgumentList[1].ArgumentName);
			Assert.AreEqual(234, classObject.AttributeList[3].ArgumentList[1].ArguementValue);
			Assert.AreEqual(123, classObject.AttributeList[3].GetAttributeParameter(0, "Parameter1", true));
			Assert.AreEqual(234, classObject.AttributeList[3].GetAttributeParameter(1, "Parameter2", true));
			Assert.Throws(typeof(ArgumentNullException), delegate { classObject.AttributeList[3].GetAttributeParameter(0, null, true); });
			Assert.Throws(typeof(ArgumentException), delegate { classObject.AttributeList[3].GetAttributeParameter(0, "Something", true); });
			Assert.Throws(typeof(ArgumentException), delegate { classObject.AttributeList[3].GetAttributeParameter(1, "Something", true); });
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[3].GetAttributeParameter(-1, "Something", true); });
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[3].GetAttributeParameter(2, "Something", true); });

			Assert.AreEqual("TestAttribute5", classObject.AttributeList[4].TypeFullName);
			Assert.IsNullOrEmpty(classObject.AttributeList[4].ArgumentList[0].ArgumentName);
			Assert.AreEqual(123, classObject.AttributeList[4].ArgumentList[0].ArguementValue);
			Assert.AreEqual("Parameter2", classObject.AttributeList[4].ArgumentList[1].ArgumentName);
			Assert.AreEqual(234, classObject.AttributeList[4].ArgumentList[1].ArguementValue);

			Assert.AreEqual(123, classObject.AttributeList[4].GetAttributeParameter(0, "Parameter1", true));
			Assert.Throws(typeof(ArgumentException), delegate { classObject.AttributeList[4].GetAttributeParameter(1, "Parameter1", true); });
			Assert.AreEqual(234, classObject.AttributeList[4].GetAttributeParameter(1, "Parameter2", true));
			Assert.Throws(typeof(ArgumentNullException), delegate { classObject.AttributeList[4].GetAttributeParameter(0, null, true); });
			Assert.Throws(typeof(ArgumentException), delegate { classObject.AttributeList[4].GetAttributeParameter(1, "Something", true); });
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[4].GetAttributeParameter(-1, "Something", true); });
			Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { classObject.AttributeList[4].GetAttributeParameter(2, "Something", true); });

			Assert.AreEqual("TestAttribute6", classObject.AttributeList[5].TypeFullName);

		}
	}
}
