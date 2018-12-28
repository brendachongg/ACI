using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.OleDb;
using System.Data;
using LogicLayer;
using System.Text;
using System.Configuration;
using System.IO.Compression;
using GeneralLayer;






namespace ACI_TMS
{
    public partial class data_migration : BasePage
    {
        public const string PAGE_NAME = "data-migration.aspx";
        Data_Migration_Management dmm = new Data_Migration_Management();


        public data_migration()
            : base(PAGE_NAME, AccessRight_Constance.DATA_MIGRATION)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            int userId = LoginID;

            string path = System.IO.Path.GetFullPath(fuExcelFile.FileName);
            string appText = "";

            //Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Sample.xlsx;Extended Properties=Excel 12.0

            if (fuExcelFile.HasFile)
            {
                string excelPath = Server.MapPath("~/Files/") + Path.GetFileName(fuExcelFile.PostedFile.FileName);
                fuExcelFile.SaveAs(excelPath);

                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelPath + ";Extended Properties=Excel 12.0";
                //string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelPath + ";Extended Properties='Excel 8.0;IMEX=1;HDR=YES'";

                OleDbConnection connExcel = new OleDbConnection(connectionString);

                OleDbCommand cmdExcel = new OleDbCommand();

                OleDbDataAdapter oda = new OleDbDataAdapter();

                DataTable dt = new DataTable();

                cmdExcel.Connection = connExcel;

                if (ddlModules.SelectedValue == "Modules")
                {
                    connExcel.Open();
                    cmdExcel.CommandText = "SELECT * From [Module$] where Code <> ''";

                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);
                    connExcel.Close();

                    Tuple<bool, string, string>[] result = dmm.InsertIntoModules(dt, userId);
                    string displayTxt = "";

                    foreach (Tuple<bool, string, string> r in result)
                    {
                        if (!r.Item1)
                        {
                            displayTxt += "<span class='label label-danger'>" + "Module: " + r.Item3 + " is not inserted successfully. Reason: " + r.Item2 + "</span><br>";
                        }
                        else
                        {
                            displayTxt += "<span class='label label-success'>" + "Module: " + r.Item3 + " is inserted successfully." + "</span><br>";
                        }
                    }

                    lbResult.Text = displayTxt;
                }

                else if (ddlModules.SelectedValue == "Bundle")
                {
                    connExcel.Close();
                    connExcel.Open();

                    cmdExcel.CommandText = "SELECT * From [Bundle$] where BundleCode <> ''";
                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);
                    connExcel.Close();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {


                        string bundleCode = dt.Rows[i]["BundleCode"].ToString();
                        string bundleType = dt.Rows[i]["Type"].ToString();

                        DataTable dtBundleMod = new DataTable();
                        connExcel.Close();
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [Bundle_Module$] where BundleCode = '" + bundleCode + "'";

                        oda.SelectCommand = cmdExcel;

                        oda.Fill(dtBundleMod);

                        Tuple<bool, string> result = dmm.insertBundle(bundleCode, bundleType, dtBundleMod, userId);

                        if (!result.Item1)
                        {
                            lbResult.Text = "<span class='label label-danger'>" + result.Item2 + "</span><br>";
                        }
                        else
                        {
                            lbResult.Text = "<span class='label label-success'>" + result.Item2 + "</span><br>";
                        }



                    }


                    connExcel.Close();
                    dt.Clear();

                }

                else if (ddlModules.SelectedValue == "Programme")
                {

                    connExcel.Close();
                    connExcel.Open();
                    cmdExcel.CommandText = "SELECT * From [Programme$] where ProgrammeCode <> ''";


                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);


                    connExcel.Close();

                    Tuple<bool, string>[] rs = new Tuple<bool, string>[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        rs[i] = dmm.insertProgramme(dt.Rows[i], 1);
                    }

                    string displayTxt = "";
                    foreach (Tuple<bool, string> result in rs)
                    {
                        if (result.Item1)
                        {
                            displayTxt += "<span class='label label-success'>" + result.Item2 + "</span><br>";



                        }
                        else
                        {
                            displayTxt += "<span class='label label-danger'>" + result.Item2 + "</span><br>";
                        }
                    }

                    lbResult.Text = displayTxt;



                }

                else if (ddlModules.SelectedValue == "Class")
                {
                    dt.Clear();
                    connExcel.Close();
                    connExcel.Open();
                    cmdExcel.CommandText = "SELECT * From [ClassDetails$] where ProgrammeCode <>''";



                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);
                    connExcel.Close();

                    Tuple<bool, string, int>[] rs = new Tuple<bool, string, int>[dt.Rows.Count];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        rs[i] = dmm.insertProgrammeBatch(dt.Rows[i], 1);

                        if (rs[i].Item1)
                        {
                            string classCode = dt.Rows[i]["ClassCode"].ToString();
                            string lessonType = dt.Rows[i]["LessonType"].ToString();
                            int programmeBatchId = rs[i].Item3;

                            connExcel.Close();
                            connExcel.Open();
                            DataTable dtModSessionDetails = new DataTable();
                            cmdExcel.CommandText = "SELECT * From [ModuleSessionDetails$] where ClassCode ='" + classCode + "' AND LessonType = '" + lessonType + "'";
                            oda.SelectCommand = cmdExcel;
                            oda.Fill(dtModSessionDetails);
                            connExcel.Close();

                            connExcel.Open();

                            cmdExcel.CommandText = "SELECT * From [SessionDetails$] where ClassCode ='" + classCode + "' AND LessonType = '" + lessonType + "'";
                            DataTable dtSessionDetails = new DataTable();
                            oda.SelectCommand = cmdExcel;
                            oda.Fill(dtSessionDetails);
                            connExcel.Close();

                            dmm.insertBatchModule(dtModSessionDetails, dtSessionDetails, userId, programmeBatchId);

                        }
                    }

                    string displayTxt = "";
                    foreach (Tuple<bool, string, int> result in rs)
                    {
                        if (result.Item1)
                        {
                            displayTxt += "<span class='label label-success'>" + result.Item2 + "</span><br>";
                        }
                        else
                        {
                            displayTxt += "<span class='label label-danger'>" + result.Item2 + "</span><br>";
                        }
                    }

                    lbResult.Text = displayTxt;


                }
                else if (ddlModules.SelectedValue == "Applicant")
                {
                    dt.Clear();
                    connExcel.Close();
                    connExcel.Open();
                    cmdExcel.CommandText = "SELECT * From [ApplicantDetails$] where ICNumber <>''";

                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);
                    connExcel.Close();
                    List<String> lsTraineeId = new List<String>();
                    List<String> rs = new List<String>();

                    foreach (DataRow row in dt.Rows)
                    {
                        Tuple<bool, string, string> insertApp = dmm.insertApplicant(row, userId);
                        if (insertApp.Item1)
                        {
                           lsTraineeId.Add(insertApp.Item2);
                        }
                        else
                        {
                            rs.Add(insertApp.Item2 + " " + insertApp.Item3);
                        }
                    }

                    string currDateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string lsApplicantFile = "ListOfApplicant" + currDateTime + ".txt";
                    using (StreamWriter sw = File.CreateText(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()) + lsApplicantFile))
                    {
                        foreach (string trainee in lsTraineeId)
                        {
                            sw.WriteLine(trainee);
                        }
                        sw.Close();
                    }
                   

                    string result = "ApplicantInsertedResult" + currDateTime + ".txt";
                    using (StreamWriter sw1 = File.CreateText(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()) + result))
                    {
                        foreach (string result1 in rs)
                        {
                            sw1.WriteLine(result1);
                        }
                        sw1.Close();
                    }

                    string dowloadPath = Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString());
                    string zipCreatePath = "Applicant" + currDateTime + ".zip";


                    if (!File.Exists(dowloadPath + result))
                        lbResult.Text = "No File Found For Download";
                    else
                    {


                        using (ZipArchive archive = ZipFile.Open(dowloadPath + zipCreatePath, ZipArchiveMode.Create))
                        {
                            List<string> files = new List<string>();
                            files.Add(result);
                            files.Add(lsApplicantFile);

                            foreach (string file in files)
                            {
                                string filePath = Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()) + file;

                                archive.CreateEntryFromFile(filePath, file);
                            }
                        }
                    }


                    //Transmit the file to client machine and start download
                    Response.ClearContent();
                    Response.Clear();
                    Response.ContentType = "text/plain";
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Disposition",
                                       "attachment; filename=" + zipCreatePath + ";");
                    Response.TransmitFile(dowloadPath + zipCreatePath);

                    //use the following instead of response.end to prevent thread abort exception error
                    HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                    HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                    HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

                    File.Delete(dowloadPath + zipCreatePath);
                    File.Delete(dowloadPath + result);
                    File.Delete(dowloadPath + lsApplicantFile);


                    //Response.ClearContent();
                    //Response.Clear();
                    //Response.ContentType = "text/plain";
                    //Response.ClearHeaders();
                    //Response.AddHeader("Content-Disposition",
                    //                   "attachment; filename=" + lsApplicantFile + ";");
                    //Response.TransmitFile(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()) + lsApplicantFile);
                    ////use the following instead of response.end to prevent thread abort exception error
                    //HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                    //HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                    //HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.


                }
                else if (ddlModules.SelectedValue == "Subsidy")
                {
                    connExcel.Close();
                    connExcel.Open();
                    cmdExcel.CommandText = "SELECT * From [Subsidy$] where ProgrammeCode <>''";

                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);
                    connExcel.Close();

                    foreach (DataRow row in dt.Rows)
                    {
                        Tuple<bool, string> result = dmm.insertSubsidy(row, userId);

                        if (result.Item1)
                            lbResult.Text += "<span class='label label-success'>" + result.Item2 + "</span><br>";
                        else
                            lbResult.Text += "<span class='label label-danger'>" + result.Item2 + "</span><br>";
                    }
                }
                else if (ddlModules.SelectedValue == "Payment")
                {
                    connExcel.Close();
                    connExcel.Open();
                    cmdExcel.CommandText = "SELECT * From [Payment$] where Applicant <>''";

                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);
                    connExcel.Close();

                    foreach (DataRow r in dt.Rows)
                    {
                        Tuple<bool, string> result = dmm.insertPayment(r, userId);

                        if (result.Item1)
                        {
                            lbResult.Text += "<span class='label label-success'>" + result.Item2 + "</span><br>";
                        }
                        else
                        {
                            lbResult.Text += "<span class='label label-danger'>" + result.Item2 + "</span><br>";
                        }
                    }
                }
                else if (ddlModules.SelectedValue == "SOA")
                {
                    connExcel.Close();
                    connExcel.Open();
                    cmdExcel.CommandText = "SELECT * From [SOA$] where TraineeId <>''";

                    oda.SelectCommand = cmdExcel;

                    oda.Fill(dt);
                    connExcel.Close();

                    foreach (DataRow row in dt.Rows)
                    {
                        Tuple<bool, string> result = dmm.processSOA(row, userId);
                        if (result.Item1)
                        {
                            lbResult.Text += "<span class='label label-success'>" + result.Item2 + "</span><br>";
                        }
                        else
                        {
                            lbResult.Text += "<span class='label label-danger'>" + result.Item2 + "</span><br>";
                        }
                    }
                }
            }

        }

        protected void btnEnrollTrainee_Click(object sender, EventArgs e)
        {

            string path = System.IO.Path.GetFullPath(fuExcelFile.FileName);
            int userId = LoginID;

            if (fuExcelFile.HasFile)
            {
                string excelPath = Server.MapPath("~/Files/") + Path.GetFileName(fuExcelFile.PostedFile.FileName);
                fuExcelFile.SaveAs(excelPath);

                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelPath + ";Extended Properties=Excel 12.0";

                OleDbConnection connExcel = new OleDbConnection(connectionString);

                OleDbCommand cmdExcel = new OleDbCommand();

                OleDbDataAdapter oda = new OleDbDataAdapter();

                DataTable dt = new DataTable();

                cmdExcel.Connection = connExcel;

                connExcel.Open();
                cmdExcel.CommandText = "SELECT * From [ApplicantToBeEnrolled$] where Applicant <> ''";

                oda.SelectCommand = cmdExcel;

                oda.Fill(dt);
                connExcel.Close();

                //foreach(DataRow row in dt.Rows){
                //    Tuple<bool, string> result = dmm.enrollTrainee(row, 1);
                //    lbResult.Text += result.Item1 == true ? "<span class='label label-success'>" + result.Item2 + "</span><br>" : "<span class='label label-danger'>" + result.Item1 + "</span><br>";
                //}

                string lsTraineeFile = "ListOfTrainee" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                List<String> lsTraineeId = new List<String>();

                using (StreamWriter sw = File.CreateText(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()) + lsTraineeFile))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Tuple<bool, string> result = dmm.enrollTrainee(row, userId);
                        lbResult.Text += result.Item1 == true ? "<span class='label label-success'>" + result.Item2 + "</span><br>" : "<span class='label label-danger'>" + result.Item1 + "</span><br>";
                        if (result.Item1)
                        {
                            sw.WriteLine(result.Item2);
                            lsTraineeId.Add(result.Item2);
                        }
                        else
                        {
                            sw.WriteLine(result.Item2);
                        }
                    }
                }


                Tuple<bool, string> generatedFile = dmm.generateSOAExcel(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), lsTraineeId, userId);


                string filename = HttpUtility.UrlDecode(generatedFile.Item2);
                string dowloadPath = Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString());
                string zipCreatePath = "Trainee_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";


                if (!File.Exists(dowloadPath + filename))
                    lbResult.Text = "No File Found For Download";
                else
                {


                    using (ZipArchive archive = ZipFile.Open(dowloadPath + zipCreatePath, ZipArchiveMode.Create))
                    {
                        List<string> files = new List<string>();
                        files.Add(lsTraineeFile);
                        files.Add(filename);

                        foreach (string file in files)
                        {
                            string filePath = Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()) + file;

                            archive.CreateEntryFromFile(filePath, file);
                        }
                    }
                }


                //Transmit the file to client machine and start download
                Response.ClearContent();
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition",
                                   "attachment; filename=" + zipCreatePath + ";");
                Response.TransmitFile(dowloadPath + zipCreatePath);

                //use the following instead of response.end to prevent thread abort exception error
                HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

                File.Delete(dowloadPath + zipCreatePath);
                File.Delete(dowloadPath + filename);
                File.Delete(dowloadPath + lsTraineeFile);



            }
        }

    }
}