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
using System.Threading;
using RenameFile;
using System.Collections;
using System.Diagnostics;
using System.Data.OleDb;

/// <summary>
/// Summary description for Query
/// </summary>
public class Query
{
    Connection cn = new Connection();
    Hashtable myHashtable;
    public List<string> getCurrency(string AccountID)
    {
        cn.ConnectConfig();
        string currency = "";
        List<string> currList = new List<string>();
        using (MySqlConnection conn = cn.dbconkp.getConnection())
        {
            conn.Open();
            try
            {
                using (cn.command = conn.CreateCommand())
                {
                    string sql = "select currency from kpadminpartners.accountdetail where " +
                                 "AccountID = '" + AccountID + "'";
                    cn.command.CommandText = sql;
                    using (MySqlDataReader rdr = cn.command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            currency = rdr["currency"].ToString();
                            currList.Add(currency);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
        }
        return currList;
    }

    public string getOldAccountID(string AccountID)
    {
        cn.ConnectConfig();
        string id = "";
        using (MySqlConnection conn = cn.dbconkp.getConnection())
        {
            conn.Open();
            try
            {
                using (cn.command = conn.CreateCommand())
                {
                    string sql = "select oldaccountid from kpadminpartners.accountlist where " +
                                 "AccountID = '" + AccountID + "' and IsActive = 1";
                    cn.command.CommandText = sql;
                    using (MySqlDataReader rdr = cn.command.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            id = rdr["oldaccountid"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
        }

        return id;
    }
    public bool CheckPartnersLogin(string AccountID, string UserID)
    {
        cn.ConnectConfig();
        bool ret = false;
        using (MySqlConnection conn = cn.dbconkp.getConnection())
        {
            conn.Open();
            try
            {
                using (cn.command = conn.CreateCommand())
                {
                    string sql = "Select AccountID from kpadminpartners.partnersusers where " +
                                 "AccountID = '" + AccountID + "' and UserID = '" + UserID + "' and IsActive = 1";
                    cn.command.CommandText = sql;
                    using (MySqlDataReader rdr = cn.command.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            ret = true;
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
        }

        return ret;
    }

    public List<string> getPartnersFile(string AccId)
    {

        //makeFolder(AccId);
        List<string> value = new List<string>();
        string dir = cn.directory + "\\" + AccId;
        //string destDir = dir + "\\UploadedFiles";
        string[] extensionList = { "*.txt", "*.xls" };

        foreach (string extension in extensionList)
        {
            foreach (string file in Directory.GetFiles(dir, extension))
            {
                FileInfo f = new FileInfo(file);
                if (DateTime.Now.Date == f.CreationTime.Date)
                {
                    //string vfile = Path.GetFileName(file);
                    //if (File.Exists(destDir + "\\" + vfile) != true)
                    //{
                    value.Add(Path.GetFileName(file));
                    //}
                }
            }
        }
        return value;
    }
    public void makeFolder(string AccId)
    {
        string dir = cn.directory + "\\" + AccId + "\\UploadedFiles";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    //*********************************P2P Parsing*************************
    public ReturnFunctions parseP2P(String FileName, String bcode, Int32 Zcode, String AccountID, String Upload, String Currency, String extension)
    {
        cn.ConnectConfig();
        string path = cn.directory + "\\" + AccountID + "\\" + FileName;

        List<string> data = new List<string>();
        int rowcount = 0;
        string rowvalue;
        int i = 0;
        int PHPcounter = 0;
        int USDcounter = 0;
        decimal totalprincipalPHP = 0;
        decimal totalprincipalUSD = 0;
        decimal totalphpcharge = 0;
        decimal totalusdcharge = 0;
        decimal DueAmountPHP = 0;
        decimal DueAmountUSD = 0;
        DateTime date = getDatabaseDate();
        try
        {
            if (File.Exists(path))
            {
                if (Upload == "File")
                {

                    StreamReader SR = new StreamReader(path);
                    while (SR.Peek() != -1)
                    {
                        rowvalue = SR.ReadLine();
                        if (rowcount == 0)
                        {
                            rowcount = 1;
                        }
                        else
                        {
                            string kptn = generateKPTNPartnersNew(bcode, Zcode, date);
                            string currency = rowvalue.Substring(71, 3).ToString();
                            decimal amount = Convert.ToDecimal(rowvalue.Substring(59, 12).ToString());
                            var x = ChargesPerLine(AccountID, amount, currency);
                            decimal charge = x.Charges;

                            string value = rowvalue.Substring(0, 5).ToString() + "|" + kptn + "|" + rowvalue.Substring(20, 19).ToString() + "|" + rowvalue.Substring(39, 20).ToString() + "|" +
                                           rowvalue.Substring(59, 12).ToString() + "|" + rowvalue.Substring(71, 3).ToString() + "|" + Convert.ToString(charge) + "|" + rowvalue.Substring(84, 15).ToString() + "|" +
                                           rowvalue.Substring(99, 20).ToString() + "|" + rowvalue.Substring(119, 30).ToString() + "|" + rowvalue.Substring(149, 20).ToString() + "|" + rowvalue.Substring(169, 20).ToString() + "|" +
                                           rowvalue.Substring(189, 20).ToString() + "|" + rowvalue.Substring(209, 10).ToString() + "|" + rowvalue.Substring(219, 20).ToString() + "|" + rowvalue.Substring(239, 10).ToString() + "|" +
                                           rowvalue.Substring(249, 10).ToString() + "|" + rowvalue.Substring(259, 1).ToString() + "|" + rowvalue.Substring(260, 30).ToString() + "|" + rowvalue.Substring(290, 25).ToString() + "|" +
                                           rowvalue.Substring(315, 25).ToString() + "|" + rowvalue.Substring(340, 20).ToString() + "|" + rowvalue.Substring(360, 20).ToString() + "|" + rowvalue.Substring(380, 20).ToString() + "|" +
                                           rowvalue.Substring(400, 20).ToString() + "|" + rowvalue.Substring(420, 30).ToString() + "|" + rowvalue.Substring(450, 25).ToString() + "|" + rowvalue.Substring(475, 25).ToString() + "|" +
                                           rowvalue.Substring(500, 10).ToString() + "|" + rowvalue.Substring(510, 1).ToString() + "|" + rowvalue.Substring(511, 20).ToString() + "|" + rowvalue.Substring(531, 50).ToString();

                            if (currency == "PHP")
                            {
                                PHPcounter = PHPcounter + 1;
                                totalprincipalPHP = totalprincipalPHP + amount;
                                totalphpcharge = totalphpcharge + charge;
                                DueAmountPHP = DueAmountPHP + (totalphpcharge + totalprincipalPHP);
                            }
                            else if (currency == "USD")
                            {
                                USDcounter = USDcounter + 1;
                                totalusdcharge = totalusdcharge + charge;
                                totalprincipalUSD = totalprincipalUSD + amount;
                                DueAmountUSD = DueAmountUSD + (totalusdcharge + totalprincipalUSD);
                            }

                            data.Add(value);
                        }
                    }
                    SR.Close();
                    SR.Dispose();
                    if (CheckingTotalAmount(totalprincipalPHP, totalprincipalUSD, AccountID).Equals(true))
                    {
                        return new ReturnFunctions
                        {
                            iden = 1,
                            PHPcounter = PHPcounter,
                            totalphpcharge = totalphpcharge,
                            totalprincipalPHP = totalprincipalPHP,
                            USDcounter = USDcounter,
                            totalusdcharge = totalusdcharge,
                            totalprincipalUSD = totalprincipalUSD,
                            AmountDuePHP = DueAmountPHP,
                            AmountDueUSD = DueAmountUSD,
                            Value = data
                        };
                    }
                    else
                    {
                        return new ReturnFunctions
                        {
                            iden = 0,
                            msg = "Total Amount is greater than the " +
                            "Running Balance or Credit Limit status is " +
                            "inactive, please contact MIS helpdesk to check your account settings."
                        };
                    }
                }
                //Manual Upload
                else
                {
                    DataSet ds = new DataSet();
                    string query = "";
                    string con = "";

                    if (extension == "xls")
                    {
                        con = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=2\"";
                        //con = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=2\"";
                    }
                    else if (extension == "xlsx")
                    {
                        con = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=2\"";
                    }

                    query = "SELECT * FROM [Sheet1$A9:L] WHERE F1 IS NOT NULL AND F2 IS NOT NULL AND F3 IS NOT NULL AND F4 IS NOT NULL AND F5 IS NOT NULL " +
                    "AND F6 IS NOT NULL AND F7 IS NOT NULL AND F8 IS NOT NULL AND F9 IS NOT NULL AND F10 IS NOT NULL AND F11 IS NOT NULL AND F12 IS NOT NULL;";

                    OleDbDataAdapter da = new OleDbDataAdapter(query, con);
                    da.Fill(ds);
                    int x = ds.Tables[0].Rows.Count;
                    for (int z = 0; z < x; z++)
                    {
                        string kptn = generateKPTNPartnersNew(bcode, Zcode, date);
                        decimal amount = Convert.ToDecimal(ds.Tables[0].Rows[z][11].ToString());
                        var y = ChargesPerLine(AccountID, amount, Currency);
                        decimal charge = y.Charges;
                        decimal total = amount + charge;

                        string value = ds.Tables[0].Rows[z][0].ToString() + "|" + ds.Tables[0].Rows[z][1].ToString() + "|" + ds.Tables[0].Rows[z][2].ToString() + "|" + ds.Tables[0].Rows[z][3].ToString() + "|" +
                                       ds.Tables[0].Rows[z][4].ToString() + "|" + ds.Tables[0].Rows[z][5].ToString() + "|" + ds.Tables[0].Rows[z][6].ToString() + "|" + ds.Tables[0].Rows[z][7].ToString() + "|" +
                                       ds.Tables[0].Rows[z][8].ToString() + "|" + ds.Tables[0].Rows[z][9].ToString() + "|" + ds.Tables[0].Rows[z][10].ToString() + "|" + ds.Tables[0].Rows[z][11].ToString() + "|" +
                                       kptn + "|" + Convert.ToString(charge) + "|" + Convert.ToString(total);

                        if (Currency == "PHP")
                        {
                            PHPcounter = PHPcounter + 1;
                            totalprincipalPHP = totalprincipalPHP + amount;
                            totalphpcharge = totalphpcharge + charge;
                            DueAmountPHP = totalphpcharge + totalprincipalPHP;
                        }
                        else if (Currency == "USD")
                        {
                            USDcounter = USDcounter + 1;
                            totalusdcharge = totalusdcharge + charge;
                            totalprincipalUSD = totalprincipalUSD + amount;
                            DueAmountUSD = totalusdcharge + totalprincipalUSD;
                        }
                        data.Add(value.ToUpper());
                    }
                }
                if (CheckingTotalAmount(totalprincipalPHP, totalprincipalUSD, AccountID).Equals(true))
                {
                    return new ReturnFunctions
                    {
                        iden = 1,
                        PHPcounter = PHPcounter,
                        totalphpcharge = totalphpcharge,
                        totalprincipalPHP = totalprincipalPHP,
                        USDcounter = USDcounter,
                        totalusdcharge = totalusdcharge,
                        totalprincipalUSD = totalprincipalUSD,
                        AmountDuePHP = DueAmountPHP,
                        AmountDueUSD = DueAmountUSD,
                        Value = data
                    };
                }
                else
                {
                    return new ReturnFunctions
                    {
                        iden = 0,
                        msg = "Total Amount is greater than the " +
                        "Running Balance or Credit Limit status is " +
                        "inactive, please contact MIS helpdesk to check your account settings."
                    };
                }
            }
            else
            {
                return new ReturnFunctions { iden = 0, msg = "File Not exist in File Server" };
            }
        }
        catch (Exception ex)
        {
            return new ReturnFunctions { iden = 0, msg = ex.Message };
        }
    }
    public ChargeValue ChargesPerLine(String AccId, Decimal amount, String Currency)
    {
        decimal value;
        string data;

        data = getChargeType(AccId, Currency);
        string[] datasplit = data.Split('|');
        string Ctype = datasplit[0].ToString();
        string charge = datasplit[1].ToString();
        if (Ctype == "Tier Bracket")
        {
            value = GetChargeValue(AccId, Currency, amount);
        }
        else
        {
            value = Convert.ToDecimal(charge);
        }

        return new ChargeValue { response = 0, Charges = value };
    }
    public string getChargeType(string Accountid, string Currency)
    {
        cn.ConnectConfig();
        string value = string.Empty;
        using (MySqlConnection con = cn.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (cn.command = con.CreateCommand())
                {
                    string sql = "select chargetype,chargeamount from kpadminpartners.accountdetail where accountid = '" + Accountid + "' and currency = '" + Currency + "' ";
                    cn.command.CommandText = sql;
                    using (MySqlDataReader rdr = cn.command.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["chargetype"].ToString() + "|" + rdr["chargeamount"].ToString();
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                con.Close();
                throw new Exception(ex.Message);
            }
        }
        return value;
    }
    private bool CheckingTotalAmount(decimal totalPHPAmount, decimal totalUSDAmount, string AccountID)
    {
        bool value = false;
        if (totalPHPAmount != 0 && totalUSDAmount == 0)
        {
            if (CheckTotalAmount(AccountID, totalPHPAmount, "PHP").Equals(true))
            {
                return value = true;
            }
        }
        else if (totalPHPAmount == 0 && totalUSDAmount != 0)
        {
            if (CheckTotalAmount(AccountID, totalUSDAmount, "USD").Equals(true))
            {
                return value = true;
            }
        }
        else if (totalPHPAmount != 0 && totalUSDAmount != 0)
        {
            if (CheckTotalAmount(AccountID, totalUSDAmount, "USD").Equals(true) &&
                CheckTotalAmount(AccountID, totalPHPAmount, "PHP").Equals(true))
            {
                return value = true;
            }
        }
        return value;
    }

    private Boolean CheckTotalAmount(String AccId, Decimal totalAmount, String Currency)
    {
        cn.ConnectConfig();
        int credLimstat;
        decimal rBalance;
        decimal newRbalance;
        decimal credLim;
        bool ret = false;
        using (MySqlConnection con = cn.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (cn.command = con.CreateCommand())
                {
                    string query = "Select creditActivation,runningbalance,creditlimit from kpadminpartners.accountdetail where accountid = @AccId and currency = @Currency";
                    cn.command.CommandText = query;
                    cn.command.Parameters.AddWithValue("AccId", AccId);
                    cn.command.Parameters.AddWithValue("Currency", Currency);
                    using (MySqlDataReader reader = cn.command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            credLimstat = Convert.ToInt32(reader["creditActivation"]);
                            rBalance = Convert.ToDecimal(reader["runningbalance"]);
                            credLim = Convert.ToDecimal(reader["creditlimit"]);

                            if (totalAmount <= rBalance)
                            {
                                ret = true;
                            }

                            else
                            {
                                if (credLimstat == 1)
                                {
                                    newRbalance = (rBalance + credLim) - totalAmount;
                                    if (newRbalance >= 0)
                                    {
                                        ret = true;
                                    }
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

        return ret;
    }
    private String generateKPTNPartnersNew(String AccounID, Int32 zonecode, DateTime dtdate)
    {
        try
        {

            Thread.Sleep(10);

            jp.takel.PseudoRandom.MersenneTwister randGen = new jp.takel.PseudoRandom.MersenneTwister((uint)HiResDateTime.UtcNow.Ticks);
            return AccounID + dtdate.ToString("dd") + zonecode.ToString() + randGen.Next(1000000000, Int32.MaxValue).ToString() + dtdate.ToString("MM"); ;
        }
        catch (Exception a)
        {
            //kplog.Fatal(a.ToString());
            throw new Exception(a.ToString());
        }
    }

    private DateTime getDatabaseDate()
    {
        DateTime serverDate;
        cn.ConnectConfig();
        using (MySqlConnection conn = cn.dbconkp.getConnection())
        {
            conn.Open();
            using (cn.command = conn.CreateCommand())
            {
                string sql = "Select NOW() as serverdt;";
                cn.command.CommandText = sql;
                using (MySqlDataReader rdr = cn.command.ExecuteReader())
                {
                    rdr.Read();

                    serverDate = Convert.ToDateTime(rdr["serverdt"]);
                    return serverDate;

                }
            }
        }

    }

    private Decimal GetChargeValue(String AccId, String Curr, Decimal amount)
    {
        decimal charge = 0;
        int result = 0;
        cn.ConnectConfig();
        using (MySqlConnection con = cn.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (cn.command = con.CreateCommand())
                {
                    string sql = "select b.Minimum,b.Maximum,b.TierCode,b.Chargeamount,a.thresholdamount from kpadminpartners.tierdetails b " +
                                   "inner join kpadminpartners.accountdetail a on b.TierCode = a.BracketTierCode " +
                                   "where a.accountid = @AccId and Currency = @Curr";
                    cn.command.Parameters.AddWithValue("AccId", AccId);
                    cn.command.Parameters.AddWithValue("Curr", Curr);
                    cn.command.CommandText = sql;
                    using (MySqlDataReader rdr = cn.command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            decimal minAmount = Convert.ToDecimal(rdr["Minimum"]);
                            decimal maxAmount = Convert.ToDecimal(rdr["Maximum"]);
                            decimal thold = maxAmount; //Convert.ToDecimal(rdr["thresholdamount"]);
                            decimal chargeAmount = Convert.ToDecimal(rdr["Chargeamount"]);
                            if ((amount >= minAmount && amount <= maxAmount))
                            {
                                charge = chargeAmount;
                            }
                            result = 1;
                        }
                    }
                    con.Close();
                    if (result == 0)
                    {
                        charge = getchargeBillspay(AccId, amount, Curr);
                    }
                }
            }
            catch (Exception ex)
            {
                con.Close();
                throw new Exception(ex.ToString());
            }

        }
        return charge;
    }
    private Decimal getchargeBillspay(String AccId, Decimal amount, String Currency)
    {
        cn.ConnectConfig();
        decimal result = 0;
        decimal minAmount, maxAmount, Thold, charge;
        using (MySqlConnection dcon = cn.dbconkp.getConnection())
        {
            dcon.Open();
            try
            {
                using (cn.command = dcon.CreateCommand())
                {
                    string query = "select b.Minimum, b.Maximum, b.ChargeCode, b.chargeamount, a.thresholdamount from kpadminpartners.billspaychargedetails b " +
                                    "inner join kpadminpartners.accountdetail a on b.ChargeCode = a.BracketTierCode " +
                                    "where a.AccountID = @AccId and a.currency = @Currency";
                    cn.command.CommandText = query;
                    cn.command.Parameters.AddWithValue("AccId", AccId);
                    cn.command.Parameters.AddWithValue("Currency", Currency);
                    using (MySqlDataReader rdr = cn.command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                            maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                            Thold = Convert.ToDecimal(rdr["thresholdamount"]);
                            charge = Convert.ToDecimal(rdr["chargeamount"].ToString());
                            if ((amount > minAmount && amount < maxAmount))
                            {
                                result = charge;
                            }
                        }
                    }
                }
                dcon.Close();
            }
            catch (Exception ex)
            {
                dcon.Close();
                throw new Exception(ex.ToString());
            }
        }
        return result;
    }

    public void CheckExcellProcesses()
    {
        Process[] AllProcesses = Process.GetProcessesByName("excel");
        myHashtable = new Hashtable();
        int iCount = 0;

        foreach (Process ExcelProcess in AllProcesses)
        {
            myHashtable.Add(ExcelProcess.Id, iCount);
            iCount = iCount + 1;
        }
    }
    public void KillExcel()
    {
        Process[] AllProcesses = Process.GetProcessesByName("excel");

        // check to kill the right process
        foreach (Process ExcelProcess in AllProcesses)
        {
            if (myHashtable.ContainsKey(ExcelProcess.Id) == true)
                ExcelProcess.Kill();
        }

        AllProcesses = null;
    }
}
