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
/// Summary description for ReturnListArray
/// </summary>
public class ReturnListArray
{
    public int resp { get; set; }
    public string Mlbnum { get; set; }
    public string AccountCode { get; set; }
    public string StationCode { get; set; }
    public string msg { get; set; }
    public List<string> data { get; set; } 
}
