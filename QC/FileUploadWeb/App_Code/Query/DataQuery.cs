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
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for DataQuery
/// </summary>
public class DataQuery
{

    private MySqlCommand command;
    public String generateBatchNo(String Upload)
    {
        Connection cn = new Connection();
        cn.ConnectConfig();
        string batchNum = "";
        string tempNum = "";
        using (MySqlConnection con = cn.dbconkp.getConnection())
        {
            try
            {
                int btyear = 0;
                int btmonth = 0;
                int ctr;
                DateTime bn = DateTime.Now;
                int year = bn.Year;
                string month = bn.ToString("MM");

                con.Open();
                using (command = con.CreateCommand())
                {
                    if (Upload == "File")
                    {
                        String query = "Select BatchNo from kppartners.souploadheader ORDER BY UploadDate DESC LIMIT 1";
                        command.CommandText = query;
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                string bnum = reader["BatchNo"].ToString();

                                reader.Close();
                                String storedMonth = "call kppartners.selectMaxMonth";
                                command.CommandText = storedMonth;
                                using (MySqlDataReader read = command.ExecuteReader())
                                {
                                    if (read.Read())
                                    {
                                        btmonth = Convert.ToInt32(read["month"].ToString());
                                    }
                                    read.Close();
                                }

                                String storedYear = "call kppartners.selectMaxYear";
                                command.CommandText = storedYear;

                                using (MySqlDataReader rd = command.ExecuteReader())
                                {
                                    if (rd.Read())
                                    {
                                        btyear = Convert.ToInt32(rd["year"].ToString());
                                    }

                                    rd.Close();
                                }


                                if ((btmonth < Convert.ToInt32(month) && btyear == year) || (btyear < year))
                                {
                                    tempNum = "00001";
                                }
                                else
                                {
                                    ctr = Convert.ToInt32(bnum.Substring(6, 5));
                                    ctr = ctr + 1;

                                    if (Convert.ToString(ctr).Length == 1)
                                    {
                                        tempNum = "0000" + Convert.ToString(ctr);
                                    }
                                    else if (Convert.ToString(ctr).Length == 2)
                                    {
                                        tempNum = "000" + Convert.ToString(ctr);
                                    }
                                    else if (Convert.ToString(ctr).Length == 3)
                                    {
                                        tempNum = "00" + Convert.ToString(ctr);
                                    }
                                    else if (Convert.ToString(ctr).Length == 4)
                                    {
                                        tempNum = "0" + Convert.ToString(ctr);
                                    }
                                    else if (Convert.ToString(ctr).Length == 5)
                                    {
                                        tempNum = Convert.ToString(ctr);
                                    }
                                }
                                batchNum = Convert.ToString(year) + month + tempNum;
                            }
                            else
                            {
                                batchNum = Convert.ToString(year) + month + "00001";
                            }
                        }
                    }
                    else
                    {
                        String query = "SELECT DATE_FORMAT(NOW(), '%y%m%d %H:%i:%s') AS BatchFile;";
                        command.CommandText = query;
                        using (MySqlDataReader read = command.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                batchNum = read["BatchFile"].ToString();
                            }
                            read.Close();
                        }
                    }
                }

                con.Close();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }
        return batchNum;
    }
    public DateTime getServerDatePartner(Boolean isOpenConnection)
    {
        try
        {
            if (!isOpenConnection)
            {
                Connection cn = new Connection();
                cn.ConnectConfig();
                using (MySqlConnection conn = cn.dbconkp.getConnection())
                {
                    conn.Open();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        DateTime serverdate;
                        command.CommandText = "Select NOW() as serverdt;";
                        using (MySqlDataReader Reader = command.ExecuteReader())
                        {
                            Reader.Read();
                            serverdate = Convert.ToDateTime(Reader["serverdt"]);
                            //Reader.Close();
                            //conn.Close();
                            return serverdate;
                        }
                    }
                }
            }
            else
            {
                DateTime serverdate;
                command.CommandText = "Select NOW() as serverdt;";
                using (MySqlDataReader Reader = command.ExecuteReader())
                {
                    Reader.Read();
                    serverdate = Convert.ToDateTime(Reader["serverdt"]);
                    Reader.Close();
                    return serverdate;
                }
            }
        }
        catch (Exception ex)
        {
            //kplog.Fatal(ex.ToString());
            throw new Exception(ex.Message);
        }
    }
    public string getChargeTypeforTiercode(string Accountid, string Currency, ref MySqlConnection conn)
    {
        string value = string.Empty;

        try
        {
            command = conn.CreateCommand();
            string sql = "select chargetype,thresholdamount from kpadminpartners.accountdetail where accountid = '" + Accountid + "' and currency = '" + Currency + "' ";
            command.CommandText = sql;
            using (MySqlDataReader rdr = command.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    rdr.Read();
                    value = rdr["chargetype"].ToString() + "|" + rdr["thresholdamount"].ToString();
                }
            }
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }

        return value;
    }
    public Boolean CheckTierCode(String AccId, Decimal amount, String Currency, ref MySqlConnection conn)
    {
        decimal minAmount, maxAmount, charge, thold, Rbalance, credlim;
        string Tcode, type, cType, thold2;
        int ret = 0;
        bool datareturn = false;
        type = getChargeTypeforTiercode(AccId, Currency, ref conn);
        string[] datasplit = type.Split('|');
        cType = datasplit[0].ToString();
        thold2 = datasplit[1].ToString();

        if (cType == "Tier Bracket")
        {
            try
            {
                command = conn.CreateCommand();
                string query = "select b.Minimum,b.Maximum,b.TierCode,b.Chargeamount,a.thresholdamount,a.runningbalance,a.creditlimit from kpadminpartners.tierdetails b " +
                               "inner join kpadminpartners.accountdetail a on b.TierCode = a.BracketTierCode " +
                               "where a.accountid = @AccId and a.currency = @Currency";

                command.Parameters.Clear();
                command.Parameters.AddWithValue("AccId", AccId);
                command.Parameters.AddWithValue("Currency", Currency);
                command.CommandText = query;
                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                        maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                        Tcode = rdr["TierCode"].ToString();
                        charge = Convert.ToDecimal(rdr["Chargeamount"].ToString());
                        //thold = maxAmount; //Convert.ToDecimal(rdr["thresholdamount"]);
                        Rbalance = Convert.ToDecimal(rdr["runningbalance"].ToString());
                        credlim = Convert.ToDecimal(rdr["creditlimit"].ToString());
                        if (amount <= maxAmount && (amount >= minAmount && amount <= maxAmount))
                        {
                            datareturn = true;
                        }
                        ret = 1;
                    }
                }

                if (ret == 0)
                {
                    datareturn = checkTiercodeBillsPay(AccId, amount, Currency);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        else
        {
            if (string.IsNullOrEmpty(thold2) & thold2 == null)
            {
                thold = 0;
            }
            else
            {
                thold = Convert.ToDecimal(thold2);
            }
            if (amount <= thold)
            {
                datareturn = true;
            }
        }
        return datareturn;
    }

    private Boolean checkTiercodeBillsPay(String AccId, Decimal amount, String Currency)
    {
        bool ret = false;
        decimal minAmount, maxAmount, Thold, charge;

        try
        {
            string query = "select b.Minimum, b.Maximum, b.ChargeCode, b.chargeamount, a.thresholdamount,a.runningbalance,a.creditlimit from kpadminpartners.billspaychargedetails b " +
                            "inner join kpadminpartners.accountdetail a on b.ChargeCode = a.BracketTierCode " +
                            "where a.AccountID = @AccId and a.currency = @Currency";
            command.Parameters.Clear();
            command.CommandText = query;
            command.Parameters.AddWithValue("AccId", AccId);
            command.Parameters.AddWithValue("Currency", Currency);
            using (MySqlDataReader rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                    maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                    Thold = Convert.ToDecimal(rdr["thresholdamount"]);
                    charge = Convert.ToDecimal(rdr["chargeamount"].ToString());
                    if (amount <= Thold && (amount > minAmount && amount < maxAmount))
                    {
                        ret = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }

        return ret;
    }

    public Int32 checkreference(String refno, String AccountCode)
    {
        int flag;

        String query = "Select ReferenceNo from kppartners.sotxnref where ReferenceNo = @refnumber and AccountCode = @AccountCode and CancelledDate is null";
        command.CommandText = query;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("refnumber", refno);
        command.Parameters.AddWithValue("AccountCode", AccountCode);
        using (MySqlDataReader rdr = command.ExecuteReader())
        {
            if (rdr.Read())
            {
                flag = 1;
            }
            else
            {
                flag = 0;
            }
        }

        return flag;
    }

    public String generateControlGlobal(String branchcode, Int32 type, String OperatorID, Int32 ZoneCode, String StationNumber, Double version, String stationcode)
    {
        String controlNumber = "";
        String controlno = "";
        try
        {
            DateTime dt = getServerDatePartner(true);

            String control = "";

            command.CommandText = "Select station, bcode, userid, nseries, zcode, type from kpadminpartners.control where station = @st and bcode = @bcode and zcode = @zcode and `type` = @tp FOR UPDATE";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("st", StationNumber);
            command.Parameters.AddWithValue("bcode", branchcode);
            command.Parameters.AddWithValue("zcode", ZoneCode);
            command.Parameters.AddWithValue("tp", type);
            MySqlDataReader Reader = command.ExecuteReader();

            if (Reader.HasRows)
            {
                //throw new Exception("Invalid type value");
                Reader.Read();
                //throw new Exception(Reader["station"].ToString() + " " + Reader["bcode"].ToString() + " " + Reader["type"].ToString());
                if (type == 0)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 1)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 2)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else if (type == 3)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else
                {
                    throw new Exception("Invalid type value");
                }

                String s = Reader["Station"].ToString();
                String nseries = Reader["nseries"].ToString().PadLeft(6, '0');
                Int32 seriesno = Convert.ToInt32(nseries) + 1;
                nseries = seriesno.ToString().PadLeft(6, '0');
                Reader.Close();

                if (isSameYear2(dt))
                {
                    controlno = control + "-" + dt.ToString("yy") + "-" + nseries;
                }
                else
                {
                    controlno = control + "-" + dt.ToString("yy") + "-" + "000001";
                }
                command.Parameters.Clear();
                command.CommandText = "Update kpadminpartners.control set nseries='" + nseries + "' where " +
                                      "station = @stu and bcode = @bcodeu and zcode = @zcodeu and `type` = @tpu;";
                command.Parameters.AddWithValue("stu", StationNumber);
                command.Parameters.AddWithValue("bcodeu", branchcode);
                command.Parameters.AddWithValue("zcodeu", ZoneCode);
                command.Parameters.AddWithValue("tpu", type);
                command.ExecuteNonQuery();

                command.Dispose();
            }
            else
            {
                Reader.Close();
                command.CommandText = "Insert into kpadminpartners.control (`station`,`bcode`,`userid`,`nseries`,`zcode`, `type`) values (@station,@branchcode,@uid,1,@zonecode,@type)";
                if (type == 0)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 1)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 2)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else if (type == 3)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else
                {
                    throw new Exception("Invalid type value");
                }
                command.Parameters.AddWithValue("station", StationNumber);
                command.Parameters.AddWithValue("branchcode", branchcode);
                command.Parameters.AddWithValue("uid", OperatorID);
                command.Parameters.AddWithValue("zonecode", ZoneCode);
                command.Parameters.AddWithValue("type", type);
                int x = command.ExecuteNonQuery();
                //if (x < 1) {
                //    conn.Close();
                //    throw new Exception("asdfsadfds");
                //}

                controlno = control + "-" + dt.ToString("yy") + "-" + "000001";
                command.Dispose();
            }
        }
        catch (Exception)
        {
            return "ErrorOnControlNumber";
        }

        return controlNumber = controlno;
    }

    private Boolean isSameYear2(DateTime date)
    {
        try
        {
            //throw new Exception(date.Year.ToString());
            if (GetYesterday2(date).Year.Equals(date.Year))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
    }
    private DateTime GetYesterday2(DateTime date)
    {
        return date.AddDays(-1);
    }
}
