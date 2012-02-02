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
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual("Test.Namespace", classObject.NamespaceName);
			Assert.AreEqual("TestClass", classObject.ClassName);
			Assert.AreEqual("Test.Namespace.TestClass", classObject.ClassFullName);
		}		

		[Test]
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

					protected class ProtectedClass
					{
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
			Assert.AreEqual(4, classList.Count);

			Assert.AreEqual("PublicClass", classList[0].ClassName);
			Assert.AreEqual(EnumProtectionLevel.Public, classList[0].ProtectionLevel);

			Assert.AreEqual("PrivateClass", classList[1].ClassName);
			Assert.AreEqual(EnumProtectionLevel.Private, classList[1].ProtectionLevel);

			Assert.AreEqual("InternalClass", classList[2].ClassName);
			Assert.AreEqual(EnumProtectionLevel.Internal, classList[2].ProtectionLevel);

			Assert.AreEqual("ProtectedClass", classList[3].ClassName);
			Assert.AreEqual(EnumProtectionLevel.Protected, classList[3].ProtectionLevel);
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
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
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
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
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
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
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
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
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
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
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
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
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
	
		[Test]
		public void TestPropertyAttributes()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
						public string Property1 { get; set; }

						[TestAttribute1]
						public int Property2 { get; set; }

						[TestAttribute2]
						[TestAttribute3, Test.Test.TestAttribute4]
						public float Property3 { get; set; }
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(3, classObject.PropertyList.Count);

			Assert.AreEqual("Property1", classObject.PropertyList[0].PropertyName);
			Assert.IsEmpty(classObject.PropertyList[0].AttributeList);

			Assert.AreEqual("Property2", classObject.PropertyList[1].PropertyName);
			Assert.AreEqual(1, classObject.PropertyList[1].AttributeList.Count);
			Assert.AreEqual("TestAttribute1", classObject.PropertyList[1].AttributeList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.PropertyList[1].AttributeList[0].TypeNamespace);
			Assert.AreEqual("TestAttribute1", classObject.PropertyList[1].AttributeList[0].TypeFullName);

			Assert.AreEqual("Property3", classObject.PropertyList[2].PropertyName);
			Assert.AreEqual(3, classObject.PropertyList[2].AttributeList.Count);
			Assert.AreEqual("TestAttribute2", classObject.PropertyList[2].AttributeList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.PropertyList[2].AttributeList[0].TypeNamespace);
			Assert.AreEqual("TestAttribute2", classObject.PropertyList[2].AttributeList[0].TypeFullName);
			Assert.AreEqual("TestAttribute3", classObject.PropertyList[2].AttributeList[1].TypeName);
			Assert.IsNullOrEmpty(classObject.PropertyList[2].AttributeList[1].TypeNamespace);
			Assert.AreEqual("TestAttribute3", classObject.PropertyList[2].AttributeList[1].TypeFullName);
			Assert.AreEqual("TestAttribute4", classObject.PropertyList[2].AttributeList[2].TypeName);
			Assert.AreEqual("Test.Test", classObject.PropertyList[2].AttributeList[2].TypeNamespace);
			Assert.AreEqual("Test.Test.TestAttribute4", classObject.PropertyList[2].AttributeList[2].TypeFullName);
		}

		[Test]
		public void TestPropertyProtectionLevel()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
						private int PrivateProperty1 { get; set; }
						int PrivateProperty2 { get; set; }

						public int PublicProperty1 { get; set; }

						protected int ProtectedProperty1 { get; set; }

						internal int InternalProperty1 { get; set; }
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(5, classObject.PropertyList.Count);

			Assert.AreEqual("PrivateProperty1", classObject.PropertyList[0].PropertyName);
			Assert.AreEqual(EnumProtectionLevel.Private, classObject.PropertyList[0].ProtectionLevel);

			Assert.AreEqual("PrivateProperty2", classObject.PropertyList[1].PropertyName);
			Assert.AreEqual(EnumProtectionLevel.Private, classObject.PropertyList[1].ProtectionLevel);

			Assert.AreEqual("PublicProperty1", classObject.PropertyList[2].PropertyName);
			Assert.AreEqual(EnumProtectionLevel.Public, classObject.PropertyList[2].ProtectionLevel);

			Assert.AreEqual("ProtectedProperty1", classObject.PropertyList[3].PropertyName);
			Assert.AreEqual(EnumProtectionLevel.Protected, classObject.PropertyList[3].ProtectionLevel);

			Assert.AreEqual("InternalProperty1", classObject.PropertyList[4].PropertyName);
			Assert.AreEqual(EnumProtectionLevel.Internal, classObject.PropertyList[4].ProtectionLevel);
		}

		[Test]
		public void TestFields()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
						public int TestField1 = 1;
						private string TestField2 = ""abc"";
						internal Test1.Test2.TestType TestField3;
						protected int TestField4;
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.IsEmpty(classObject.PropertyList);
			Assert.AreEqual(4, classObject.FieldList.Count);
			
			Assert.AreEqual("TestField1", classObject.FieldList[0].FieldName);
			Assert.AreEqual("int", classObject.FieldList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.FieldList[0].TypeNamespace);
			Assert.AreEqual("int", classObject.FieldList[0].TypeFullName);
			Assert.AreEqual(EnumProtectionLevel.Public, classObject.FieldList[0].ProtectionLevel);

			Assert.AreEqual("TestField2", classObject.FieldList[1].FieldName);
			Assert.AreEqual("string", classObject.FieldList[1].TypeName);
			Assert.IsNullOrEmpty(classObject.FieldList[1].TypeNamespace);
			Assert.AreEqual("string", classObject.FieldList[1].TypeFullName);
			Assert.AreEqual(EnumProtectionLevel.Private, classObject.FieldList[1].ProtectionLevel);

			Assert.AreEqual("TestField3", classObject.FieldList[2].FieldName);
			Assert.AreEqual("TestType", classObject.FieldList[2].TypeName);
			Assert.AreEqual("Test1.Test2", classObject.FieldList[2].TypeNamespace);
			Assert.AreEqual("Test1.Test2.TestType", classObject.FieldList[2].TypeFullName);
			Assert.AreEqual(EnumProtectionLevel.Internal, classObject.FieldList[2].ProtectionLevel);

			Assert.AreEqual("TestField4", classObject.FieldList[3].FieldName);
			Assert.AreEqual("int", classObject.FieldList[3].TypeName);
			Assert.IsNullOrEmpty(classObject.FieldList[3].TypeNamespace);
			Assert.AreEqual("int", classObject.FieldList[3].TypeFullName);
			Assert.AreEqual(EnumProtectionLevel.Protected, classObject.FieldList[3].ProtectionLevel);

			Assert.AreEqual("TestField2", classObject.FieldList[1].FieldName);
			Assert.AreEqual("string", classObject.FieldList[1].TypeName);
			Assert.IsNullOrEmpty(classObject.FieldList[1].TypeNamespace);
			Assert.AreEqual("string", classObject.FieldList[1].TypeFullName);
			Assert.AreEqual(EnumProtectionLevel.Private, classObject.FieldList[1].ProtectionLevel);
		}

		[Test]
		public void TestFieldAttributes()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
						public string Field1;

						[TestAttribute1]
						public int Field2;

						[TestAttribute2]
						[TestAttribute3, Test.Test.TestAttribute4]
						public float Field3;
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(3, classObject.FieldList.Count);

			Assert.AreEqual("Field1", classObject.FieldList[0].FieldName);
			Assert.IsEmpty(classObject.FieldList[0].AttributeList);

			Assert.AreEqual("Field2", classObject.FieldList[1].FieldName);
			Assert.AreEqual(1, classObject.FieldList[1].AttributeList.Count);
			Assert.AreEqual("TestAttribute1", classObject.FieldList[1].AttributeList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.FieldList[1].AttributeList[0].TypeNamespace);
			Assert.AreEqual("TestAttribute1", classObject.FieldList[1].AttributeList[0].TypeFullName);

			Assert.AreEqual("Field3", classObject.FieldList[2].FieldName);
			Assert.AreEqual(3, classObject.FieldList[2].AttributeList.Count);
			Assert.AreEqual("TestAttribute2", classObject.FieldList[2].AttributeList[0].TypeName);
			Assert.IsNullOrEmpty(classObject.FieldList[2].AttributeList[0].TypeNamespace);
			Assert.AreEqual("TestAttribute2", classObject.FieldList[2].AttributeList[0].TypeFullName);
			Assert.AreEqual("TestAttribute3", classObject.FieldList[2].AttributeList[1].TypeName);
			Assert.IsNullOrEmpty(classObject.FieldList[2].AttributeList[1].TypeNamespace);
			Assert.AreEqual("TestAttribute3", classObject.FieldList[2].AttributeList[1].TypeFullName);
			Assert.AreEqual("TestAttribute4", classObject.FieldList[2].AttributeList[2].TypeName);
			Assert.AreEqual("Test.Test", classObject.FieldList[2].AttributeList[2].TypeNamespace);
			Assert.AreEqual("Test.Test.TestAttribute4", classObject.FieldList[2].AttributeList[2].TypeFullName);
		}

		[Test]
		public void TestCompoundFields()
		{
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
						public string Field1 = ""abc"";
						public int Field2, Field3=4;
						public double Field5;
					}
				}
			";
			ClassParser parser = new ClassParser();
			var classList = parser.ParseString(data.ToString(), "TestFileName.cs");
			Assert.AreEqual(1, classList.Count);
			var classObject = classList[0];
			Assert.AreEqual(4, classObject.FieldList.Count);

			Assert.AreEqual("Field1", classObject.FieldList[0].FieldName);
			Assert.AreEqual("string", classObject.FieldList[0].TypeFullName);
			Assert.AreEqual("abc", classObject.FieldList[0].FieldValue);

			Assert.AreEqual("Field2", classObject.FieldList[1].FieldName);
			Assert.AreEqual("int", classObject.FieldList[1].TypeFullName);
			Assert.IsNull(classObject.FieldList[1].FieldValue);

			Assert.AreEqual("Field3", classObject.FieldList[2].FieldName);
			Assert.AreEqual("int", classObject.FieldList[2].TypeFullName);
			Assert.AreEqual(4, classObject.FieldList[2].FieldValue);

			Assert.AreEqual("Field5", classObject.FieldList[3].FieldName);
			Assert.AreEqual("double", classObject.FieldList[3].TypeFullName);
			Assert.IsNull(classObject.FieldList[3].FieldValue);
		}

		[Test]
		public void UpdateExistingClass()
		{
			var existingClass1 = new CSClass()
			{
				ClassName = "TestClassLeaveAlone",
				NamespaceName = "Test.Namespace",
				PropertyList = new List<CSProperty>() 
				{
					new CSProperty {  TypeName = "int", PropertyName = "TestProperty1" },
					new CSProperty {  TypeName = "int", PropertyName = "TestProperty2" }
				},
				FieldList = new List<CSField>()
				{
					new CSField {  TypeName = "int", FieldName = "TestField1" },
					new CSField {  TypeName = "int", FieldName = "TestField2" }
				}
			};
			var existingClass2 = new CSClass()
			{
				ClassName = "TestClassUpdate",
				NamespaceName = "Test.Namespace",
				PropertyList = new List<CSProperty>() 
				{
					new CSProperty {  TypeName = "int", PropertyName = "TestProperty1" },
					new CSProperty {  TypeName = "int", PropertyName = "TestProperty2" }
				},
				FieldList = new List<CSField>()
				{
					new CSField {  TypeName = "int", FieldName = "TestField1" },
					new CSField {  TypeName = "int", FieldName = "TestField2" }
				}
			};
			List<CSClass> existingClassList = new List<CSClass>() { existingClass1, existingClass2 };
			string data =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClassUpdate
					{
						public int TestProperty3 { get; set; }
						public int TestProperty4 { get; set; }

						public int TestField3;
						public int TestField4;
					}
					public class TestClassNew
					{
						public int TestProperty1 { get; set; }
						public int TestProperty2 { get; set; }

						public int TestField1;
						public int TestField2;
					}
				}
			";
			ClassParser parser = new ClassParser();
			var newClassList = parser.ParseString(data, "TestFile.cs", existingClassList);

			Assert.AreEqual(3, newClassList.Count);

			var testClassLeaveAlone = newClassList[0];
			Assert.AreEqual("Test.Namespace.TestClassLeaveAlone", testClassLeaveAlone.ClassFullName);
			Assert.AreEqual(2, testClassLeaveAlone.PropertyList.Count);
			Assert.AreEqual("TestProperty1", testClassLeaveAlone.PropertyList[0].PropertyName);
			Assert.AreEqual("TestProperty2", testClassLeaveAlone.PropertyList[1].PropertyName);
			Assert.AreEqual(2, testClassLeaveAlone.FieldList.Count);
			Assert.AreEqual("TestField1", testClassLeaveAlone.FieldList[0].FieldName);
			Assert.AreEqual("TestField2", testClassLeaveAlone.FieldList[1].FieldName);

			var testClassUpdate = newClassList[1];
			Assert.AreEqual("Test.Namespace.TestClassUpdate", testClassUpdate.ClassFullName);
			Assert.AreEqual(4, testClassUpdate.PropertyList.Count);
			Assert.AreEqual("TestProperty1", testClassUpdate.PropertyList[0].PropertyName);
			Assert.AreEqual("TestProperty2", testClassUpdate.PropertyList[1].PropertyName);
			Assert.AreEqual("TestProperty3", testClassUpdate.PropertyList[2].PropertyName);
			Assert.AreEqual("TestProperty4", testClassUpdate.PropertyList[3].PropertyName);
			Assert.AreEqual(4, testClassUpdate.FieldList.Count);
			Assert.AreEqual("TestField1", testClassUpdate.FieldList[0].FieldName);
			Assert.AreEqual("TestField2", testClassUpdate.FieldList[1].FieldName);
			Assert.AreEqual("TestField3", testClassUpdate.FieldList[2].FieldName);
			Assert.AreEqual("TestField4", testClassUpdate.FieldList[3].FieldName);

			var testClassNew = newClassList[2];
			Assert.AreEqual("Test.Namespace.TestClassNew", testClassNew.ClassFullName);
			Assert.AreEqual(2, testClassNew.PropertyList.Count);
			Assert.AreEqual("TestProperty1", testClassNew.PropertyList[0].PropertyName);
			Assert.AreEqual("TestProperty2", testClassNew.PropertyList[1].PropertyName);
			Assert.AreEqual(2, testClassNew.FieldList.Count);
			Assert.AreEqual("TestField1", testClassNew.FieldList[0].FieldName);
			Assert.AreEqual("TestField2", testClassNew.FieldList[1].FieldName);
		}

		[Test]
		public void TestDependentUponFileList()
		{
			string data1 =
			@"
				using System;

				namespace Test.Namespace
				{
					public class TestClass
					{
					}
				}
			";
			string data2 =
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
			var classList = parser.ParseString(data1, "TestFile.cs", null, new string[] { "DependentUponFile1.cs", "DependentUponFile2.cs"});
			classList = parser.ParseString(data2, "TestFile2.cs", classList, new string[] { "DependentUponFile3.cs" });

			Assert.AreEqual(1, classList.Count);
			Assert.AreEqual(3, classList[0].DependentUponFilePathList.Count);
			Assert.AreEqual("DependentUponFile1.cs", classList[0].DependentUponFilePathList[0]);
			Assert.AreEqual("DependentUponFile2.cs", classList[0].DependentUponFilePathList[1]);
			Assert.AreEqual("DependentUponFile3.cs", classList[0].DependentUponFilePathList[2]);
		}
	}
}
