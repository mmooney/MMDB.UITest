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
			public void NewTargetClassWithLink() 
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
				            FieldName = "ExistingTargetField"
				        }
				    }
				};
				TargetClass targetClass = new TargetClass();
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				Assert.IsTrue(targetClass.TargetFieldList[0].IsDirty);
				Assert.AreEqual("System.Web.UI.WebControls.HyperLink", targetClass.TargetFieldList[0].SourceClassFullName);
				Assert.AreEqual("ExistingTargetField", targetClass.TargetFieldList[0].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.Link, targetClass.TargetFieldList[0].TargetControlType);
			}

			[Test]
			public void NewTargetClassWithTextBox()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
						new SourceWebControl
						{
				            ClassFullName = "System.Web.UI.WebControls.TextBox",
				            FieldName = "TestTargetField"
						}
				    }
				};
				TargetClass targetClass = new TargetClass();
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				Assert.IsTrue(targetClass.TargetFieldList[0].IsDirty);
				Assert.AreEqual("System.Web.UI.WebControls.TextBox", targetClass.TargetFieldList[0].SourceClassFullName);
				Assert.AreEqual("TestTargetField", targetClass.TargetFieldList[0].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.TextBox, targetClass.TargetFieldList[0].TargetControlType);
			}

			[Test]
			public void NewTargetClassWithLabel()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
						new SourceWebControl
						{
				            ClassFullName = "System.Web.UI.WebControls.Label",
				            FieldName = "TestTargetField"
						}
				    }
				};
				TargetClass targetClass = new TargetClass();
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				Assert.IsTrue(targetClass.TargetFieldList[0].IsDirty);
				Assert.AreEqual("System.Web.UI.WebControls.Label", targetClass.TargetFieldList[0].SourceClassFullName);
				Assert.AreEqual("TestTargetField", targetClass.TargetFieldList[0].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.Label, targetClass.TargetFieldList[0].TargetControlType);
			}

			[Test]
			public void ExistingClassExistingIrrelevantFields() 
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
				            FieldName = "NewTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = "TestSourcenamespace.TestSourceClass",
							SourceFieldName = "ExistingTargetField",
							TargetControlType = EnumTargetControlType.Unknown
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(2, targetClass.TargetFieldList.Count);

				Assert.IsFalse(targetClass.TargetFieldList[0].IsDirty);
				Assert.AreEqual("TestSourcenamespace.TestSourceClass", targetClass.TargetFieldList[0].SourceClassFullName);
				Assert.AreEqual("ExistingTargetField", targetClass.TargetFieldList[0].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.Unknown, targetClass.TargetFieldList[0].TargetControlType);

				Assert.IsTrue(targetClass.TargetFieldList[1].IsDirty);
				Assert.AreEqual("System.Web.UI.WebControls.HyperLink", targetClass.TargetFieldList[1].SourceClassFullName);
				Assert.AreEqual("NewTargetField", targetClass.TargetFieldList[1].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.Link, targetClass.TargetFieldList[1].TargetControlType);
			}

			[Test]
			public void ExistingClassExistingField()
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
				            FieldName = "TestTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = "System.Web.UI.WebControls.HyperLink",
							SourceFieldName = "TestTargetField",
							TargetControlType = EnumTargetControlType.Link
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);

				Assert.IsFalse(targetClass.TargetFieldList[0].IsDirty);
				Assert.AreEqual("System.Web.UI.WebControls.HyperLink", targetClass.TargetFieldList[0].SourceClassFullName);
				Assert.AreEqual("TestTargetField", targetClass.TargetFieldList[0].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.Link, targetClass.TargetFieldList[0].TargetControlType);
			}

			[Test]
			public void ExistingClassExistingFieldChangedSourceType()
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
				            FieldName = "TestTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = "System.Web.UI.WebControls.TextBox",
							SourceFieldName = "TestTargetField",
							TargetControlType = EnumTargetControlType.Link
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);

				Assert.IsTrue(targetClass.TargetFieldList[0].IsDirty);
				Assert.AreEqual("System.Web.UI.WebControls.HyperLink", targetClass.TargetFieldList[0].SourceClassFullName);
				Assert.AreEqual("TestTargetField", targetClass.TargetFieldList[0].SourceFieldName);
				Assert.AreEqual(EnumTargetControlType.Link, targetClass.TargetFieldList[0].TargetControlType);
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
