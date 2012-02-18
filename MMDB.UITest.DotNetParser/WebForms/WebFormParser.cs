using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.RegularExpressions;

namespace MMDB.UITest.DotNetParser.WebForms
{
	public class WebFormParser
	{
		private static Regex _directiveRegex = new DirectiveRegex();
		private static Regex _commentRegex = new Regex(@"\G<%--(?<comment>.*?)--%>", RegexOptions.Multiline | RegexOptions.Singleline);
		private static Regex _startTagOpeningBracketRegex = new Regex(@"\G<[^%\/]", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
		private static Regex _endTagRegex = new EndTagRegex();
		private static Regex _aspCodeRegex = new AspCodeRegex();
		private static Regex _aspExprRegex = new AspExprRegex();
		private static Regex _aspEncodedExprRegex = new AspEncodedExprRegex();
		private static Regex _textRegex = new TextRegex();
		private static Regex _runatServerTagRegex = new Regex(@"\G<(?<tagname>[\w:\.]+)(?<attributes>[^>]*?(?:runat\W*server){1}[^>]*?)(?<empty>/)?>", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.Compiled);
		private static Regex _scriptRegex = new Regex(@"\G\s*\<script.*?\>.*?\<\/script\>\s*", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);;
		private static Regex _doctypeRegex = new Regex(@"\G\s*\<!DOCTYPE.*?\>\s*", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

		//Borrowed much of this from https://github.com/telerik/razor-converter
		public WebFormFile ParseString(string input)
		{
			Match match;
			int startAt = 0;

			var root = new WebFormsNode { Type = NodeType.Document };
			IWebFormsNode parentNode = root;

			do
			{
				if ((match = _textRegex.Match(input, startAt)).Success)
				{
					AppendTextNode(parentNode, match);
					startAt = match.Index + match.Length;
				}

				if (startAt != input.Length)
				{
					if ((match = _directiveRegex.Match(input, startAt)).Success)
					{
						var directiveNode = this.NodeFactory.CreateNode(match, NodeType.Directive);
						parentNode.Children.Add(directiveNode);
					}
					else if ((match = commentRegex.Match(input, startAt)).Success)
					{
						var commentNode = NodeFactory.CreateNode(match, NodeType.Comment);
						parentNode.Children.Add(commentNode);
					}
					else if ((match = runatServerTagRegex.Match(input, startAt)).Success)
					{
						var serverControlNode = NodeFactory.CreateNode(match, NodeType.ServerControl);
						parentNode.Children.Add(serverControlNode);
						parentNode = serverControlNode;
					}
					else if ((match = doctypeRegex.Match(input, startAt)).Success)
					{
						AppendTextNode(parentNode, match);
					}
					else if ((match = scriptRegex.Match(input, startAt)).Success)
					{
						AppendTextNode(parentNode, match);
					}
					else if ((match = startTagOpeningBracketRegex.Match(input, startAt)).Success)
					{
						AppendTextNode(parentNode, match);
					}
					else if ((match = endTagRegex.Match(input, startAt)).Success)
					{
						var tagName = match.Groups["tagname"].Captures[0].Value;
						var serverControlParent = parentNode as IWebFormsServerControlNode;
						if (serverControlParent != null && tagName.ToLowerInvariant() == serverControlParent.TagName.ToLowerInvariant())
						{
							parentNode = parentNode.Parent;
						}
						else
						{
							AppendTextNode(parentNode, match);
						}
					}
					else if ((match = aspExprRegex.Match(input, startAt)).Success || (match = aspEncodedExprRegex.Match(input, startAt)).Success)
					{
						var expressionBlockNode = NodeFactory.CreateNode(match, NodeType.ExpressionBlock);
						parentNode.Children.Add(expressionBlockNode);
					}
					else if ((match = aspCodeRegex.Match(input, startAt)).Success)
					{
						var codeBlockNode = NodeFactory.CreateNode(match, NodeType.CodeBlock);
						parentNode.Children.Add(codeBlockNode);
					}
					else
					{
						throw new Exception(
							string.Format("Unrecognized page element: {0}...", input.Substring(startAt, 20)));
					}

					startAt = match.Index + match.Length;
				}
			}
			while (startAt != input.Length);

			ApplyPostprocessingFilters(root);

		}

		private void AppendTextNode(IWebFormsNode parentNode, Match match)
		{
			throw new NotImplementedException();
		}
	}
}
