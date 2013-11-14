using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

public partial class MainMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblName.Text = Session["xxxcheckLoginInfo"].ToString();
        }
        catch(Exception ex)
        {
            ex.ToString();
        }
    }
    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Response.Redirect("LogoutPage.aspx");
    }
}
