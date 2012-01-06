using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ScrewturnWikiProxySample
{
	[MMDB.UITest.Core.UIClientPageAttribute("testNamespaceName.testClassName")]
	[Serializable]
	[XmlType(IncludeInSchema=false)]
	public partial class TestClass
	{
		[XmlAttribute("test")]
		public int TestProperty { get; set; }
	}
}
