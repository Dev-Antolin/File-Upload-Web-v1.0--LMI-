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
using System.IO;
using System.Collections.Generic;
using System.Web.SessionState;
using Microsoft.VisualBasic;
using System.Data.OleDb;

public partial class SecurityForm : System.Web.UI.Page
{
    private Connection cn = new Connection();
    private Query q = new Query();
    private string drpcurrency = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        errorLabel.Text = "";
        if (!IsPostBack)
        {
            lblFilename.Text = "File: " + (string)Session["_FileName"];
            List<string> currency = new List<string>();
            if ((string)Session["Upload"] == "File")
            {
                lblCurrency.Visible = false;
                drpCurrency.Visible = false;
            }
            else
            {
                string accountID = (string)Session["_AccId"];
                currency = q.getCurrency(accountID);
                if (currency.Count != 0)
                {
                    drpCurrency.Items.Clear();
                    foreach (string c in currency)
                    {
                        drpCurrency.Items.Add(c);
                    }
                }
                lblCurrency.Visible = true;
                drpCurrency.Visible = true;
                drpcurrency = "PHP";
                TxtamountUSD.Enabled = false;

                errorLabel.Text = Request.QueryString["msg"];
            }
        }
        else
        {
            if ((string)Session["Upload"] == "Manual")
            {
                drpcurrency = drpCurrency.SelectedItem.ToString();
            }
        }
    }

    private CheckingFunctions CheckFunctions(string fileName, string Accountid)
    {
        cn.ConnectConfig();
        int numchar = 0;
        decimal tPHP = 0;
        decimal tUSD = 0;
        string oldbranchId = "";
        string directory = cn.directory + "\\" + Accountid + "\\" + fileName;
        if ((string)Session["Upload"] == "File")
        {

            StreamReader SR = new StreamReader(directory);
            string rdr;
            rdr = SR.ReadLine();

            oldbranchId = rdr.Substring(10, 6).ToString();
            while (SR.Peek() != -1)
            {
                rdr = SR.ReadLine();

                numchar = numchar + 1;
                string currency = rdr.Substring(71, 3).ToString();
                decimal amount = Convert.ToDecimal(rdr.Substring(59, 12).ToString());
                if (currency == "PHP")
                {
                    tPHP = tPHP + amount;
                }
                else if (currency == "USD")
                {
                    tUSD = tUSD + amount;
                }
            }
            SR.Close();
            SR.Dispose();
        }
        else //Manual Upload
        {
            DataSet ds = new DataSet();
            string query = "";
            string con = "";

            if ((string)Session["Extension"] == "xls")
            {
                con = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + directory + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=2\"";
            }
            else if ((string)Session["Extension"] == "xlsx")
            {
                con = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + directory + ";Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=2\"";
            }

            query = "SELECT COUNT(F1), SUM(F12) FROM [Sheet1$A9:L] WHERE F1 IS NOT NULL AND F2 IS NOT NULL AND F3 IS NOT NULL AND F4 IS NOT NULL AND F5 IS NOT NULL " +
                    "AND F6 IS NOT NULL AND F7 IS NOT NULL AND F8 IS NOT NULL AND F9 IS NOT NULL AND F10 IS NOT NULL AND F11 IS NOT NULL AND F12 IS NOT NULL OR F12 <= 0;";

            OleDbDataAdapter da = new OleDbDataAdapter(query, con);
            da.Fill(ds);
            numchar = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            decimal principal = Convert.ToDecimal(ds.Tables[0].Rows[0][1].ToString());
            if (drpcurrency == "PHP")
            {
                tPHP = principal;
            }
            else if (drpcurrency == "USD")
            {
                tUSD = principal;
            }
        }
        return new CheckingFunctions { iden = numchar, oldId = oldbranchId, AmountPHP = tPHP, AmountUSD = tUSD };
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        try
        {
            decimal txtboxUsd = 0;
            decimal txtboxPhp = 0;
            string fname = (string)Session["_FileName"];
            string extens = (string)Session["Upload"];
            string secId = (string)Session["_AccId"];
            if (txtRecords.Text == "")
            {
                errorLabel.Text = "Please input no. of Records";
                return;
            }
            if ((string)Session["Upload"] != "File")
            {
                if (drpCurrency.SelectedItem.ToString() == "PHP")
                {
                    if (TxtamountPHP.Text == "")
                    {
                        errorLabel.Text = "Please input Total amount PHP";
                        return;
                    }
                }
                else if (drpCurrency.SelectedItem.ToString() == "USD")
                {
                    if (TxtamountUSD.Text == "")
                    {
                        errorLabel.Text = "Please input Total amount USD";
                        return;
                    }
                }
            }
            var x = CheckFunctions(fname, secId);
            if ((string)Session["Upload"] == "File")
            {
                if (x.oldId == (string)HttpContext.Current.Session["_oldAccId"])
                {
                    if (!Information.IsNumeric(TxtamountUSD.Text))
                    {
                        txtboxUsd = 0;
                    }
                    else if (!Information.IsNumeric(TxtamountPHP.Text))
                    {
                        txtboxPhp = 0;
                    }
                    else
                    {
                        txtboxUsd = TxtamountUSD.Text == string.Empty ? 0 : Convert.ToDecimal(TxtamountUSD.Text);
                        txtboxPhp = TxtamountPHP.Text == string.Empty ? 0 : Convert.ToDecimal(TxtamountPHP.Text);
                    }

                    if (txtRecords.Text == Convert.ToString(x.iden) && txtboxPhp == x.AmountPHP && txtboxUsd == x.AmountUSD)
                    {
                        Response.Redirect("SubmitForm.aspx");
                    }
                    else
                    {
                        txtRecords.Text = "";
                        TxtamountPHP.Text = "";
                        TxtamountUSD.Text = "";
                        errorLabel.Text = "File Information did not match. Please try again.";
                    }
                }
                else
                {
                    txtRecords.Text = "";
                    TxtamountPHP.Text = "";
                    TxtamountUSD.Text = "";
                    errorLabel.Text = "Invalid File information. Please review file content.";
                }
            }
            else //Manual Upload
            {
                drpcurrency = drpCurrency.SelectedItem.ToString();
                this.Session.Add("Currency", drpcurrency);
                txtboxUsd = TxtamountUSD.Text == string.Empty ? 0 : Convert.ToDecimal(TxtamountUSD.Text);
                txtboxPhp = TxtamountPHP.Text == string.Empty ? 0 : Convert.ToDecimal(TxtamountPHP.Text);
                if (drpcurrency == "PHP")
                {
                    if (txtRecords.Text == Convert.ToString(x.iden) && txtboxPhp == x.AmountPHP)
                    {
                        Response.Redirect("SubmitForm.aspx");
                    }
                    else
                    {
                        txtRecords.Text = "";
                        TxtamountPHP.Text = "";
                        TxtamountUSD.Text = "";
                        errorLabel.Text = "File Information did not match. Possible cause : " +
                        " 1) Number of records did not match  " +
                        " 2) Total amount PHP did not match or  " +
                        " 3) There is an empty cell in Excel file (Check Excel File). Please try again.";
                    }
                }
                else if (drpcurrency == "USD")
                {
                    if (txtRecords.Text == Convert.ToString(x.iden) && txtboxUsd == x.AmountUSD)
                    {
                        Response.Redirect("SubmitForm.aspx");
                    }
                    else
                    {
                        txtRecords.Text = "";
                        TxtamountPHP.Text = "";
                        TxtamountUSD.Text = "";
                        errorLabel.Text = "File Information did not match. Possible cause : " +
                        " 1) Number of records did not match " +
                        " 2) Total amount USD did not match or " +
                        " 3) There is an empty cell in Excel file (Check Excel File). Please try again.";
                    }
                }
            }

        }
        catch (Exception error)
        {
            if (error.Message == "No value given for one or more required parameters.")
            {
                errorLabel.Text = "Unknown column(s). Please check Excel file.";
            }
            else { errorLabel.Text = error.Message; }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string AccId = (string)Session["_AccId"];
        string UserId = (string)Session["_UserId"];
        Response.Redirect("MainMenu.aspx?UserId=" + UserId + "&AccountId=" + AccId + "");
    }
    protected void drpCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        drpcurrency = drpCurrency.SelectedItem.ToString();
        errorLabel.Text = "";
        if (drpcurrency == "PHP")
        {
            TxtamountPHP.Enabled = true;
            TxtamountUSD.Enabled = false;
            TxtamountUSD.Text = "";
        }
        else
        {
            TxtamountUSD.Enabled = true;
            TxtamountPHP.Enabled = false;
            TxtamountPHP.Text = "";
        }
    }
    protected void txtRecords_TextChanged(object sender, EventArgs e)
    {
        errorLabel.Text = "";
    }
    protected void TxtamountUSD_TextChanged(object sender, EventArgs e)
    {
        errorLabel.Text = "";
    }
    protected void TxtamountPHP_TextChanged(object sender, EventArgs e)
    {
        errorLabel.Text = "";
    }
}
