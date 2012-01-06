using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;

namespace MMDB.UITest.Generator.Library
{
	public class TargetField
	{
		public string FieldName { get; set; }
		public string TypeFullName { get; set; }

		public static TargetField TryLoad(CSProperty csProperty)
		{
			TargetField returnValue = null;
			var uiClientPropertyAttribute = csProperty.AttributeList.SingleOrDefault(i => i.TypeName == typeof(UIClientPropertyAttribute).Name && i.TypeNamespace == typeof(UIClientPropertyAttribute).Namespace);
			if (uiClientPropertyAttribute != null)
			{
				returnValue = new TargetField
				{
					FieldName = uiClientPropertyAttribute.GetAttributeParameter(0, "SourceFieldName", true),
					TypeFullName = uiClientPropertyAttribute.GetAttributeParameter(1, "SourceFieldTypeFullName", true)
				};
			}
			return returnValue;

		}
	}
}
