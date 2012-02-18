﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.RegularExpressions;
using Telerik.RazorConverter.WebForms.Parsing;
using Telerik.RazorConverter.WebForms.Filters;
using Telerik.RazorConverter.WebForms.DOM;

namespace MMDB.UITest.DotNetParser.WebForms
{
	public class CSWebFormParser
	{
		public WebFormContainer ParseString(string input)
		{
			WebFormContainer returnValue;
			var nodeFactory = new WebFormsNodeFactory();
			var codeGroupNodeFactory = new WebFormsCodeGroupFactory();
			var nodeFilterProvider = new WebFormsNodeFilterProvider(codeGroupNodeFactory);
			WebFormsParser telerikParser = new WebFormsParser(nodeFactory, nodeFilterProvider);
			var document = telerikParser.Parse(input);
			
			var directiveNode = document.RootNode.Children.FirstOrDefault(i=>i is DirectiveNode);
			if(directiveNode == null)
			{
				throw new ArgumentException("Failed to find directive node");
			}
			if(directiveNode.Attributes.ContainsKey("page"))
			{
				returnValue = new WebFormPage
				{
					CodeBehindFile = directiveNode.Attributes["codebehind"],
					ClassFullName = directiveNode.Attributes["inherits"]
				};
				var controlList = document.RootNode.Children.Where(i=>i is ServerControlNode);
				foreach (ServerControlNode control in controlList)
				{
					this.LoadControl(control, returnValue);
				}
			}
			else 
			{
				returnValue = null;
			}
			return returnValue;
		}

		public void LoadControl(ServerControlNode controlNode, WebFormContainer container, string prefix=null)
		{
			var prependingTagNames = new string[] 
			{
				"asp:panel"
			};
			var controlItem = new WebFormServerControl
			{
				TagName = controlNode.TagName,
				ControlID = controlNode.Attributes["id"],
				Prefix = prefix
			};
			container.Controls.Add(controlItem);
			string newPrefix = prefix ?? string.Empty;
			if(prependingTagNames.Contains(controlItem.TagName, StringComparer.CurrentCultureIgnoreCase))
			{
				newPrefix += controlItem.ControlID + "_";
			}
			var childControlNodeList = controlNode.Children.Where(i=>i is ServerControlNode);
			foreach(ServerControlNode childControlNode in childControlNodeList)
			{
				this.LoadControl(childControlNode, container, newPrefix);
			}
		}
	}
}
