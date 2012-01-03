using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ScrewturnWikiProxySample
{
	[MMDB.UITest.Core.UIClient("testClassName","testNamespaceName")]
	[Serializable]
	[XmlType(IncludeInSchema=false)]
	public partial class TestClass
	{
		public int TestProperty { get; set; }
	}
}
