using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
// if type of namespace does not exist in namespace, https://stackoverflow.com/questions/12830017/microsoft-visualbasic-fileio-does-not-exist
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;

namespace DataLayer
{
    public class DB_DataAnalytics
    {


        private SqlConnection sqlConnection = new SqlConnection();

        //Initialize connection string
        Database_Connection dbConnection = new Database_Connection();
        string conString = ConfigurationManager.ConnectionStrings["TMSDBConnection"].ConnectionString;

        // Add csv data into dataTable
        public DataTable GetDataTableFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return csvData;
        }

        // mapping of columns between csv data and tableName in database
        public void copyTable(DataTable csvData, string tableName)
        {



            //      SqlConnection con = dbConnection.getDBConnection();


            string conString = ConfigurationManager.ConnectionStrings["TMSDBConnection"].ConnectionString;
            using (SqlBulkCopy s = new SqlBulkCopy(conString))
            {

                //    sqlConnection.Open();
                dbConnection.getDBConnection().Open();
                s.DestinationTableName = tableName;

                foreach (var column in csvData.Columns)
                {



                    s.ColumnMappings.Add(column.ToString(), column.ToString());
                }
                s.WriteToServer(csvData);


            }
        }

        // get all the tables names in the database(exclude the sysdiagrams table)
        public DataTable getAllDatabaseTableNamedt()
        {

            try
            {
                string queryV2 = "SELECT name FROM sys.tables t WHERE t.is_ms_shipped = 0 and t.name <> 'sysdiagrams' and t.name<> 'aci_access_rights_log'and t.name<> 'aci_functions_log'and t.name<> 'aci_role_log'and t.name<> 'aci_suspended_list_log'and t.name<> 'aci_user_log'and t.name<> 'applicant_log'and t.name<> 'applicant_interview_result_log'and t.name<>'aci_user_role_log' and t.name<> 'batch_module_log'and t.name <>'batch_module_session_log' and t.name<> 'batchModule_session_log'and t.name<> 'bundle_log'and t.name<> 'case_logging_log'and t.name<> 'code_reference_log'and t.name<> 'employment_history_log'and t.name<> 'module_structure_log'and t.name<> 'payment_history_log'and t.name<> 'notification_log'and t.name<> 'payment_history_log'and t.name<> 'programme_batch_log'and t.name<> 'programme_structure_log'and t.name<> 'short_course_subsidy_log'and t.name<> 'trainee_log'and t.name<> 'trainee_absence_record_log'and t.name<> 'trainee_absence_removed_log'and t.name<> 'trainee_module_log'and t.name<> 'trainee_programme_log' and t.name<> 'venue_log' and t.name<> 'venue_booking_record_log' and t.name<> 'notification_log'and t.name<>'programme_subsidy_log' and t.name<>'programme_subsidy_scheme_log' and t.name<>'bulk_insert' and t.name<>'programme_subsidy_value_log' and t.name<>'registration_fee_log' and t.name<>'case_logging_category_log' order by name ";

                //string queryV1 = "SELECT name FROM sys.tables t WHERE t.is_ms_shipped = 0 and t.name <> 'sysdiagrams' order by name";
                SqlDataAdapter da = new SqlDataAdapter(queryV2, conString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {
                return null;
            }



        }

        // get specific column name in database(exclude the sysdiagrams table), use for dropdown list filter
        public DataTable getSpecificTableColumns(string columnName)
        {

            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT distinct t.name FROM sys.columns c JOIN sys.tables t ON c.object_id = t.object_id WHERE c.name LIKE '%'+@columnName+'%' and t.name <> 'sysdiagrams' and t.name<> 'aci_access_rights_log'and t.name<> 'aci_functions_log'and t.name<> 'aci_role_log'and t.name<> 'aci_suspended_list_log'and t.name<> 'aci_user_log'and t.name<> 'applicant_log'and t.name<> 'applicant_interview_result_log'and t.name<>'aci_user_role_log' and t.name<> 'batch_module_log'and t.name <>'batch_module_session_log' and t.name<> 'batchModule_session_log'and t.name<> 'bundle_log'and t.name<> 'case_logging_log'and t.name<> 'code_reference_log'and t.name<> 'employment_history_log'and t.name<> 'module_structure_log'and t.name<> 'payment_history_log'and t.name<> 'notification_log'and t.name<> 'payment_history_log'and t.name<> 'programme_batch_log'and t.name<> 'programme_structure_log'and t.name<> 'short_course_subsidy_log'and t.name<> 'trainee_log'and t.name<> 'trainee_absence_record_log'and t.name<> 'trainee_absence_removed_log'and t.name<> 'trainee_module_log'and t.name<> 'trainee_programme_log' and t.name<> 'venue_log' and t.name<> 'venue_booking_record_log' and t.name<> 'notification_log'and t.name<>'programme_subsidy_log' and t.name<>'programme_subsidy_log' and t.name<>'programme_subsidy_scheme_log' and t.name<>'aci_access_rights_log_view' and t.name<>'bulk_insert' and t.name<>'programme_subsidy_value_log' and t.name<>'registration_fee_log' and t.name<>'case_logging_category_log' ORDER BY t.name", conString);
                da.SelectCommand.Parameters.AddWithValue("@columnName", columnName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {
                return null;
            }


        }



        // get the columns of different table
        public DataTable getColumnByTableName(string tableName)
        {

            try
            {
                SqlDataAdapter da = new SqlDataAdapter("EXECUTE('SELECT TOP 0 * FROM  ' + @TableName + '')", conString);

                da.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }



        }
        public DataTable getDataByTableName(string tableName)
        {

            try
            {
                SqlDataAdapter da = new SqlDataAdapter("EXECUTE('SELECT * FROM  ' + @TableName + '')", conString);

                da.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }



        }

        public DataTable getCurrentPkRowsInDbByTableName(string tableName, List<string> listofPks)
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < listofPks.Count; k++)
                {
                    if (k != listofPks.Count - 1)
                    {
                        sb.Append(listofPks[k] + ",");
                    }
                    else
                    {
                        sb.Append(listofPks[k]);
                    }

                }
                string list = sb.ToString();
                SqlDataAdapter da = new SqlDataAdapter("EXECUTE('SELECT " + list + " FROM  ' + @TableName + '')", conString);

                da.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }



        }
        public DataTable getCurrentDbRecord(string tableName)
        {

            try
            {

                SqlDataAdapter da = new SqlDataAdapter("EXECUTE('SELECT * FROM  ' + @TableName + '')", conString);

                da.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }



        }

        // get the columns of different table
        public DataTable getColumnBySheet(string sheetName)
        {
            try
            {
                string statement = "EXECUTE('SELECT * FROM  ' + @SheetName + '')";
                SqlDataAdapter da = new SqlDataAdapter(statement, conString);

                da.SelectCommand.Parameters.AddWithValue("@SheetName", sheetName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {
                return null;
            }

        }


        // get related table names by primary/composite keys
        public DataTable getRelatedTablebyKeys(DataTable dtKeys)
        {
            try
            {
                try
                {

                    string a = dtKeys.Rows[0][0].ToString();
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                // select the first key
                string statement = "SELECT name FROM sysobjects o WHERE id IN ( SELECT id FROM syscolumns WHERE name = @key0 and o.name <> 'sysdiagrams'  and o.name <> 'sysdiagrams' and o.name<> 'aci_access_rights_log'and o.name<> 'aci_functions_log'and o.name<> 'aci_role_log'and o.name<> 'aci_suspended_list_log'and o.name<> 'aci_user_log'and o.name<> 'applicant_log'and o.name<> 'applicant_interview_result_log'and o.name<>'aci_user_role_log' and o.name<> 'batch_module_log'and o.name<> 'batch_module_session_log'and o.name<> 'bundle_log'and o.name<> 'case_logging_log'and o.name<> 'code_reference_log'and o.name<> 'employment_history_log'and o.name<> 'module_structure_log'and o.name<> 'payment_history_log'and o.name<> 'notification_log'and o.name<> 'payment_history_log'and o.name<> 'programme_batch_log'and o.name<> 'programme_structure_log'and o.name<> 'short_course_subsidy_log'and o.name<> 'trainee_log'and o.name<> 'trainee_absence_record_log'and o.name<> 'trainee_absence_removed_log'and o.name<> 'trainee_module_log'and o.name<> 'trainee_programme_log' and o.name<> 'venue_log' and o.name<> 'venue_booking_record_log' and o.name<> 'notification_log'and o.name<>'programme_subsidy_log' and o.name<>'programme_subsidy_scheme_log' and o.name<>'aci_access_rights_log_view' and o.name<>'bulk_insert' and o.name<>'programme_subsidy_value_log' and o.name<>'registration_fee_log' and o.name<>'case_logging_category_log')";

                string statement2 = "SELECT name FROM sysobjects o WHERE id IN ( SELECT id FROM syscolumns WHERE name = @key0 )";
                //       SqlDataAdapter da = new SqlDataAdapter(statement, conString);
                SqlDataAdapter da2 = new SqlDataAdapter(statement2, conString);
                SqlDataAdapter da = new SqlDataAdapter(statement, conString);
                // if there a composite keys
                if (dtKeys.Rows.Count > 1)
                {
                    int lastId = dtKeys.Rows.Count - 1;
                    for (int i = 1; i < dtKeys.Rows.Count; i++)
                    {
                        if (i != lastId)
                        {
                            // loop and add new statments 
                            string b = dtKeys.Rows[i][0].ToString();
                            statement2 += " and id in ( SELECT id FROM syscolumns WHERE name = @key" + i + " )";
                            da = new SqlDataAdapter(statement, conString);
                        }
                        //da.SelectCommand.Parameters.AddWithValue("@key" + i, dtKeys.Rows[i][0].ToString());

                        if (i == lastId)
                        {
                            statement2 += " and id in ( SELECT id FROM syscolumns WHERE name = @key" + i + " and o.name <> 'sysdiagrams' and o.name<> 'aci_access_rights_log'and o.name<> 'aci_functions_log'and o.name<> 'aci_role_log'and o.name<> 'aci_suspended_list_log'and o.name<> 'aci_user_log'and o.name<> 'applicant_log'and o.name<> 'applicant_interview_result_log'and o.name<>'aci_user_role_log' and o.name<> 'batch_module_log'and o.name<> 'batch_module_session_log'and o.name<> 'bundle_log'and o.name<> 'case_logging_log'and o.name<> 'code_reference_log'and o.name<> 'employment_history_log'and o.name<> 'module_structure_log'and o.name<> 'payment_history_log'and o.name<> 'notification_log'and o.name<> 'payment_history_log'and o.name<> 'programme_batch_log'and o.name<> 'programme_structure_log'and o.name<> 'short_course_subsidy_log'and o.name<> 'trainee_log'and o.name<> 'trainee_absence_record_log'and o.name<> 'trainee_absence_removed_log'and o.name<> 'trainee_module_log'and o.name<> 'trainee_programme_log' and o.name<> 'venue_log' and o.name<> 'venue_booking_record_log' and o.name<> 'notification_log'and o.name<>'programme_subsidy_log' and o.name<>'programme_subsidy_scheme_log' and o.name<>'aci_access_rights_log_view' and o.name<>'bulk_insert' )";
                            da2 = new SqlDataAdapter(statement2, conString);
                        }
                    }

                    for (int i = 0; i < dtKeys.Rows.Count; i++)
                    {
                        // add the keys para in to the statement
                        da2.SelectCommand.Parameters.AddWithValue("@key" + i, dtKeys.Rows[i][0].ToString());
                    }
                }

                else
                {



                    for (int i = 0; i < dtKeys.Rows.Count; i++)
                    {
                        // add the keys para in to the statement
                        da.SelectCommand.Parameters.AddWithValue("@key" + i, dtKeys.Rows[i][0].ToString());
                    }



                }

                DataTable dt = new DataTable();
                if (dtKeys.Rows.Count > 1)
                {

                    da2.Fill(dt);

                }
                else
                {
                    da.Fill(dt);

                }



                return dt;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public DataTable getPKbyTable(string tableName)
        {
            try
            {
                string query = "SELECT ColumnName = col.column_name FROM information_schema.table_constraints tc INNER JOIN information_schema.key_column_usage col ON col.Constraint_Name = tc.Constraint_Name AND col.Constraint_schema = tc.Constraint_schema WHERE tc.Constraint_Type = 'Primary Key' AND col.Table_name = @tableName";
                SqlDataAdapter da = new SqlDataAdapter(query, conString);

                da.SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }


        //public DataTable getDataTypeByTable(string tableName)
        //{
        //    try
        //    {
        //        SqlDataAdapter da = new SqlDataAdapter("SELECT DATA_TYPE, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName", conString);

        //        da.SelectCommand.Parameters.AddWithValue("@tableName", tableName);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        return dt;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}



        public void insertLastModifiedDate(DataTable dtTable)
        {
            dtTable.Columns.Add(new DataColumn("lastModifiedDate", typeof(System.DateTime)));
            foreach (DataRow dr in dtTable.Rows)
            {
                string a = DateTime.Now.ToString();
                dr["lastModifiedDate"] = DateTime.Now.ToString();


            }

        }

        // fill the datatable with excel data
        public DataTable FillDataTableWithExcelData(string sheet, string extension, string excelPath)
        {
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07 or higher
                    conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                    break;

            }


            conString = string.Format(conString, excelPath);

            DataTable dt = new DataTable();
            string query = "SELECT * FROM [" + sheet + "]";
            OleDbConnection excel_con = new OleDbConnection(conString);
            using (OleDbDataAdapter oda = new OleDbDataAdapter(query, excel_con))
            {
                try
                {
                    oda.Fill(dt);
                    return dt;

                }
                catch (Exception ex)
                {

                    return null;
                }


            }

        }

        public DataTable FillSampleRow(string sheet, string extension, string excelPath)
        {
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07 or higher
                    conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                    break;

            }


            conString = string.Format(conString, excelPath);

            DataTable dt = new DataTable();
            string query = "SELECT top 2 * FROM [" + sheet + "]";
            OleDbConnection excel_con = new OleDbConnection(conString);
            using (OleDbDataAdapter oda = new OleDbDataAdapter(query, excel_con))
            {
                try
                {
                    oda.Fill(dt);
                    return dt;

                }
                catch (Exception ex)
                {

                    return null;
                }


            }
        }

        public DataTable FillDataTableWithExcelPKcols(string sheet, string extension, string excelPath, List<string> pkCols)
        {
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07 or higher
                    conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                    break;

            }


            conString = string.Format(conString, excelPath);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pkCols.Count; i++)
            {


                if (i != pkCols.Count - 1)
                {

                    sb = sb.Append(pkCols[i] + ",");

                }
                else
                {
                    sb.Append(pkCols[i] + " ");
                }
            }


            DataTable dt = new DataTable();
            string query = "SELECT " + sb + " FROM [" + sheet + "]";
            OleDbConnection excel_con = new OleDbConnection(conString);
            using (OleDbDataAdapter oda = new OleDbDataAdapter(query, excel_con))
            {
                try
                {
                    oda.Fill(dt);
                    return dt;

                }
                catch (Exception ex)
                {

                    return null;
                }


            }



        }

        // find the duplicate primary key rows(got bug: becasue it will read the sample rows also, and oledb don't support delete rows --> solved)
        public DataTable FindDuplicatePkRows(string sheet, string extension, string excelPath, List<string> PKList, DataTable pkRowDatas)
        {
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07 or higher
                    conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                    break;

            }


            conString = string.Format(conString, excelPath);

            DataTable dt = new DataTable();
            //string query = "SELECT courseCode, projectCode,traineeId,moduleCode,COUNT(*) as duplicates FROM [" + sheet + "] group by courseCode, projectCode,traineeId, moduleCode having count(*) > 1";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < PKList.Count; i++)
            {

                sb.Append(PKList[i] + ",");

            }
            if (PKList.Count > 0)
            {
                string sb2 = sb.ToString();
                string grpby = sb.ToString();
                int lastChar = grpby.Length - 1;
                // remove the last comma
                grpby = grpby.Substring(0, lastChar);

                string query = "SELECT " + sb2 + " COUNT(*) as duplicates FROM [" + sheet + "] group by " + grpby + " having count(*) > 1";
                OleDbConnection excel_con = new OleDbConnection(conString);
                using (OleDbDataAdapter oda = new OleDbDataAdapter(query, excel_con))
                {



                    try
                    {
                        oda.Fill(dt);


                        // since its using a count(*) function, it will count the rows of both. so if one row thats one duplicate --> count as 2. hence: -1 to get the duplicates 
                        for (int d = 0; d < dt.Rows.Count; d++)
                        {
                            string cell = dt.Rows[d]["duplicates"].ToString();
                            int dupNum = Convert.ToInt32(cell);
                            dupNum--;
                            cell = dupNum.ToString();
                            dt.Rows[d]["duplicates"] = cell;
                            dt.AcceptChanges();
                        }
                        // use of dt2 to copy the dt ---> remove the number of duplicate columns
                        DataTable dt2 = dt.Copy();

                        dt2.Columns.Remove("duplicates");
                        dt2.AcceptChanges();


                        // Note: rowNum starts from 0
                        // compare the two table and return duplicate sample row index
                        // if only when there is sample row present
                        if (pkRowDatas != null)
                        {
                            int rowNum = CompareRowsGetRowIndex(dt2, pkRowDatas);

                            // if there is equal value
                            if (rowNum > -1)
                            {
                                string rows = dt.Rows[rowNum]["duplicates"].ToString();
                                // if only the duplicates rows contains only 1 row then delete
                                if (dt.Rows[rowNum]["duplicates"].ToString().Equals("1"))
                                {
                                    // delete by using index
                                    dt.Rows[rowNum].Delete();
                                    dt.AcceptChanges();
                                }
                                else
                                {
                                    // else if its more than 1, it will count the sample row as duplicates: so reduce one number from the column
                                    string dupColCell = dt.Rows[rowNum]["duplicates"].ToString();
                                    int dupCell = Convert.ToInt32(dupColCell);
                                    dt.Rows[rowNum]["duplicates"] = dupCell - 1;
                                    dt.AcceptChanges();
                                }
                            }

                        }
                        // return the final correct dt(without sample row)
                        return dt;

                    }
                    catch (Exception ex)
                    {
                        return null;
                    }


                }
            }
            else
            {
                return dt;
            }

        }

        // compare the rows between two datatable -> get the duplicate index --> solved the problem of "having sample row being the duplicate rows"
        // returns only one number because only got one sample row
        public int CompareRowsGetRowIndex(DataTable table1, DataTable table2)
        {
            bool status = true;
            int rowNum = 0;
            foreach (DataRow row1 in table1.Rows)
            {
                int index = table1.Rows.IndexOf(row1);
                foreach (DataRow row2 in table2.Rows)
                {
                    var array1 = row1.ItemArray;
                    var array2 = row2.ItemArray;

                    if (array1.SequenceEqual(array2))
                    {

                        // if there is equal value, return the index;
                        status = false;
                        rowNum = index;
                    }

                }
            }
            // if there is no equal values return -1
            if (status == true)
            {
                rowNum = -1;
            }
            return rowNum;
        }

        // returns a list because there are two sample rows, hence need to check if is row 1 or row 2.
        public List<int> CompareRowsGetListOfRowIndex(DataTable table1, DataTable table2)
        {

            List<int> indexList = new List<int>();
            foreach (DataRow row1 in table1.Rows)
            {
                int index = table1.Rows.IndexOf(row1);
                foreach (DataRow row2 in table2.Rows)
                {
                    var array1 = row1.ItemArray;
                    var array2 = row2.ItemArray;

                    if (array1.SequenceEqual(array2))
                    {

                        // if there is equal value, add the row index into the list of int        
                        indexList.Add(index);
                    }

                }
            }

            return indexList;
        }



        public DataTable GetExcelSheetName(string extension, string excelPath)
        {
            string conString = string.Empty;

            DataTable dt = new DataTable();
            DataSet dsAllData = new DataSet();

            switch (extension)
            {
                case ".xls": //Excel 97-03
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07 or higher
                    conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                    break;

            }


            conString = string.Format(conString, excelPath);



            //2. READING FROM EXCEL AND CREATING DATATABLE 

            using (OleDbConnection excel_con = new OleDbConnection(conString))
            {
                excel_con.Open();
                // sheet count is column name in n rows
                int sheetCount = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows.Count;
                // get the sheets, if sheet is this the name of the table call and fill the specific datatable that pre-created
                string sheet1 = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();
                for (int i = 0; i < sheetCount; i++)
                {
                    // loop through the rows and get the different sheetName, format: tableName$
                    string sheets = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[i]["TABLE_NAME"].ToString();

                    string TrimSheetName = sheets.Trim('$');
                    // dt contains the sheetsName as the column names
                    dt.Columns.Add(TrimSheetName);


                }
                excel_con.Close();
                return dt;



            }

        }




        // Insert excel data
        public DataTable InsertExcelData(DataSet dsAllData, int userId)
        {


            // 3.INSERT INTO DATABASE
            string consString = ConfigurationManager.ConnectionStrings["TMSDBConnection"].ConnectionString;
            //         bool parentInsertStatus = false;
            SqlConnection con = new SqlConnection(consString);
            //   int result = 0;

            int NumOftable = dsAllData.Tables.Count;    // number of tables in the dataset
            // number of inserted record for each table
            DataTable dtRecord = new DataTable();
            DataColumn dc0 = new DataColumn("Table Name", typeof(string));
            dtRecord.Columns.Add(dc0);
            DataColumn dc1 = new DataColumn("Record", typeof(int));
            dtRecord.Columns.Add(dc1);
            DataRow drow = dtRecord.NewRow();

            // loop through the tables in insert
            for (int i = 0; i < NumOftable; i++)
            {
                //string dataSetTableName = dsAllData.Tables[i].TableName.ToString();
                // delete first row sample data
                //dsAllData.Tables[i].Rows[0].Delete();

                //dsAllData.AcceptChanges();

                int row = 0;
                SqlConnection connection = null;
                SqlBulkCopy bulkCopy = null;
                connection = new SqlConnection(consString);

                DataTable dataTable1 = new DataTable();

                dataTable1 = dsAllData.Tables[i];
                // load  data into the DataTable
                IDataReader reader = dataTable1.CreateDataReader();

                try
                {
                    if (dataTable1.Rows.Count > 0)
                    {

                        connection.Open();
                        bulkCopy = new SqlBulkCopy(consString, SqlBulkCopyOptions.FireTriggers);
                        //bulkCopy = new SqlBulkCopy(connection);
                        bulkCopy.DestinationTableName = dsAllData.Tables[i].TableName.ToString();
                        // column mapping, cause order of column changes(lastModifiedDate)
                        foreach (DataColumn dc in dsAllData.Tables[i].Columns)
                        {
                            bulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                        }



                        bulkCopy.WriteToServer(reader);
                        row = SqlBulkCopyExtension.RowsCopiedCount(bulkCopy);
                        string tableName = dsAllData.Tables[i].TableName;
                        bool status = insertBulkRecord(tableName, userId);


                    }


                    drow = dtRecord.NewRow();
                    drow["Table Name"] = dsAllData.Tables[i].TableName.ToString();
                    drow["Record"] = row;
                    dtRecord.Rows.Add(drow);



                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        string a = ex.ToString();
                        return dtRecord;
                    }
                    if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                    {
                        string pattern = @"\d+";
                        Match match = Regex.Match(ex.Message.ToString(), pattern);
                        var index = Convert.ToInt32(match.Value) - 1;

                        FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                        var sortedColumns = fi.GetValue(bulkCopy);
                        var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                        FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                        var metadata = itemdata.GetValue(items[index]);

                        var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                        var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                        throw new FormatException(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));

                    }

                    throw;
                }
                finally
                {

                    if (connection != null && con.State == ConnectionState.Open)
                    {

                        connection.Close();
                    }

                }

            }

            return dtRecord;

        }

        // Validate different datatype ---> change to try parse
        //public bool ValidateDataType(DataTable tempdt, string CheckDatecolumnName, string dataType)
        //{
        //    bool validationStatus = true;

        //    // this is to check the datatype of the columns in temp table
        //    string datatype = tempdt.Columns[CheckDatecolumnName].DataType.ToString();
        //    // this is the datatype that I want, as input parameter 
        //    string systemType = System.Type.GetType(dataType).ToString();

        //    if (datatype == systemType || datatype == "System.Double")
        //    {
        //        validationStatus = true;

        //        //  msg3 = "receivedSOADate column contains invalid date <br/>";
        //        //       msg.Append("receivedSOADate, ");
        //        //    list.Add("receivedSOADate");

        //    }
        //    else
        //    {
        //        validationStatus = false;

        //    }

        //    return validationStatus;

        //}

        // get a list of a "xx" type columns from the tempdatatable
        public List<string> getListofCertainDataTypeColumns(DataTable TempDataTable, string dataType)
        {
            List<string> ListofType = new List<string>();
            try
            {
                for (int k = 0; k < TempDataTable.Columns.Count; k++)
                {
                    string colName = TempDataTable.Columns[k].ColumnName;
                    string datatype = TempDataTable.Columns[colName].DataType.ToString();
                    string systemType = System.Type.GetType(dataType).ToString();


                    if (datatype == systemType)
                    {
                        ListofType.Add(colName);

                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
            return ListofType;
        }

        public StringBuilder GetErrorListMsg(List<string> ErrorList)
        {
            string item = "";

            StringBuilder sb2 = new StringBuilder();
            if (ErrorList.Count > 0)
            {

                for (int n = 0; n < ErrorList.Count; n++)
                {
                    item = ErrorList[n];
                    // if is the last item
                    if (n == ErrorList.Count - 1)
                    {
                        item = ErrorList[n] + " ";
                        //      item = item.Replace(",", " ");
                        sb2.Append(item);

                    }
                    else
                    {
                        item = ErrorList[n] + ", ";
                        sb2.Append(item);
                    }

                }
            }

            return sb2;

        }

        // get the nonNullable column in the pre-created table
        public List<string> getNonNullableColumn(DataTable PreDataTable)
        {
            List<string> ListofType = new List<string>();
            try
            {
                int k = 0;
                foreach (DataColumn dc in PreDataTable.Columns)
                {
                    if (PreDataTable.Columns.Count > k)
                    {


                        if (dc.AllowDBNull == false)
                        {
                            string a = dc.ColumnName;
                            ListofType.Add(a);
                        }

                        k++;
                    }

                }


            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
            return ListofType;
        }

        // get the nullable column in the pre-created table
        public List<string> getNullableColumn(DataTable PreDataTable)
        {
            List<string> ListofType = new List<string>();
            try
            {
                int k = 0;
                foreach (DataColumn dc in PreDataTable.Columns)
                {
                    if (PreDataTable.Columns.Count > k)
                    {


                        if (dc.AllowDBNull == true)
                        {
                            string a = dc.ColumnName;
                            ListofType.Add(a);
                        }

                        k++;
                    }

                }


            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
            return ListofType;
        }

        // get columns that contains null value in a table
        public StringBuilder validateNull(DataTable dtTable, List<string> listOfnonNullable, string tableName)
        {
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.Append("");
            bool result = true;
            string a = "";
            List<string> list = new List<string>();

            for (int k = 0; k < listOfnonNullable.Count; k++)
            {
                result = true;
                string colName = listOfnonNullable[k];
                foreach (DataRow row in dtTable.Rows)
                {

                    object value = row[colName];
                    if (value == DBNull.Value)
                    {
                        result = false;
                        string msg = colName + sbMsg;
                        list.Add(colName);
                    }

                }
                if (result == false)
                {
                    sbMsg.Append(colName + ",");
                    a = sbMsg.ToString();
                    int lastChar = a.Length - 1;
                    // remove the last comma
                    a = a.Substring(0, lastChar);
                    result = true;

                    //  sbMsg = sbMsg.Substring(0, sbMsg.length() - 1);

                }
            }
            if (list.Count > 0)
            {

                sbMsg = new StringBuilder();
                a = a + " column(s) in the " + tableName + " sheet should not contain(s) null/empty value";
                sbMsg.Append(a);

            }
            else
            {
                sbMsg.Append("");
            }
            return sbMsg;
        }


        //// preCreatedTable becomes in one method ---->GetPreDefineTable
        //public DataTable getPreTraineeModuleTable()
        //{
        //    DataTable dtTraineeModule = new DataTable();

        //    try
        //    {


        //        //        new DataColumn("lastModifiedDate",typeof(DateTime))

        //        DataColumn dc0 = new DataColumn("courseCode", typeof(string));
        //        //         dc0.MaxLength = 10;
        //        dc0.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc0);

        //        DataColumn dc1 = new DataColumn("projectCode", typeof(string));
        //        //        dc1.MaxLength = 10;
        //        dc1.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc1);

        //        DataColumn dc2 = new DataColumn("moduleCode", typeof(string));
        //        //           dc2.MaxLength = 10;
        //        dc2.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc2);

        //        DataColumn dc3 = new DataColumn("classCode", typeof(string));
        //        //          dc3.MaxLength = 10;
        //        dc3.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc3);

        //        DataColumn dc4 = new DataColumn("traineeId", typeof(string));
        //        //          dc4.MaxLength = 10;
        //        dc4.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc4);
        //        DataColumn dc5 = new DataColumn("moduleResult", typeof(string));
        //        //         dc5.MaxLength = 10;
        //        dc5.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc5);

        //        DataColumn dc6 = new DataColumn("sitInModule", typeof(string));
        //        //         dc6.MaxLength = 10;
        //        dc6.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc6);

        //        DataColumn dc7 = new DataColumn("assessmentCompleted", typeof(string));
        //        //        dc7.MaxLength = 10;
        //        dc7.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc7);

        //        DataColumn dc8 = new DataColumn("makeUpAssessment", typeof(string));
        //        //      dc8.MaxLength = 10;
        //        dc8.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc8);

        //        DataColumn dc9 = new DataColumn("reAssessment", typeof(string));
        //        //      dc9.MaxLength = 10;
        //        dc9.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc9);

        //        DataColumn dc10 = new DataColumn("originalAssessmentDate", typeof(DateTime));
        //        dtTraineeModule.Columns.Add(dc10);
        //        dc10.AllowDBNull = false;

        //        DataColumn dc11 = new DataColumn("reTakeModule", typeof(string));
        //        //      dc11.MaxLength = 10;
        //        dc11.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc11);

        //        DataColumn dc12 = new DataColumn("finalAssessmentDate", typeof(DateTime));
        //        //  dc12.MaxLength = 10;
        //        dc12.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc12);

        //        DataColumn dc13 = new DataColumn("finalAssessorId", typeof(string));
        //        //        dc13.MaxLength = 10;
        //        dc13.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc13);

        //        DataColumn dc14 = new DataColumn("SOAStatus", typeof(string));
        //        //       dc14.MaxLength = 10;
        //        dc14.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc14);

        //        DataColumn dc15 = new DataColumn("processSOADate", typeof(DateTime));
        //        //  dc15.MaxLength = 20;
        //        dc15.AllowDBNull = false;
        //        dtTraineeModule.Columns.Add(dc15);

        //        DataColumn dc16 = new DataColumn("receivedSOADate", typeof(DateTime));
        //        //    dc16.MaxLength = 20;
        //        dc16.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc16);


        //        DataColumn dc17 = new DataColumn("traineeModuleRemarks", typeof(string));
        //        //dc17.MaxLength = 10 , MAX in database;
        //        dc17.AllowDBNull = true;
        //        dtTraineeModule.Columns.Add(dc17);
        //    }
        //    catch (Exception)
        //    {

        //        return null;

        //    }
        //    return dtTraineeModule;
        //}

        //public DataTable getPrePaymentHistoryTable()
        //{
        //    DataTable dtPaymentHistory = new DataTable();

        //    try
        //    {

        //        DataColumn dc0 = new DataColumn("paymentId", typeof(int));
        //        dc0.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc0);

        //        DataColumn dc1 = new DataColumn("applicantId", typeof(string));
        //        //        dc1.MaxLength = 20;
        //        dc1.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc1);

        //        DataColumn dc2 = new DataColumn("traineeId", typeof(string));
        //        //         dc2.MaxLength = 20;
        //        dc2.AllowDBNull = true;
        //        dtPaymentHistory.Columns.Add(dc2);

        //        DataColumn dc3 = new DataColumn("courseCode", typeof(string));
        //        ///           dc3.MaxLength = 20;
        //        dc3.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc3);

        //        DataColumn dc4 = new DataColumn("projectCode", typeof(string));
        //        //           dc4.MaxLength = 10;
        //        dc4.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc4);
        //        DataColumn dc5 = new DataColumn("paymentDate", typeof(DateTime));
        //        //   dc5.MaxLength = 10;
        //        dc5.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc5);

        //        DataColumn dc6 = new DataColumn("paymentAmount", typeof(double));
        //        //    dc6.MaxLength = 18; //(18,2)
        //        dc6.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc6);

        //        DataColumn dc7 = new DataColumn("selfSponsored", typeof(string));
        //        //         dc7.MaxLength = 1;
        //        dc7.AllowDBNull = true;
        //        dtPaymentHistory.Columns.Add(dc7);

        //        DataColumn dc8 = new DataColumn("idNumber", typeof(string));
        //        //        dc8.MaxLength = 50;
        //        dc8.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc8);

        //        DataColumn dc9 = new DataColumn("paymentMode", typeof(string));
        //        //          dc9.MaxLength = 6;
        //        dc9.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc9);

        //        DataColumn dc10 = new DataColumn("receiptNumber", typeof(string));
        //        //         dc10.MaxLength = 20;              
        //        dc10.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc10);

        //        DataColumn dc11 = new DataColumn("referenceNumber", typeof(string));
        //        //          dc11.MaxLength = 20;
        //        dc11.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc11);

        //        DataColumn dc12 = new DataColumn("bankInDate", typeof(DateTime));
        //        //  dc12.MaxLength = 10;
        //        dc12.AllowDBNull = false;
        //        dtPaymentHistory.Columns.Add(dc12);

        //        DataColumn dc13 = new DataColumn("paymentRemarks", typeof(string));
        //        //           dc13.MaxLength = 255;
        //        dc13.AllowDBNull = true;
        //        dtPaymentHistory.Columns.Add(dc13);
        //    }
        //    catch (Exception)
        //    {

        //        return null;

        //    }
        //    return dtPaymentHistory;
        //}

        //public DataTable getPreTraineeCourseTable()
        //{
        //    DataTable dTtraineeCourse = new DataTable();

        //    try
        //    {

        //        DataColumn dc0 = new DataColumn("courseCode", typeof(string));
        //        dc0.AllowDBNull = false;
        //        dTtraineeCourse.Columns.Add(dc0);

        //        DataColumn dc1 = new DataColumn("projectCode", typeof(string));
        //        //        dc1.MaxLength = 20;
        //        dc1.AllowDBNull = false;
        //        dTtraineeCourse.Columns.Add(dc1);

        //        DataColumn dc2 = new DataColumn("packageCode", typeof(string));
        //        //         dc2.MaxLength = 20;
        //        dc2.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc2);

        //        DataColumn dc3 = new DataColumn("traineeId", typeof(string));
        //        ///           dc3.MaxLength = 20;
        //        dc3.AllowDBNull = false;
        //        dTtraineeCourse.Columns.Add(dc3);

        //        DataColumn dc4 = new DataColumn("enrolDate", typeof(DateTime));
        //        //           dc4.MaxLength = 10;
        //        dc4.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc4);
        //        DataColumn dc5 = new DataColumn("coursePayableAmount", typeof(double));
        //        //   dc5.MaxLength = 10;
        //        dc5.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc5);

        //        DataColumn dc6 = new DataColumn("registrationFee", typeof(double));
        //        //    dc6.MaxLength = 18; //(18,2)
        //        dc6.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc6);

        //        DataColumn dc7 = new DataColumn("registrationFeePaymentId", typeof(int));
        //        //         dc7.MaxLength = 1;
        //        dc7.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc7);

        //        DataColumn dc8 = new DataColumn("registrationFeeStatus", typeof(string));
        //        //        dc8.MaxLength = 50;
        //        dc8.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc8);

        //        DataColumn dc9 = new DataColumn("traineeStatus", typeof(string));
        //        //          dc9.MaxLength = 6;
        //        dc9.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc9);

        //        DataColumn dc10 = new DataColumn("traineeCourseRemarks", typeof(string));
        //        //         dc10.MaxLength = 20;              
        //        dc10.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc10);

        //        DataColumn dc11 = new DataColumn("applicationDate", typeof(DateTime));
        //        //          dc11.MaxLength = 20;
        //        dc11.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc11);

        //        DataColumn dc12 = new DataColumn("subsidyType", typeof(string));
        //        //  dc12.MaxLength = 10;
        //        dc12.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc12);

        //        DataColumn dc13 = new DataColumn("subsidyRate", typeof(double));
        //        //           dc13.MaxLength = 255;
        //        dc13.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc13);

        //        DataColumn dc14 = new DataColumn("courseWithdrawnDate", typeof(DateTime));
        //        //       dc14.MaxLength = 10;
        //        dc14.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc14);

        //        DataColumn dc15 = new DataColumn("courseWithdrawnRemarks", typeof(string));
        //        //  dc15.MaxLength = 20;
        //        dc15.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc15);

        //        DataColumn dc16 = new DataColumn("classMode", typeof(string));
        //        //    dc16.MaxLength = 20;
        //        dc16.AllowDBNull = true;
        //        dTtraineeCourse.Columns.Add(dc16);


        //        //    DataColumn dc17 = new DataColumn("exemptedModule", typeof(string));
        //        //    //dc17.MaxLength = 10 , MAX in database;
        //        //    dc17.AllowDBNull = true;
        //        //    dTtraineeCourse.Columns.Add(dc17);



        //        //    DataColumn dc18 = new DataColumn("traineeModuleRemarks", typeof(string));
        //        //    //dc18.MaxLength = 10 , MAX in database;
        //        //    dc18.AllowDBNull = true;
        //        //    dTtraineeCourse.Columns.Add(dc18);
        //    }
        //    catch (Exception)
        //    {

        //        return null;

        //    }
        //    return dTtraineeCourse;
        //}




        // don need to hardcode the predefind table
        public DataTable GetPreDefineTable(string dataSetName, string tableName)
        {
            DataTable table = new DataTable(dataSetName);


            SqlDataAdapter adapter = new SqlDataAdapter(
                "SELECT TOP 0 * FROM dbo." + tableName + "", conString);

            DataTableMapping mapping = adapter.TableMappings.Add("Table", tableName);

            //mapping.ColumnMappings.Add("CompanyName", "Name");
            //mapping.ColumnMappings.Add("ContactName", "Contact");

            // fill schema to get the properties such as: AllowDBNull, AutoIncrement,MaxLength,ReadOnly,Unique
            adapter.FillSchema(table, SchemaType.Mapped);
            adapter.Fill(table);
            return table;

        }

        // get list of datatype errors( no longer in use) -----> change to using the method of tryParse and display error message in the table with specific cell
        //public List<string> getListOfDatatypeErrors(DataTable dtActualTable, DataTable tempDataTable, string tableName, string extension, string excelPath)
        //{

        //    //  List<string> listofDuplicates = new List<string>();
        //    List<string> listofErrorMsg = new List<string>();
        //    bool validation = true;        // status for the datatype

        //    // datatype validation
        //    StringBuilder msg = new StringBuilder();
        //    List<string> DatetimeErrorlist = new List<string>();
        //    List<string> DecimalErrorlist = new List<string>();
        //    List<string> IntErrorList = new List<string>();
        //    List<string> listOfIntegerCol = new List<string>();
        //    List<string> listOfDateCol = new List<string>();
        //    List<string> listOfStringCol = new List<string>();
        //    //    List<string> listOfDoubleCol = new List<string>();
        //    List<string> listOfDecimalCol = new List<string>();
        //    DB_DataAnalytics dm = new DB_DataAnalytics();



        //    // get the list of datetime col in the precreated datatable
        //    listOfDateCol = dm.getListofCertainDataTypeColumns(dtActualTable, "System.DateTime");
        //    // get the list of string col in precreated datatble
        //    listOfStringCol = dm.getListofCertainDataTypeColumns(dtActualTable, "System.String");
        //    // get the list of decimal col in precreated datatable
        //    //     listOfDoubleCol = dm.getListofCertainDataTypeColumns(dtActualTable, "System.Double");
        //    listOfIntegerCol = dm.getListofCertainDataTypeColumns(dtActualTable, "System.Int32");
        //    listOfDecimalCol = dm.getListofCertainDataTypeColumns(dtActualTable, "System.Decimal");

        //    // validate dates format
        //    for (int a = 0; a < listOfDateCol.Count; a++)
        //    {
        //        string listColName = listOfDateCol[a];

        //        validation = dm.ValidateDataType(tempDataTable, listColName, "System.DateTime");

        //        if (validation == false)
        //        {
        //            //    validationStatus = false;
        //            DatetimeErrorlist.Add(listColName);
        //        }

        //    }

        //    // validate decimal format
        //    for (int a = 0; a < listOfDecimalCol.Count; a++)
        //    {
        //        string listColName = listOfDecimalCol[a];

        //        validation = dm.ValidateDataType(tempDataTable, listColName, "System.Decimal");

        //        if (validation == false)
        //        {
        //            //      validationStatus = false;
        //            DecimalErrorlist.Add(listColName);
        //        }

        //    }
        //    // validate integer type
        //    for (int a = 0; a < listOfIntegerCol.Count; a++)
        //    {
        //        string listColName = listOfIntegerCol[a];
        //        validation = dm.ValidateDataType(tempDataTable, listColName, "System.Int32");
        //        if (validation == false)
        //        {
        //            IntErrorList.Add(listColName);
        //        }
        //    }

        //    List<string> listOfnonNullable = new List<string>();
        //    // get the list of columns from the precreated table that not allow null, then check for null
        //    listOfnonNullable = dm.getNonNullableColumn(dtActualTable);

        //    StringBuilder sb = new StringBuilder();
        //    //1. check if there is any non-nullable columns that contains null in the table            
        //    sb = dm.validateNull(tempDataTable, listOfnonNullable, tableName);
        //    if (sb.ToString() != string.Empty)
        //    {
        //        //        validationStatus = false;
        //        string finalMsg2 = sb.ToString();
        //        listofErrorMsg.Add(finalMsg2);
        //    }


        //    StringBuilder sbDateTypeMsg = new StringBuilder();
        //    sbDateTypeMsg = dm.GetErrorListMsg(DatetimeErrorlist);
        //    //2. check if there is invalid date error
        //    if (sbDateTypeMsg.ToString() != string.Empty)
        //    {
        //        //   validationStatus = false;
        //        string finalMsg = sbDateTypeMsg.ToString() + " column(s) in the " + tableName + " table contains invalid date";
        //        listofErrorMsg.Add(finalMsg);

        //    }
        //    StringBuilder sbDecimalTypeMsg = new StringBuilder();
        //    sbDecimalTypeMsg = dm.GetErrorListMsg(DecimalErrorlist);
        //    //2. check if there is invalid decimaltype error
        //    if (sbDecimalTypeMsg.ToString() != string.Empty)
        //    {
        //        // validationStatus = false;
        //        string finalMsg = sbDecimalTypeMsg.ToString() + " column(s) in the " + tableName + " table contains invalid amount";
        //        listofErrorMsg.Add(finalMsg);

        //    }
        //    StringBuilder sbIntTypeMsg = new StringBuilder();
        //    sbIntTypeMsg = dm.GetErrorListMsg(IntErrorList);
        //    if (sbIntTypeMsg.ToString() != string.Empty)
        //    {
        //        string finalMsg = sbIntTypeMsg.ToString() + " column(s) in the " + tableName + " table contains invalid whole number";
        //        listofErrorMsg.Add(finalMsg);
        //    }

        //    return listofErrorMsg;



        //    //if (validationStatus == true) // when the validation is true then add the datatable into dataset
        //    //{

        //    //    // method to Add lastModifiedDate columns in the datatable and Assign value to every rows in 'lastModifiedDate'the column 
        //    //    // dm.insertLastModifiedDate(TempdtTrainneCourse);
        //    //    // add datatable into the dataset dsAllData and name the table
        //    //    dsAllData.Tables.Add(TempdtTrainneCourse);
        //    //    dsAllData.Tables[i].TableName = "trainee_course";
        //    //}




        //}

        // method to get the numeric_precision = digits and numeric_scale = decimal place of the list of Decimal columns 
        public DataTable getPrecisonScaleByColumns(List<string> columnNameList, string tableName)
        {
            DataTable dtPrecision = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            for (int i = 0; i < columnNameList.Count; i++)
            {
                string query = "SELECT COLUMN_NAME as columnName ,NUMERIC_PRECISION as Digits ,NUMERIC_SCALE as Decimal FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = @tableName AND COLUMN_NAME = @ColumnName";
                adapter = new SqlDataAdapter(query, conString);
                adapter.SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                adapter.SelectCommand.Parameters.AddWithValue("@ColumnName", columnNameList[i]);
                adapter.Fill(dtPrecision);

            }

            return dtPrecision;
        }




        public void getColumnExampleVer1(DataColumn dc, DataTable dt4)
        {

            // excel sheet needs to define the column's datatype and store the correct type

            //   DateTime dt = new DateTime();

            //   dt = DateTime.ParseExact("25/01/2013", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            switch (dc.ColumnName)
            {
                // some example not confirm because database schema not ready
                // payment_history, trainee course, trainee_module
                case "courseCode":
                    dt4.Rows[0]["courseCode"] = "FB-HCT-09-1";
                    break;
                case "projectCode":
                    dt4.Rows[0]["projectCode"] = "PC17005";
                    break;
                case "moduleCode":
                    dt4.Rows[0]["moduleCode"] = "FB-FBP-224E-0";
                    break;
                case "classCode":
                    dt4.Rows[0]["classCode"] = "FBCRT17/01D";
                    break;
                case "traineeId":
                    dt4.Rows[0]["traineeId"] = "oAJRKLA3";
                    break;
                case "moduleResult":
                    dt4.Rows[0]["moduleResult"] = "C/NYC/PA/EXEM";
                    break;
                case "sitInModule":
                    dt4.Rows[0]["sitInModule"] = "Y/N";
                    break;
                case "assessmentCompleted":
                    dt4.Rows[0]["assessmentCompleted"] = "Y/N";
                    break;
                case "makeUpAssessment":
                    dt4.Rows[0]["makeUpAssessment"] = "Y/N";
                    break;
                case "reAssessment":
                    dt4.Rows[0]["reAssessment"] = "Y/N";
                    break;
                case "originalAssessmentDate":
                    //    dt = DateTime.ParseExact("21/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);            
                    dt4.Rows[0]["originalAssessmentDate"] = "21/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["originalAssessmentDate"] = DateTime.Parse(dr["originalAssessmentDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "reTakeModule":
                    dt4.Rows[0]["reTakeModule"] = "Y/N";
                    break;
                case "finalAssessmentDate":
                    // dt = DateTime.ParseExact("30/1/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["finalAssessmentDate"] = "30/1/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["finalAssessmentDate"] = DateTime.Parse(dr["finalAssessmentDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "finalAssessorId":
                    dt4.Rows[0]["finalAssessorId"] = "17281HJ";
                    break;
                case "SOAStatus":
                    dt4.Rows[0]["SOAStatus"] = "NYA/PROC/PSOA";
                    break;
                case "processSOADate":
                    // dt = DateTime.ParseExact("20/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["processSOADate"] = "20/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["processSOADate"] = DateTime.Parse(dr["processSOADate"].ToString()).ToShortDateString();

                    }
                    break;
                case "receivedSOADate":
                    //    dt = DateTime.ParseExact( "28/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["receivedSOADate"] = "28/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["receivedSOADate"] = DateTime.Parse(dr["receivedSOADate"].ToString()).ToShortDateString();

                    }
                    break;
                case "traineeModuleRemarks":
                    dt4.Rows[0]["traineeModuleRemarks"] = "This trainee module is about training of ...";
                    break;
                case "packageCode":
                    dt4.Rows[0]["packageCode"] = "P1";
                    break;
                case "enrolDate":
                    //   dt = DateTime.ParseExact("20/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["enrolDate"] = "20/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["enrolDate"] = DateTime.Parse(dr["enrolDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "coursePayableAmount":
                    dt4.Rows[0]["coursePayableAmount"] = "50.20";
                    break;
                case "registrationFeePaymentId":
                    dt4.Rows[0]["registrationFeePaymentId"] = "1";
                    break;
                case "registrationFee":
                    dt4.Rows[0]["registrationFee"] = "50.00";
                    break;
                case "traineeStatus":
                    dt4.Rows[0]["traineeStatus"] = "E/WD/CC";
                    break;
                case "courseCancelledDate":
                    //   dt = DateTime.ParseExact("20/05/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseCancelledDate"] = "20/05/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseCancelledDate"] = DateTime.Parse(dr["courseCancelledDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "traineeCourseRemarks":
                    dt4.Rows[0]["traineeCourseRemarks"] = "This trainee course is about ...";
                    break;
                case "applicationDate":
                    //   dt = DateTime.ParseExact("18/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["applicationDate"] = "18/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["applicationDate"] = DateTime.Parse(dr["applicationDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "subsidyType":
                    dt4.Rows[0]["subsidyType"] = "SGPR/WTS/NULL";
                    break;
                case "subsidyRate":
                    dt4.Rows[0]["subsidyRate"] = "0.9";
                    break;
                case "courseWithdrawnDate":
                    //      dt = DateTime.ParseExact("18/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseWithdrawnDate"] = "18/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["courseWithdrawnDate"] = DateTime.Parse(dr["courseWithdrawnDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "classMode":
                    dt4.Rows[0]["classMode"] = "FT/PT";
                    break;
                case "paymentId":
                    dt4.Rows[0]["paymentId"] = "1";
                    break;
                case "applicantId":
                    dt4.Rows[0]["applicantId"] = "KFEgNG61";
                    break;
                case "paymentDate":
                    //    dt = DateTime.ParseExact( "25/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["paymentDate"] = "25/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["paymentDate"] = DateTime.Parse(dr["paymentDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "paymentAmount":
                    dt4.Rows[0]["paymentAmount"] = "200.50";
                    break;
                case "selfSponsored":
                    dt4.Rows[0]["selfSponsored"] = "Y/N";
                    break;
                case "idNumber":
                    dt4.Rows[0]["idNumber"] = "S6812345F";
                    break;
                case "paymentMode":
                    dt4.Rows[0]["paymentMode"] = "CHEQ/NETS/SKFT/PSEA";
                    break;
                case "receiptNumber":
                    dt4.Rows[0]["receiptNumber"] = "123109";
                    break;
                case "referenceNumber":
                    dt4.Rows[0]["referenceNumber"] = "JW02124";
                    break;
                case "bankInDate":
                    //   dt = DateTime.ParseExact("28/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["bankInDate"] = "28/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["bankInDate"] = DateTime.Parse(dr["bankInDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "paymentRemarks":
                    dt4.Rows[0]["paymentRemarks"] = "Payment is make through...";
                    break;
                case "courseWithdrawnRemarks":
                    dt4.Rows[0]["courseWithdrawnRemarks"] = "Withdraw because ...";
                    break;

                case "registrationFeeStatus":
                    dt4.Rows[0]["registrationFeeStatus"] = "Paid/Pending";
                    break;
                // applicant table

                case "fullName":
                    dt4.Rows[0]["fullName"] = "Varsha Roxane Erskine Alexandros Gottlieb";
                    break;
                case "idType":
                    dt4.Rows[0]["idType"] = "SGC/SGPR/FIN/PP";
                    break;
                case "nationality":
                    dt4.Rows[0]["nationality"] = "SG";
                    break;
                case "gender":
                    dt4.Rows[0]["gender"] = "F/M";
                    break;
                case "contactNumber1":

                    dt4.Rows[0]["contactNumber1"] = "81234567";
                    break;
                case "contactNumber2":
                    dt4.Rows[0]["contactNumber2"] = "81234567";
                    break;
                case "emailAddress":
                    dt4.Rows[0]["emailAddress"] = "Nicholas_Bishop123@hotmail.com";
                    break;
                case "race":
                    dt4.Rows[0]["race"] = "C/E/I/M/O";
                    break;
                case "birthDate":
                    //    dt = DateTime.ParseExact("24/02/1987", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["birthDate"] = "24/02/1987";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["birthDate"] = DateTime.Parse(dr["birthDate"].ToString()).ToShortDateString();
                    }
                    //   drow["Cell Value"] = DateTime.Parse((drow["Cell Value"].ToString())).ToShortDateString();

                    break;
                case "addressLine":
                    dt4.Rows[0]["addressLine"] = "432 Summerview Junction";
                    break;
                case "postalCode":
                    dt4.Rows[0]["postalCode"] = "337076";
                    break;
                case "highestEducation":
                    dt4.Rows[0]["highestEducation"] = "PRI/SEC/OLEVEL/ALEVEL/DIP/DEG/OTH";
                    break;
                case "highestEduRemarks":
                    dt4.Rows[0]["highestEduRemarks"] = "The qualification ...";
                    break;
                case "spokenLanguage":
                    dt4.Rows[0]["spokenLanguage"] = "English: 1, Chinese: 3";
                    break;
                case "writtenLanguage":
                    dt4.Rows[0]["writtenLanguage"] = "English: 1, Chinese: 2";
                    break;
                case "occupationCode":
                    dt4.Rows[0]["occupationCode"] = "1";
                    break;
                case "occupationRemarks":
                    dt4.Rows[0]["occupationRemarks"] = "NULL";
                    break;

                case "interviewStatus":
                    dt4.Rows[0]["interviewStatus"] = "F/NREQ/NYD/P/PD";
                    break;
                case "shortlistStatus":
                    dt4.Rows[0]["shortlistStatus"] = "Y/N";
                    break;
                case "recommendStatus":
                    dt4.Rows[0]["recommendStatus"] = "Y/N";
                    break;
                case "enrolApproval":
                    dt4.Rows[0]["enrolApproval"] = "Y/N";
                    break;
                case "blacklistStatus":
                    dt4.Rows[0]["blacklistStatus"] = "Y/N";
                    break;
                case "rejectStatus":
                    dt4.Rows[0]["rejectStatus"] = "Y/N";
                    break;
                case "getToKnowChannel":
                    dt4.Rows[0]["getToKnowChannel"] = "Facebook/Magazine/Word of mouth";
                    break;
                case "applicantRemarks":
                    dt4.Rows[0]["applicantRemarks"] = "The applicant...";
                    break;
                case "applicationStatus":
                    dt4.Rows[0]["applicationStatus"] = "NEW/PD/WD/WL";
                    break;
                case "applicantExemptedModule":
                    dt4.Rows[0]["applicantExemptedModule"] = "this is an exempted module...";
                    break;

                // aci_suspended_list

                case "byOrganization":
                    dt4.Rows[0]["byOrganization"] = "ACI";
                    break;
                case "startDate":
                    //   dt = DateTime.ParseExact("20/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["startDate"] = "20/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["startDate"] = DateTime.Parse(dr["startDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "endDate":
                    //     dt = DateTime.ParseExact("20/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["endDate"] = "20/01/2018";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["endDate"] = DateTime.Parse(dr["endDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "suspendedRemarks":
                    dt4.Rows[0]["suspendedRemarks"] = "Disciplinary issue";
                    break;
                // applicant_interview_result

                case "interviewerId":
                    dt4.Rows[0]["interviewerId"] = "NULL";
                    break;
                case "interviewDate":
                    //   dt = DateTime.ParseExact("18/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["interviewDate"] = "18/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["interviewDate"] = DateTime.Parse(dr["interviewDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "interviewRemarks":
                    dt4.Rows[0]["interviewRemarks"] = "NULL";
                    break;
                // employment_history
                case "employmentHistoryId": // and paymentId is auto 
                    dt4.Rows[0]["employmentHistoryId"] = "1";
                    break;
                case "companyName":
                    dt4.Rows[0]["companyName"] = "Gabspot";
                    break;
                case "companyDepartment":
                    dt4.Rows[0]["companyDepartment"] = "Accounting";
                    break;
                case "salaryAmount":
                    dt4.Rows[0]["salaryAmount"] = "1593.50";
                    break;
                case "position":
                    dt4.Rows[0]["position"] = "Operator";
                    break;
                case "employmentStatus":
                    dt4.Rows[0]["employmentStatus"] = "E/EN/S/SN";
                    break;
                case "currentEmployment":
                    dt4.Rows[0]["currentEmployment"] = "Y/N";
                    break;
                case "employmentStartDate":
                    //       dt = DateTime.ParseExact("20/06/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["employmentStartDate"] = "20/06/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["employmentStartDate"] = DateTime.Parse(dr["employmentStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "employmentEndDate":
                    //     dt = DateTime.ParseExact("18/05/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["employmentEndDate"] = "18/05/2016";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["employmentEndDate"] = DateTime.Parse(dr["employmentEndDate"].ToString()).ToShortDateString();
                    }
                    break;

                // trainee       
                case "traineeRemarks":
                    dt4.Rows[0]["traineeRemarks"] = "NULL";
                    break;
                // trainee_absence_makeup_record + trainee_absence_makeup_removed
                case "sessionNumber":
                    dt4.Rows[0]["sessionNumber"] = "1";
                    break;
                case "isAbsentValid":
                    dt4.Rows[0]["isAbsentValid"] = "Y/N";
                    break;
                case "absentRemarks":
                    dt4.Rows[0]["absentRemarks"] = "Medical";
                    break;
                case "insertedClassCode":
                    dt4.Rows[0]["insertedClassCode"] = "FBCRT17/01D";
                    break;
                case "insertedSessionNumber":
                    dt4.Rows[0]["insertedSessionNumber"] = "1";
                    break;
                // course_structure
                case "courseVersion":
                    dt4.Rows[0]["courseVersion"] = "1";
                    break;
                case "courseLevel":
                    dt4.Rows[0]["courseLevel"] = "ACT/CRT/DIP/HCT/NA";
                    break;
                case "courseCategory":
                    dt4.Rows[0]["courseCategory"] = "BS/CA/FB/PB";
                    break;
                case "courseTitle":
                    dt4.Rows[0]["courseTitle"] = "Certificate in Culinary Arts";
                    break;
                case "courseDescription":
                    dt4.Rows[0]["courseDescription"] = "Certificate in Culinary Arts";
                    break;
                case "numberOfSOA":
                    dt4.Rows[0]["numberOfSOA"] = "12";
                    break;
                case "WSQCode":
                    dt4.Rows[0]["WSQCode"] = "FB-CRT-04-1";
                    break;
                case "isShortCourse":
                    dt4.Rows[0]["isShortCourse"] = "Y/N";
                    break;
                // for those img column...just ask them to leave blank?
                case "imageUrl":
                    dt4.Rows[0]["imageUrl"] = "NULL";
                    break;
                case "defunct":
                    dt4.Rows[0]["defunct"] = "Y/N";
                    break;
                case "courseImage":
                    dt4.Rows[0]["courseImage"] = "NULL";
                    break;
                // module structure

                case "moduleVersion":
                    dt4.Rows[0]["moduleVersion"] = "1";
                    break;
                case "moduleLevel":
                    dt4.Rows[0]["moduleLevel"] = "1";
                    break;
                case "moduleTitle":
                    dt4.Rows[0]["moduleTitle"] = "Research and Analyse Business Opportunities";
                    break;
                case "moduleDescription":
                    dt4.Rows[0]["moduleDescription"] = "Research and Analyse Business Opportunities";
                    break;
                case "moduleCredit":
                    dt4.Rows[0]["moduleCredit"] = "4";
                    break;
                case "moduleCost":
                    dt4.Rows[0]["moduleCost"] = "109.4";
                    break;
                case "WSQCompetencyCode":
                    dt4.Rows[0]["WSQCompetencyCode"] = "NULL";
                    break;
                case "moduleEffectDate":
                    //        dt = DateTime.ParseExact("21/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["moduleEffectDate"] = "21/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["moduleEffectDate"] = DateTime.Parse(dr["moduleEffectDate"].ToString()).ToShortDateString();
                    }
                    break;
                // package
                case "packageType":
                    dt4.Rows[0]["packageType"] = "FQ/SC";
                    break;
                case "moduleNumOfSession":
                    dt4.Rows[0]["moduleNumOfSession"] = "3";
                    break;
                case "packageEffectDate":
                    //       dt = DateTime.ParseExact("22/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["packageEffectDate"] = "22/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["packageEffectDate"] = DateTime.Parse(dr["packageEffectDate"].ToString()).ToShortDateString();
                    }
                    break;
                // course_batch

                case "courseRegStartDate":
                    //        dt = DateTime.ParseExact( "20/02/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseRegStartDate"] = "20/02/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseRegStartDate"] = DateTime.Parse(dr["courseRegStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "courseRegEndDate":
                    //        dt = DateTime.ParseExact("20/02/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseRegEndDate"] = "20/02/2018";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseRegEndDate"] = DateTime.Parse(dr["courseRegEndDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "courseStartDate":
                    //        dt = DateTime.ParseExact("21/04/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseStartDate"] = "21/04/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseStartDate"] = DateTime.Parse(dr["courseStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "courseCompletionDate":
                    //         dt = DateTime.ParseExact("21/05/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseCompletionDate"] = "21/05/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseCompletionDate"] = DateTime.Parse(dr["courseCompletionDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "batchCapacity":
                    dt4.Rows[0]["batchCapacity"] = "30";
                    break;

                // class structure
                case "classDay":
                    dt4.Rows[0]["classDay"] = "1;2;3;4;5;6;7(can be multiple value)";
                    break;
                case "classStartDate":
                    //         dt = DateTime.ParseExact("21/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["classStartDate"] = "21/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["classStartDate"] = DateTime.Parse(dr["classStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "classEndDate":
                    //          dt = DateTime.ParseExact( "21/04/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["classEndDate"] = "21/04/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["classEndDate"] = DateTime.Parse(dr["classEndDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "trainerUserId1":
                    dt4.Rows[0]["trainerUserId1"] = "NULL";
                    break;
                case "trainerUserId2":
                    dt4.Rows[0]["trainerUserId2"] = "NULL";
                    break;
                case "assessorUserId":
                    dt4.Rows[0]["assessorUserId"] = "NULL";
                    break;
                case "assessmentDate":
                    //           dt = DateTime.ParseExact("28/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["assessmentDate"] = "28/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["assessmentDate"] = DateTime.Parse(dr["assessmentDate"].ToString()).ToShortDateString();
                    }
                    break;
                // class session

                case "sessionDate":
                    //       dt = DateTime.ParseExact("27/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["sessionDate"] = "27/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["sessionDate"] = DateTime.Parse(dr["sessionDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "sessionPeriod":
                    dt4.Rows[0]["sessionPeriod"] = "AM/PM";
                    break;
                case "sessionCapacity":
                    dt4.Rows[0]["sessionCapacity"] = "30";
                    break;
                case "venueId":
                    dt4.Rows[0]["venueId"] = "V02";
                    break;
                // venue
                case "venueLocation":
                    dt4.Rows[0]["venueLocation"] = "Bakery & Pastry Kitchen";
                    break;
                case "venueCapacity":
                    dt4.Rows[0]["venueCapacity"] = "30";
                    break;
                case "venueDesc":
                    dt4.Rows[0]["venueDesc"] = "Bakery & Pastry Kitchen";
                    break;
                case "venueEffectDate":
                    //          dt = DateTime.ParseExact("20/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["venueEffectDate"] = "20/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["venueEffectDate"] = DateTime.Parse(dr["venueEffectDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "bookingId":
                    dt4.Rows[0]["bookingId"] = "1"; // auto
                    break;
                case "bookingDate":
                    //           dt = DateTime.ParseExact( "15/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["bookingDate"] = "15/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["bookingDate"] = DateTime.Parse(dr["bookingDate"].ToString()).ToShortDateString();

                    }
                    break;
                case "bookingPurpose":
                    dt4.Rows[0]["bookingPurpose"] = "Lesson";
                    break;
                case "bookingRemarks":
                    dt4.Rows[0]["bookingRemarks"] = "NULL";
                    break;
                // aci_user
                case "userId":
                    dt4.Rows[0]["userId"] = "1";
                    break;
                case "userName":
                    dt4.Rows[0]["userName"] = "Helen Hart";
                    break;
                case "userEmail":
                    dt4.Rows[0]["userEmail"] = "hhart11@bing.com";
                    break;
                case "userAuthorization":
                    dt4.Rows[0]["userAuthorization"] = "Y/N";
                    break;
                // aci_role , aci_user_role
                case "roleId":
                    dt4.Rows[0]["roleId"] = "1";
                    break;
                case "roleTitle":
                    dt4.Rows[0]["roleTitle"] = "System Admin/Front Desk/Program Admin";
                    break;
                case "roleDescription":
                    dt4.Rows[0]["roleDescription"] = "System Admin/Front Desk/Program Admin";
                    break;
                case "roleLevel":
                    dt4.Rows[0]["roleLevel"] = "1";
                    break;
                // code reference
                case "codeType":
                    dt4.Rows[0]["codeType"] = "EDU";
                    break;
                case "codeDesc":
                    dt4.Rows[0]["codeDesc"] = "Education Level";
                    break;
                case "codeValue":
                    dt4.Rows[0]["codeValue"] = "PRI";
                    break;
                case "codeDisplay":
                    dt4.Rows[0]["codeDisplay"] = "PRIMARY";
                    break;
                case "codeOrder":
                    dt4.Rows[0]["codeOrder"] = "1";
                    break;
                case "codeRemarks":
                    dt4.Rows[0]["codeRemarks"] = "NULL";
                    break;
                case "codeEffectDate":
                    //    dt = DateTime.ParseExact("25/11/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["codeEffectDate"] = "25/11/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["codeEffectDate"] = DateTime.Parse(dr["codeEffectDate"].ToString()).ToShortDateString();
                    }
                    break;

                default:
                    break;

            }
        }

        public void getColumnExampleVer2(DataColumn dc, DataTable dt4)
        {

            // excel sheet needs to define the column's datatype and store the correct type

            //   DateTime dt = new DateTime();

            //   dt = DateTime.ParseExact("25/01/2013", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            switch (dc.ColumnName)
            {

                // some example not confirm because database schema not ready
                // payment_history, trainee course, trainee_module


                // new columns
                case "paymentSeperated":
                    dt4.Rows[0]["paymentSeperated"] = "Y/N";
                    break;
                case "receiptStatus":
                    dt4.Rows[0]["receiptStatus"] = "Void/NA";
                    break;
                case "voidDate":
                    dt4.Rows[0]["voidDate"] = "25/11/2016";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["voidDate"] = DateTime.Parse(dr["voidDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "voidBy":
                    dt4.Rows[0]["voidBy"] = "Steven Yeo";
                    break;
                case "voidReason":
                    dt4.Rows[0]["voidReason"] = "The reason is because...";
                    break;
                case "OverrideReceiptNum":
                    dt4.Rows[0]["OverrideReceiptNum"] = "20171234510001";
                    break;
                case "paymentType":
                    dt4.Rows[0]["paymentType"] = "REGIS_FEE/COURSE_FEE/NA";
                    break;

                case "fixedSubsidyAmount":
                    dt4.Rows[0]["fixedSubsidyAmount"] = "100";
                    break;
                case "programmeBatchId":
                    dt4.Rows[0]["programmeBatchId"] = "1";
                    break;
                case "programmeId":
                    dt4.Rows[0]["programmeId"] = "1";
                    break;
                case "programmeTitle":
                    dt4.Rows[0]["programmeTitle"] = "Certificate in Culinary Arts";
                    break;
                case "projectCode":
                    dt4.Rows[0]["projectCode"] = "150801";
                    break;
                case "batchCode":
                    dt4.Rows[0]["batchCode"] = "CBP15/01E";
                    break;
                case "programmeRegStartDate":
                    dt4.Rows[0]["programmeRegStartDate"] = "18/2/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["programmeRegStartDate"] = DateTime.Parse(dr["programmeRegStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "programmeRegEndDate":
                    dt4.Rows[0]["programmeRegEndDate"] = "18/5/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["programmeRegEndDate"] = DateTime.Parse(dr["programmeRegEndDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "programmeStartDate":
                    dt4.Rows[0]["programmeStartDate"] = "20/6/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["programmeStartDate"] = DateTime.Parse(dr["programmeStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "programmeCompletionDate":
                    dt4.Rows[0]["programmeCompletionDate"] = "20/12/2016";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["programmeCompletionDate"] = DateTime.Parse(dr["programmeCompletionDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "batchCapacity":
                    dt4.Rows[0]["batchCapacity"] = "30";
                    break;
                case "firstAssessmentDate":
                    dt4.Rows[0]["firstAssessmentDate"] = "20/12/2016";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["firstAssessmentDate"] = DateTime.Parse(dr["firstAssessmentDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "firstAssessorId":
                    dt4.Rows[0]["firstAssessorId"] = "1";
                    break;

                case "classMode":
                    dt4.Rows[0]["classMode"] = "FT/PT";
                    break;
                case "traineeId":
                    dt4.Rows[0]["traineeId"] = "oAJRKLA3";
                    break;
                case "moduleResult":
                    dt4.Rows[0]["moduleResult"] = "C/NYC/PA/EXEM";
                    break;
                case "sitInModule":
                    dt4.Rows[0]["sitInModule"] = "Y/N";
                    break;
                case "assessmentCompleted":
                    dt4.Rows[0]["assessmentCompleted"] = "Y/N";
                    break;
                case "makeUpAssessment":
                    dt4.Rows[0]["makeUpAssessment"] = "Y/N";
                    break;
                case "reAssessment":
                    dt4.Rows[0]["reAssessment"] = "Y/N";
                    break;
                case "originalAssessmentDate":
                    //    dt = DateTime.ParseExact("21/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);            
                    dt4.Rows[0]["originalAssessmentDate"] = "21/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["originalAssessmentDate"] = DateTime.Parse(dr["originalAssessmentDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "reTakeModule":
                    dt4.Rows[0]["reTakeModule"] = "Y/N";
                    break;
                case "finalAssessmentDate":
                    // dt = DateTime.ParseExact("30/1/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["finalAssessmentDate"] = "30/1/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["finalAssessmentDate"] = DateTime.Parse(dr["finalAssessmentDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "finalAssessorId":
                    dt4.Rows[0]["finalAssessorId"] = "17281HJ";
                    break;
                case "SOAStatus":
                    dt4.Rows[0]["SOAStatus"] = "NYA/PROC/PSOA";
                    break;
                case "processSOADate":
                    // dt = DateTime.ParseExact("20/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["processSOADate"] = "20/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["processSOADate"] = DateTime.Parse(dr["processSOADate"].ToString()).ToShortDateString();

                    }
                    break;
                case "receivedSOADate":
                    //    dt = DateTime.ParseExact( "28/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["receivedSOADate"] = "28/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["receivedSOADate"] = DateTime.Parse(dr["receivedSOADate"].ToString()).ToShortDateString();

                    }
                    break;
                case "traineeModuleRemarks":
                    dt4.Rows[0]["traineeModuleRemarks"] = "This trainee module is about training of ...";
                    break;
                case "packageCode":
                    dt4.Rows[0]["packageCode"] = "P1";
                    break;
                case "enrolDate":
                    //   dt = DateTime.ParseExact("20/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["enrolDate"] = "20/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["enrolDate"] = DateTime.Parse(dr["enrolDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "coursePayableAmount":
                    dt4.Rows[0]["coursePayableAmount"] = "500.20";
                    break;
                case "registrationFeePaymentId":
                    dt4.Rows[0]["registrationFeePaymentId"] = "1";
                    break;
                case "registrationFee":
                    dt4.Rows[0]["registrationFee"] = "50.00";
                    break;
                case "traineeStatus":
                    dt4.Rows[0]["traineeStatus"] = "E/WD/CC";
                    break;
                case "courseCancelledDate":
                    //   dt = DateTime.ParseExact("20/05/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseCancelledDate"] = "20/05/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseCancelledDate"] = DateTime.Parse(dr["courseCancelledDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "traineeCourseRemarks":
                    dt4.Rows[0]["traineeCourseRemarks"] = "This trainee course is about ...";
                    break;
                case "applicationDate":
                    //   dt = DateTime.ParseExact("18/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["applicationDate"] = "18/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["applicationDate"] = DateTime.Parse(dr["applicationDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "subsidyType":
                    dt4.Rows[0]["subsidyType"] = "SGPR/WTS";
                    break;
                case "subsidyRate":
                    dt4.Rows[0]["subsidyRate"] = "0.9";
                    break;
                case "courseWithdrawnDate":
                    //      dt = DateTime.ParseExact("18/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseWithdrawnDate"] = "18/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["courseWithdrawnDate"] = DateTime.Parse(dr["courseWithdrawnDate"].ToString()).ToShortDateString();
                    }
                    break;

                case "paymentId":
                    dt4.Rows[0]["paymentId"] = "1";
                    break;
                case "applicantId":
                    dt4.Rows[0]["applicantId"] = "KFEgNG61";
                    break;
                case "paymentDate":
                    //    dt = DateTime.ParseExact( "25/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["paymentDate"] = "25/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["paymentDate"] = DateTime.Parse(dr["paymentDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "paymentAmount":
                    dt4.Rows[0]["paymentAmount"] = "200.50";
                    break;
                case "selfSponsored":
                    dt4.Rows[0]["selfSponsored"] = "Y/N";
                    break;
                case "idNumber":
                    dt4.Rows[0]["idNumber"] = "S6812345F";
                    break;
                case "paymentMode":
                    dt4.Rows[0]["paymentMode"] = "CHEQ/NETS/SKFT/PSEA";
                    break;
                case "receiptNumber":
                    dt4.Rows[0]["receiptNumber"] = "20171000010001";
                    break;
                case "referenceNumber":
                    dt4.Rows[0]["referenceNumber"] = "1224";
                    break;
                case "bankInDate":
                    //   dt = DateTime.ParseExact("28/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["bankInDate"] = "28/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["bankInDate"] = DateTime.Parse(dr["bankInDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "paymentRemarks":
                    dt4.Rows[0]["paymentRemarks"] = "Payment is make through...";
                    break;
                case "courseWithdrawnRemarks":
                    dt4.Rows[0]["courseWithdrawnRemarks"] = "Withdraw because ...";
                    break;

                case "registrationFeeStatus":
                    dt4.Rows[0]["registrationFeeStatus"] = "Paid/Pending";
                    break;
                // applicant table

                case "fullName":
                    dt4.Rows[0]["fullName"] = "Varsha Roxane Erskine Alexandros Gottlieb";
                    break;
                case "idType":
                    dt4.Rows[0]["idType"] = "SGC/SGPR/FIN/PP";
                    break;
                case "nationality":
                    dt4.Rows[0]["nationality"] = "SG";
                    break;
                case "gender":
                    dt4.Rows[0]["gender"] = "F/M";
                    break;
                case "contactNumber1":

                    dt4.Rows[0]["contactNumber1"] = "81234567";
                    break;
                case "contactNumber2":
                    dt4.Rows[0]["contactNumber2"] = "81234567";
                    break;
                case "emailAddress":
                    dt4.Rows[0]["emailAddress"] = "Nicholas_Bishop123@hotmail.com";
                    break;
                case "race":
                    dt4.Rows[0]["race"] = "C/E/I/M/O";
                    break;
                case "birthDate":
                    //    dt = DateTime.ParseExact("24/02/1987", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["birthDate"] = "24/02/1987";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        // convert datatime format to short date string(excel show time value, so remove it)
                        dr["birthDate"] = DateTime.Parse(dr["birthDate"].ToString()).ToShortDateString();
                    }
                    //   drow["Cell Value"] = DateTime.Parse((drow["Cell Value"].ToString())).ToShortDateString();

                    break;
                case "addressLine":
                    dt4.Rows[0]["addressLine"] = "432 Summerview Junction";
                    break;
                case "postalCode":
                    dt4.Rows[0]["postalCode"] = "337076";
                    break;
                case "highestEducation":
                    dt4.Rows[0]["highestEducation"] = "PRI/SEC/OLEVEL/ALEVEL/DIP/DEG/OTH";
                    break;
                case "highestEduRemarks":
                    dt4.Rows[0]["highestEduRemarks"] = "The qualification ...";
                    break;
                case "spokenLanguage":
                    dt4.Rows[0]["spokenLanguage"] = "English: 1, Chinese: 3";
                    break;
                case "writtenLanguage":
                    dt4.Rows[0]["writtenLanguage"] = "English: 1, Chinese: 2";
                    break;
                case "occupationCode":
                    dt4.Rows[0]["occupationCode"] = "1";
                    break;
                case "occupationRemarks":
                    dt4.Rows[0]["occupationRemarks"] = "The occupation ...";
                    break;

                case "interviewStatus":
                    dt4.Rows[0]["interviewStatus"] = "F/NREQ/NYD/P/PD";
                    break;
                case "shortlistStatus":
                    dt4.Rows[0]["shortlistStatus"] = "Y/N";
                    break;
                case "recommendStatus":
                    dt4.Rows[0]["recommendStatus"] = "Y/N";
                    break;
                case "enrolApproval":
                    dt4.Rows[0]["enrolApproval"] = "Y/N";
                    break;
                case "blacklistStatus":
                    dt4.Rows[0]["blacklistStatus"] = "Y/N";
                    break;
                case "rejectStatus":
                    dt4.Rows[0]["rejectStatus"] = "Y/N";
                    break;
                case "getToKnowChannel":
                    dt4.Rows[0]["getToKnowChannel"] = "Facebook/Magazine/Word of mouth";
                    break;
                case "applicantRemarks":
                    dt4.Rows[0]["applicantRemarks"] = "This applicant ...";
                    break;
                case "applicationStatus":
                    dt4.Rows[0]["applicationStatus"] = "NEW/PD/WD/WL";
                    break;
                case "applicantExemptedModule":
                    dt4.Rows[0]["applicantExemptedModule"] = "This is an exempted module..";
                    break;

                // aci_suspended_list

                case "byOrganization":
                    dt4.Rows[0]["byOrganization"] = "ACI";
                    break;
                case "startDate":
                    //   dt = DateTime.ParseExact("20/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["startDate"] = "20/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["startDate"] = DateTime.Parse(dr["startDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "endDate":
                    //     dt = DateTime.ParseExact("20/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["endDate"] = "20/01/2018";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["endDate"] = DateTime.Parse(dr["endDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "suspendedRemarks":
                    dt4.Rows[0]["suspendedRemarks"] = "Disciplinary issue";
                    break;
                // applicant_interview_result

                case "interviewerId":
                    dt4.Rows[0]["interviewerId"] = "1";
                    break;
                case "interviewDate":
                    //   dt = DateTime.ParseExact("18/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["interviewDate"] = "18/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["interviewDate"] = DateTime.Parse(dr["interviewDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "interviewRemarks":
                    dt4.Rows[0]["interviewRemarks"] = "The interview...";
                    break;
                // employment_history
                case "employmentHistoryId": // and paymentId is auto 
                    dt4.Rows[0]["employmentHistoryId"] = "1";
                    break;
                case "companyName":
                    dt4.Rows[0]["companyName"] = "Gabspot";
                    break;
                case "companyDepartment":
                    dt4.Rows[0]["companyDepartment"] = "Accounting";
                    break;
                case "salaryAmount":
                    dt4.Rows[0]["salaryAmount"] = "1593.50";
                    break;
                case "position":
                    dt4.Rows[0]["position"] = "Operator";
                    break;
                case "employmentStatus":
                    dt4.Rows[0]["employmentStatus"] = "E/EN/S/SN";
                    break;
                case "currentEmployment":
                    dt4.Rows[0]["currentEmployment"] = "Y/N";
                    break;
                case "employmentStartDate":
                    //       dt = DateTime.ParseExact("20/06/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["employmentStartDate"] = "20/06/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["employmentStartDate"] = DateTime.Parse(dr["employmentStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "employmentEndDate":
                    //     dt = DateTime.ParseExact("18/05/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["employmentEndDate"] = "18/05/2016";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["employmentEndDate"] = DateTime.Parse(dr["employmentEndDate"].ToString()).ToShortDateString();
                    }
                    break;

                // trainee       
                case "traineeRemarks":
                    dt4.Rows[0]["traineeRemarks"] = "The trainee ...";
                    break;
                // trainee_absence_makeup_record + trainee_absence_makeup_removed
                case "sessionId":
                    dt4.Rows[0]["sessionId"] = "1";
                    break;
                case "sessionDate":
                    dt4.Rows[0]["sessionDate"] = "19/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["sessionDate"] = DateTime.Parse(dr["sessionDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "sessionPeriod":
                    dt4.Rows[0]["sessionPeriod"] = "AM/PM";
                    break;
                case "sessionNumber":
                    dt4.Rows[0]["sessionNumber"] = "1";
                    break;
                case "sessionCapacity":
                    dt4.Rows[0]["sessionCapacity"] = "30";
                    break;
                case "isAbsentValid":
                    dt4.Rows[0]["isAbsentValid"] = "Y/N";
                    break;
                case "absentRemarks":
                    dt4.Rows[0]["absentRemarks"] = "Medical";
                    break;
                case "insertedClassCode":
                    dt4.Rows[0]["insertedClassCode"] = "FBCRT17/01D";
                    break;
                case "insertedSessionNumber":
                    dt4.Rows[0]["insertedSessionNumber"] = "1";
                    break;
                // course_structure
                case "programmeCode":
                    dt4.Rows[0]["programmeCode"] = "WSCC3";
                    break;
                case "programmeVersion":
                    dt4.Rows[0]["programmeVersion"] = "1";
                    break;
                case "programmeLevel":
                    dt4.Rows[0]["programmeLevel"] = "ACT/CRT/DIP/HCT/NA";
                    break;
                case "programmeCategory":
                    dt4.Rows[0]["programmeCategory"] = "BS/CA/FB/PB";
                    break;
                case "programmeDescription":
                    dt4.Rows[0]["programmeDescription"] = "Certificate in Culinary Arts";
                    break;
                case "numberOfSOA":
                    dt4.Rows[0]["numberOfSOA"] = "12";
                    break;
                case "WSQCode":
                    dt4.Rows[0]["WSQCode"] = "FB-CRT-04-1";
                    break;
                case "isShortCourse":
                    dt4.Rows[0]["isShortCourse"] = "Y/N";
                    break;
                case "SSGRefNum":
                    dt4.Rows[0]["SSGRefNum"] = "1029";
                    break;
                // for those img column...just ask them to leave blank?
                case "bundleCode":
                    dt4.Rows[0]["bundleCode"] = "PBC01";
                    break;
                case "defunct":
                    dt4.Rows[0]["defunct"] = "Y/N";
                    break;
                case "programmeType":
                    dt4.Rows[0]["programmeType"] = "FQ/SCWSQ/SCNWSQ";
                    break;
                // module structure

                case "moduleId":
                    dt4.Rows[0]["moduleId"] = "1";
                    break;
                case "moduleVersion":
                    dt4.Rows[0]["moduleVersion"] = "1";
                    break;
                case "moduleLevel":
                    dt4.Rows[0]["moduleLevel"] = "5";
                    break;

                case "moduleTitle":
                    dt4.Rows[0]["moduleTitle"] = "Research and Analyse Business Opportunities";
                    break;
                case "moduleDescription":
                    dt4.Rows[0]["moduleDescription"] = "Research and Analyse Business Opportunities";
                    break;
                case "moduleCredit":
                    dt4.Rows[0]["moduleCredit"] = "4";
                    break;
                case "moduleCost":
                    dt4.Rows[0]["moduleCost"] = "109.4";
                    break;
                case "WSQCompetencyCode":
                    dt4.Rows[0]["WSQCompetencyCode"] = "FB-FBP-110E-0";
                    break;
                case "moduleEffectDate":
                    //        dt = DateTime.ParseExact("21/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["moduleEffectDate"] = "21/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["moduleEffectDate"] = DateTime.Parse(dr["moduleEffectDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "moduleTrainingHour":
                    dt4.Rows[0]["moduleTrainingHour"] = "45";
                    break;
                // package
                case "packageType":
                    dt4.Rows[0]["packageType"] = "FQ/SC";
                    break;
                case "moduleNumOfSession":
                    dt4.Rows[0]["moduleNumOfSession"] = "3";
                    break;
                case "packageEffectDate":
                    //       dt = DateTime.ParseExact("22/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["packageEffectDate"] = "22/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["packageEffectDate"] = DateTime.Parse(dr["packageEffectDate"].ToString()).ToShortDateString();
                    }
                    break;
                // course_batch

                case "courseRegStartDate":
                    //        dt = DateTime.ParseExact( "20/02/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseRegStartDate"] = "20/02/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseRegStartDate"] = DateTime.Parse(dr["courseRegStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "courseRegEndDate":
                    //        dt = DateTime.ParseExact("20/02/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseRegEndDate"] = "20/02/2018";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseRegEndDate"] = DateTime.Parse(dr["courseRegEndDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "courseStartDate":
                    //        dt = DateTime.ParseExact("21/04/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseStartDate"] = "21/04/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseStartDate"] = DateTime.Parse(dr["courseStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "courseCompletionDate":
                    //         dt = DateTime.ParseExact("21/05/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["courseCompletionDate"] = "21/05/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["courseCompletionDate"] = DateTime.Parse(dr["courseCompletionDate"].ToString()).ToShortDateString();
                    }
                    break;


                // class structure
                case "classDay":
                    dt4.Rows[0]["classDay"] = "1;2;3;4;5;6;7(can be multiple value)";
                    break;
                case "classStartDate":
                    //         dt = DateTime.ParseExact("21/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["classStartDate"] = "21/02/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["classStartDate"] = DateTime.Parse(dr["classStartDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "classEndDate":
                    //          dt = DateTime.ParseExact( "21/04/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["classEndDate"] = "21/04/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["classEndDate"] = DateTime.Parse(dr["classEndDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "trainerUserId1":
                    dt4.Rows[0]["trainerUserId1"] = "1";
                    break;
                case "trainerUserId2":
                    dt4.Rows[0]["trainerUserId2"] = "2";
                    break;
                case "assessorUserId":
                    dt4.Rows[0]["assessorUserId"] = "3";
                    break;
                case "assessmentDate":
                    //           dt = DateTime.ParseExact("28/03/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["assessmentDate"] = "28/03/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["assessmentDate"] = DateTime.Parse(dr["assessmentDate"].ToString()).ToShortDateString();
                    }
                    break;
                // class session

                case "venueId":
                    dt4.Rows[0]["venueId"] = "V02";
                    break;
                // venue
                case "venueLocation":
                    dt4.Rows[0]["venueLocation"] = "Bakery & Pastry Kitchen";
                    break;
                case "venueCapacity":
                    dt4.Rows[0]["venueCapacity"] = "30";
                    break;
                case "venueDesc":
                    dt4.Rows[0]["venueDesc"] = "Bakery & Pastry Kitchen";
                    break;
                case "venueEffectDate":
                    //          dt = DateTime.ParseExact("20/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["venueEffectDate"] = "20/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["venueEffectDate"] = DateTime.Parse(dr["venueEffectDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "bookingId":
                    dt4.Rows[0]["bookingId"] = "1"; // auto
                    break;
                case "bookingDate":
                    //           dt = DateTime.ParseExact( "15/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["bookingDate"] = "15/01/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {

                        dr["bookingDate"] = DateTime.Parse(dr["bookingDate"].ToString()).ToShortDateString();

                    }
                    break;
                case "classId":
                    dt4.Rows[0]["classId"] = "1"; // auto
                    break;
                case "bookingPurpose":
                    dt4.Rows[0]["bookingPurpose"] = "Lesson";
                    break;
                case "bookingRemarks":
                    dt4.Rows[0]["bookingRemarks"] = "The booking is ...";
                    break;
                // aci_user
                case "userId":
                    dt4.Rows[0]["userId"] = "1";
                    break;
                case "userName":
                    dt4.Rows[0]["userName"] = "Helen Hart";
                    break;
                case "userEmail":
                    dt4.Rows[0]["userEmail"] = "hhart11@bing.com";
                    break;
                case "userAuthorization":
                    dt4.Rows[0]["userAuthorization"] = "Y/N";
                    break;
                // aci_role , aci_user_role
                case "roleId":
                    dt4.Rows[0]["roleId"] = "1";
                    break;
                case "roleTitle":
                    dt4.Rows[0]["roleTitle"] = "System Admin/Front Desk/Program Admin";
                    break;
                case "roleDescription":
                    dt4.Rows[0]["roleDescription"] = "System Admin/Front Desk/Program Admin";
                    break;
                case "roleLevel":
                    dt4.Rows[0]["roleLevel"] = "1";
                    break;
                // code reference
                case "codeType":
                    dt4.Rows[0]["codeType"] = "EDU";
                    break;
                case "codeDesc":
                    dt4.Rows[0]["codeDesc"] = "Education Level";
                    break;
                case "codeValue":
                    dt4.Rows[0]["codeValue"] = "PRI";
                    break;
                case "codeDisplay":
                    dt4.Rows[0]["codeDisplay"] = "PRIMARY";
                    break;
                case "codeOrder":
                    dt4.Rows[0]["codeOrder"] = "1";
                    break;
                case "codeRemarks":
                    dt4.Rows[0]["codeRemarks"] = "This code is refers to ...";
                    break;
                case "codeEffectDate":
                    //    dt = DateTime.ParseExact("25/11/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt4.Rows[0]["codeEffectDate"] = "25/11/2015";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["codeEffectDate"] = DateTime.Parse(dr["codeEffectDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "batchModuleId":
                    dt4.Rows[0]["batchModuleId"] = "1";
                    break;
                case "AbsentRemarks":
                    dt4.Rows[0]["AbsentRemarks"] = "The absent record ...";
                    break;
                case "insertedbatchModuleId":
                    dt4.Rows[0]["insertedbatchModuleId"] = "1";
                    break;
                case "insertedSessionId":
                    dt4.Rows[0]["insertedSessionId"] = "1";
                    break;

                case "EnrolDate":
                    dt4.Rows[0]["EnrolDate"] = "15/1/2017";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["EnrolDate"] = DateTime.Parse(dr["EnrolDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "programmePayableAmount":
                    dt4.Rows[0]["programmePayableAmount"] = "50.50";
                    break;
                case "programmeCancelledDate":
                    dt4.Rows[0]["programmeCancelledDate"] = "19/9/2018";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["programmeCancelledDate"] = DateTime.Parse(dr["programmeCancelledDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "traineeProgrammeRemarks":
                    dt4.Rows[0]["traineeProgrammeRemarks"] = "This trainee programme remark ...";
                    break;
                case "programmeWithdrawnDate":
                    dt4.Rows[0]["programmeWithdrawnDate"] = "28/11/2018";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["programmeWithdrawnDate"] = DateTime.Parse(dr["programmeWithdrawnDate"].ToString()).ToShortDateString();
                    }
                    break;
                case "programmeWithdrawnRemarks":
                    dt4.Rows[0]["programmeWithdrawnRemarks"] = "The trainee withdrawn because ...";
                    break;

                // aci_access_rights
                case "accessId":
                    dt4.Rows[0]["accessId"] = "1";
                    break;
                case "functionId":
                    dt4.Rows[0]["FunctionId"] = "1";
                    break;
                case "functionName":
                    dt4.Rows[0]["functionName"] = "Create Programme";
                    break;
                case "functionGrp":
                    dt4.Rows[0]["functionGrp"] = "Create";
                    break;
                case "functionDesc":
                    dt4.Rows[0]["functionDesc"] = "This function is to allow IT admin to create programme";
                    break;
                case "bundleModuleId":
                    dt4.Rows[0]["bundleModuleId"] = "1";
                    break;
                case "bundleType":
                    dt4.Rows[0]["bundleType"] = "A";
                    break;
                case "bundleEffectDate":
                    dt4.Rows[0]["bundleEffectDate"] = "15/02/2016";
                    foreach (DataRow dr in dt4.Rows)
                    {
                        dr["bundleEffectDate"] = DateTime.Parse(dr["bundleEffectDate"].ToString()).ToShortDateString();
                    }
                    break;


                default:
                    break;
            }
        }
        // clone to correct table
        public DataTable CloneDataTable(DataTable dtOldTable, string dsName, string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable PreCreated = new DataTable();
            PreCreated = dm.GetPreDefineTable(dsName, tableName);

            // remove the lastModifiedDate
            DataColumnCollection columns = PreCreated.Columns;
            if (columns.Contains("lastModifiedDate"))
            {
                PreCreated.Columns.Remove("lastModifiedDate");
                PreCreated.AcceptChanges();
            }

            List<string> listOfString = new List<string>();
            listOfString = dm.getListofCertainDataTypeColumns(PreCreated, "System.String");
            //List<string> nonNullList = new List<string>();
            List<string> listOfDecimal = new List<string>();
            listOfDecimal = dm.getListofCertainDataTypeColumns(PreCreated, "System.Decimal");
            List<string> listOfDate = new List<string>();
            listOfDate = dm.getListofCertainDataTypeColumns(PreCreated, "System.DateTime");
            List<string> listOfInt = new List<string>();
            listOfInt = dm.getListofCertainDataTypeColumns(PreCreated, "System.Int32");

            DataTable dtClone = new DataTable();
            dtClone = dtOldTable.Clone(); //just copy structure, no data
            for (int i = 0; i < dtClone.Columns.Count; i++)
            {

                // Convert list of string
                string col = dtClone.Columns[i].ToString();
                for (int k = 0; k < listOfString.Count; k++)
                {
                    string listItem = listOfString[k];
                    if (col.Equals(listItem))
                    {
                        if (dtClone.Columns[i].DataType != typeof(string))
                        {
                            dtClone.Columns[i].DataType = typeof(string);
                        }
                    }

                }
                // Convert list of Decimal
                for (int k = 0; k < listOfDecimal.Count; k++)
                {
                    string listItem = listOfDecimal[k];
                    if (col.Equals(listItem))
                    {
                        if (dtClone.Columns[i].DataType != typeof(decimal))
                        {
                            dtClone.Columns[i].DataType = typeof(decimal);
                        }
                    }

                }
                // Convert list of DateTime
                for (int k = 0; k < listOfDate.Count; k++)
                {
                    string listItem = listOfDate[k];
                    if (col.Equals(listItem))
                    {
                        if (dtClone.Columns[i].DataType != typeof(DateTime))
                        {
                            dtClone.Columns[i].DataType = typeof(DateTime);
                        }
                    }

                }
                // Convert list of Int
                for (int k = 0; k < listOfInt.Count; k++)
                {
                    string listItem = listOfInt[k];
                    if (col.Equals(listItem))
                    {
                        if (dtClone.Columns[i].DataType != typeof(Int32))
                        {
                            dtClone.Columns[i].DataType = typeof(Int32);
                        }
                    }

                }





            }


            foreach (DataRow dr in dtOldTable.Rows)
            {
                // cannot import rows with invalid datatype
                dtClone.ImportRow(dr);
            }

            return dtClone;
        }
        public DataTable CloneToString(DataTable dtOldTable)
        {

            DataTable dtCloneString = new DataTable();
            dtCloneString = dtOldTable.Clone(); //just copy structure, no data
            for (int i = 0; i < dtCloneString.Columns.Count; i++)
            {

                // Convert list of string
                string col = dtCloneString.Columns[i].ToString();



                if (dtCloneString.Columns[i].DataType != typeof(string))
                {

                    dtCloneString.Columns[i].DataType = typeof(string);
                }


            }
            foreach (DataRow dr in dtOldTable.Rows)
            {
                // cannot import rows with invalid datatype
                dtCloneString.ImportRow(dr);
            }
            return dtCloneString;

        }

        public bool IsValidDateTimeTest(string dateTime)
        {
            string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/M/yyyy", 
                                   
                             "dd/MM/yyyy hh:mm:ss tt", "d/M/yyyy hh:mm:ss tt", "d/MM/yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt" };



            DateTime parsedDateTime;
            bool parsed = DateTime.TryParseExact(dateTime.Trim(), formats, new CultureInfo("en-SG"),
                                           DateTimeStyles.None, out parsedDateTime);
            return parsed;
        }



        public bool isValidDecimal(string decValue)
        {

            decimal d;
            if (decimal.TryParse(decValue, out d))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool isValidInteger(string IntValue)
        {

            int i;
            if (int.TryParse(IntValue, out i))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ConvertToDate(string datatime)
        {
            DateTime datet = DateTime.ParseExact(datatime.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

            string s = datet.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        // SG NRIC Validation
        public bool isValidSgFin(string strValueToCheck)
        {
            strValueToCheck = strValueToCheck.Trim();

            Regex objRegex = new Regex("^(s|t)[0-9]{7}[a-jz]{1}$", RegexOptions.IgnoreCase);

            if (!objRegex.IsMatch(strValueToCheck))
            {
                return false;
            }

            string strNums = strValueToCheck.Substring(1, 7);

            int intSum = 0;
            int checkDigit = 0;
            string checkChar = "";
            intSum = Convert.ToUInt16(strNums.Substring(0, 1)) * 2;
            intSum = intSum + (Convert.ToUInt16(strNums.Substring(1, 1)) * 7);
            intSum = intSum + (Convert.ToUInt16(strNums.Substring(2, 1)) * 6);
            intSum = intSum + (Convert.ToUInt16(strNums.Substring(3, 1)) * 5);
            intSum = intSum + (Convert.ToUInt16(strNums.Substring(4, 1)) * 4);
            intSum = intSum + (Convert.ToUInt16(strNums.Substring(5, 1)) * 3);
            intSum = intSum + (Convert.ToUInt16(strNums.Substring(6, 1)) * 2);

            if (strValueToCheck.Substring(0, 1).ToLower() == "t")
            {
                //prefix T
                intSum = intSum + 4;
            }

            checkDigit = 11 - (intSum % 11);

            checkChar = strValueToCheck.Substring(8, 1).ToLower();

            if (checkDigit == 1 && checkChar == "a")
            {
                return true;
            }
            else if (checkDigit == 2 && checkChar == "b")
            {
                return true;
            }
            else if (checkDigit == 3 && checkChar == "c")
            {
                return true;
            }
            else if (checkDigit == 4 && checkChar == "d")
            {
                return true;
            }
            else if (checkDigit == 5 && checkChar == "e")
            {
                return true;
            }
            else if (checkDigit == 6 && checkChar == "f")
            {
                return true;
            }
            else if (checkDigit == 7 && checkChar == "g")
            {
                return true;
            }
            else if (checkDigit == 8 && checkChar == "h")
            {
                return true;
            }
            else if (checkDigit == 9 && checkChar == "i")
            {
                return true;
            }
            else if (checkDigit == 10 && checkChar == "z")
            {
                return true;
            }
            else if (checkDigit == 11 && checkChar == "j")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public void bulkUpdat(string applicantId)
        //{

        //    try
        //    {
        //        string sqlStatement = "MERGE INTO dbo.payment_history AS C USING dbo.tbl_CustomersTemp AS CT ON C.CustID = CT.CustID WHEN MATCHED THEN UPDATE SET C.CompanyName = CT.CompanyName, C.Phone = CT.Phone WHEN NOT MATCHED THEN  INSERT (CustID, CompanyName, Phone) VALUES (CT.CustID, CT.CompanyName, CT.Phone)";

        //        SqlCommand cmd = new SqlCommand(sqlStatement);
        //        cmd.Parameters.AddWithValue("@applicantId", applicantId);
        //        DataTable dtPaymentHistory = dbConnection.getDataTable(cmd);


        //    }
        //    catch (Exception errorMsg)
        //    {

        //    }





        //}
        public string getCategoryDescription(string selectedItem)
        {

            string description = "";

            if (selectedItem == "Show All")
            {
                description = "The category shows all the tables in the database";
            }
            if (selectedItem == "Applicant")
            {
                description = "The category contains all the information related to the applicant. Information includes: applicant's personal information, interview result, employment history and payment history.";

            }
            if (selectedItem == "Bundle")
            {

                description = "The category contains information related to the bundle of the various programmes. ";
            }
            if (selectedItem == "Programme")
            {
                description = "The category contains information related to the programme. Information includes: programme batch, programme structure, programme subsidy etc.";
            }
            if (selectedItem == "Module")
            {
                description = "The category contains information related to the module. Information includes: module of the different batch, module session and trainee information etc";
            }
            if (selectedItem == "Batch")
            {
                description = "The category contains information related to all the different batch. Information includes: programme batch and modules of the different batch of trainee.";
            }
            if (selectedItem == "Trainee")
            {
                description = "The category contains all the information related to the trainee. Information includes: trainee's personal information, trainee's programme, module and their attendance record etc.";
            }
            if (selectedItem == "User")
            {
                description = "The catogory contains information related to the aci staff user.";
            }

            return description;

        }
        public string getTableDescription(string selectedTable)
        {
            string lblDivText = "";
            if (selectedTable == "aci access rights")
            {
                //DivTableDescription.Visible = true;
                lblDivText = "This table contains all the access rights to programs for users.";
            }
            if (selectedTable == "aci role")
            {
                //DivTableDescription.Visible = true;
                lblDivText = "This table contains all the user role in the system.";
            }
            if (selectedTable == "aci functions")
            {
                lblDivText = "This table contains all the functions in the application.";


            }
            if (selectedTable == "aci suspended list")
            {
                lblDivText = "This table contains all the personal information of the suspended applicant.";


            }
            if (selectedTable == "aci user")
            {
                lblDivText = "This table contains all the ACI staff login details.";


            }
            if (selectedTable == "applicant")
            {
                lblDivText = "The table contains all the details of the applicant that show interest in ACI programmes.";


            }
            if (selectedTable == "applicant interview result")
            {
                lblDivText = "This table contains all the interview details and result of the all applicants.";


            }
            if (selectedTable == "batch module session")
            {
                lblDivText = "This table describes the session of each module of the batch.";


            }
            if (selectedTable == "batch module")
            {
                lblDivText = "This table describes the modules that each batch runs.";

            }
            if (selectedTable == "bundle")
            {
                lblDivText = "This table describes all the modules which are mapped to each programme.";


            }
            if (selectedTable == "code reference")
            {
                lblDivText = "This table contains all the reference of the codes used in the database.";


            }
            if (selectedTable == "code reference old")
            {
                lblDivText = "This table contains all the different roles of ACI users (OLD)";

            }
            if (selectedTable == "employment history")
            {
                lblDivText = "This table contains all the passed employment details for each applicant or trainee.";


            }
            if (selectedTable == "module structure")
            {
                lblDivText = "This table describes all the modules details(including decommissioned ones) of each programme.";

            }
            if (selectedTable == "module structure log")
            {
                lblDivText = "This table is for audit trait purpose.";

            }
            if (selectedTable == "payment history")
            {
                lblDivText = "This table contains all the payment transaction details for each applicant or trainee.";


            }
            if (selectedTable == "payment details")
            {
                lblDivText = "This table contains how each payment is link to other records";

            }
            if (selectedTable == "programme batch")
            {
                lblDivText = "This table describe the details of each programme structure(qualification) that ACI conduct per batch.";


            }
            if (selectedTable == "programme structure")
            {
                lblDivText = "This table contains the details of the programme structure(qualification).";


            }
            if (selectedTable == "programme structure log")
            {
                lblDivText = "This table is for audit trait purpose.";


            }
            if (selectedTable == "refund history")
            {
                lblDivText = "This table contains all refund transaction details.";


            }
            if (selectedTable == "trainee")
            {
                lblDivText = "This table contains details of the trainee who had successfully enrolled for a programme.";


            }
            if (selectedTable == "trainee absence record")
            {
                lblDivText = "This table contains all the absence record for each trainee.";


            }
            if (selectedTable == "trainee absence record log")
            {
                lblDivText = "This table is for audit trait purpose.";

            }
            if (selectedTable == "trainee absence removed")
            {
                lblDivText = "This table contains all the absence record for each trainee once trainee has attended the makeup lesson.";


            }
            if (selectedTable == "trainee absence removed log")
            {
                lblDivText = "This table is for audit trait purpose.";

            }
            if (selectedTable == "trainee module")
            {
                lblDivText = "This table contains all the modules that the trainee taking(including exempted ones) for each programme and his/her result.";


            }
            if (selectedTable == "trainee module log")
            {
                lblDivText = "This table is for audit trait purpose.";


            }
            if (selectedTable == "trainee programme")
            {
                lblDivText = "This table contains the programme that the trainee has registered. Each trainee may attend more than one programme.";


            }
            if (selectedTable == "venue")
            {
                lblDivText = "This table contains all the venue used by ACI including decommissioned venues.";


            }
            if (selectedTable == "venue log")
            {
                lblDivText = "This table is for audit trait purpose.";


            }
            if (selectedTable == "venue booking record")
            {
                lblDivText = "This table contains all the record of the booked/blocked venue(for session) including external events.";


            }
            if (selectedTable == "venue booking record log")
            {
                lblDivText = "This table is for audit trait purpose.";


            }
            // new tables
            if (selectedTable == "registration fee")
            {

                lblDivText = "This table contains the registration fee changes over the time ";
            }
            //if (selectedTable == "programme subsidy")
            //{
            //    lblDivText = "This table contains all the different programme subsidy rate";


            //}

            if (selectedTable == "subsidy")
            {
                lblDivText = "This table contains all the different programme subsidy rate/amount for different scheme";


            }
            if (selectedTable == "programme subsidy scheme")
            {
                lblDivText = "This table contains all the different programme subsidy scheme in ACI";


            }
            if (selectedTable == "case logging")
            {

                lblDivText = "This table contains all the case logging/feedback of the system";


            }
            if (selectedTable == "case logging category")
            {

                lblDivText = "This table contains all the case logging category settings";


            }
            if (selectedTable == "aci access rights")
            {

                lblDivText = "This table contains all the access rights to programmes for users";
            }
            if (selectedTable == "aci user role")
            {
                lblDivText = "This table contains all the different roles of ACI users";

            }
            if (selectedTable == "batchModule session")
            {
                lblDivText = "This table contains all the session of each module of the batch";

            }
            if (selectedTable == "bundle module")
            {
                lblDivText = "This table contains all the modules which are mapped to each bundle";

            }


            return lblDivText;
        }
        public void convertDateTimeRowToString(DataTable dtInString, List<string> dateList)
        {
            foreach (DataColumn dc in dtInString.Columns)
            {

                for (int k = 0; k < dateList.Count; k++)
                {
                    if (dc.ColumnName.Equals(dateList[k].ToString()))
                    {
                        foreach (DataRow dr in dtInString.Rows)
                        {

                            // convert datatime format to short date string(excel show time value, so remove it)
                            string date = DateTime.Parse(dr[dc.ColumnName].ToString()).ToShortDateString();
                            dr[dc.ColumnName] = date;


                        }
                    }
                }

            }
        }
        public void convertDateTimeToMDY(DataTable dtInString, List<string> dateList)
        {
            foreach (DataColumn dc in dtInString.Columns)
            {

                for (int k = 0; k < dateList.Count; k++)
                {
                    if (dc.ColumnName.Equals(dateList[k].ToString()))
                    {
                        foreach (DataRow dr in dtInString.Rows)
                        {

                            // convert datatime format to short date string(excel show time value, so remove it)
                            string cell = dr[dc.ColumnName].ToString();
                            if (!(cell.Equals("")))
                            {
                                DateTime date = Convert.ToDateTime(cell);
                                string date2 = date.ToString("MM/dd/yyyy");

                                dr[dc.ColumnName] = date2;
                            }

                        }
                    }
                }

            }
        }

        public int updateExistingRecord(List<string> ListOfpks, DataTable DupRecord, string tableName, DataTable currentDbRecord, DataTable newExcelRecord, int userId)
        {


            DB_DataAnalytics dm = new DB_DataAnalytics();
            List<string> datelist = new List<string>();

            datelist = dm.getListofCertainDataTypeColumns(currentDbRecord, "System.DateTime");
            DataTable cloneCurrentExcelRecordToString = dm.CloneToString(newExcelRecord);
            // copy of a table that still keep the correct datatype
            DataTable copyNewExcelRecord = new DataTable();
            copyNewExcelRecord = newExcelRecord.Copy();
            // convert the datatime format into MM/dd/yyyy
            //     dm.convertDateTimeRowToString(cloneCurrentRecordToString, datelist);
            //      DataTable cloneDupRecord = dm.CloneToString(DupRecord);
            //     dm.convertDateTimeToMDY(cloneDupRecord,datelist);


            dm.convertDateTimeToMDY(cloneCurrentExcelRecordToString, datelist);
            DataTable dtRemovePkExcelRecord = cloneCurrentExcelRecordToString.Copy();
            bool success = true;
            int countSuccess = 0;

            for (int k = 0; k < ListOfpks.Count; k++)
            {
                foreach (DataColumn dc in cloneCurrentExcelRecordToString.Columns)
                {
                    if (dc.ColumnName == ListOfpks[k].ToString())
                    {
                        // remove the pk columns, so no pk columns is set in the update statement
                        dtRemovePkExcelRecord.Columns.Remove(dc.ColumnName);
                        copyNewExcelRecord.Columns.Remove(dc.ColumnName);
                        dtRemovePkExcelRecord.AcceptChanges();
                        copyNewExcelRecord.AcceptChanges();
                    }


                }
            }


            try
            {
                for (int n = 0; n < DupRecord.Rows.Count; n++)
                {


                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb2 = new StringBuilder();
                    // building of the columns = values

                    for (int r = 0; r < dtRemovePkExcelRecord.Columns.Count; r++)
                    {

                        string columns = dtRemovePkExcelRecord.Columns[r].ToString();
                        string values = dtRemovePkExcelRecord.Rows[n][dtRemovePkExcelRecord.Columns[r]].ToString();
                        // get the datatype of the columns
                        string datatype = copyNewExcelRecord.Columns[r].DataType.ToString();

                        // if the column datatype is a number, set to default 0 --> if not update null value will have error 
                        if (datatype == "System.Decimal" || datatype == "System.Int32" || datatype == "System.Double")
                        {
                            if (values == "")
                            {
                                values = "0";
                            }
                        }
                        //if(dtRemovePkExcelRecord.Columns[r].DataType = Decimal || dtRemovePkExcelRecord.Columns[r].DataType = "int" ||dtRemovePkExcelRecord.Columns[r].DataType = double )
                        if (r != dtRemovePkExcelRecord.Columns.Count - 1)
                        {
                            //add the single quote because some data contains comma add a single quote to separate it from query comma
                            sb.Append(columns + " = '" + values + "', ");

                        }
                        else
                        {

                            // sb.Append(dtRemovePkExcelRecord.Columns[k] + " = " + dtRemovePkExcelRecord.Rows[rowNum][dtRemovePkExcelRecord.Columns[k]]);
                            sb.Append(columns + " = '" + values + "'");
                        }

                    }

                    string sql1 = sb.ToString();
                    // buliding of the WHERE id = values;

                    for (int c = 0; c < ListOfpks.Count; c++)
                    {


                        // when it only have one pk
                        if (ListOfpks.Count == 1)
                        {
                            sb2.Append(ListOfpks[c].ToString() + " = '" + DupRecord.Rows[n][ListOfpks[c].ToString()] + "'");
                        }
                        if (ListOfpks.Count > 1)
                        {
                            // when is not the last one
                            if (c != ListOfpks.Count - 1)
                            {
                                sb2.Append(ListOfpks[c].ToString() + " = '" + DupRecord.Rows[n][ListOfpks[c].ToString()] + "' and ");
                            }
                            else
                            // if is the last one
                            {
                                sb2.Append(ListOfpks[c].ToString() + " = '" + DupRecord.Rows[n][ListOfpks[c].ToString()] + "'");
                            }

                        }



                    }
                    string sql2 = sb2.ToString();


                    string sqlStatement = @"DECLARE @CONTEXT_TEST int;
                                                      DECLARE @CONTEXT VARBINARY(1); 
                                                      SET @CONTEXT_TEST = @userId
                                                      SET @CONTEXT = CAST (@CONTEXT_TEST AS varbinary(1));
                                                      SET CONTEXT_INFO @CONTEXT;
                                                      UPDATE " + tableName + " SET " + sb + " WHERE " + sb2;



                    SqlCommand cmd = new SqlCommand(sqlStatement);
                    cmd.Parameters.Add("@userId", userId);


                    try
                    {

                        success = dbConnection.executeNonQuery(cmd);

                    }
                    catch (Exception e)
                    {
                        string error = e.ToString();


                    }
                    if (success == true)
                    {
                        countSuccess++;
                    }
                    if (success == false)
                    {

                        int a = n;
                    }



                }


                if (countSuccess == DupRecord.Rows.Count)
                {
                    // if all the records are updated correctly
                    success = true;
                }
                else
                {
                    // db problem
                    success = false;

                }

                if (success == true)
                {
                    return countSuccess;
                }
            }
            catch (Exception errorMsg)
            {
                errorMsg.ToString();
            }
            return countSuccess;
        }


        public bool insertBulkRecord(string tableAffected, int userId)
        {
            bool success;
            string sqlStatement = "INSERT INTO [tmsdb].[dbo].[bulk_insert] (userId,tableAffected,lastModifiedDate) VALUES (" + userId + ", '" + tableAffected + "'," + "GETDATE())";



            SqlCommand cmd = new SqlCommand(sqlStatement);
            try
            {
                success = dbConnection.executeNonQuery(cmd);
            }
            catch (Exception e)
            {
                success = false;
            }
            return success;


        }
        public DataTable getGroupTables(string aciGrp)
        {

            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("table", typeof(string));
            dt.Columns.Add(dc);
            DataColumn dc1 = new DataColumn("table2", typeof(string));
            dt.Columns.Add(dc1);
            //    DataRow dr = dt.NewRow();
            if (aciGrp == "Front Desk")
            {
                dt.Rows.Add("applicant");
                dt.Rows.Add("aci suspended list");
                dt.Rows.Add("trainee");
                dt.Rows.Add("applicant interview result");
                dt.Rows.Add("employment history");
                //      dt.Rows.Add("registration fee");
            }
            if (aciGrp == "Programme")
            {
                dt.Rows.Add("programme structure");
                dt.Rows.Add("module structure");
                dt.Rows.Add("bundle");
                dt.Rows.Add("programme batch");
                dt.Rows.Add("batch module");
                dt.Rows.Add("batch module session");
                //     dt.Rows.Add("programme subsidy");
                dt.Rows.Add("programme subsidy scheme");
                dt.Rows.Add("programme subsidy value");

                dt.Rows[0]["table2"] = "trainee";
                dt.Rows[1]["table2"] = "trainee programme";
                dt.Rows[2]["table2"] = "trainee module";
                dt.Rows[3]["table2"] = "trainee absence record";
                dt.Rows[4]["table2"] = "trainee absence removed";
                dt.Rows[5]["table2"] = "venue";
                dt.Rows[6]["table2"] = "venue booking record";
            }

            if (aciGrp == "Finance")
            {
                dt.Rows.Add("applicant");
                dt.Rows.Add("payment history");
                dt.Rows.Add("registration fee");
                dt.Rows.Add("trainee programme");
                dt.Rows.Add("programme subsidy scheme");
                dt.Rows.Add("programme subsidy value");
            }
            if (aciGrp == "IT")
            {
                dt.Rows.Add("applicant");
                dt.Rows.Add("case logging");
                dt.Rows.Add("case logging category");
                // some unsure tables 
                //            dt.Rows.Add("aci functions");
                //            dt.Rows.Add("aci access rights");
                //            dt.Rows.Add("aci user");
                //            dt.Rows.Add("aci user role");
            }
            if (aciGrp == "Business Development")
            {
                dt.Rows.Add("applicant");
                dt.Rows.Add("aci suspended list");
                dt.Rows.Add("trainee");
                dt.Rows.Add("applicant interview result");
                dt.Rows.Add("employment history");
                dt.Rows.Add("programme structure");
                dt.Rows.Add("module structure");
                dt.Rows.Add("bundle");
                dt.Rows.Add("programme batch");
                dt.Rows.Add("batch module");
                // table2
                dt.Rows[0]["table2"] = "batch module session";
                dt.Rows[1]["table2"] = "programme subsidy scheme";
                dt.Rows[2]["table2"] = "programme subsidy value";
                dt.Rows[3]["table2"] = "trainee programme";
                dt.Rows[4]["table2"] = "trainee module";
                dt.Rows[5]["table2"] = "trainee absence record";
                dt.Rows[6]["table2"] = "trainee absence removed";
                dt.Rows[7]["table2"] = "venue";
                dt.Rows[8]["table2"] = "venue booking record";
                //dt.Rows.Add("batch module session");
                //dt.Rows.Add("programme subsidy scheme");
                //dt.Rows.Add("programme subsidy value");
                //dt.Rows.Add("trainee programme");
                //dt.Rows.Add("trainee module");
                //dt.Rows.Add("trainee absence record");
                //dt.Rows.Add("trainee absence removed");
                //dt.Rows.Add("venue");
                //dt.Rows.Add("venue booking record");
            }
            if (aciGrp == "Directorate")
            {
                //first table
                dt.Rows.Add("applicant");
                dt.Rows.Add("aci access rights");
                dt.Rows.Add("aci functions");
                dt.Rows.Add("aci role");
                dt.Rows.Add("aci user");
                dt.Rows.Add("aci user role");
            

                dt.Rows.Add("batch module");
                dt.Rows.Add("batchModule session");
                dt.Rows.Add("bundle");
                dt.Rows.Add("bundle module");

                dt.Rows.Add("employment history");

                dt.Rows.Add("module structure");
    
                dt.Rows.Add("payment details");
                dt.Rows.Add("payment history");
                dt.Rows.Add("programme batch");



                //second table
                dt.Rows[0]["table2"] = "programme structure";
         


                dt.Rows[1]["table2"] = "registration fee";
                dt.Rows[2]["table2"] = "subsidy";

                dt.Rows[3]["table2"] = "trainee";
                dt.Rows[4]["table2"] = "trainee module";
                dt.Rows[5]["table2"] = "trainee programme";

                dt.Rows[6]["table2"] = "venue";
                dt.Rows[7]["table2"] = "venue booking record";
            }

           
            return dt;
        }

        public string getDepartDes(string aciGrp)
        {
            string description = "";
            if (aciGrp.Equals("Front Desk"))
            {
                description = "Front Desk department is responsibile for all the information that's related to the applicant";


            }
            if (aciGrp.Equals("Programme"))
            {
                description = "Programme department is responsible for all the programme carry out in the ACI";

            }
            if (aciGrp.Equals("Finance"))
            {
                description = "Finance department is responsible for all the financial activities/transactions occured in the ACI";

            }

            if (aciGrp.Equals("IT"))
            {
                description = "IT department is responsible for all the IT related issues/problems occurs in the system";
            }
            if (aciGrp.Equals("Business Development"))
            {
                description = "Business Development department is responsible in making decision for business in ACI";
            }
            if (aciGrp.Equals("Directorate"))
            {
                description = "Directorate department is able to view all the information related to all the various department";
            }
            return description;
        }




    }



    // extention class to get the number of rows inserted in bulk copy

    public static class SqlBulkCopyExtension
    {
        const String _rowsCopiedFieldName = "_rowsCopied";
        static FieldInfo _rowsCopiedField = null;

        public static int RowsCopiedCount(this SqlBulkCopy bulkCopy)
        {
            if (_rowsCopiedField == null) _rowsCopiedField = typeof(SqlBulkCopy).GetField(_rowsCopiedFieldName, BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            return (int)_rowsCopiedField.GetValue(bulkCopy);
        }


    }
}