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
			Assert.AreEqual(designerFileRelativePath, classResult.DesignerFileRelativePath);
			Assert.AreEqual(userFileRelativePath, classResult.UserFileRelativePath);
			Assert.AreEqual(sourceClassFullName, classResult.SourceClassFullName);
			Assert.AreEqual(targetClassFullName, classResult.TargetClassFullName);
			Assert.AreEqual(targetObjectType, classResult.SourceObjectType);
			Assert.AreEqual(expectedUrl, classResult.ExpectedUrl);
		}
	}
}
