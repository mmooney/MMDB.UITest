using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.DotNetParser.WebForms
{
	public enum NodeType
	{
		Document,
		Directive,
		CodeBlock,
		ExpressionBlock,
		EncodedExpressionBlock,
		ServerControl,
		Text,
		Comment
	}
	
	public class WebFormsNode : IWebFormsNode
	{
		public NodeType Type { get; set; }
	}
}
