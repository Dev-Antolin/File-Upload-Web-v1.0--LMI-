<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MainMenu.aspx.cs" Inherits="Login" %>

<%@ Register Assembly="FlashControl" Namespace="Bewise.Web.UI.WebControls" TagPrefix="Bewise" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="CSS/Main.css" rel="stylesheet" type="text/css" />
    <title>Main Menu Page</title>
    <style type="text/css">
        .style1
        {
            color: #FFFFFF;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div id="outerDiv">
        <div id="header">
            <div id="mlkpLogo1">
            </div>
            <div id="mlkpLogo2">
            </div>
        </div>
    </div>
    <div id="menuDiv">
        <div id="menuHolder">
            <h2 class="style1">
                Web Upload version 2</h2>
        </div>
    </div>
    <div id="content" style="background-color: #E4E6E7; height: 378px;">
        <div style="background-color: #990000;">
            <div style="margin-left: 180px;">
                <asp:Label ID="Label1" runat="server" Text="Uploaded Files" Font-Bold="True" Font-Size="Larger"
                    ForeColor="WHITE"></asp:Label>
            </div>
        </div>
        <asp:Panel ID="PanelonGrid" runat="server">
            <div style="margin-left: 180px; width: 553px; height: 300px; overflow: scroll; margin-top: 15px">
                <%-- <div align="center">
            <br />--%>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="White"
                    BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" Width="535px"
                    OnSelectedIndexChanged="GridView1_SelectedIndexChanged1">
                    <RowStyle BackColor="White" ForeColor="#330099" />
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" ButtonType="Button">
                            <ControlStyle Font-Strikeout="False" />
                            <ItemStyle HorizontalAlign="Center" Width="16px" />
                        </asp:CommandField>
                        <asp:TemplateField HeaderText="Filename">
                            <ItemTemplate>
                                <%#Container.DataItem%>
                            </ItemTemplate>
                            <ControlStyle BorderColor="Black" BorderStyle="Solid" Font-Bold="True" Font-Names="Times New Roman" />
                            <HeaderStyle ForeColor="White" />
                            <ItemStyle BorderStyle="Solid" HorizontalAlign="Left" Font-Size="Smaller" />
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                    <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                    <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                </asp:GridView>
                <%-- </div>--%>
            </div>
        </asp:Panel>
        <div align="center" style="margin-top: 20px; font-weight: bold; color: Red;">
            <asp:Label ID="idenEmpty" runat="server" Text=""></asp:Label>
        </div>
    </div>
    <div id="footerDiv">
        <div id="labelFooter">
            <div id="leftLabel">
                M.Lhuillier Philippines Inc.
            </div>
            <div id="rightLabel">
                All Rights Reserved.
            </div>
        </div>
    </div>
    </form>
</body>
</html>
