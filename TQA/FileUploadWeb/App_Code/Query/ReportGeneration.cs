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
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MySql.Data.MySqlClient;
/// <summary>
/// Summary description for ReportGeneration
/// </summary>
/// 

public class ReportGeneration
{
    string pathErrorFile = AppDomain.CurrentDomain.BaseDirectory + "ReportsFormat\\rptFileUploadError.rpt";
    public DataSet ds = new DataSet();
    public DataTable GenerateErrorReport(List<string> arr, string PartnersBnumber, string MlBatchNumber, string PartnersName)
    {
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();
        DataTable dt3 = new DataTable();
        string dtTime = DateTime.Now.ToString("MMMM dd,yyyy hh:mm:ss tt");
        DataTable MergeTable = new DataTable("MergeTable");

        dt3 = rptError(arr, "InsertSuccess", dt3, PartnersName);

        dt.Merge(dt1);
        dt.Merge(dt2);
        dt.Merge(dt3);
        MergeTable = dt;

        return MergeTable;
    }
    public DataTable GenerateSuccessReport(List<string> arr)
    {
        DataTable paramDt = ds.Tables.Add("uploadreport");
        DataColumn itemno = new DataColumn("itemno", typeof(System.String));
        DataColumn KPTN = new DataColumn("KPTN", typeof(System.String));
        DataColumn Refno = new DataColumn("Refno", typeof(System.String));
        DataColumn sendersname = new DataColumn("sendersname", typeof(System.String));
        DataColumn receiversname = new DataColumn("receiversname", typeof(System.String));
        DataColumn principalamount = new DataColumn("principalamount", typeof(System.Double));
        DataColumn charges = new DataColumn("charges", typeof(System.Double));
        DataColumn Currency = new DataColumn("Currency", typeof(System.String));
        DataColumn AppNum = new DataColumn("AppNum", typeof(System.String));
        paramDt.Columns.Add(itemno);
        paramDt.Columns.Add(KPTN);
        paramDt.Columns.Add(Refno);
        paramDt.Columns.Add(sendersname);
        paramDt.Columns.Add(receiversname);
        paramDt.Columns.Add(principalamount);
        paramDt.Columns.Add(charges);
        paramDt.Columns.Add(Currency);
        paramDt.Columns.Add(AppNum);

        int var = 0;
        DataRow dtr = null;

        foreach (string item in arr)
        {
            if (item != null)
            {
                var = var + 1;
                dtr = paramDt.NewRow();

                string[] datasplit = item.Split('|');
                // dtr["itemno"] = var;
                dtr["KPTN"] = datasplit[1].ToString();
                dtr["Refno"] = datasplit[3].ToString();
                dtr["sendersname"] = datasplit[10].ToString() + "," + datasplit[11].ToString() + " " + datasplit[12].ToString();
                dtr["receiversname"] = datasplit[22].ToString() + "," + datasplit[23].ToString() + " " + datasplit[24].ToString();
                dtr["principalamount"] = datasplit[4].ToString();
                dtr["charges"] = System.Convert.ToDecimal(datasplit[6].ToString());
                dtr["Currency"] = datasplit[5].ToString();
                dtr["AppNum"] = datasplit[0].ToString();

                paramDt.Rows.Add(dtr);
                paramDt.AcceptChanges();
            }
        }
        return paramDt;
    }

    private DataTable rptError(List<string> arr, string table, DataTable paramDt, String PartnersName)
    {
        paramDt = ds.Tables.Add(table);
        DataColumn ID = new DataColumn("ID", typeof(System.Int32));
        DataColumn kptn = new DataColumn("kptn", typeof(System.String));
        DataColumn refno = new DataColumn("refno", typeof(System.String));
        DataColumn SenderName = new DataColumn("Sname", typeof(System.String));
        DataColumn ReceiverName = new DataColumn("Rname", typeof(System.String));
        DataColumn Currency = new DataColumn("Currency", typeof(System.String));
        DataColumn Principal = new DataColumn("PAmount", typeof(System.Decimal));
        DataColumn Charges = new DataColumn("Charges", typeof(System.Decimal));
        DataColumn ErrorMsg = new DataColumn("Error", typeof(System.String));
        paramDt.Columns.Add(ID);
        paramDt.Columns.Add(kptn);
        paramDt.Columns.Add(refno);
        paramDt.Columns.Add(SenderName);
        paramDt.Columns.Add(ReceiverName);
        paramDt.Columns.Add(Currency);
        paramDt.Columns.Add(Principal);
        paramDt.Columns.Add(Charges);
        paramDt.Columns.Add(ErrorMsg);

        paramDt.PrimaryKey = new DataColumn[] { ID };
        int var = 0;
        DataRow dtr = null;

        foreach (string item in arr)
        {
            if (item != null)
            {
                var = var + 1;
                dtr = paramDt.NewRow();

                string[] datasplit = item.Split('|');
                dtr["ID"] = var;

                dtr["kptn"] = datasplit[1].ToString().Trim();
                dtr["refno"] = datasplit[3].ToString().Trim();
                dtr["Sname"] = datasplit[10].ToString().Trim() + "," + datasplit[11].ToString().Trim() + " " + datasplit[12].ToString().Trim();
                dtr["Rname"] = datasplit[22].ToString().Trim() + "," + datasplit[23].ToString().Trim() + " " + datasplit[24].ToString().Trim();
                dtr["Currency"] = datasplit[5].ToString().Trim();
                dtr["PAmount"] = System.Convert.ToDecimal(datasplit[4].ToString());
                dtr["Charges"] = System.Convert.ToDecimal(datasplit[6].ToString());
                dtr["Error"] = datasplit[32].ToString().Trim();

                paramDt.Rows.Add(dtr);
                paramDt.AcceptChanges();
            }
        }
        return paramDt;
    }

    public DataTable GenerateSuccessReportManual(List<string> arr)
    {
        DataTable dt = ds.Tables.Add("ManualReport");
        DataColumn Reference = new DataColumn("Reference", typeof(System.String));
        DataColumn KPTN = new DataColumn("KPTN", typeof(System.String));
        DataColumn Sender = new DataColumn("Sender", typeof(System.String));
        DataColumn Receiver = new DataColumn("Receiver", typeof(System.String));
        DataColumn ReceiverAddress = new DataColumn("ReceiverAddress", typeof(System.String));
        DataColumn ReceiverContact = new DataColumn("ReceiverContact", typeof(System.String));
        DataColumn Principal = new DataColumn("Principal", typeof(System.Double));
        DataColumn Charge = new DataColumn("Charge", typeof(System.Double));
        DataColumn Total = new DataColumn("Total", typeof(System.Double));
        DataColumn SenderAddress = new DataColumn("SenderAddress", typeof(System.String));
        DataColumn SenderContact = new DataColumn("SenderContact", typeof(System.String));

        dt.Columns.Add(Reference);
        dt.Columns.Add(KPTN);
        dt.Columns.Add(Sender);
        dt.Columns.Add(Receiver);
        dt.Columns.Add(ReceiverAddress);
        dt.Columns.Add(ReceiverContact);
        dt.Columns.Add(Principal);
        dt.Columns.Add(Charge);
        dt.Columns.Add(Total);
        dt.Columns.Add(SenderAddress);
        dt.Columns.Add(SenderContact);

        int var = 0;
        DataRow dtr = null;

        foreach (string item in arr)
        {
            if (item != null)
            {
                var = var + 1;
                dtr = dt.NewRow();

                string[] datasplit = item.Split('|');
                dtr["Reference"] = datasplit[0].ToString();
                dtr["KPTN"] = datasplit[12].ToString();
                dtr["Sender"] = datasplit[2].ToString() + " " + datasplit[3].ToString() + " " + datasplit[1].ToString();
                dtr["Receiver"] = datasplit[7].ToString() + " " + datasplit[8].ToString() + " " + datasplit[6].ToString();
                dtr["ReceiverAddress"] = datasplit[9].ToString();
                dtr["ReceiverContact"] = datasplit[10].ToString();
                dtr["Principal"] = System.Convert.ToDouble(datasplit[11].ToString());
                dtr["Charge"] = System.Convert.ToDouble(datasplit[13].ToString());
                dtr["Total"] = System.Convert.ToDouble(datasplit[14].ToString());
                dtr["SenderAddress"] = datasplit[4].ToString();
                dtr["SenderContact"] = datasplit[5].ToString();

                dt.Rows.Add(dtr);
                dt.AcceptChanges();
            }
        }
        return dt;
    }

    public String getOperatorName(string operatorID, string accountcode)
    {
        Connection con = new Connection();
        String fullname = "";
        MySqlCommand command;
        MySqlTransaction trans;
        con.ConnectConfig();
        MySqlConnection mycon = con.dbconkp.getConnection();
        try
        {
            mycon.Open();
            using (command = mycon.CreateCommand())
            {
                trans = mycon.BeginTransaction();
                command.Transaction = trans;

                command.CommandText = "Select Concat(Firstname,' ',Middlename,' ',Lastname) as Fullname FROM kpadminpartners.partnersusers WHERE userID = '" + operatorID + "' and accountid = '" + accountcode + "';";
                using (MySqlDataReader Reader = command.ExecuteReader())
                {
                    if (Reader.Read())
                    {
                        fullname = Reader["Fullname"].ToString();
                    }
                    Reader.Close();
                }
            }
            mycon.Close();
        }
        catch (Exception error)
        {
            error.ToString();
        }

        return fullname;
    }
}

