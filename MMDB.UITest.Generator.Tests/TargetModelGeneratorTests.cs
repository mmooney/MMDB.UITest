using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.UITest.Generator.Library;

namespace MMDB.UITest.Generator.Tests
{
	public class TargetModelGeneratorTests
	{
		[TestFixture]
		public class UpdateProject 
		{
			[Test]
			public void SimpleTargetProject()
			{
				var sourceProject = new SourceWebProject 
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace"
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetProject = targetModelGenerator.UpdateProject(targetProject, sourceProject);
				Assert.AreEqual(1, targetProject.TargetClassList.Count);
				Assert.AreEqual(@"Client\Pages\Test1\TestItemPageClient.designer.cs", targetProject.TargetClassList[0].DesignerFilePath);
				Assert.AreEqual(@"Client\Pages\Test1\TestItemPageClient.cs", targetProject.TargetClassList[0].UserFilePath);
				Assert.AreEqual(@"TestSourceNamespace.Test1.TestItem", targetProject.TargetClassList[0].SourceClassFullName);
				Assert.AreEqual(@"TestTargetNamespace.Client.Pages.Test1.TestItemPageClient", targetProject.TargetClassList[0].TargetClassFullName);
				Assert.AreEqual(EnumTargetObjectType.WebPage,targetProject.TargetClassList[0].TargetObjectType);
				Assert.AreEqual("TestWebPage.aspx", targetProject.TargetClassList[0].PageUrl);
			}

			[Test]
			public void DifferentSourceNamespace()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "SomeOtherNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace"
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetProject = targetModelGenerator.UpdateProject(targetProject, sourceProject);
				Assert.AreEqual(1, targetProject.TargetClassList.Count);
				Assert.AreEqual(@"Client\Pages\SomeOtherNamespace\Test1\TestItemPageClient.designer.cs", targetProject.TargetClassList[0].DesignerFilePath);
				Assert.AreEqual(@"Client\Pages\SomeOtherNamespace\Test1\TestItemPageClient.cs", targetProject.TargetClassList[0].UserFilePath);
				Assert.AreEqual(@"SomeOtherNamespace.Test1.TestItem", targetProject.TargetClassList[0].SourceClassFullName);
				Assert.AreEqual(@"TestTargetNamespace.Client.Pages.SomeOtherNamespace.Test1.TestItemPageClient", targetProject.TargetClassList[0].TargetClassFullName);
				Assert.AreEqual(EnumTargetObjectType.WebPage, targetProject.TargetClassList[0].TargetObjectType);
				Assert.AreEqual("TestWebPage.aspx", targetProject.TargetClassList[0].PageUrl);
			}

			[Test]
			public void ExistingTargetClassInNormalPosition()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace",
					TargetClassList = new List<TargetClass>() 
					{
						new TargetClass
						{
							SourceClassFullName = "TestSourceNamespace.Test1.TestItem",
							DesignerFilePath = @"Client\Pages\Test1\TestItemPageClient.designer.cs",
							UserFilePath = @"Client\Pages\Test1\TestItemPageClient.cs",
							TargetClassFullName = "TestTargetNamespace.Client.Pages.Test1.TestItemPageClient",
							TargetObjectType = EnumTargetObjectType.WebPage,
							PageUrl = "TestWebPage.aspx"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetProject = targetModelGenerator.UpdateProject(targetProject, sourceProject);
				Assert.AreEqual(1, targetProject.TargetClassList.Count);
				Assert.AreEqual(@"Client\Pages\Test1\TestItemPageClient.designer.cs", targetProject.TargetClassList[0].DesignerFilePath);
				Assert.AreEqual(@"Client\Pages\Test1\TestItemPageClient.cs", targetProject.TargetClassList[0].UserFilePath);
				Assert.AreEqual("TestSourceNamespace.Test1.TestItem", targetProject.TargetClassList[0].SourceClassFullName);
				Assert.AreEqual("TestTargetNamespace.Client.Pages.Test1.TestItemPageClient", targetProject.TargetClassList[0].TargetClassFullName);
				Assert.AreEqual(EnumTargetObjectType.WebPage, targetProject.TargetClassList[0].TargetObjectType);
				Assert.AreEqual("TestWebPage.aspx", targetProject.TargetClassList[0].PageUrl);
			}

			[Test]
			public void ExistingTargetClassInDifferentPosition()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace",
					TargetClassList = new List<TargetClass>() 
					{
						new TargetClass
						{
							SourceClassFullName = "TestSourceNamespace.Test1.TestItem",
							DesignerFilePath = @"SomeOtherLocation\TestItemPageClient.designer.cs",
							UserFilePath = @"SomeOtherLocation\TestItemPageClient.cs",
							TargetClassFullName = "SomeOtherLocation.TestItemPageClient",
							TargetObjectType = EnumTargetObjectType.WebPage,
							PageUrl = "TestWebPage.aspx"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetProject = targetModelGenerator.UpdateProject(targetProject, sourceProject);
				Assert.AreEqual(1, targetProject.TargetClassList.Count);
				Assert.AreEqual(@"SomeOtherLocation\TestItemPageClient.designer.cs", targetProject.TargetClassList[0].DesignerFilePath);
				Assert.AreEqual(@"SomeOtherLocation\TestItemPageClient.cs", targetProject.TargetClassList[0].UserFilePath);
				Assert.AreEqual("TestSourceNamespace.Test1.TestItem", targetProject.TargetClassList[0].SourceClassFullName);
				Assert.AreEqual("SomeOtherLocation.TestItemPageClient", targetProject.TargetClassList[0].TargetClassFullName);
				Assert.AreEqual(EnumTargetObjectType.WebPage, targetProject.TargetClassList[0].TargetObjectType);
				Assert.AreEqual("TestWebPage.aspx", targetProject.TargetClassList[0].PageUrl);
			}

			[Test]
			public void ExistingTargetClassWithDifferentName()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace",
					TargetClassList = new List<TargetClass>() 
					{
						new TargetClass
						{
							SourceClassFullName = "TestSourceNamespace.Test1.TestItem",
							DesignerFilePath = @"SomeOtherLocation\SomeOtherClassName.designer.cs",
							UserFilePath = @"SomeOtherLocation\SomeOtherClassName.cs",
							TargetClassFullName = "SomeOtherLocation.SomeOtherClassName",
							TargetObjectType = EnumTargetObjectType.WebPage,
							PageUrl = "TestWebPage.aspx"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetProject = targetModelGenerator.UpdateProject(targetProject, sourceProject);
				Assert.AreEqual(1, targetProject.TargetClassList.Count);
				Assert.AreEqual(@"SomeOtherLocation\SomeOtherClassName.designer.cs", targetProject.TargetClassList[0].DesignerFilePath);
				Assert.AreEqual(@"SomeOtherLocation\SomeOtherClassName.cs", targetProject.TargetClassList[0].UserFilePath);
				Assert.AreEqual("TestSourceNamespace.Test1.TestItem", targetProject.TargetClassList[0].SourceClassFullName);
				Assert.AreEqual("SomeOtherLocation.SomeOtherClassName", targetProject.TargetClassList[0].TargetClassFullName);
				Assert.AreEqual(EnumTargetObjectType.WebPage, targetProject.TargetClassList[0].TargetObjectType);
				Assert.AreEqual("TestWebPage.aspx", targetProject.TargetClassList[0].PageUrl);
			}

		}

		[TestFixture]
		public class UpdateClass
		{
			[Test]
			public void NewTargetClass() 
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
				        new SourceWebControl
				        {
				            ClassFullName = "System.Web.UI.WebControls.HyperLink",
				            FieldName = "TestLink"
				        }
				    }
				};
				var targetModelGenerator = new TargetModelGenerator();
				TargetClass targetClass = targetModelGenerator.UpdateClass(sourceWebPage, new TargetClass());
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				Assert.IsTrue(targetClass.TargetFieldList[0].IsDirty);
				Assert.AreEqual("System.Web.UI.WebControls.HyperLink", targetClass.TargetFieldList[0].SourceClassFullName);
				Assert.AreEqual("TestLink", targetClass.TargetFieldList[0].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.Link, targetClass.TargetFieldList[0].TargetControlType);
			}

			[Test]
			public void ExistingClassNewFields() 
			{
				Assert.Fail();
			}

			[Test]
			public void ExistingClassExistingFields()
			{
				Assert.Fail();
			}

			[Test]
			public void ExistingClassChangedTargetFieldName()
			{
				Assert.Fail();
			}

			[Test]
			public void ExistingClassChangedTargetFieldType()
			{
				Assert.Fail();
			}
		}

		[TestFixture]
		public class LoadProjectFile
		{
			[Test]
			public void LoadBasicFile()
			{
				Assert.Fail();
			}
		}

	}
}
