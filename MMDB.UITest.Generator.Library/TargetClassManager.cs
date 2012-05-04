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
			var uiClientPageAttribute = csClass.AttributeList.SingleOrDefault(i => i.TypeName == typeof(UIClientPageAttribute).Name && i.TypeNamespace == typeof(UIClientPageAttribute).Namespace);
			if (uiClientPageAttribute != null)
			{
				returnValue = new TargetClass
				{
					SourceClassFullName = Convert.ToString(uiClientPageAttribute.GetAttributeParameter(0, "SourceClassFullName", true)),
					TargetClassFullName = csClass.ClassFullName
				};

				//If there is only one field, that is the user and designer file.
				//If there are two or more files and one ends with ".designer.cs", that is the designer file and the the first of the others is the user file
				//If there are two or more files and none ends with ".designer.cs", then the first one is the designer and user file
				if (csClass.FilePathList.Count == 1)
				{
					returnValue.DesignerFilePath = csClass.FilePathList[0];
					returnValue.UserFilePath = csClass.FilePathList[0];
				}
				else if (csClass.FilePathList.Count > 1)
				{
					returnValue.DesignerFilePath = csClass.FilePathList.FirstOrDefault(i => i.EndsWith(".designer.cs", StringComparison.CurrentCultureIgnoreCase));
					if (string.IsNullOrEmpty(returnValue.DesignerFilePath))
					{
						returnValue.DesignerFilePath = csClass.FilePathList[0];
						returnValue.UserFilePath = csClass.FilePathList[0];
					}
					else
					{
						returnValue.UserFilePath = csClass.FilePathList.FirstOrDefault(i => i != returnValue.DesignerFilePath);
					}
				}

				foreach (var csProperty in csClass.PropertyList)
				{
					var targetField = TargetField.TryLoad(csProperty);
					if (targetField != null)
					{
						returnValue.TargetFieldList.Add(targetField);
					}
				}
			}
			return returnValue;
		}
	}
}
