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
using System.IO;

/// <summary>
/// Summary description for SubMitFunctions
/// </summary>
public class SMFunctions
{

    string[] column;
    int numdata = 1000;
    string File = "";
    string filename = "";
    string tinuodngaFilename = "";
    string message = "";
    string mappath = AppDomain.CurrentDomain.BaseDirectory;
    private Connection cn = new Connection();
    private DataQuery ser = new DataQuery();
    private MySqlTransaction trans;
    string MLbatchNum = "";
    string SO_Upload = "";
    string errorname = "";
    public List<string> ListSuccess = new List<string>();
    public List<string> ListError = new List<string>();
    public String ermsg = "";
    int arraycount = 0;
    int arraytotal = 0;
    public string BatchNo { get; set; }
    public string AccountCode { get; set; }
    public int NoOfTrans { get; set; }
    public string UploadDate { get; set; }
    public string CorporateBatchNo { get; set; }
    public string MLbNumber { get; set; }

    public string _upload { get; set; }
    public string StationCode { get; set; }
    public string BatchFile { get; set; }

    public ReturnListArray P2PTransactions(List<string> Data, String AccountId, String batchnumforPartners, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId, String Upload)
    {
        if (SaveTransactionP2PUpload(Data, AccountId, batchnumforPartners, bname, bcode, stationcode, zonecode, operatorID, stationID, Currency, SessionId, Upload) == "success")
        {
            return new ReturnListArray { resp = 1, data = ListSuccess, Mlbnum = MLbatchNum, AccountCode = AccountId, StationCode = stationcode, msg = message };
        }
        else
        {
            return new ReturnListArray { resp = 0, data = ListError, Mlbnum = MLbatchNum, AccountCode = AccountId, StationCode = stationcode, msg = message };
        }
    }

    public string SaveTransactionP2PUpload(List<string> Data, String AccountId, String batchnumforPartners, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId, String Upload)
    {
        string output = "";
        string ermsg = "";
        int Inserted = 0;
        filename = DateTime.Now.Ticks.ToString() + ".csv";
        string ItemToCSV = "";
        cn.ConnectConfig();
        MLbatchNum = ser.generateBatchNo(Upload);
        MLbNumber = MLbatchNum;
        //AccountCode = AccountId;
        //BatchFile = MLbNumber;
        //StationCode = stationcode;
        //_upload = Upload;
        tinuodngaFilename = mappath + filename;
        string tmpTable = "kppartners.tmpp2ptable";
        String pdate = ser.getServerDatePartner(false).ToString("MM-dd").Replace("-", "");
        String SO_Date = ser.getServerDatePartner(false).ToString("yyyy-MM-dd H:mm:ss");
        //String SO_Date = Convert.ToString(Date);
        MySqlConnection conn = cn.dbconkp.getConnection();
        String query = "";

        try
        {
            conn.Open();
            cn.command = conn.CreateCommand();
            trans = conn.BeginTransaction();
            cn.command.Transaction = trans;
            foreach (string item in Data)
            {
                column = item.Split('|');
                ItemToCSV = item.Replace(",", "");
                SO_Upload = Upload;
                if (Upload == "File")
                {
                    decimal principal = Convert.ToDecimal(column[4].ToString());
                    string reference = column[3].ToString();
                    string curr = column[5].ToString();
                    int zc = Convert.ToInt32(zonecode);

                    arraytotal += 1;
                    if (ser.CheckTierCode(AccountId, principal, curr, ref conn).Equals(true))
                    {
                        int ret = ser.checkreference(reference, AccountId);
                        if (ret == 0)
                        {
                            arraycount += 1;

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                            if (ControlNumber == "ErrorOnControlNumber")
                            {
                                ermsg = "Error on generating ControlNumber";
                                trans.Rollback();
                                conn.Close();
                                return output = "Error on Control Number";
                            }
                            File = File + "," + ControlNumber + "," + AccountId + "," + batchnumforPartners + "," + MLbatchNum + "," + bname + "," + bcode + "," + stationID + "," +
                                   stationcode + "," + operatorID + "," + zonecode + "," + SessionId + "," +
                                   ItemToCSV.Replace('|', ',') + "\r\n";
                            if (numdata == arraycount || Data.Count == arraytotal)
                            {
                                File = File.Substring(0, File.Length - 2);
                                Inserted = Inserted + InsertSuccess(arraycount, ref conn, tmpTable, File);
                                if (Inserted == 0)
                                {
                                    output = "Error Upload";
                                    ermsg = "Error in uploading";
                                    trans.Rollback();
                                    conn.Close();
                                    return output;
                                }
                                File = "";
                            }
                            ListSuccess.Add(item + "|Success");
                        }
                        else
                        {
                            errorname = "Reference no. already exist";
                            ListError.Add(item + "|" + errorname);
                        }
                    }
                    else
                    {
                        errorname = "Amount is Greater than the Threshold";
                        ListError.Add(item + "|" + errorname);
                    }

                    ItemToCSV = "";
                }
                //Manual Upload
                else
                {
                    decimal principal = Convert.ToDecimal(column[11].ToString());
                    string reference = column[0].ToString();
                    string curr = Currency;
                    int zc = Convert.ToInt32(zonecode);
                    arraytotal += 1;

                    if (ser.CheckTierCode(AccountId, principal, curr, ref conn).Equals(true))
                    {
                        int ret = ser.checkreference(reference, AccountId);
                        if (ret == 0)
                        {
                            arraycount += 1;

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                            if (ControlNumber == "ErrorOnControlNumber")
                            {
                                message = "Error on generating Control Number";
                                trans.Rollback();
                                conn.Close();
                                return message;
                            }

                            string Sender_Name = column[2].ToString() + " " + column[3].ToString() + " " + column[1].ToString();
                            string Receiver_Name = column[7].ToString() + " " + column[8].ToString() + " " + column[6].ToString();

                            query = query + "INSERT INTO kppartners.tempManualUpload(Referenceno, Controlno, KptnNo, SenderLName, SenderFName, SenderMName, SenderAddress, SenderContactNo, " +
                             "ReceiverLName, ReceiverFName, ReceiverMName, ReceiverAddress, ReceiverContactNo, Principal, Charge, " +
                             "Total, TransDate, AccountCode, Currency, OperatorId, StationId, BranchCode, ZoneCode, BatchFile, SenderName, ReceiverName) VALUES ('" + reference + "', '" + ControlNumber + "', " +
                             "'" + column[12].ToString() + "', '" + column[1].ToString() + "', '" + column[2].ToString() + "', '" + column[3].ToString() + "',  '" + column[4].ToString() + "','" + column[5].ToString() + "', '" + column[6].ToString() + "', '" + column[7].ToString() + "', '" + column[8].ToString() + "', '" + column[9].ToString() + "', '" + column[10].ToString() + "', " +
                             "'" + column[11].ToString() + "', '" + column[13].ToString() + "', '" + column[14].ToString() + "', '" + SO_Date + "', '" + AccountId + "', '" + Currency + "', '" + operatorID + "', '" + stationID + "', '" + bcode + "', '" + zonecode + "', '" + MLbNumber + "', '" + Sender_Name + "', '" + Receiver_Name + "');" + Environment.NewLine;

                            ListSuccess.Add(item + "|Success");
                        }
                        else
                        {
                            message = "Reference no. '" + reference + "' already exist. Also check other reference no. then back to main page.";
                            return message;
                            //ListError.Add(item + "|" + errorname);
                        }
                    }
                    else
                    {
                        message = "Amount '" + principal + "' Greater than the Threshold. Also check other amount hten back to main page.";
                        return message;
                        //ListError.Add(item + "|" + errorname);
                    }

                    ItemToCSV = "";
                }
            }

            if (Upload == "Manual")
            {
                if (query != "")
                {
                    cn.command.CommandText = query;

                    int x = cn.command.ExecuteNonQuery();

                    if (x < 1)
                    {
                        message = "Error in Uploading";
                        ermsg = "Error in Uploading";
                        trans.Rollback();
                        conn.Close();
                        return message;
                    }
                    else
                    {
                        Inserted = Data.Count;
                    }
                }
            }
            if (Inserted == Data.Count)
            {
                output = "success";
                trans.Commit();
                conn.Close();
                return output;
            }
            else
            {
                output = "failed";
                trans.Rollback();
                conn.Close();
                return output;
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
            output = ex.ToString();
            ermsg = ex.ToString();
            trans.Rollback();
            conn.Close();
            return output;
        }
    }

    private int InsertSuccess(int arraycount, ref MySqlConnection conn, string pDate, string str)
    {
        CreateLog(str, filename);
        int insert = BulkInsert(ref conn, pDate, tinuodngaFilename);
        int Reply = 0;

        if (insert != arraycount)
        {
            Reply = 0;
            return Reply;
        }
        System.IO.File.Delete(mappath + filename);
        Reply = insert;
        arraycount = 0;
        return Reply;
    }
    private void CreateLog(string str, string filename)
    {
        filename = mappath + filename;
        FileStream objFile;
        System.IO.StreamWriter objWriter;
        if (System.IO.File.Exists(filename))
        {
            System.IO.File.Delete(filename);
        }

        objFile = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write);

        objWriter = new System.IO.StreamWriter(objFile);
        objWriter.BaseStream.Seek(0, SeekOrigin.End);
        objWriter.WriteLine(str);
        objWriter.Close();

    }
    private int BulkInsert(ref MySqlConnection conn, string table, string fname)
    {
        int insert = 0;

        var bl = new MySqlBulkLoader(conn);
        bl.TableName = table;
        bl.FieldTerminator = ",";
        bl.LineTerminator = "\n";
        bl.FileName = fname;
        bl.NumberOfLinesToSkip = 0;
        insert = bl.Load();

        return insert;
    }
}
