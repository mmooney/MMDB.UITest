using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.DotNetParser.WebForms
{
	public class WebFormContainer
	{
		public enum EnumWebFormContainerType
		{
			Unknown,
			WebPage,
			MasterPage,
			UserControl
		}

		public List<WebFormServerControl> Controls { get; set; }
		public EnumWebFormContainerType ContainerType { get; set; }
		public string CodeBehindFile { get; set; }
		public string ClassName { get; set; }
		public string NamespaceName { get; set; }
		public string ClassFullName 
		{ 
			get
			{
				return DotNetParserHelper.BuildFullName(this.NamespaceName, this.ClassName);
			}
			set
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.ClassName = className;
				this.NamespaceName = namespaceName;
			}
		}

		public WebFormContainer()
		{
			this.Controls = new List<WebFormServerControl>();
		}

	}
}
