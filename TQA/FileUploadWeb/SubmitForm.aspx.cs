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
using System.Web.SessionState;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class SubmitForm : System.Web.UI.Page
{
    Query q = new Query();
    Connection cn = new Connection();
    protected void Page_Load(object sender, EventArgs e)
    {
        List<string> newVal = new List<string>();
        lblFilename.Text = "File: " + (string)HttpContext.Current.Session["_FileName"];
        string upload = (string)Session["Upload"];
        string currency = (string)Session["Currency"];
        if (!IsPostBack)
        {
            string fname = (string)HttpContext.Current.Session["_FileName"];
            string AccId = (string)HttpContext.Current.Session["_AccId"];
            //newVal = CurrencyTrappings(AccId);

            var x = q.parseP2P(fname, "001", 1, AccId, upload, currency, (string)Session["Extension"]);
            if (x.iden == 1)
            {
                txtPrincipalPHP.Text = Convert.ToString(x.totalprincipalPHP);
                txtPrincipalUSD.Text = Convert.ToString(x.totalprincipalUSD);
                txtNumTransPHP.Text = Convert.ToString(x.PHPcounter);
                txtNumTransUSD.Text = Convert.ToString(x.USDcounter);
                txtPHPCharge.Text = Convert.ToString(x.totalphpcharge);
                txtUSDCharge.Text = Convert.ToString(x.totalusdcharge);
                this.Session.Add("_val", x.Value);
            }
            else
            {
                lblIden.Text = x.msg;
                if ((string)Session["Upload"] == "Manual")
                {
                    Response.Redirect("SecurityForm.aspx?msg=" + x.msg + "");  
                }
            }
        }
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            SMFunctions sf = new SMFunctions();
            string AccId = (string)HttpContext.Current.Session["_AccId"];
            // string session = (string)HttpContext.Current.Session["_session"];
            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
            String sessionId = manager.CreateSessionID(Context);
            this.Session.Add("_session", sessionId);
            string upload = (string)Session["Upload"];
            List<string> v = (List<string>)Session["_val"];
            string OperatorID = (string)Session["_UserId"];
            cn.IniValue();
            var res = sf.P2PTransactions(v, AccId, "", cn.BranchName, cn.Branchcode, cn.StationCode, cn.ZoneCode, OperatorID.ToUpper(), cn.StationNumber, (string)Session["Currency"], sessionId, upload);
            if (res.resp == 0)
            {
                this.Session.Add("_arr", res.data);
                this.Session.Add("bnum", res.Mlbnum);
                if (upload == "File")
                {
                    Response.Write("<script language=javascript>");
                    Response.Write("alert('" + "File submitted contains error. Please click OK button to download Error Transactions report." + "')");
                    Response.Write("</script>");
                    Redirect("ReportForm.aspx?Identifier=2", "xxx", "xxx");
                }
                else
                {
                    lblIden.Text = res.msg;
                }
            }
            else
            {
                btnUpload.Enabled = false;
                btnSubmit.Enabled = true;
                backBtn.Visible = false;
                this.Session.Add("bnum", res.Mlbnum);
                this.Session.Add("_success", res.data);
                this.Session.Add("stationcode", res.StationCode);
            }
        }
        catch (Exception error)
        {
            lblIden.Text = error.Message;
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            UploadData ud = new UploadData();
            string AccId = (string)HttpContext.Current.Session["_AccId"];
            string session = (string)HttpContext.Current.Session["_session"];
            string batchfile = (string)Session["bnum"];
            string stationcode = (string)Session["stationcode"];
            string upload = (string)Session["Upload"];
            if (ud.SubmitToDatabase(session, AccId, batchfile, stationcode, upload, (string)HttpContext.Current.Session["_FileName"]) == "Success")
            {
                btnSubmit.Enabled = false;
                lblIden.Text = "Successfully Upload";
                backBtn.Visible = true;
                this.Session.Add("_Pusd", txtPrincipalUSD.Text);
                this.Session.Add("_Pphp", txtPrincipalPHP.Text);
                this.Session.Add("_Cusd", txtUSDCharge.Text);
                this.Session.Add("_Cphp", txtPHPCharge.Text);
                this.Session.Add("_Tusd", txtNumTransUSD.Text);
                this.Session.Add("_Tphp", txtNumTransPHP.Text);

                Response.Write("<script language=javascript>");
                Response.Write("alert('" + "File successfully uploaded. Please click OK button to download report." + "')");
                Response.Write("</script>");
                RenameFiles();
                Redirect("ReportForm.aspx?Identifier=1", "xxx", "xxx");
                //}
            }
            else
            {
                lblIden.Text = "Error in Inserting";
                backBtn.Visible = true;
            }
        }
        catch (Exception error)
        {
            lblIden.Text = error.Message;
        }
    }

    private void RenameFiles()
    {
        Connection cn = new Connection();
        RenameFile.RenameFile rn = new RenameFile.RenameFile();
        cn.ConnectConfig();
        string fname = (string)Session["_FileName"];
        string AccID = (string)Session["_AccId"];
        string path = cn.directory + "\\" + AccID + "\\" + fname;
        string newpath = cn.directory + "\\" + AccID + "\\" + fname + ".done";
        rn.Rename(path, newpath);

        // System.IO.File.Move(path, newpath);
        // Object obj = Interaction.CreateObject("Scripting.FileSystemObject","");
        // obj.MoveFile(path, newpath);
    }

    public static void Redirect(string url, string target, string windowFeatures)
    {
        HttpContext context = HttpContext.Current;
        if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && String.IsNullOrEmpty(windowFeatures))
        {
            context.Response.Redirect(url);
        }
        else
        {
            Page page = (Page)context.Handler;
            if (page == null)
            {
                throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
            }
            url = page.ResolveClientUrl(url);
            string script = null;
            if (!String.IsNullOrEmpty(windowFeatures))
            {
                script = "window.open(\"{0}\", \"{1}\", \"{2}\");";
            }
            else
            {
                script = "window.open(\"{0}\", \"{1}\");";
            }
            script = String.Format(script, url, target, windowFeatures);
            ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
        }
    }

    protected void backBtn_Click(object sender, EventArgs e)
    {
        string AccId = (string)Session["_AccId"];
        string UserId = (string)Session["_UserId"];
        Response.Redirect("MainMenu.aspx?UserId=" + UserId + "&AccountId=" + AccId + "");
    }

    private void ReportSuccessManual()
    {
        ReportGeneration rg = new ReportGeneration();
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
