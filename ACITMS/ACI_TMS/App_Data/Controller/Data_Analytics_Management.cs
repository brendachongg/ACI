using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Data;
using System.Web.UI.WebControls;

namespace LogicLayer
{
    public class Data_Analytics_Management
    {
        // filled dataTable with data in csv file
        public DataTable getDataTableFromCsv(string csv_path)
        {
            DB_DataAnalytics data = new DB_DataAnalytics();
            DataTable Dt = data.GetDataTableFromCSVFile(csv_path);
            return Dt;
        }

        // mapping of the colunms between csv and table in database
        public void DataMapping(DataTable csvData, string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();

            dm.copyTable(csvData, tableName);


        }
        // get all the table name from database
        public DataTable getAllTableName()
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getAllDatabaseTableNamedt();
            return dt;
        }

        // get columns by the table name
        public DataTable getColumnByTableName(string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getColumnByTableName(tableName);
            return dt;
        }
        public DataTable getDataByTableName(string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getDataByTableName(tableName);
            return dt;
        }

        public DataTable getCurrentPkRowsInDbByTableName(string tableName, List<string> listOfPks)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getCurrentPkRowsInDbByTableName(tableName, listOfPks);
            return dt;

        }
        public DataTable getCurrentDbRecord(string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getCurrentDbRecord(tableName);
            return dt;

        }

        public DataTable getColumnBySheet(string sheetName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getColumnBySheet(sheetName);
            return dt;

        }
        // columns containing the keyword
        public DataTable getSpecificTableColumns(string columnName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getSpecificTableColumns(columnName);
            return dt;
        }


        public DataTable getRelatedTablebyKeys(DataTable d)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getRelatedTablebyKeys(d);
            return dt;
        }
        public DataTable getPKbyTable(string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.getPKbyTable(tableName);
            return dt;
        }

        //public DataTable getDataTypeByTable(string tableName)
        //{
        //    DB_DataAnalytics dm = new DB_DataAnalytics();
        //    DataTable dt = new DataTable();
        //    dt = dm.getDataTypeByTable(tableName);
        //    return dt;

        //}

        public void insertLastModifiedDate(DataTable dtTable)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            dm.insertLastModifiedDate(dtTable);

        }
        public DataTable FillDataTableWithExcelData(string sheet, string extension, string excelPath)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();

            dt = dm.FillDataTableWithExcelData(sheet, extension, excelPath);
            return dt;
        }
        public DataTable FillSampleRow(string sheet, string extension, string excelPath)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();

            dt = dm.FillSampleRow(sheet, extension, excelPath);
            return dt;

        }
        public DataTable FindDuplicatePkRows(string sheet, string extension, string excelPath, List<string> PKList, DataTable pkRowDatas)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();

            dt = dm.FindDuplicatePkRows(sheet, extension, excelPath, PKList, pkRowDatas);
            return dt;

        }
        public int CompareRowsGetRowIndex(DataTable table1, DataTable table2)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            int num = dm.CompareRowsGetRowIndex(table1, table2);
            return num;


        }

        public List<int> CompareRowsGetListOfRowIndex(DataTable table1, DataTable table2)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            List<int> indexList = new List<int>();
            indexList = dm.CompareRowsGetListOfRowIndex(table1, table2);
            return indexList;


        }

        public DataTable FillDataTableWithExcelPKcols(string sheet, string extension, string excelPath, List<string> pkCols)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = dm.FillDataTableWithExcelPKcols(sheet, extension, excelPath, pkCols);
            return dt;
        }



        //public string getDuplicateMessage(DataTable dtDuplicatePkRows, string tableName)
        //{
        //    DB_DataAnalytics dm = new DB_DataAnalytics();
        //    DataTable dt = new DataTable();
        //    string msg = dm.getDuplicateMessage(dtDuplicatePkRows, tableName);
        //    return msg;

        //}
        public DataTable GetExcelSheetName(string extension, string excelPath)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.GetExcelSheetName(extension, excelPath);
            return dt;

        }
        public DataTable InsertExcelData(DataSet dsAllData, int userId)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dtRecord = dm.InsertExcelData(dsAllData, userId);
            return dtRecord;

        }

        // not in use, all change to try parse
        //public bool ValidateDataType(DataTable tempdt, string CheckDatecolumnName, string dataType)
        //{
        //    bool validationStatus = true;
        //    DB_DataAnalytics dm = new DB_DataAnalytics();
        //    validationStatus = dm.ValidateDataType(tempdt, CheckDatecolumnName, dataType);
        //    return validationStatus;


        //}

        public List<string> getListofCertainDataTypeColumns(DataTable TempDataTable, string dataType)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            List<string> list = new List<string>();
            list = dm.getListofCertainDataTypeColumns(TempDataTable, dataType);
            return list;

        }

        public StringBuilder GetErrorListMsg(List<string> ErrorList)
        {
            StringBuilder sbMsg = new StringBuilder();
            DB_DataAnalytics dm = new DB_DataAnalytics();
            sbMsg = dm.GetErrorListMsg(ErrorList);
            return sbMsg;
        }

        public List<string> getNonNullableColumn(DataTable PreDataTable)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            List<string> list = new List<string>();
            list = dm.getNonNullableColumn(PreDataTable);
            return list;
        }

        public List<string> getNullableColumn(DataTable PreDataTable)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            List<string> list = new List<string>();
            list = dm.getNullableColumn(PreDataTable);
            return list;
        }

        public StringBuilder validateNull(DataTable dtTable, List<string> listOfnonNullable, string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            StringBuilder sb = new StringBuilder();
            sb = dm.validateNull(dtTable, listOfnonNullable, tableName);
            return sb;
        }

        //public DataTable getPreTraineeModuleTable()
        //{
        //    DB_DataAnalytics dm = new DB_DataAnalytics();
        //    DataTable dt = new DataTable();
        //    dt = dm.getPreTraineeModuleTable();
        //    return dt;

        //}

        //public DataTable getPrePaymentHistoryTable()
        //{
        //    DB_DataAnalytics dm = new DB_DataAnalytics();
        //    DataTable dt = new DataTable();
        //    dt = dm.getPrePaymentHistoryTable();
        //    return dt;

        //}
        //public DataTable getPreTraineeCourseTable()
        //{
        //    DB_DataAnalytics dm = new DB_DataAnalytics();
        //    DataTable dt = new DataTable();
        //    dt = dm.getPreTraineeCourseTable();
        //    return dt;

        //}
        public DataTable GetPreDefineTable(string dataSetName, string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = new DataTable();
            dt = dm.GetPreDefineTable(dataSetName, tableName);
            return dt;
        }
        // no in use 
        //public List<string> getListOfDatatypeErrors(DataTable dtActualTable, DataTable tempDataTable, string tableName, string extension, string excelPath)
        // {
        //    DB_DataAnalytics dm = new DB_DataAnalytics();
        //    List<string> listofDatatypeError = new List<string>();
        //    listofDatatypeError = dm.getListOfDatatypeErrors(dtActualTable, tempDataTable, tableName, extension, excelPath);
        //    return listofDatatypeError;

        //}
        public DataTable getPrecisonScaleByColumns(List<string> columnNameList, string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dtPrecision = dm.getPrecisonScaleByColumns(columnNameList, tableName);
            return dtPrecision;
        }

        public void getColumnExampleVer1(DataColumn dc, DataTable dt4)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            dm.getColumnExampleVer1(dc, dt4);

        }
        public void getColumnExampleVer2(DataColumn dc, DataTable dt4)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            dm.getColumnExampleVer2(dc, dt4);

        }
        public DataTable CloneDataTable(DataTable dtOldTable, string dsName, string tableName)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable clone = new DataTable();
            clone = dm.CloneDataTable(dtOldTable, dsName, tableName);
            return clone;


        }
        public DataTable CloneToString(DataTable dtOldTable)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable clone = new DataTable();
            clone = dm.CloneToString(dtOldTable);
            return clone;

        }
        public bool IsValidDateTimeTest(string dateTime)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            bool formatCorrect = dm.IsValidDateTimeTest(dateTime);
            return formatCorrect;

        }
        public bool isValidDecimal(string decValue)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            bool formatCorrect = dm.isValidDecimal(decValue);
            return formatCorrect;
        }
        public bool isValidInteger(string IntValue)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            bool formatCorrect = dm.isValidInteger(IntValue);
            return formatCorrect;

        }
        public void ConvertToDate(string datatime)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            dm.ConvertToDate(datatime);


        }
        public bool isValidSgFin(string strValueToCheck)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            bool isValid = dm.isValidSgFin(strValueToCheck);
            return isValid;


        }

        public string getCategoryDescription(string selectedItem)
        {

            DB_DataAnalytics dm = new DB_DataAnalytics();
            string description = dm.getCategoryDescription(selectedItem);
            return description;
        }
        public string getTableDescription(string selectedTable)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            string description = dm.getTableDescription(selectedTable);
            return description;

        }
        public void convertDateTimeRowToString(DataTable dtInString, List<string> dateList)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            dm.convertDateTimeRowToString(dtInString, dateList);




        }
        public int updateExistingRecord(List<string> ListOfpks, DataTable DupRecord, string tableName, DataTable currentDbRecord, DataTable excelRecord, int userId)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            int countSucceed = dm.updateExistingRecord(ListOfpks, DupRecord, tableName, currentDbRecord, excelRecord, userId);
            return countSucceed;
        }
        public DataTable getGroupTables(string aciGrp)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            DataTable dt = dm.getGroupTables(aciGrp);
            return dt;

        }
        public string getDepartDes(string aciGrp)
        {
            DB_DataAnalytics dm = new DB_DataAnalytics();
            string description = dm.getDepartDes(aciGrp);
            return description;



        }
    }

}