using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for CheckingFunctions
/// </summary>
public class CheckingFunctions
{
    public string msg { get; set; }
    public int iden { get; set; }
    public string oldId { get; set; }
    public decimal AmountUSD { get; set; }
    public decimal AmountPHP { get; set; }
}
