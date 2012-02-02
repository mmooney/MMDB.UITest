using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace MMDB.UITest.DotNetParser.Tests
{
	[TestFixture]
	public class ProjectTests
	{
		[Test]
		public void TestBasicProject()
		{
			string data =
			@"<?xml version=""1.0"" encoding=""utf-8""?>
				<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
					<ItemGroup>
						<Compile Include=""TestClass1.cs"" />
					</ItemGroup>
				</Project>
			";
			Mock<IClassParser> classParser = new Mock<IClassParser>();
			List<CSClass> classList = new List<CSClass>();
			var testClass1 = new CSClass()
			{
				ClassName = "TestClass1",
				NamespaceName = "Test.Test1"
			};
			classList.Add(testClass1);
			classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.cs", It.IsAny<IEnumerable<CSClass>>(), It.IsAny<IEnumerable<string>>())).Returns(classList);

			ProjectParser parser = new ProjectParser(classParser.Object);

			CSProjectFile project = parser.ParseString(data, "C:\\Test\\TestProject.csproj");
			
			Assert.AreEqual(1, project.ClassList.Count);
			Assert.AreEqual("Test.Test1.TestClass1", project.ClassList[0].ClassFullName);
		}

		[Test]
		public void TestDependentFiles()
		{
			string data =
			@"<?xml version=""1.0"" encoding=""utf-8""?>
				<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
					<ItemGroup>
						<Compile Include=""TestClass1.aspx.cs"">
							<DependentUpon>TestClass1.aspx</DependentUpon>
						</Compile>
						<Compile Include=""TestClass1.aspx.SomethingElse.cs"">
							<DependentUpon>TestClass1.aspx</DependentUpon>
						</Compile>
						<Compile Include=""TestClass2.aspx.cs"">
							<DependentUpon>TestClass2.aspx</DependentUpon>
						</Compile>
					</ItemGroup>
					<ItemGroup>
						<Content Include=""TestClass1.aspx"" />
						<Content Include=""TestClass2.aspx"" />
					</ItemGroup>
				</Project>
			";
			Mock<IClassParser> classParser = new Mock<IClassParser>();
			List<CSClass> classList = new List<CSClass>();
			var testClass1 = new CSClass()
			{
				ClassName = "TestClass1",
				NamespaceName = "Test.Test1",
				PropertyList = new List<CSProperty>() 
				{
					new CSProperty { PropertyName = "TestProperty1", TypeName = "int" }
				},
				DependentUponFilePathList = new List<string>()
				{
					"TestClass1.aspx"
				}
			};
			var testClass1SomethingElse = new CSClass()
			{
				ClassName = "TestClass1",
				NamespaceName = "Test.Test1",
				PropertyList = new List<CSProperty>() 
				{
					new CSProperty { PropertyName = "TestProperty2", TypeName = "int" }
				},
				DependentUponFilePathList = new List<string>()
				{
					"TestClass1.aspx"
				}
			};
			var testClass2 = new CSClass()
			{
				ClassName = "TestClass2",
				NamespaceName = "Test.Test2",
				PropertyList = new List<CSProperty>()
				{
					new CSProperty { PropertyName = "TestProperty3", TypeName = "int" }
				},
				DependentUponFilePathList = new List<string>()
				{
					"TestClass2.aspx"
				}
			};
			classList.Add(testClass1);
			classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.aspx.cs", It.IsAny<IEnumerable<CSClass>>(), It.IsAny<IEnumerable<string>>()))
						.Returns
						(
							(string filePath, IEnumerable<CSClass> inputClassList, IEnumerable<string> inputDependentUponFileList)
								=>AppendClassToList(inputClassList, testClass1)
						);
			classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.aspx.SomethingElse.cs", It.IsAny<IEnumerable<CSClass>>(), It.IsAny<IEnumerable<string>>()))
						.Returns
						(
							(string filePath, IEnumerable<CSClass> inputClassList, IEnumerable<string> inputDependentUponFileList)
								=> AppendClassToList(inputClassList, testClass1SomethingElse)
						);

			classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass2.aspx.cs", It.IsAny<IEnumerable<CSClass>>(), It.IsAny<IEnumerable<string>>()))
						.Returns
						(
							(string filePath, IEnumerable<CSClass> inputClassList, IEnumerable<string> inputDependentUponFileList)
								=> AppendClassToList(inputClassList, testClass2)
						);

			ProjectParser parser = new ProjectParser(classParser.Object);

			CSProjectFile project = parser.ParseString(data, "C:\\Test\\TestProject.csproj");

			Assert.AreEqual(2, project.ClassList.Count);

			Assert.AreEqual("Test.Test1.TestClass1", project.ClassList[0].ClassFullName);
			Assert.AreEqual(2, project.ClassList[0].PropertyList.Count);
			Assert.AreEqual(1, project.ClassList[0].DependentUponFilePathList.Count);
			Assert.AreEqual("TestClass1.aspx", project.ClassList[0].DependentUponFilePathList[0]);

			Assert.AreEqual("Test.Test2.TestClass2", project.ClassList[1].ClassFullName);
			Assert.AreEqual(1, project.ClassList[1].PropertyList.Count);
			Assert.AreEqual(1, project.ClassList[1].DependentUponFilePathList.Count);
			Assert.AreEqual("TestClass2.aspx", project.ClassList[1].DependentUponFilePathList[0]);
		}

		private List<CSClass> AppendClassToList(IEnumerable<CSClass> inputClassList, CSClass classObject)
		{
			var newList = new List<CSClass>(inputClassList??new CSClass[]{});
			var existingClass = inputClassList.SingleOrDefault(i=>i.ClassFullName == classObject.ClassFullName);
			if(existingClass == null)
			{
				newList.Add(classObject);
			}
			else 
			{
				existingClass.AttributeList = existingClass.AttributeList.Union(classObject.AttributeList).ToList();
				existingClass.DependentUponFilePathList = existingClass.DependentUponFilePathList.Union(classObject.DependentUponFilePathList, StringComparer.CurrentCultureIgnoreCase).ToList();
				existingClass.PropertyList = existingClass.PropertyList.Union(classObject.PropertyList).ToList();
				existingClass.FieldList = existingClass.FieldList.Union(classObject.FieldList).ToList();
			}
			return newList;
		}
	}
}
