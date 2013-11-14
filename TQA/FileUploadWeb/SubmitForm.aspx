<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SubmitForm.aspx.cs" Inherits="SubmitForm" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="FlashControl" Namespace="Bewise.Web.UI.WebControls" TagPrefix="Bewise" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="CSS/Main.css" rel="stylesheet" type="text/css" />
    <title>Submit Page</title>
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
                <asp:Label ID="lblFilename" runat="server" Text="" Font-Bold="True" Font-Size="Larger"
                    ForeColor="WHITE"></asp:Label>
            </div>
        </div>
        <div>
            <asp:Button ID="backBtn" runat="server" Text="&lt;&lt; back to Main Page" BorderStyle="None"
                Font-Overline="False" Font-Underline="True" ForeColor="Maroon" Width="154px"
                OnClick="backBtn_Click" />
            <br />
        </div>
        <div align="center">
            <asp:Label ID="Label7" runat="server" Text="File Information Summary" Font-Bold="true"
                Font-Size="smaller" ForeColor="BLACK"></asp:Label>
            <asp:Panel ID="InformationPanel" runat="server" CssClass="modalPopup" Height="230px"
                Width="750px">
                <div style="margin-top: 30px;" align="center">
                    <table style="width: 600px">
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text="Total principal PHP :" Font-Size="12px"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPrincipalPHP" runat="server" CssClass="input" Enabled="False"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="Label2" runat="server" Text="Total principal USD :" Font-Size="12px"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPrincipalUSD" runat="server" CssClass="input" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label3" runat="server" Font-Size="12px" Text="Total PHP Charge :"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPHPCharge" runat="server" CssClass="input" Enabled="False"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="Label4" runat="server" Text="Total USD Charge :" Font-Size="12px"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtUSDCharge" runat="server" CssClass="input" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label5" runat="server" Font-Size="12px" Text="Total Transaction PHP :"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNumTransPHP" runat="server" CssClass="input" Enabled="False"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="Label6" runat="server" Font-Size="12px" Text="Total Transaction USD :"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNumTransUSD" runat="server" CssClass="input" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                </div>
                <div style="margin-top: 40px; margin-left: 400px;">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnUpload" runat="server" Text="Upload" Height="22px" Width="90px"
                                    OnClick="btnUpload_Click" />
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" Height="22px" Width="90px"
                                    Enabled="False" OnClick="btnSubmit_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
        </div>
        <div align="center" style="margin-top: 20px;">
            <asp:Label ID="lblIden" runat="server" Text=""></asp:Label>
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
