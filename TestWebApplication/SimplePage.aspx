<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimplePage.aspx.cs" Inherits="TestWebApplication.SimplePage" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<body>
    <form id="form1" runat="server">
		<div>
			<asp:Label ID="_lblOutside" runat="server" />
			<asp:Panel ID="_pnlPanel1" runat="server">
				<asp:Label ID="_lblInside" runat="server" />
			</asp:Panel>
		</div>
    </form>
</body>
</html>
