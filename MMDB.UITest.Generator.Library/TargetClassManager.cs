using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;

namespace MMDB.UITest.Generator.Library
{
	public class TargetClassManager
	{
		public TargetClass TryLoadTargetClass(CSClass csClass)
		{
			TargetClass returnValue = null;
			var uiClientPageAttribute = csClass.AttributeList.SingleOrDefault(i => i.TypeFullName == typeof(UIClientPageAttribute).FullName);
			if (uiClientPageAttribute != null)
			{
				returnValue = new TargetClass
				{
					SourceClassFullName = Convert.ToString(uiClientPageAttribute.GetAttributeParameter(0, "sourceClassFullName", true)),
					TargetClassFullName = csClass.ClassFullName,
					//ExpectedUrl = Convert.ToString(uiClientPageAttribute.GetAttributeParameter(1, "ExpectedUrl", false))
				};
				
				//If there is only one file, that is the user and designer file.
				//If there are two or more files and one ends with ".designer.cs", that is the designer file and the the first of the others is the user file
				//If there are two or more files and none ends with ".designer.cs", then the first one is the designer and user file
				if (csClass.FileRelativePathList.Count == 1)
				{
					returnValue.DesignerFileRelativePath = csClass.FileRelativePathList[0];
					returnValue.UserFileRelativePath = csClass.FileRelativePathList[0];
				}
				else if (csClass.FileRelativePathList.Count > 1)
				{
					returnValue.DesignerFileRelativePath = csClass.FileRelativePathList.FirstOrDefault(i => i.EndsWith(".designer.cs", StringComparison.CurrentCultureIgnoreCase));
					if (string.IsNullOrEmpty(returnValue.DesignerFileRelativePath))
					{
						returnValue.DesignerFileRelativePath = csClass.FileRelativePathList[0];
						returnValue.UserFileRelativePath = csClass.FileRelativePathList[0];
					}
					else
					{
						returnValue.UserFileRelativePath = csClass.FileRelativePathList.FirstOrDefault(i => i != returnValue.DesignerFileRelativePath);
					}
				}

				foreach (var csProperty in csClass.PropertyList)
				{
					var targetField = TryLoadField(csProperty);
					if (targetField != null)
					{
						returnValue.TargetFieldList.Add(targetField);
					}
				}
			}
			return returnValue;
		}

		public TargetField TryLoadField(CSProperty csProperty)
		{
			TargetField returnValue = null;
			var uiClientPropertyAttribute = csProperty.AttributeList.SingleOrDefault(i => i.TypeFullName == typeof(UIClientPropertyAttribute).FullName);
			if (uiClientPropertyAttribute != null)
			{
				string sourceFieldName = Convert.ToString(uiClientPropertyAttribute.GetAttributeParameter(0, "SourceFieldName", true));
				string sourceClassFullName = Convert.ToString(uiClientPropertyAttribute.GetAttributeParameter(1, "SourceFieldTypeFullName", true));
				returnValue = new TargetField
				{
					SourceFieldName = sourceFieldName,
					SourceClassFullName = sourceClassFullName,
					TargetFieldName = csProperty.PropertyName,
					TargetControlType = GetTargetControlType(sourceClassFullName)
				};
			}
			return returnValue;

		}

		public EnumTargetControlType GetTargetControlType(string sourceClassFullName)
		{
			EnumTargetControlType returnValue = EnumTargetControlType.Unknown;
			switch (sourceClassFullName)
			{
				case "System.Web.UI.WebControls.HyperLink":
					returnValue = EnumTargetControlType.Link;
					break;
				case "System.Web.UI.WebControls.TextBox":
					returnValue = EnumTargetControlType.TextBox;
					break;
				case "System.Web.UI.WebControls.Label":
					returnValue = EnumTargetControlType.Label;
					break;
			}
			return returnValue;
		}
	}
}
