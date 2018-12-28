using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Drawing;
using GeneralLayer;


namespace ACI_TMS
{
    public partial class data_analytics : BasePage
    {


        public const string PAGE_NAME = "data-analytics.aspx";

        public data_analytics()
            : base(PAGE_NAME, AccessRight_Constance.DATA_ANALYTICS)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            btnGetData.Visible = false;
            DivInfoDes.Visible = false;
            downloadPanel.Visible = false;


            if (DdlDept.SelectedIndex > 0)
            {
                DivInfoDes.Visible = true;
            }

        }
        protected void DdlDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlDept.SelectedIndex == 0)
            {
                tablePanel.Visible = false;
                validation.Text = "";
                Label3.Visible = false;
                Label4.Visible = false;
                startYear.Visible = false;
                endYear.Visible = false;
            }
            if (DdlDept.SelectedIndex > 0)
            {
                btnGetData.Visible = true;
                DivInfoDes.Visible = true;
                tablePanel.Visible = true;
                validation.Text = "";
                Label3.Visible = true;
                Label4.Visible = true;
                startYear.Visible = true;
                endYear.Visible = true;
                Data_Analytics_Management dm = new Data_Analytics_Management();


                lblDepartment.Text = dm.getDepartDes(DdlDept.SelectedItem.Text);
                DataTable dt = dm.getGroupTables(DdlDept.SelectedItem.Text);
                dt.Columns.Add("DataContent");
                dt.Columns.Add("DataContent2");


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tName = dt.Rows[i]["table"].ToString();
                    // get table description
                    string desp = dm.getTableDescription(tName);
                    dt.Rows[i]["DataContent"] = desp;
                }


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tName = dt.Rows[i]["table2"].ToString();
                    // get table description
                    if (tName != "")
                    {
                        string desp = dm.getTableDescription(tName);
                        dt.Rows[i]["DataContent2"] = desp;
                    }
                }
                string a = dt.Rows[0]["table2"].ToString();
                if (a == "")
                {
                    gvTable.Columns[1].Visible = false;

                }
                else
                {
                    gvTable.Columns[1].Visible = true;
                }

                gvTable.DataSource = dt;
                gvTable.DataBind();

            }
            else
            {
                DivInfoDes.Visible = false;
                btnGetData.Visible = false;

            }
        }

        protected void btnGetData_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(endYear.SelectedValue) <= Convert.ToInt32(startYear.SelectedValue)) {
                validation.ForeColor = Color.Red;
                validation.Text = "Start year cannot be later than End Year";
            } else{
                validation.Text = "";
                downloadPanel.Visible = true;
            }
            
        }

        //start of excel exporting
        protected void lbDownloadData_Click(object sender, EventArgs e)
        {
            Data_Analytics_Management dm = new Data_Analytics_Management();
            DataTable dt = dm.getGroupTables(DdlDept.SelectedItem.Text);//GET ALL THE DATA
            XLWorkbook wb = new XLWorkbook();
            string startDate="01/01/"+ startYear.SelectedValue;
            string endDate = "30/03/" + endYear.SelectedValue;  
            DateTime end_date = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string strOut = end_date.ToString("MM/dd/yyyy");


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // loop through all table's name
                string tableName = dt.Rows[i]["table"].ToString().Replace(" ", "_"); //GET CURRENT TABLE NAME
                DataTable dtData = dm.getDataByTableName(tableName); //GET DATA OF CURRENT TABLE

                //test
                if (dtData != null)
                {
                    DataColumnCollection columns = dtData.Columns;
                    if (columns.Contains("createdOn"))
                    {
                        //var filter = dtData.Select("highestEducation <= 92"); //highestEducation>90 doesnt work BUT highestEducation<90 works (lesser than 90 deleted)
                        //var firstfilter = dtData.Select("gender = 'M'"); //works
                        //var filter = dtData.Select("fullName = 'Antoine Castro'"); //works

                        var firstfilter = dtData.Select("createdOn < #" + startDate + "#"); //works
                        var secondfilter = dtData.Select("createdOn > #" + strOut + "#"); //works
                        foreach (var row in firstfilter)
                            row.Delete();
                        foreach (var row in secondfilter)
                            row.Delete();
                    }
                    dtData.AcceptChanges();
                }
                //test

                var ws = wb.Worksheets.Add(dtData, tableName); //ADD CURRENT TABLE DATA AND NAME TO WORKSHEET
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tableName = dt.Rows[i]["table2"].ToString().Replace(" ", "_");
                if (tableName != "")
                {
                    DataTable dtData = dm.getDataByTableName(tableName);
                    var ws = wb.Worksheets.Add(dtData, tableName);
                }
            }


            //Export the Excel file.
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=ACIData.xlsx");
            using (MemoryStream MyMemoryStream = new MemoryStream())
            {
                wb.SaveAs(MyMemoryStream);
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        //end of excel exporting





    }
}