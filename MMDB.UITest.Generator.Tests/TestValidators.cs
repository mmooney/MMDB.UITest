using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.Generator.Library;
using NUnit.Framework;

namespace MMDB.UITest.Generator.Tests
{
	public static class TestValidators
	{
		public static void ValidateTargetField(TargetField targetField, bool isDirty, string sourceClassFullName, string sourceFieldName, EnumTargetControlType targetControlType, string targetFieldName)
		{
			Assert.AreEqual(isDirty, targetField.IsDirty);
			Assert.AreEqual(sourceClassFullName, targetField.SourceClassFullName);
			Assert.AreEqual(sourceFieldName, targetField.SourceFieldName);
			Assert.AreEqual(targetControlType, targetField.TargetControlType);
			Assert.AreEqual(targetFieldName, targetField.TargetFieldName);
		}

		public static void ValidateTargetClassComparisonResult(TargetClassComparisonResult classResult, string designerFileRelativePath, string userFileRelativePath, string sourceClassFullName, string targetClassFullName, EnumSourceObjectType targetObjectType, string expectedUrl)
		{
			Assert.AreEqual(@"Client\Pages\Test1\TestItemPageClient.designer.cs", designerFileRelativePath);
			Assert.AreEqual(@"Client\Pages\Test1\TestItemPageClient.cs", userFileRelativePath);
			Assert.AreEqual(@"TestSourceNamespace.Test1.TestItem", sourceClassFullName);
			Assert.AreEqual(@"TestTargetNamespace.Client.Pages.Test1.TestItemPageClient", targetClassFullName);
			Assert.AreEqual(EnumSourceObjectType.WebPage, targetObjectType);
			Assert.AreEqual("TestWebPage.aspx", expectedUrl);
		}
	}
}
