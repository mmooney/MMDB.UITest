﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.UITest.DotNetParser.WebForms;

namespace MMDB.UITest.DotNetParser.Tests
{
	[TestFixture]
	public class WebFormParserTests 
	{
		[Test]
		public void BasicWebFormTest()
		{
			string data = 
			@"
				<%@ Page Language=""C#"" AutoEventWireup=""true"" CodeBehind=""SimplePage.aspx.cs"" Inherits=""TestWebApplication.SimplePage"" %>
				<html xmlns=""http://www.w3.org/1999/xhtml"">
				<body>
					<form id=""form1"" runat=""server"">
						<div>
							<asp:Label ID=""_lblTest"" runat=""server"" />
							<asp:TextBox ID=""_txtTest"" runat=""server"" />    
						</div>
					</form>
				</body>
				</html>
			";
			var parser = new CSWebFormParser();
			var result = parser.ParseString(data, "C:\\Test\\Test.aspx");
			Assert.IsNotNull(result);
			Assert.AreEqual("TestWebApplication.SimplePage", result.ClassFullName);
			Assert.AreEqual("SimplePage.aspx.cs", result.CodeBehindFile);
			Assert.AreEqual(EnumWebFormContainerType.WebPage, result.ContainerType);
			Assert.AreEqual("C:\\Test\\Test.aspx", result.FilePath);

			Assert.AreEqual(3, result.Controls.Count);

			Assert.AreEqual("form", result.Controls[0].TagName);
			Assert.AreEqual("form1", result.Controls[0].ControlID);
			Assert.IsNullOrEmpty(result.Controls[0].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[1].TagName);
			Assert.AreEqual("_lblTest", result.Controls[1].ControlID);
			Assert.IsNullOrEmpty(result.Controls[1].Prefix);

			Assert.AreEqual("asp:TextBox", result.Controls[2].TagName);
			Assert.AreEqual("_txtTest", result.Controls[2].ControlID);
			Assert.IsNullOrEmpty(result.Controls[2].Prefix);

		}

		[Test]
		public void TestPanel()
		{
			string data =
			@"
				<%@ Page Language=""C#"" AutoEventWireup=""true"" CodeBehind=""SimplePage.aspx.cs"" Inherits=""TestWebApplication.SimplePage"" %>
				<html xmlns=""http://www.w3.org/1999/xhtml"">
				<body>
					<form id=""form1"" runat=""server"">
						<div>
							<asp:Label ID=""_lblOutside1"" runat=""server"" />
							<asp:Panel ID=""_pnlPanel1"" runat=""server"">
								<asp:Label ID=""_lblInside"" runat=""server"" />
							</asp:Panel>
							<asp:Label ID=""_lblOutside2"" runat=""server"" />
						</div>
					</form>
				</body>
				</html>
			";
			var parser = new CSWebFormParser();
			var result = parser.ParseString(data, "C:\\Test\\Test.aspx");
			Assert.IsNotNull(result);
			Assert.AreEqual("C:\\Test\\Test.aspx", result.FilePath);

			Assert.AreEqual(5, result.Controls.Count);

			Assert.AreEqual("form", result.Controls[0].TagName);
			Assert.AreEqual("form1", result.Controls[0].ControlID);
			Assert.IsNullOrEmpty(result.Controls[0].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[1].TagName);
			Assert.AreEqual("_lblOutside1", result.Controls[1].ControlID);
			Assert.IsNullOrEmpty(result.Controls[1].Prefix);

			Assert.AreEqual("asp:Panel", result.Controls[2].TagName);
			Assert.AreEqual("_pnlPanel1", result.Controls[2].ControlID);
			Assert.IsNullOrEmpty(result.Controls[2].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[3].TagName);
			Assert.AreEqual("_lblInside", result.Controls[3].ControlID);
			Assert.AreEqual("_pnlPanel1_", result.Controls[3].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[4].TagName);
			Assert.AreEqual("_lblOutside2", result.Controls[4].ControlID);
			Assert.IsNullOrEmpty(result.Controls[4].Prefix);
		}

		[Test]
		public void TestNextedPanels()
		{
			string data =
			@"
				<%@ Page Language=""C#"" AutoEventWireup=""true"" CodeBehind=""SimplePage.aspx.cs"" Inherits=""TestWebApplication.SimplePage"" %>
				<html xmlns=""http://www.w3.org/1999/xhtml"">
				<body>
					<form id=""form1"" runat=""server"">
						<div>
							<asp:Label ID=""_lblOutside1"" runat=""server"" />
							<asp:Panel ID=""_pnlPanel1"" runat=""server"">
								<asp:Label ID=""_lblInside1"" runat=""server"" />
								<asp:Panel ID=""_pnlPanel2"" runat=""server"">
									<asp:Label ID=""_lblInside2"" runat=""server"" />
								</asp:Panel>
							</asp:Panel>
							<asp:Label ID=""_lblOutside2"" runat=""server"" />
						</div>
					</form>
				</body>
				</html>
			";
			var parser = new CSWebFormParser();
			var result = parser.ParseString(data, "C:\\Test\\Test.aspx");
			Assert.IsNotNull(result);
			Assert.AreEqual("C:\\Test\\Test.aspx", result.FilePath);

			Assert.AreEqual(7, result.Controls.Count);

			Assert.AreEqual("form", result.Controls[0].TagName);
			Assert.AreEqual("form1", result.Controls[0].ControlID);
			Assert.IsNullOrEmpty(result.Controls[0].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[1].TagName);
			Assert.AreEqual("_lblOutside1", result.Controls[1].ControlID);
			Assert.IsNullOrEmpty(result.Controls[1].Prefix);

			Assert.AreEqual("asp:Panel", result.Controls[2].TagName);
			Assert.AreEqual("_pnlPanel1", result.Controls[2].ControlID);
			Assert.IsNullOrEmpty(result.Controls[2].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[3].TagName);
			Assert.AreEqual("_lblInside1", result.Controls[3].ControlID);
			Assert.AreEqual("_pnlPanel1_", result.Controls[3].Prefix);

			Assert.AreEqual("asp:Panel", result.Controls[4].TagName);
			Assert.AreEqual("_pnlPanel2", result.Controls[4].ControlID);
			Assert.AreEqual("_pnlPanel1_", result.Controls[4].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[5].TagName);
			Assert.AreEqual("_lblInside2", result.Controls[5].ControlID);
			Assert.AreEqual("_pnlPanel1__pnlPanel2_", result.Controls[5].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[6].TagName);
			Assert.AreEqual("_lblOutside2", result.Controls[6].ControlID);
			Assert.IsNullOrEmpty(result.Controls[6].Prefix);
		}

		[Test]
		public void TestMasterPage()
		{
			string data =
			@"
				<%@ Master Language=""C#"" AutoEventWireup=""true"" CodeBehind=""TestMasterPage.master.cs"" Inherits=""TestWebApplication.TestMasterPage"" %>
				<html xmlns=""http://www.w3.org/1999/xhtml"">
				<head runat=""server"">
					<title></title>
					<asp:ContentPlaceHolder ID=""head"" runat=""server"">
					</asp:ContentPlaceHolder>
				</head>
				<body>
					<form id=""form1"" runat=""server"">
					<div>
						<asp:ContentPlaceHolder ID=""ContentPlaceHolder1"" runat=""server"">
						</asp:ContentPlaceHolder>
						<asp:Panel id=""_pnlTest"" runat=""server"">
							<asp:ContentPlaceHolder ID=""ContentPlaceHolder2"" runat=""server"">
							</asp:ContentPlaceHolder>
						</asp:Panel>
					</div>
					</form>
				</body>
				</html>
			";
			var parser = new CSWebFormParser();
			var result = parser.ParseString(data, "C\\Test\\Test.Master");
			Assert.AreEqual("TestMasterPage.master.cs", result.CodeBehindFile);
			Assert.AreEqual("TestWebApplication.TestMasterPage", result.ClassFullName);
			Assert.AreEqual(EnumWebFormContainerType.MasterPage, result.ContainerType);
			Assert.AreEqual("C\\Test\\Test.Master", result.FilePath);
			
			Assert.AreEqual(5, result.Controls.Count);
		
			Assert.AreEqual("asp:ContentPlaceHolder", result.Controls[0].TagName);
			Assert.AreEqual("head", result.Controls[0].ControlID);
			Assert.IsNullOrEmpty(result.Controls[0].Prefix);

			Assert.AreEqual("form", result.Controls[1].TagName);
			Assert.AreEqual("form1", result.Controls[1].ControlID);
			Assert.IsNullOrEmpty(result.Controls[1].Prefix);

			Assert.AreEqual("asp:ContentPlaceHolder", result.Controls[2].TagName);
			Assert.AreEqual("ContentPlaceHolder1", result.Controls[2].ControlID);
			Assert.IsNullOrEmpty(result.Controls[2].Prefix);

			Assert.AreEqual("asp:Panel", result.Controls[3].TagName);
			Assert.AreEqual("_pnlTest", result.Controls[3].ControlID);
			Assert.IsNullOrEmpty(result.Controls[3].Prefix);

			Assert.AreEqual("asp:ContentPlaceHolder", result.Controls[4].TagName);
			Assert.AreEqual("ContentPlaceHolder2", result.Controls[4].ControlID);
			Assert.AreEqual("_pnlTest_", result.Controls[4].Prefix);
		}

		[Test]
		public void TestUserControl()
		{
			string data = 
			@"
				<%@ Control Language=""C#"" AutoEventWireup=""true"" CodeBehind=""SimpleUserControl.ascx.cs"" Inherits=""TestWebApplication.SimpleUserControl"" %>
				<asp:Label ID=""_lblTest1"" runat=""server""/>
				<asp:TextBox ID=""_txtTest1"" runat=""server"" />
				<asp:Label ID=""_lblTest2"" runat=""server""/>
				<asp:TextBox ID=""_txtTest2"" runat=""server"" />
			";
			var parser = new CSWebFormParser();
			var result = parser.ParseString(data, "C\\Test\\Test.ascx");
			Assert.AreEqual(EnumWebFormContainerType.UserControl, result.ContainerType);
			Assert.AreEqual("SimpleUserControl.ascx.cs", result.CodeBehindFile);
			Assert.AreEqual("TestWebApplication.SimpleUserControl", result.ClassFullName);
			Assert.AreEqual("C\\Test\\Test.ascx", result.FilePath);

			Assert.AreEqual(4, result.Controls.Count);

			Assert.AreEqual("asp:Label", result.Controls[0].TagName);
			Assert.AreEqual("_lblTest1", result.Controls[0].ControlID);
			Assert.IsNullOrEmpty(result.Controls[0].Prefix);

			Assert.AreEqual("asp:TextBox", result.Controls[1].TagName);
			Assert.AreEqual("_txtTest1", result.Controls[1].ControlID);
			Assert.IsNullOrEmpty(result.Controls[1].Prefix);

			Assert.AreEqual("asp:Label", result.Controls[2].TagName);
			Assert.AreEqual("_lblTest2", result.Controls[2].ControlID);
			Assert.IsNullOrEmpty(result.Controls[2].Prefix);

			Assert.AreEqual("asp:TextBox", result.Controls[3].TagName);
			Assert.AreEqual("_txtTest2", result.Controls[3].ControlID);
			Assert.IsNullOrEmpty(result.Controls[3].Prefix);
		}

		[Test]
		public void TestBadData()
		{
			string data = "this data is noooo gooooood";
			var parser = new CSWebFormParser();
			Assert.Throws(typeof(ArgumentException), delegate { parser.ParseString(data, "C\\Test\\Test.aspx"); });
		}
	}
}
