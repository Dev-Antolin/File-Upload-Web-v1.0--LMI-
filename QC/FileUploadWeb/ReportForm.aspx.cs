using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class ReportForm : System.Web.UI.Page
{
    public ReportGeneration rg = new ReportGeneration();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            int iden = Convert.ToInt32(Request.QueryString["identifier"]);
            if (iden == 2)
            {
                if ((string)Session["Upload"] == "File")
                {
                    ReportError();
                }
            }
            else if (iden == 1)
            {
                if ((string)Session["Upload"] == "File")
                {
                    ReportSuccess();
                }
                else
                {
                    ReportSuccessManual();
                }
            }
        }
    }
    private void ReportError()
    {
        string pathErrorFile = AppDomain.CurrentDomain.BaseDirectory + "ReportsFormat\\rptFileUploadError.rpt";
        string datetime1 = DateTime.Now.ToString("MMMM dd,yyyy hh:mm:ss tt");
        string OpId = (string)Session["_UserId"];
        ReportDocument rpt = new ReportDocument();
        List<string> value = (List<string>)Session["_arr"];
        string Mlbnum = (string)Session["bnum"];
        rpt.Load(pathErrorFile);
        DataTable dt = new DataTable();
        String partnersname = (string)Session["_Accname"];
        dt = rg.GenerateErrorReport(value, "PartnersBnum", Mlbnum, partnersname);
        rpt.SetDataSource(dt);
        rpt.SetParameterValue("PartnersBatchNumber", "");
        rpt.SetParameterValue("MLbatchNo", Mlbnum);
        rpt.SetParameterValue("partnersname", partnersname);
        rpt.SetParameterValue("OperatorId", OpId);
        rpt.SetParameterValue("Date", datetime1);
        Response.Buffer = false;
        Response.ClearContent();
        Response.ClearHeaders();
        string file = (string)Session["_FileName"] + "_" + Convert.ToString(DateTime.Now.Ticks);

        rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, partnersname + "_" + file);
        //CrystalReportSource1.Report.FileName = "~/rptFileUploadError.rpt";
        //CrystalReportViewer1.ReportSource = rpt;
    }
    private void ReportSuccess()
    {
        string pathSuccessFile = AppDomain.CurrentDomain.BaseDirectory + "ReportsFormat\\rptFileUpload.rpt";
        string datetime1 = DateTime.Now.ToString("MMMM dd,yyyy hh:mm:ss tt");
        ReportDocument rpt = new ReportDocument();
        List<string> value = (List<string>)Session["_success"];
        string Mlbnum = (string)Session["bnum"];

        rpt.Load(pathSuccessFile);
        DataTable dt = new DataTable();
        String partnersbtchnum = "";
        String partnersname = (string)Session["_Accname"];
        String usdprin = (string)Session["_Pusd"];
        String phpprin = (string)Session["_Pphp"];
        String usdC = (string)Session["_Cusd"];
        String phpC = (string)Session["_Cphp"];
        String notxtn = (string)Session["_Tphp"];
        String notxnUsd = (string)Session["_Tusd"];
        String OpId = (string)Session["_UserId"];
        dt = rg.GenerateSuccessReport(value);
        rpt.SetDataSource(dt);
        rpt.SetParameterValue("partnerBatchNo", partnersbtchnum);
        rpt.SetParameterValue("MLbatchNo", Mlbnum);
        rpt.SetParameterValue("partnersname", partnersname);
        rpt.SetParameterValue("Pusd", usdprin);
        rpt.SetParameterValue("Pphp", phpprin);
        rpt.SetParameterValue("UsdCharge", usdC);
        rpt.SetParameterValue("PhpCharge", phpC);
        rpt.SetParameterValue("txnNumber", notxtn);
        rpt.SetParameterValue("txnNumberusd", notxnUsd);
        rpt.SetParameterValue("OperatorId", OpId);
        rpt.SetParameterValue("Date", datetime1);

        //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, True, myreport.reportTitle & " " & Replace(myreport.valueDate2, "/", "-"))
        string file = (string)Session["_FileName"] + "_" + Convert.ToString(DateTime.Now.Ticks);
        Response.Buffer = false;
        Response.ClearContent();
        Response.ClearHeaders();
        rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, partnersname + "_" + file);
        //CrystalReportSource1.Report.FileName = "~/rptFileUpload.rpt";
        //CrystalReportViewer1.ReportSource = rpt;
    }

    private void ReportSuccessManual()
    {
        string pathSuccessFile = AppDomain.CurrentDomain.BaseDirectory + "ReportsFormat\\ManualUpload.rpt";
        ReportDocument rpt = new ReportDocument();
        List<string> value = (List<string>)Session["_success"];

        rpt.Load(pathSuccessFile);
        DataTable dt = new DataTable();

        String upload = (string)Session["Upload"];
        String PartnerName = (string)Session["_Accname"];
        String currency = (string)Session["Currency"];
        String OperatorID = (string)Session["_UserId"];
        String AccountCode = (string)Session["_AccId"];
        String OperatorName = rg.getOperatorName(OperatorID, AccountCode);
        
        dt = rg.GenerateSuccessReportManual(value);
        rpt.SetDataSource(dt);
        rpt.SetParameterValue("CorpName", PartnerName);
        rpt.SetParameterValue("PrintedBy", OperatorName);
        rpt.SetParameterValue("Currency", currency);

        string filename = DateTime.Now.ToString("yyyy-MM-dd hhmmss tt") + " " + PartnerName;
        Response.Buffer = false;
        Response.ClearContent();
        Response.ClearHeaders();
        rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);
    }
}
