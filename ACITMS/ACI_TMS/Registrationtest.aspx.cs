using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Text;
using Microsoft.VisualBasic;
using GeneralLayer;
using LogicLayer;
using System.Globalization;




public partial class Registrationtest : System.Web.UI.Page
{
    Registrationtest1 test = new Registrationtest1();
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Page.IsPostBack == false)
        {
            loaddata();
        }

    }
    private void loaddata()
    {

        List<Registrationtest1> create = new List<Registrationtest1>();
        create = test.getRegistration();
        GridView1.DataSource = create;
        GridView1.DataBind();

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        DataTable ds = 
        //if (!Page.IsValid) return;

        //Tuple<bool, string> status = (new Applicant_Management()).registerApplicantQuick(tbFullName.Text, DateTime.ParseExact(tbBirthDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
        //    (ddlIdType.SelectedValue == ((int)IDType.Oth).ToString()) ? tbPPIdentification.Text : ddlIdFirstLetter.SelectedValue + tbLocalIdentification.Text + ddlIdLastLetter.Text,
        //    (IDType)int.Parse(ddlIdType.SelectedValue), (Sponsorship)Enum.Parse(typeof(Sponsorship), ddlSponsorship.SelectedValue), int.Parse(ddlProgramme.SelectedValue), int.Parse(ddlStartDate.SelectedValue),
        //    LoginID);

        //if (status.Item1)
        //{
        //    lblSucc.Text = status.Item2;
        //}
        //else
        //{
        //    lblError.Text = status.Item2;
        //}

    }

     //    public void ValidateCsv(string fileContents)
     //{
     //    var fileLines = fileContents.Split(
     //          new string[] { "\r\n", "\n" }, StringSplitOptions.None);

     //     if (fileLines.Count < 1)
     //        //fail - no data row.

     //     ValidateRows(fileLines.Skip(1));
     //}


     //public bool ValidateRows(IEnumerable<string> rows)
     //{
     //     foreach(row in rows)
     //     {
     //         var cells = row.Split(',');

     //          //ensure gender is correct
     //          if (cells[3] != "M" && cells[3] != "F")
     //              return false;

     //          //perform any additional row checks relevant to your domain
     //     }
     //}
    protected void Button2_Click(object sender, EventArgs e)
    {
        string csvPath = Server.MapPath("~/UploadedCSVFiles/") + Path.GetFileName(FileUpload2.PostedFile.FileName);
        FileUpload2.SaveAs(csvPath);

        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[8] 
        { 
                new DataColumn("fullName"),
                new DataColumn("birthDate"),
                new DataColumn("idType"),
                new DataColumn("idNumber") ,
                new DataColumn("Programme_Category") ,
                new DataColumn("Available_Programme") ,
                new DataColumn("Class_Start_Date") ,
                new DataColumn("selfSponsored") 
    });

        //Read the contents of CSV file.
        string csvData = File.ReadAllText(csvPath);
        bool skip = true;
        foreach (string row in csvData.Split('\n'))
        {
            if (!string.IsNullOrEmpty(row))
            {
                if (skip == true)
                {
                    skip = false;
                }
                else
                {
                    dt.Rows.Add();
                    int i = 0;

                    //Execute a loop over the columns.
                    foreach (string cell in row.Split(','))
                    {
                        dt.Rows[dt.Rows.Count - 1][i] = cell;
                        i++;
                    }
                }
            }

            //List<DataRow> listOfBadRows = new List<DataRow>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (!int.TryParse(dr["id"].ToString()))
            //        Response.Write("Wrong input for id");
                    
            //}

            //if (listOfBadRows.Count > 0)
            //{
            //    /*Few bad rows are there; need to do something*/
            //}
            //else
            //{
            //    /*Viola no bad rows...here start processing*/
            //}


        }
        GridView1.DataSource = dt;
        GridView1.DataBind();

        //string conn = ConfigurationManager.ConnectionStrings["tmsdbConnection"].ConnectionString;
        //using (SqlConnection dbConnection = new SqlConnection(conn))
        //{
        //    using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
        //    {
        //        s.DestinationTableName = "batchreg";
        //        dbConnection.Open();
        //        s.WriteToServer(dt);
        //        dbConnection.Close();

        //    }
        //}
    }
}        
    

