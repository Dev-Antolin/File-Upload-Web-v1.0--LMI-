<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SecurityForm.aspx.cs" Inherits="SecurityForm" %>

<%@ Register Assembly="FlashControl" Namespace="Bewise.Web.UI.WebControls" TagPrefix="Bewise" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="CSS/Main.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript"><!--
         function filterDigits(eventInstance) { 
            eventInstance = eventInstance || window.event;
                key = eventInstance.keyCode || eventInstance.which;
            if ((47 < key) && (key < 58) || key == 45 || key == 8 || key == 46) {
               return true;
            } else {
                    if (eventInstance.preventDefault) eventInstance.preventDefault();
                    eventInstance.returnValue = false;
                    return false;
                } //if
         } //filterDigits
      -->
        function filterDigitsOnly(eventInstance) { 
            eventInstance = eventInstance || window.event;
                key = eventInstance.keyCode || eventInstance.which;
            if ((47 < key) && (key < 58) || key == 45 || key == 8) {
               return true;
            } else {
                    if (eventInstance.preventDefault) eventInstance.preventDefault();
                    eventInstance.returnValue = false;
                    return false;
                } //if
         }
    </script>

    <title>Security Page</title>
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
    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe" runat="server" TargetControlID="TxtamountUSD"
        FilterType="Custom, Numbers" ValidChars="." />
    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
        TargetControlID="TxtamountPHP" FilterType="Custom, Numbers" ValidChars="." />
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
        <div style="margin-top: 15px;">
            <asp:Button ID="Button1" runat="server" Text="&lt;&lt; back to Main Page" BorderStyle="None"
                Font-Overline="False" Font-Underline="True" ForeColor="Maroon" OnClick="Button1_Click"
                Width="154px" />
            <br />
        </div>
        <div align="center">
            <asp:Label ID="Label5" runat="server" Text="Required file information" Font-Bold="true"
                Font-Size="smaller" ForeColor="BLACK"></asp:Label>
            <asp:Panel ID="InformationPanel" runat="server" CssClass="modalPopup" Height="180px"
                Width="350px">
                <div style="margin-top: 30px;" align="center">
                    <table>
                        <tr>
                            <td align="right">
                                <asp:Label ID="lblCurrency" runat="server" Text="Currency :" Font-Size="12px"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="drpCurrency" runat="server" Width="133px" OnSelectedIndexChanged="drpCurrency_SelectedIndexChanged"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="Label1" runat="server" Text="Input no. of Records :" Font-Size="12px"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtRecords" runat="server" onkeypress="filterDigitsOnly(event)"
                                    MaxLength="12" Width="130px" AutoPostBack="True" OnTextChanged="txtRecords_TextChanged"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="Label2" runat="server" Text="Total Amount USD :" Font-Size="12px"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="TxtamountUSD" runat="server" onkeypress="filterDigits(event)" MaxLength="12"
                                    Width="130px" AutoPostBack="True" OnTextChanged="TxtamountUSD_TextChanged"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="Label3" runat="server" Text="Total Amount PHP :" Font-Size="12px"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="TxtamountPHP" runat="server" onkeypress="filterDigits(event)" MaxLength="12"
                                    Width="130px" AutoPostBack="True" OnTextChanged="TxtamountPHP_TextChanged"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <div style="margin-top: 10px; margin-left: 50px;">
                                    <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Height="22px" Width="90px"
                                        OnClick="btnConfirm_Click" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
        </div>
        <div align="center" style="margin-top: 10px">
            <asp:Label ID="errorLabel" runat="server" ForeColor="Maroon"></asp:Label>
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
