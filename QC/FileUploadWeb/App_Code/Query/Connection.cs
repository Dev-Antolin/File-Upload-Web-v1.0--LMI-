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
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using MapDrive.Network;
using System.IO;

/// <summary>
/// Summary description for Connection
/// </summary>
public class Connection
{
    private MapDrive.Network.MapDrive mapdriver = new MapDrive.Network.MapDrive();
    public DBConnect dbconkp;
    public MySqlCommand command;
    public string directory;
    public string ZoneCode = "";
    public string StationCode = "";
    public string StationNumber = "";
    public string Branchcode = "";
    public string BranchName = "";
    public string AccId;

    private string pathDSWD = AppDomain.CurrentDomain.BaseDirectory + "FileUploadINI.ini";

    public void ConnectConfig()
    {
        try
        {
            IniFile ini = new IniFile(pathDSWD);

            directory = ini.IniReadValue("DBConfig DSWD", "dir");
            string mapserver = ini.IniReadValue("DBConfig DSWD", "mapdrive");
            string mapuser = ini.IniReadValue("DBConfig DSWD", "mapuser");
            string mappass = ini.IniReadValue("DBConfig DSWD", "mappass");

            String Serv = ini.IniReadValue("DBConfig Partner", "Server");
            String DB = ini.IniReadValue("DBConfig Partner", "Database"); ;
            String UID = ini.IniReadValue("DBConfig Partner", "UID"); ;
            String Password = ini.IniReadValue("DBConfig Partner", "Password");
            String pool = ini.IniReadValue("DBConfig Partner", "Pool");
            Int32 maxcon = Convert.ToInt32(ini.IniReadValue("DBConfig Partner", "MaxCon"));
            Int32 mincon = Convert.ToInt32(ini.IniReadValue("DBConfig Partner", "MinCon"));
            Int32 tout = Convert.ToInt32(ini.IniReadValue("DBConfig Partner", "Tout"));

            dbconkp = new DBConnect(Serv, DB, UID, Password, pool, maxcon, mincon, tout);

            if (Directory.Exists(directory) != true)
            {
                bool res = mapdriver.MapNetworkDrive(mapserver, 'Z', true, mapuser, mappass);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void IniValue()
    {
        IniFile ini = new IniFile(pathDSWD);
        ZoneCode = ini.IniReadValue("INIvalue Partner", "ZoneCode");
        StationCode = ini.IniReadValue("INIvalue Partner", "StationCode"); ;
        StationNumber = ini.IniReadValue("INIvalue Partner", "StationNumber"); ;
        Branchcode = ini.IniReadValue("INIvalue Partner", "Branchcode");
        BranchName = ini.IniReadValue("INIvalue Partner", "BranchName");
    }
}
