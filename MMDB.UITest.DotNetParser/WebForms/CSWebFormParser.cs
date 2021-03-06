﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.RegularExpressions;
using Telerik.RazorConverter.WebForms.Parsing;
using Telerik.RazorConverter.WebForms.Filters;
using Telerik.RazorConverter.WebForms.DOM;
using System.IO;

namespace MMDB.UITest.DotNetParser.WebForms
{
	public class CSWebFormParser
	{
		public virtual WebFormContainer ParseFile(string filePath)
		{
			string data = File.ReadAllText(filePath);
			return this.ParseString(data, filePath);
		}

		public virtual WebFormContainer ParseString(string input, string filePath)
		{
			WebFormContainer returnValue;
			var nodeFactory = new WebFormsNodeFactory();
			var codeGroupNodeFactory = new WebFormsCodeGroupFactory();
			var nodeFilterProvider = new WebFormsNodeFilterProvider(codeGroupNodeFactory);
			WebFormsParser telerikParser = new WebFormsParser(nodeFactory, nodeFilterProvider);
			var document = telerikParser.Parse(input);
			
			var directiveNode = document.RootNode.Children.FirstOrDefault(i=>i is DirectiveNode && !i.Attributes.ContainsKey("Register"));
			if(directiveNode == null)
			{
				throw new ArgumentException("Failed to find directive node");
			}
			returnValue = new WebFormContainer
			{
				CodeBehindFile = directiveNode.Attributes["codebehind"],
				ClassFullName = directiveNode.Attributes["inherits"],
				FilePath = filePath
			};
			if (directiveNode.Attributes.ContainsKey("page"))
			{
				returnValue.ContainerType = EnumWebFormContainerType.WebPage;
			}
			else if (directiveNode.Attributes.ContainsKey("master"))
			{
				returnValue.ContainerType = EnumWebFormContainerType.MasterPage;
			}
			else if(directiveNode.Attributes.ContainsKey("control"))
			{
				returnValue.ContainerType = EnumWebFormContainerType.UserControl;
			}
			else
			{
				throw new Exception("Unrecognized directive");
			}
			var controlList = document.RootNode.Children.Where(i=>i is ServerControlNode);
			foreach (ServerControlNode control in controlList)
			{
				this.LoadControl(control, returnValue);
			}
			return returnValue;
		}

		public void LoadControl(ServerControlNode controlNode, WebFormContainer container, string prefix=null)
		{
			var prependingTagNames = new string[] 
			{
				"asp:panel"
			};
			string newPrefix = prefix ?? string.Empty;
			if (controlNode.Attributes.ContainsKey("id"))
			{
				var controlItem = new WebFormServerControl
				{
					TagName = controlNode.TagName,
					ControlID = controlNode.Attributes["id"],
					Prefix = prefix
				};
				container.Controls.Add(controlItem);
				if (prependingTagNames.Contains(controlItem.TagName, StringComparer.CurrentCultureIgnoreCase))
				{
					newPrefix += controlItem.ControlID + "_";
				}
			}
			var childControlNodeList = controlNode.Children.Where(i=>i is ServerControlNode);
			foreach(ServerControlNode childControlNode in childControlNodeList)
			{
				this.LoadControl(childControlNode, container, newPrefix);
			}
		}

	}
}
