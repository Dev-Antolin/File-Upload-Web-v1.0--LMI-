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

public partial class LogoutPage : System.Web.UI.Page
{
    private Connection cn = new Connection();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string userId = Request.QueryString["UserId"];
            string password = Request.QueryString["AccountId"];

            cn.ConnectConfig();
            if (!accountId(userId, password).Equals(false))
            {
                this.Session.Add("_UserId", userId);
                Response.Redirect("MainMenu.aspx");
            }
            else
            {
                lblTxt.Text = "ACCESS DENIED";
            }
        }
        catch (Exception)
        {
            lblTxt.Text = "Unable to connect to any specified hosts.";
        }
    }
    private bool accountId(string usr, string pass)
    {
        bool res = false;
        using (MySqlConnection conn = cn.dbconkp.getConnection())
        {
            try
            {
                conn.Open();
                using (cn.command = conn.CreateCommand())
                {
                    string sql = "Select AccountID from kpadminpartners.partnersusers where " +
                                 "UserID = '" + usr + "' and Password = '" + pass + "'";
                    cn.command.CommandText = sql;

                    using (MySqlDataReader rdr = cn.command.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            string AccId = rdr["AccountID"].ToString();
                            this.Session.Add("_AccId", AccId);
                            res = true;
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                res = false;
                throw new Exception(ex.Message);
            }
        }

        return res;
    }
}
