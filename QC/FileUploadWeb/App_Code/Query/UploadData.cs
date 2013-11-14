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

/// <summary>
/// Summary description for UploadData
/// </summary>
public class UploadData
{
    Connection ser = new Connection();
    SMFunctions smf = new SMFunctions();
    private MySqlCommand command;
    private MySqlTransaction trans;

    private string upload = "";
    private string batchfile = "";
    private string accountCode = "";
    private string stationcode = "";
    private string filename = "";

    public int NoOfTrans { get; set; }
    public string BatchNo { get; set; }
    public string AccountCode { get; set; }
    public string CorporateBatchNo { get; set; }

    public string SubmitToDatabase(string sessionId, string _accountCode, string _batchfile, string _stationcode, string _upload, string _filename)
    {
        string Msg;
        string tmpTable = "";
        upload = _upload;
        batchfile = _batchfile;
        accountCode = _accountCode;
        stationcode = _stationcode;
        filename = _filename;

        ser.ConnectConfig();
        MySqlConnection con = ser.dbconkp.getConnection();
        try
        {
            con.Open();
            using (command = con.CreateCommand())
            {
                trans = con.BeginTransaction();
                command.Transaction = trans;
                if (upload == "File")
                {
                    tmpTable = "kppartners.tmpp2ptable";
                }
                else
                {
                    tmpTable = "kppartners.tempManualUpload";
                }

                if (InsertTable365(ref con, sessionId, tmpTable) == 0)
                {
                    Msg = "Error";
                    return Msg;
                }
                return Msg = "Success";
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            con.Close();
            throw ex;
        }
    }

    private int InsertTable365(ref MySqlConnection conn, string sessionID, string temptable)
    {
        int i = 0;
        //int DeleteTmpTbl = 0;
        int soUploadInserted = 0;
        int CorporateInserted = 0;
        int TransLogInserted = 0;
        int soTransRefInserted = 0;
        bool UpdateTmpTbl = false;
        using (command = conn.CreateCommand())
        {
            DateTime serverdate;
            string table;
            command.CommandText = "Select NOW() as serverdt;";
            using (MySqlDataReader Reader = command.ExecuteReader())
            {
                Reader.Read();
                serverdate = Convert.ToDateTime(Reader["serverdt"]);
                Reader.Close();
            }

            table = serverdate.ToString("MM-dd").Replace("-", "");
            string sql = "";
            //SELECT Fields under Temp Table
            if (upload == "File")
            {
                sql = "INSERT INTO kppartners.sendout" + table + " (ControlNo, ReferenceNo, Currency, Principal," +
                                                               "Charge, AccountCode, TransDate, " +
                                                               "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                               "ReceiverName, ReceiverGender, ReceiverContactNo,ReceiverBirthDate, " +
                                                               "SenderGender, SenderContactNo, sessionID, OtherDetails, OperatorID," +
                                                               "StationID, ReceiverStreet, ReceiverCountry, KPTN, Message, BranchCode, SenderBirthdate) " +
                               "Select ControlNumber,refno,currency,principal,charge,AccountId,now(),senderfname,senderlname,sendermname,CONCAT(senderfname,' ',sendermname,' ',SenderLName) AS SenderName, " +
                               "receiverfname,receiverlname,receivermname,CONCAT(ReceiverFName,' ',Receivermname,' ',ReceiverLName) AS ReceiverName, receivergender, receiverphonenum,receiverbdate,sendergender,senderphone, " +
                               "sessionId,CONCAT (itemno,'|',kptn,'|',dttiled,'|',sourceoffund,'|',relationtoreceiver,'|',purpose,'|',senderIdtype,'|',senderIdnum,'|',senderidexpiredate) AS OtherDetails, " +
                               "OperatorID,stationID,receiverstreet,receiverscountry,kptn,message,branchcode,senderbirthdate " +
                               "FROM " + temptable + " where sessionId = '" + sessionID + "'";
            }
            else
            {
                sql = "INSERT INTO kppartners.sendout" + table + " (ControlNo, ReferenceNo, KPTN, SenderName, SenderLName, SenderFName, SenderMName, SenderAddress, SenderContactNo, " +
                                                               "ReceiverName, ReceiverLName, ReceiverFName, ReceiverMName, ReceiverAddress, ReceiverContactNo, " +
                                                               "Principal, Charge, Total, AccountCode, Currency, OperatorID, StationID, BranchCode, TransDate) " +
                               "Select ControlNo, ReferenceNo, KptnNo, SenderName, SenderLName, SenderFName, SenderMName, SenderAddress, SenderContactNo, " +
                               "ReceiverName, ReceiverLName, ReceiverFName, ReceiverMName, ReceiverAddress, ReceiverContactNo, " +
                               "Principal, Charge, (Principal + Charge), AccountCode, Currency, OperatorID, StationID, BranchCode, now() " +
                               "FROM " + temptable + " WHERE BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
            }
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
            NoOfTrans = i;
            if (i <= 0)//Error Insert in 365
            {
                //i = 0;
                trans.Rollback();
                conn.Close();
                return i = 0;
            }
            else
            {
                if (upload == "File")
                {
                    sql = "SELECT DISTINCT Currency,AccountId,batchNumPartners,MlbatchNum FROM " + temptable + " WHERE sessionId = '" + sessionID + "'";
                }
                else
                {
                    sql = "SELECT DISTINCT Currency,AccountCode,'',BatchFile FROM " + temptable + " WHERE BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
                }

                command.CommandText = sql;
                List<string> Currency = new List<string>();

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Currency.Add(rdr[0].ToString());
                            AccountCode = rdr[1].ToString();
                            CorporateBatchNo = rdr[2].ToString();
                            BatchNo = rdr[3].ToString();
                        }
                        rdr.Close();
                        foreach (string item in Currency)
                        {
                            UpdateTmpTbl = getNewRB(AccountCode, SumAmount(item, sessionID, ref conn, temptable), item, ref conn);
                            if (!UpdateTmpTbl)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                        }
                        //i = 1;
                    }
                    else
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                }
                if (UpdateTmpTbl)
                {
                    soUploadInserted = InsertSoUploadHeader(ref conn);
                    if (soUploadInserted <= 0)
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                    else
                    {
                        CorporateInserted = InsertCorporateSendout(ref conn, temptable, sessionID);
                        if (CorporateInserted <= 0)
                        {
                            trans.Rollback();
                            conn.Close();
                            return i = 0;
                        }
                        else
                        {
                            TransLogInserted = InsertTransactionslog(ref conn, sessionID, temptable);
                            if (TransLogInserted <= 0)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                            else
                            {
                                soTransRefInserted = InsertSotxnRef(ref conn, table, sessionID, temptable);
                                if (soTransRefInserted <= 0)
                                {
                                    trans.Rollback();
                                    conn.Close();
                                    return i = 0;
                                }
                                else
                                {
                                    bool result = true;
                                    if (upload == "File")
                                    {
                                        sql = "INSERT INTO kpUploadArchive.tmpp2ptable(ControlNumber, AccountId, batchNumPartners, MlbatchNum, branchName, branchcode, stationID, stationnumber, OperatorID, ";
                                        sql = sql + "zonecode, sessionId, itemno, kptn, dttiled, refno, principal, currency, charge, sourceoffund, relationtoreceiver, ";
                                        sql = sql + "purpose, senderlname, senderfname, sendermname, senderIdtype, senderIdnum, senderidexpiredate, senderbirthdate, sendergender, ";
                                        sql = sql + "senderstreet, senderprovince, sendercountry, senderphone, receiverlname, receiverfname, receivermname, receiverstreet, receiversprovince, ";
                                        sql = sql + "receiverscountry, receiverbdate, receivergender, receiverphonenum, message, filename)";

                                        sql = sql + "select ControlNumber, 	AccountId, batchNumPartners, MlbatchNum, branchName, branchcode, stationID, stationnumber, OperatorID, ";
                                        sql = sql + "zonecode, sessionId, itemno, kptn, dttiled, refno, principal, currency, charge, sourceoffund, relationtoreceiver, ";
                                        sql = sql + "purpose, senderlname, senderfname, sendermname, senderIdtype, senderIdnum, senderidexpiredate, senderbirthdate, sendergender,  ";
                                        sql = sql + "senderstreet, senderprovince, sendercountry, senderphone, receiverlname, receiverfname, receivermname, receiverstreet, receiversprovince,";
                                        sql = sql + "receiverscountry, receiverbdate, receivergender, receiverphonenum, message, '" + filename + "' from kppartners.tmpp2ptable where sessionId = '" + sessionID + "'";
                                    }
                                    else
                                    {
                                        sql = "INSERT INTO kpUploadArchive.tempManualUpload(Referenceno, Controlno, KptnNo,SenderName, SenderLName,SenderFName,SenderMName, SenderAddress, SenderContactNo, ";
                                        sql = sql + "ReceiverName, ReceiverLName,ReceiverFName,ReceiverMName,ReceiverAddress, ReceiverContactNo, Principal, Charge, ";
                                        sql = sql + "Total, AccountCode, Currency, OperatorId, StationId, BranchCode,ZoneCode, TransDate, BatchFile, Filename)";

                                        sql = sql + "SELECT ReferenceNo, ControlNo, Kptnno, SenderName, SenderLName, SenderFName, SenderMName, SenderAddress, SenderContactNo, ";
                                        sql = sql + "ReceiverName, ReceiverLName, ReceiverFName, ReceiverMName, ReceiverAddress, ReceiverContactNo, Principal, Charge, ";
                                        sql = sql + "(Principal + Charge), AccountCode, Currency, OperatorID, StationId, BranchCode, ZoneCode, now(), BatchFile, '" + filename + "' ";
                                        sql = sql + "FROM kppartners.tempManualUpload WHERE BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
                                    }

                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }
                                    if (upload == "File")
                                    {
                                        sql = "DELETE FROM kppartners.tmpp2ptable where sessionId = '" + sessionID + "'";
                                    }
                                    else
                                    {
                                        sql = "DELETE FROM kppartners.tempManualUpload WHERE BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
                                    }
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }
                                    if (result)
                                    {
                                        trans.Commit();
                                        conn.Close();
                                        return i = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return i;
    }

    public bool getNewRB(String accid, Decimal TotalAmount, String currency, ref MySqlConnection conn)
    {
        decimal NeWr;
        bool result = false;
        //Int16 ret = 0;
        NeWr = getRBalance(accid, TotalAmount, currency, ref conn);
        result = UpdateRunningBalance(NeWr, accid, currency, ref conn);
        return result;
    }

    private int InsertSoUploadHeader(ref MySqlConnection con)
    {
        int i = 0;
        if (upload != "File")
        {
            i = 1;
            goto Manual;
        }

        string sql;
        using (command = con.CreateCommand())
        {
            sql = "INSERT INTO kppartners.souploadheader (BatchNo,AccountCode,NoOfTrxn,UploadDate,CorporateBatchNo)" +
                  "VALUES('" + BatchNo + "','" + AccountCode + "','" + NoOfTrans + "',NOW(),'" + CorporateBatchNo + "')";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
    Manual:
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertCorporateSendout(ref MySqlConnection conn, string table, string sessionId)
    {
        int i = 0;
        string sql;
        using (command = conn.CreateCommand())
        {
            if (upload == "File")
            {
                sql = "insert into kppartnerstransactions.corporatesendouts (controlno,kptn,oldkptn,referenceno," + //4
                        "accountid,currency,transdate,stationno,isremote,operatorid,branchcode,zonecode,remoteoperatorid," + //9
                        "remotebranchcode,remotezonecode,principal,chargeto,chargeamount,othercharge,total,forex,traceno," + //9
                        "sessionid,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,isactive)" +//8

                        "select ControlNumber,kptn,'',refno,accountId,currency,NOW(),stationnumber,'0',OperatorId,branchcode,zonecode,'','','', " + //15
                        "principal,'',charge,'','0.00','0.00','',sessionId,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,'1' " +
                        "FROM " + table + " WHERE sessionId = '" + sessionId + "'";
            }
            else
            {
                sql = "INSERT INTO kppartnerstransactions.corporatesendouts(referenceno, controlno, kptn, senderLname, senderFname, senderMname," + //6
                      "receiverLname, receiverFname, receiverMname, principal, chargeamount, total, accountid, currency, operatorID, stationno, branchcode, zonecode, transdate, isactive)" +//14

                        "SELECT referenceno, controlno, kptnno, senderLName, senderFName, senderMName, receiverLName, receiverFName, receiverMName, " + //9
                        "Principal, Charge, (Principal + Charge), AccountCode, Currency, OperatorID, StationId, BranchCode, ZoneCode, now(), 1 " +
                        "FROM " + table + " WHERE BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
            }

            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertTransactionslog(ref MySqlConnection con, string sessionId, string table)
    {
        int i = 0;
        string sql;
        using (command = con.CreateCommand())
        {
            if (upload == "File")
            {
                sql = "Insert into kpadminpartnerslog.transactionslogs (kptnno,refno,AccountCode,action,type,branchcode" + //6
                      ",operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno,isremote) " + //11

                      "Select kptn,refno,AccountId,'SENDOUT','kppartners',branchcode,OperatorID,stationnumber,zonecode,stationId,now(),Currency," +
                      "CONCAT(senderfname,' ',sendermname,' ',SenderLName) AS SenderName,CONCAT(ReceiverFName,' ',Receivermname,' ',ReceiverLName) AS ReceiverName," +
                      "principal,ControlNumber,0 from " + table + " WHERE sessionId = '" + sessionId + "'";
            }
            else
            {
                sql = "INSERT INTO kpadminpartnerslog.transactionslogs (kptnno, refno, accountCode, action, isremote, txndate, stationcode, stationno, " +
                      "zonecode, operatorid, branchCode, Type, currency, sendername, receivername, controlno, principal) " + //11

                     "SELECT kptnno, referenceno, AccountCode, 'SENDOUT', 0, now(), '" + stationcode + "', StationId, ZoneCode, OperatorID, BranchCode, " +
                     "'kppartners', Currency, SenderName, ReceiverName, ControlNo, Principal " +
                     "FROM " + table + " WHERE BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
            }

            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertSotxnRef(ref MySqlConnection con, string pdate, string sessionId, string table)
    {
        int i = 0;
        string sql;
        string tblref = "sendout" + pdate;
        using (command = con.CreateCommand())
        {
            if (upload == "File")
            {
                sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) " +
                 "select refno,'" + tblref + "',AccountId,MlbatchNum,now(),Currency,'2' FROM " + table + " where sessionid = '" + sessionId + "'";
            }
            else
            {
                sql = "INSERT INTO kppartners.sotxnref(referenceno, tablereference, accountcode, transdate, currency, transactiontype, BatchNo) " +
                 "SELECT ReferenceNo, '" + tblref + "', AccountCode, now(), Currency, '4', BatchFile FROM " + table + " WHERE BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
            }
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    public decimal SumAmount(string Currency, string sessionid, ref MySqlConnection con, string tableName)
    {
        decimal Sum;
        string sql = "";
        using (command = con.CreateCommand())
        {
            if (upload == "File")
            {
                sql = "select (sum(principal) + sum(charge)) as totalPrincipal from " + tableName + " where Currency = '" + Currency + "' and sessionid = '" + sessionid + "'";
            }
            else
            {
                sql = "SELECT (sum(principal) + sum(charge)) as totalPrincipal from " + tableName + " where Currency = '" + Currency + "' and BatchFile = '" + batchfile + "' and AccountCode = '" + accountCode + "'";
            }

            command.CommandText = sql;
            using (MySqlDataReader rdr = command.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    rdr.Read();
                    Sum = Convert.ToDecimal(rdr[0]);
                }
                else
                {
                    Sum = 0;
                }
            }
        }
        return Sum;
    }

    private decimal getRBalance(String AccId, Decimal TINAmount, String Currency, ref MySqlConnection con)
    {
        decimal NewBalance = 0;
        using (command = con.CreateCommand())
        {
            string query = "Select runningbalance,creditlimit from kpadminpartners.accountdetail where accountid = @AccId and currency = @Currency";
            command.CommandText = query;
            command.Parameters.AddWithValue("AccId", AccId);
            command.Parameters.AddWithValue("Currency", Currency);

            using (MySqlDataReader rdr = command.ExecuteReader())
            {
                if (rdr.Read())
                {
                    decimal rB = Convert.ToDecimal(rdr["runningbalance"].ToString());
                    decimal cL = Convert.ToDecimal(rdr["creditlimit"].ToString());
                    decimal result = rB - TINAmount;
                    NewBalance = result;
                    if (result < 0)
                    {
                        result = (rB + cL) - TINAmount;
                    }
                }
            }
        }
        return NewBalance;
    }

    private bool UpdateRunningBalance(Decimal NewRbalance, String AccId, String Currency, ref MySqlConnection conn)
    {
        bool res = false;
        try
        {
            using (command = conn.CreateCommand())
            {
                string query = " UPDATE kpadminpartners.accountdetail Set runningbalance = @NewRbalance where accountid = @AccId and currency = @Currency";
                command.CommandText = query;
                command.Parameters.AddWithValue("NewRbalance", NewRbalance);
                command.Parameters.AddWithValue("AccId", AccId);
                command.Parameters.AddWithValue("Currency", Currency);
                int res1 = command.ExecuteNonQuery();
                if (res1 <= 0)
                {
                    res = false;
                }
                else
                {
                    res = true;
                }
            }
        }
        catch (Exception)
        {
            res = false;
        }
        return res;
    }
}
