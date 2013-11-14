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
using System.Collections.Generic;

/// <summary>
/// Summary description for ReturnFunctions
/// </summary>
public class ReturnFunctions
{
    public string msg { get; set; }
    public int iden { get; set; }
    public List<string> Value { get; set; }
    public int PHPcounter { get; set; }
    public decimal totalprincipalPHP { get; set; }
    public decimal totalphpcharge { get; set; }
    public int USDcounter { get; set; }
    public decimal totalprincipalUSD { get; set; }
    public decimal totalusdcharge { get; set; }
    public decimal AmountDuePHP { get; set; }
    public decimal AmountDueUSD { get; set; }
}
