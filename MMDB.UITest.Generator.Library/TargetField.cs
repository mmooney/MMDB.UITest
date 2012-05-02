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
		public string SourceFieldName { get; set; }
		public string SourceClassFullName 
		{ 
			get 
			{
				return DotNetParserHelper.BuildFullName(this.SourceNamespaceName, this.SourceClassName);
			}
			set 
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.SourceClassName = className;
				this.SourceNamespaceName = namespaceName;
			}
		}
		public string SourceClassName { get; set; }
		public string SourceNamespaceName { get; set; }
		public EnumTargetControlType TargetControlType { get; set; }
		public bool IsDirty { get; set; }

		public static TargetField TryLoad(CSProperty csProperty)
		{
			TargetField returnValue = null;
			var uiClientPropertyAttribute = csProperty.AttributeList.SingleOrDefault(i => i.TypeName == typeof(UIClientPropertyAttribute).Name && i.TypeNamespace == typeof(UIClientPropertyAttribute).Namespace);
			if (uiClientPropertyAttribute != null)
			{
				returnValue = new TargetField
				{
					SourceFieldName = Convert.ToString(uiClientPropertyAttribute.GetAttributeParameter(0, "SourceFieldName", true)),
					SourceClassFullName = Convert.ToString(uiClientPropertyAttribute.GetAttributeParameter(1, "SourceFieldTypeFullName", true))
				};
			}
			return returnValue;

		}
	}
}
