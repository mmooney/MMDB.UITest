using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;

namespace MMDB.UITest.Generator.Library
{
	public class TargetClass
	{
		public string SourceClassName { get; set; }
		public string SourceClassNamespace { get; set;}

		public TargetClass()
		{
		}


		public static bool IsTargetClass(CSClass csClass)
		{
			return csClass.AttributeList.Any(i=>i.TypeName == "UIClient" && i.TypeNamespace  ==  typeof(UIClientAttribute).Namespace);
		}

		internal static TargetClass TryLoad(CSClass csClass)
		{
			TargetClass returnValue = null;
			var uiClientAttribute = csClass.AttributeList.SingleOrDefault(i=>i.TypeName == "UIClient" && i.TypeNamespace  ==  typeof(UIClientAttribute).Namespace);
			if(uiClientAttribute != null)
			{
				returnValue = new TargetClass
				{
					SourceClassName = uiClientAttribute.GetAttributeParameter(0, "SourceClassName", true),
					SourceClassNamespace = uiClientAttribute.GetAttributeParameter(1, "SourceClassNamespace", true)
				};
			}
			return returnValue;
		}
	}
}
