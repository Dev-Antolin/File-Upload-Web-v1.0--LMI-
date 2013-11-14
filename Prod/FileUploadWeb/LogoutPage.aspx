<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LogoutPage.aspx.cs" Inherits="LogoutPage"
    Title="Log-Out" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="CSS/Main.css" rel="stylesheet" type="text/css" />
    <title>Log-Out Page</title>
    <style type="text/css">
        #footerallrightsreserved
        {
            width: 567px;
        }
        .style1
        {
            color: #FFFFFF;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
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
    <div id="content" style="background-color: #E4E6E7; height: 374px;">
        <div align="center">
            <asp:Label ID="lblTxt" runat="server" Font-Bold="True" Font-Size="Larger" ForeColor="Maroon"></asp:Label>
        </div>
    </div>
    <div id="footerDiv">
        <div id="labelFooter">
        </div>
    </div>
    </form>
</body>
