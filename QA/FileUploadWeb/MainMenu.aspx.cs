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
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using MapDrive.Network;
using System.IO;

public partial class Login : System.Web.UI.Page
{
    private Query q = new Query();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string userId = (string)Session["_UserId"];
            string AccId = (string)Session["_AccId"];
            this.Session.Add("_UserId", userId);
            this.Session.Add("_AccId", AccId);
            if (!IsPostBack)
            {
                List<string> v = new List<string>();
                if (q.CheckPartnersLogin(AccId, userId).Equals(true))
                {
                    string oldAccountID = q.getOldAccountID(AccId);

                    getPArtnersName(AccId);
                    this.Session.Add("_oldAccId", oldAccountID);
                    v = q.getPartnersFile(AccId);

                    if (v.Count != 0)
                    {
                        this.Session.Add("files", v);
                        GridView1.DataSource = v;
                        GridView1.DataBind();
                        Label1.Visible = true;
                        PanelonGrid.Visible = true;
                    }
                    else
                    {
                        idenEmpty.Text = "NO FILES FOUND";
                        PanelonGrid.Visible = false;
                        Label1.Visible = false;
                    }
                }
            }
        }
        catch (Exception error)
        {
            idenEmpty.Text = error.Message;
            PanelonGrid.Visible = false;
            Label1.Visible = false;
        }
    }

    private void getPArtnersName(string Accid)
    {
        Connection cn = new Connection();
        cn.ConnectConfig();
        using (MySqlConnection conn = cn.dbconkp.getConnection())
        {
            conn.Open();
            using (cn.command = conn.CreateCommand())
            {
                string sql = "select Accountname from kpadminpartners.accountlist " +
                             "where AccountId = '" + Accid + "'";
                cn.command.CommandText = sql;
                using (MySqlDataReader rdr = cn.command.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        string Accname = rdr["Accountname"].ToString();
                        this.Session.Add("_Accname", Accname);
                    }
                }
            }
        }
    }

    protected void GridView1_SelectedIndexChanged1(object sender, EventArgs e)
    {
        int a;
        //int i = 0;
        string FileName = "";
        a = GridView1.SelectedIndex;
        List<string> files = (List<string>)Session["files"];
        FileName = files[a];
        string[] ext = FileName.Split('.');
        string extension = ext[ext.Length - 1].ToString();
        if (extension.ToLower() == "txt")
        {
            this.Session.Add("Upload", "File");
        }
        else
        {
            this.Session.Add("Upload", "Manual");
            this.Session.Add("Extension", extension);
        }

        this.Session.Add("_FileName", FileName);
        Response.Redirect("SecurityForm.aspx");
    }
}
