<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportForm.aspx.cs" Inherits="ReportForm" %>

<%@ Register Assembly="FlashControl" Namespace="Bewise.Web.UI.WebControls" TagPrefix="Bewise" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="CSS/Main.css" rel="stylesheet" type="text/css" />
    <title>Report Page</title>
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
    <div id="content" style="background-color: #E4E6E7;">
        <div align="center">
            <%-- <asp:Panel ID="InformationPanel" runat="server" CssClass="modalPopup" Height="363px"
                Width="909px">--%>
            <div style="margin-top: 2px;" align="center">
                <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="True"
                    DisplayGroupTree="False" ReportSourceID="CrystalReportSource1" Height="500px"
                    Width="100%" HasCrystalLogo="False" HasDrillUpButton="False" HasExportButton="False"
                    HasGotoPageButton="False" HasPageNavigationButtons="False" HasSearchButton="False"
                    HasToggleGroupTreeButton="False" HasViewList="False" HasZoomFactorList="False"
                    BestFitPage="False" />
                <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
                    <Report>
                    </Report>
                </CR:CrystalReportSource>
            </div>
            <%--</asp:Panel>--%>
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
