using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Globalization;

namespace LogicLayer
{
    public class Report_Management
    {
        private DB_Report dbRep = new DB_Report();
        private string _path = null;
        private int _userId = -1;

        public Report_Management(string path, int userId)
        {
            _path = path;
            _userId = userId;
        }

        public Report_Management() { }

        public DataTable getSettlementMode()
        {
            return dbRep.getSettlementMode();
        }

        private string encode(string str){
            return str.Replace("\"", "\"\"");
        }

        public Tuple<bool, string> genTraineeReport(DateTime dtStart, DateTime dtEnd, bool showParticulars, bool showModResults, string classCode)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "TraineeReport_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.TraineeReport(dtStart, dtEnd, showParticulars, showModResults, classCode);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                DataTable dtLang = (new DB_Applicant()).getAllLanguageCodeReference();
                DataTable dtProf = (new DB_Applicant()).getAllLanguageProficiencyCodeReference();
                Cryptography decrypt = new Cryptography();
                string[] langArr;

                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    //show header
                    sw.Write("\"ID\",\"Name\",\"NRIC/FIN/Passport\"");
                    if (showParticulars)
                    {
                        sw.Write(",\"Type\",\"Nationality\",\"Race\",\"Gender\",\"Contact No. 1\",\"Contact No. 2\",\"Email\",\"Date of Birth\",\"Address\",\"Postal Code\""
                            + ",\"Highest Education\",\"Education Remarks\",\"Spoken Lang 1\",\"Spoken Lang 1 Proficiency\",\"Spoken Lang 2\",\"Spoken Lang 2 Proficiency\""
                            + ",\"Spoken Lang 3\",\"Spoken Lang 3 Proficiency\",\"Written Lang 1\",\"Written Lang 1 Proficiency\",\"Written Lang 2\",\"Written Lang 2 Proficiency\""
                            + ",\"Written Lang 3\",\"Written Lang 3 Proficiency\",\"Remarks\"");
                    }
                    sw.Write(",\"Class Code\",\"Project Code\",\"Course\",\"Class Start Date\",\"Class End Date\"");
                    if (showModResults)
                    {
                        sw.Write(",\"Module Code\",\"Module Title\",\"Result\",\"First Assessment Date\",\"First Assessor\",\"Final Assessment Date\",\"Final Assessor\",\"SOA Status\",\"SOA Process Date\",\"SOA Receive Date\"");
                    }
                    sw.WriteLine("");

                    foreach (DataRow dr in dt.Rows)
                    {
                        sw.Write("\"" + dr["traineeId"].ToString() + "\",\"" + dr["fullName"].ToString() + "\",\"" + decrypt.decryptInfo(dr["idNumber"].ToString()) + "\"");
                        if (showParticulars)
                        {
                            sw.Write(",\"" + encode(dr["idTypeDisp"].ToString()) + "\",\"" + encode(dr["nationalityDisp"].ToString()) + "\",\"" + encode(dr["raceDisp"].ToString()) + "\",\"" + encode(dr["gender"].ToString()) + "\""
                                + ",\"" + (dr["contactNumber1"] == DBNull.Value || dr["contactNumber1"].ToString() == "" ? "" : encode(decrypt.decryptInfo(dr["contactNumber1"].ToString()))) + "\""
                                + ",\"" + (dr["contactNumber2"] == DBNull.Value || dr["contactNumber2"].ToString() == "" ? "" : encode(decrypt.decryptInfo(dr["contactNumber2"].ToString()))) + "\""
                                + ",\"" + (dr["emailAddress"] == DBNull.Value || dr["emailAddress"].ToString() == "" ? "" : encode(decrypt.decryptInfo(dr["emailAddress"].ToString()))) + "\",\"" + encode(dr["birthDateDisp"].ToString()) + "\""
                                + ",\"" + encode(decrypt.decryptInfo(dr["addressLine"].ToString())) + "\",\"" + encode(decrypt.decryptInfo(dr["postalCode"].ToString())) + "\",\"" + encode(dr["highestEducationDisp"].ToString()) + "\""
                                + ",\"" + (dr["highestEduRemarks"] == DBNull.Value || dr["highestEduRemarks"].ToString() == "" ? "" : encode(dr["highestEduRemarks"].ToString())) + "\",");

                            int cnt = 0;
                            if (dr["spokenLanguage"] != DBNull.Value)
                            {
                                foreach (string tmp in dr["spokenLanguage"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    langArr = tmp.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (langArr.Length < 2) continue;
                                    sw.Write("\"" + encode(dtLang.Select("codeValue='" + langArr[0] + "'")[0]["codeValueDisplay"].ToString()) + "\",");
                                    sw.Write("\"" + encode(dtProf.Select("codeValue='" + langArr[1] + "'")[0]["codeValueDisplay"].ToString()) + "\",");
                                    cnt++;
                                }
                                for (int n = cnt; n < 3; n++) sw.Write("\"\",\"\",");
                            }
                            else sw.Write("\"\",\"\",\"\",\"\",\"\",\"\",");

                            cnt = 0;
                            if (dr["writtenLanguage"] != DBNull.Value)
                            {
                                foreach (string tmp in dr["writtenLanguage"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    langArr = tmp.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (langArr.Length < 2) continue;
                                    sw.Write("\"" + encode(dtLang.Select("codeValue='" + langArr[0] + "'")[0]["codeValueDisplay"].ToString()) + "\",");
                                    sw.Write("\"" + encode(dtProf.Select("codeValue='" + langArr[1] + "'")[0]["codeValueDisplay"].ToString()) + "\",");
                                    cnt++;
                                }
                                for (int n = cnt; n < 3; n++) sw.Write("\"\",\"\",");
                            }
                            else sw.Write("\"\",\"\",\"\",\"\",\"\",\"\",");

                            sw.Write("\"" + (dr["traineeRemarks"] == DBNull.Value || dr["traineeRemarks"].ToString() == "" ? "" : encode(dr["traineeRemarks"].ToString())) + "\"");
                        }
                        sw.Write(",\"" + encode(dr["batchCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" + encode(dr["programmeStartDateDisp"].ToString()) + "\""
                            + ",\"" + encode(dr["programmeCompletionDate"].ToString()) + "\",");
                        if (showModResults)
                        {
                            sw.Write("\"" + encode(dr["moduleCode"].ToString()) + "\",\"" + encode(dr["moduleTitle"].ToString()) + "\",\"" + encode(dr["moduleResultDisp"].ToString()) + "\""
                                + ",\"" + (dr["firstAssessmentDateDisp"] == DBNull.Value ? "" : encode(dr["firstAssessmentDateDisp"].ToString())) + "\",\"" + (dr["firstAssessorName"] == DBNull.Value ? "" : encode(dr["firstAssessorName"].ToString())) + "\""
                                + ",\"" + (dr["finalAssessmentDateDisp"] == DBNull.Value ? "" : encode(dr["finalAssessmentDateDisp"].ToString())) + "\",\"" + (dr["finalAssessorName"] == DBNull.Value ? "" : encode(dr["finalAssessorName"].ToString())) + "\""
                                + ",\"" + encode(dr["SOAStatusDsip"].ToString()) + "\",\"" + (dr["processSOADateDisp"] == DBNull.Value ? "" : encode(dr["processSOADateDisp"].ToString())) + "\""
                                + ",\"" + (dr["receivedSOADateDisp"] == DBNull.Value ? "" : encode(dr["receivedSOADateDisp"].ToString())) + "\",");
                        }
                        sw.WriteLine("");
                    }
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genTraineeReport()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public DataTable getNetSettlement(DateTime dtSettlementDate, string settlementMode)
        {


            return dbRep.getSettlement(dtSettlementDate, settlementMode);
        }


        public Tuple<bool, string> getNetsSettlement(DateTime dtSettlementDate, string settlementMode)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "NetsSettlementReport" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            Cryptography decrypt = new Cryptography();

            try
            {
                DataTable dt = dbRep.getSettlement(dtSettlementDate, settlementMode);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");
                using (StreamWriter sw = File.CreateText(_path + fileName))
                {

                    // select cte.payment, t.fullName, t.idnumber, ps.programmeTitle, pb.programmeStartDate, pb.programmeCompletionDate, 
                    //pb.projectcode, ps.programmeCode, tp.programmePayableAmount, tp.registrationFee, tp.subsidyAmt, ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) as regGST, 
                    //tp.GSTPayableAmount, isnull(tp.registrationFee, 0) +  ROUND(((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0))),2) as totalReg, tp.programmePayableAmount + tp.GSTPayableAmount as totalProgrammeAmt, 
                    //tp.subsidyAmt, ROUND((tp.programmePayableAmount + tp.GSTPayableAmount +  ROUND((" + General_Constance.GST_RATE + @" * tp.registrationFee), 2) + tp.registrationFee - tp.subsidyAmt),2) 
                    //as afterSubsidyFees from  trainee_programme tp left join programme_batch pb on tp.programmeBatchId = pb.programmeBatchId left join trainee t on t.traineeId = 
                    //tp.traineeId left join programme_structure ps on pb.programmeId = ps.programmeId left join cte on cte.idnumber = t.idNumber ORDER BY  tp.programmeBatchId";
                    //sw.WriteLine("\"S/N\",\"Name Of Applicatns\",\"NRIC\"\" Programme Title \"\" Programme Start Date \"\" Programme End Date \"\" Course Code \"\" Project Code \"");

                    sw.WriteLine("\"S/N\",\"Name Of Applicants\",\"NRIC\",\" Programme Title \",\" Programme \n Start Date \",\" Programme \n End Date \",\" Course Code \",\" Project Code  \",\" Admin Fee \n (excl of GST) \n S$ \",\" GST Amount \n S$ \",\" Admin Fee \n (incl of GST) \n S$ \",\" Course Fee \n (excl of GST) \nS$ \",\" GST Amount \n S$ \",\" Course Fee \n (incl of GST) \nS$ \",\" Less: Scheme \n (WTS/MES/SFC) \",\" Total Course Fee \n (incl of GST) \n S$ \",\" Total Fee Collected \n (incl. of GST) \n S$ \"");



                    //\"");

                    //\",\"
                    int count = 1;
                    int ProgrammeBatchId = int.Parse(dt.Rows[0]["programmeBatchId"].ToString());


                    decimal subReg = 0;
                    decimal subRegGST = 0;
                    decimal subRegTotal = 0;

                    decimal subCourse = 0;
                    decimal subCourseGST = 0;
                    decimal subCourseTotal = 0;

                    decimal subsidySub = 0;
                    decimal afterSubFeesSub = 0;
                    decimal paymentFeeSub = 0;

                    string strSubsidy = "";

                    decimal totalReg = 0;
                    decimal totalRegGST = 0;
                    decimal totalRegTotal = 0;

                    decimal totalCourse = 0;
                    decimal totalCourseGST = 0;
                    decimal totalCourseTotal = 0;

                    decimal totalSubsidy = 0;
                    decimal totalAfterSubsFee = 0;
                    decimal totalPaymentFee = 0;


                    foreach (DataRow dr in dt.Rows)
                    {
                        int currBatchId = int.Parse(dr["programmeBatchId"].ToString());
                        
                       
                        if (currBatchId == ProgrammeBatchId)
                        {
                                subReg += decimal.Parse(dr["registrationFee"].ToString());
                                subRegGST += decimal.Parse(dr["regGST"].ToString());
                                subRegTotal += decimal.Parse(dr["totalReg"].ToString());

                                subCourse += decimal.Parse(dr["programmePayableAmount"].ToString());
                                subCourseGST += decimal.Parse(dr["GSTPayableAmount"].ToString());
                                subCourseTotal += decimal.Parse(dr["totalProgrammeAmt"].ToString());

                                subsidySub += decimal.Parse(dr["subsidyAmt"].ToString());
                                afterSubFeesSub += decimal.Parse(dr["afterSubsidyFees"].ToString());
                                paymentFeeSub += decimal.Parse(dr["payment"].ToString());
                   

                        }
                        else
                        {
                            ProgrammeBatchId = currBatchId;


                            strSubsidy = (subsidySub > 0) ? ("-" + subsidySub.ToString()) : "0";

                            sw.WriteLine("\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "Sub Total Collection" + "\",\"" + subReg.ToString() + "\",\""
                                + subRegGST.ToString() + "\",\"" + subRegTotal.ToString() + "\",\"" + subCourse.ToString() + "\",\"" + subCourseGST.ToString() + "\",\"" + subCourseTotal.ToString() + "\",\"" +
                               encode(strSubsidy) + "\",\"" + afterSubFeesSub.ToString() + "\",\"" + paymentFeeSub.ToString() + "\"");
                            subReg = 0;
                            subRegGST = 0;
                            subRegTotal = 0;

                            subCourse = 0;
                            subCourseGST = 0;
                            subCourseTotal = 0;

                            subsidySub = 0;
                            afterSubFeesSub = 0;
                            paymentFeeSub = 0;

                            subReg += decimal.Parse(dr["registrationFee"].ToString());
                            subRegGST += decimal.Parse(dr["regGST"].ToString());
                            subRegTotal += decimal.Parse(dr["totalReg"].ToString());
                            subCourse += decimal.Parse(dr["programmePayableAmount"].ToString());
                            subCourseGST += decimal.Parse(dr["GSTPayableAmount"].ToString());
                            subCourseTotal += decimal.Parse(dr["totalProgrammeAmt"].ToString());

                            subsidySub += decimal.Parse(dr["subsidyAmt"].ToString());
                            afterSubFeesSub += decimal.Parse(dr["afterSubsidyFees"].ToString());
                            paymentFeeSub += decimal.Parse(dr["payment"].ToString());

                            //totalReg += subReg;
                            //totalRegGST += subRegGST;
                            //totalRegTotal += subRegTotal;
                            //totalCourse += subCourse;
                            //totalCourseGST += subCourseGST;
                            //totalCourseTotal += subCourseTotal;
                            //totalSubsidy += subsidySub;
                            //totalAfterSubsFee += afterSubFeesSub;
                            //totalPaymentFee += paymentFeeSub;
                        }

                        string subsidy = (decimal.Parse(dr["subsidyAmt"].ToString()) > 0) ? "-" + dr["subsidyAmt"].ToString() : "0";
                        sw.WriteLine("\"" + count.ToString() + "\",\"" + encode(dr["fullName"].ToString()) + "\",\"" + encode(decrypt.decryptInfo(dr["idnumber"].ToString())) + "\",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" +
                                    encode(dr["StartDate"].ToString()) + "\",\"" + encode(dr["EndDate"].ToString()) + "\",\"" + encode(dr["courseCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\",\"" +
                                    dr["registrationFee"].ToString() + "\",\"" + dr["regGST"].ToString() + "\",\"" + dr["totalReg"].ToString() + "\",\"" + dr["programmePayableAmount"].ToString() + "\",\"" +
                                    dr["GSTPayableAmount"].ToString() + "\",\"" + dr["totalProgrammeAmt"].ToString() + "\",\"" + subsidy + "\",\"" + dr["afterSubsidyFees"].ToString() + "\",\"" + dr["payment"].ToString() + "\"");

                        count++;
                        totalReg += decimal.Parse(dr["registrationFee"].ToString());
                        totalRegGST += decimal.Parse(dr["regGST"].ToString());
                        totalRegTotal += decimal.Parse(dr["totalReg"].ToString());
                        totalCourse += decimal.Parse(dr["programmePayableAmount"].ToString());
                        totalCourseGST += decimal.Parse(dr["GSTPayableAmount"].ToString());
                        totalCourseTotal += decimal.Parse(dr["totalProgrammeAmt"].ToString());
                        totalSubsidy += decimal.Parse(dr["subsidyAmt"].ToString());
                        totalAfterSubsFee += decimal.Parse(dr["afterSubsidyFees"].ToString());
                        totalPaymentFee += decimal.Parse(dr["payment"].ToString());
                    }
                    strSubsidy = (subsidySub > 0) ? ("-" + subsidySub.ToString()) : "0";
                    string strTotalSubsidy = (totalSubsidy > 0) ? ("-" + totalSubsidy.ToString()) : "0";

                    sw.WriteLine("\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "Sub Total Collection" + "\",\"" + subReg.ToString() + "\",\""
                        + subRegGST.ToString() + "\",\"" + subRegTotal.ToString() + "\",\"" + subCourse.ToString() + "\",\"" + subCourseGST.ToString() + "\",\"" + subCourseTotal.ToString() + "\",\"" +
                       strSubsidy + "\",\"" + afterSubFeesSub.ToString() + "\",\"" + paymentFeeSub.ToString() + "\"");

                    sw.WriteLine("\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "" + "\",\"" + "TOTAL COLLECTION" + "\",\"" + totalReg.ToString() + "\",\""
                    + totalRegGST.ToString() + "\",\"" + totalRegTotal.ToString() + "\",\"" + totalCourse.ToString() + "\",\"" + totalCourseGST.ToString() + "\",\"" + totalCourseTotal.ToString() + "\",\"" +
                    strTotalSubsidy + "\",\"" + totalAfterSubsFee.ToString() + "\",\"" + totalPaymentFee.ToString() + "\"");
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genCourseFeeReceived()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }


        }
        public Tuple<bool, string> genCourseFeeReceived(int fy)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "CourseFeeReceived_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.CourseFeeReceived(fy);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");
                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    sw.WriteLine("\"Course Code\",\"Project Code\",\"Nett Fee received\"");

                    foreach (DataRow dr in dt.Rows)
                    {
                        sw.WriteLine("\"" + encode(dr["courseCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\",\"" + dr["nettFee"].ToString() + "\"");
                    }
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genCourseFeeReceived()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public Tuple<bool, string> genCourseFeeCollection(string progLvl, DateTime dtStart, DateTime dtEnd)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string folderName = "CourseFeeCollection_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            DirectoryInfo di = null, diSub = null;
            StreamWriter sw = null;
            Cryptography decrypt = new Cryptography();

            try
            {
                DataTable dt = dbRep.CourseFeeCollection(progLvl, dtStart, dtEnd);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                if (Directory.Exists(_path + folderName)) return new Tuple<bool, string>(false, "Duplicate folder. Unable to generate report.");
                di = Directory.CreateDirectory(_path + folderName);

                string prevCat = dt.Rows[0]["programmeCategory"].ToString();
                int prevBatch = (int)dt.Rows[0]["programmeBatchId"];
                decimal prevProgTotalAmt = (decimal)dt.Rows[0]["programmeTotalAmount"];             

                //create sub folder for each programme category
                diSub = Directory.CreateDirectory(di.FullName + Path.DirectorySeparatorChar + prevCat);

                sw = File.CreateText(diSub.FullName + Path.DirectorySeparatorChar + dt.Rows[0]["batchCode"].ToString().Replace("/", "_") + " " + dt.Rows[0]["programmeStartDate"].ToString() + ".csv");
                //display header
                sw.WriteLine("\"Course Code:\",\"\",\"" + encode(dt.Rows[0]["courseCode"].ToString()) + "\"");
                sw.WriteLine("\"Project Code:\",\"\",\"" + encode(dt.Rows[0]["projectCode"].ToString()) + "\"");
                sw.WriteLine("\"Class Code:\",\"\",\"" + encode(dt.Rows[0]["batchCode"].ToString()) + "\"");
                sw.WriteLine("\"Programme Title:\",\"\",\"" + encode(dt.Rows[0]["programmeTitle"].ToString()) + "\"");
                sw.WriteLine("\"Date of Programme:\",\"\",\"" + dt.Rows[0]["programmeStartDate"].ToString() + " - " + dt.Rows[0]["programmeCompletionDate"].ToString() + "\"");
                sw.WriteLine(Environment.NewLine);
                sw.WriteLine("\"S/N\",\"Name of Participants\",\"NRIC\",\"Admin Fee (excl GST) (A)\",\"Admin Fee (GST) (B)\",\"Cheque No / Receipt No\",\"Payment Mode\",\"Payment Date\",\"Payment Bank-In Date\",\"Course Fee (excl GST) (A)\""
                            + ",\"Course Fee (GST) (B)\",\"Total Course Fee (A+B)\",\"Remarks\"");

                int sn = 1;
                decimal totalRegAmt = 0, totalRegGSTAmt = 0, totalNettProgAmt = 0, totalProgGSTAmt = 0, totalProgAmt = 0;
                decimal subTotalRegAmt = 0, subTotalRegGSTAmt = 0, subTotalNettProgAmt = 0, subTotalProgGSTAmt = 0, subTotalProgAmt = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (prevProgTotalAmt != (decimal)dr["programmeTotalAmount"] && !(prevCat != dr["programmeCategory"].ToString() || prevBatch != (int)dr["programmeBatchId"]))
                    {
                        //display subtotals
                        sw.WriteLine("\"\",\"\",\"Subtotal\",\"" + subTotalRegAmt + "\",\"" + subTotalRegGSTAmt + "\",\"\",\"\",\"\",\"\",\"" + subTotalNettProgAmt + "\",\"" + subTotalProgGSTAmt + "\",\"" + subTotalProgAmt + "\"");
                        //reset var
                        subTotalRegAmt = subTotalRegGSTAmt = subTotalNettProgAmt = subTotalProgGSTAmt = subTotalProgAmt = 0;

                        prevProgTotalAmt = (decimal)dr["programmeTotalAmount"];
                    }

                    if (prevCat != dr["programmeCategory"].ToString() || prevBatch != (int)dr["programmeBatchId"])
                    {
                        //display subtotals
                        sw.WriteLine("\"\",\"\",\"Subtotal\",\"" + subTotalRegAmt + "\",\"" + subTotalRegGSTAmt + "\",\"\",\"\",\"\",\"\",\"" + subTotalNettProgAmt + "\",\"" + subTotalProgGSTAmt + "\",\"" + subTotalProgAmt + "\"");
                        
                        //display total
                        sw.WriteLine("\"\",\"TOTAL\",\"\",\"" + totalRegAmt + "\",\"" + totalRegGSTAmt + "\",\"\",\"\",\"\",\"\",\"" + totalNettProgAmt + "\",\"" + totalProgGSTAmt + "\",\"" + totalProgAmt + "\"");
                        sw.Close();
                        sw.Dispose();

                        //reset var
                        sn = 1;
                        prevBatch = (int)dr["programmeBatchId"];
                        subTotalRegAmt = subTotalRegGSTAmt = subTotalNettProgAmt = subTotalProgGSTAmt = subTotalProgAmt = 0;
                        prevProgTotalAmt = (decimal)dr["programmeTotalAmount"];

                        if (prevCat != dr["programmeCategory"].ToString())
                        {
                            //reset var
                            prevCat = dr["programmeCategory"].ToString();
                            totalRegAmt = totalRegGSTAmt = totalNettProgAmt = totalProgGSTAmt = totalProgAmt = 0;
                            //create new sub folder
                            diSub = Directory.CreateDirectory(di.FullName + Path.DirectorySeparatorChar + prevCat);
                        }

                        //create new file
                        sw = File.CreateText(diSub.FullName + Path.DirectorySeparatorChar + dr["batchCode"].ToString().Replace("/", "_") + " " + dr["programmeStartDate"].ToString() + ".csv");

                        //display header
                        sw.WriteLine("\"Course Code:\",\"\",\"" + encode(dr["courseCode"].ToString()) + "\"");
                        sw.WriteLine("\"Project Code:\",\"\",\"" + encode(dr["projectCode"].ToString()) + "\"");
                        sw.WriteLine("\"Class Code:\",\"\",\"" + encode(dr["batchCode"].ToString()) + "\"");
                        sw.WriteLine("\"Programme Title:\",\"\",\"" + encode(dr["programmeTitle"].ToString()) + "\"");
                        sw.WriteLine("\"Date of Programme:\",\"\",\"" + dr["programmeStartDate"].ToString() + " - " + dr["programmeCompletionDate"].ToString() + "\"");
                        sw.WriteLine(Environment.NewLine);
                        sw.WriteLine("\"S/N\",\"Name of Participants\",\"NRIC\",\"Admin Fee (excl GST) (A)\",\"Admin Fee (GST) (B)\",\"Cheque No / Receipt No\",\"Payment Mode\",\"Payment Date\",\"Payment Bank-In Date\",\"Course Fee (excl GST) (A)\""
                            + ",\"Course Fee (GST) (B)\",\"Total Course Fee (A+B)\",\"Remarks\"");
                    }

                    sw.WriteLine("\"" + sn + "\",\"" + encode(dr["fullName"].ToString()) + "\",\"" + encode(decrypt.decryptInfo(dr["idNumber"].ToString())) + "\",\"" + dr["registrationFee"].ToString() + "\",\"" + dr["registrationFeeGST"].ToString() + "\""
                        + ",\"" + encode(dr["referenceNumber"].ToString()) + "\",\"" + encode(dr["paymentMode"].ToString()) + "\",\"" + dr["paymentDate"].ToString() + "\",\"" + (dr["bankInDate"] == DBNull.Value ? "" : dr["bankInDate"].ToString()) + "\""
                        + ",\"" + dr["programmeNettAmount"].ToString() + "\",\"" + dr["programmeGSTAmount"].ToString() + "\",\"" + dr["programmeTotalAmount"].ToString() + "\",\"" + encode(dr["paymentRemarks"].ToString()) + "\"");

                    sn++;

                    totalRegAmt += (decimal)dr["registrationFee"];
                    totalRegGSTAmt += (decimal)dr["registrationFeeGST"];
                    totalNettProgAmt += (decimal)dr["programmeNettAmount"];
                    totalProgGSTAmt += (decimal)dr["programmeGSTAmount"];
                    totalProgAmt += (decimal)dr["programmeTotalAmount"];

                    subTotalRegAmt += (decimal)dr["registrationFee"];
                    subTotalRegGSTAmt += (decimal)dr["registrationFeeGST"];
                    subTotalNettProgAmt += (decimal)dr["programmeNettAmount"];
                    subTotalProgGSTAmt += (decimal)dr["programmeGSTAmount"];
                    subTotalProgAmt += (decimal)dr["programmeTotalAmount"];
                }

                //display subtotals
                sw.WriteLine("\"\",\"\",\"Subtotal\",\"" + subTotalRegAmt + "\",\"" + subTotalRegGSTAmt + "\",\"\",\"\",\"\",\"\",\"" + subTotalNettProgAmt + "\",\"" + subTotalProgGSTAmt + "\",\"" + subTotalProgAmt + "\"");
                //display total
                sw.WriteLine("\"\",\"TOTAL\",\"\",\"" + totalRegAmt + "\",\"" + totalRegGSTAmt + "\",\"\",\"\",\"\",\"\",\"" + totalNettProgAmt + "\",\"" + totalProgGSTAmt + "\",\"" + totalProgAmt + "\"");

                sw.Close();
                sw.Dispose();

                //create the zip file
                ZipFile.CreateFromDirectory(di.FullName, _path + folderName + ".zip");
                //delete the directory
                di.Delete(true);

                return new Tuple<bool, string>(true, folderName + ".zip");
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genCourseFeeCollection()", ex.Message, -1);

                if (di != null) di.Delete(true);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public DataTable genQPOSummary(DateTime dtStart, DateTime dtEnd)
        {
            return dbRep.QPOSummary(dtStart, dtEnd);
        }

        public Tuple<bool, string> genQPODetails(DateTime ayStart)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "QPOFlatFile_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.QPODetails(ayStart);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                Cryptography decrypt = new Cryptography();
                int sn = 1;
                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    sw.WriteLine("\"Academic Year (YYYY)\",\"Programme Type\",\"Programme Name\",\"Programme Start Date (YYYYMMDD)\",\"Programme End Date (YYYYMMDD)\",\"Course Code\",\"Project Code\",\"No. of Participants (Headcounts)\""
                        + ",\"No. of Participants (Training Places) \"");

                    foreach (DataRow dr in dt.Rows)
                    {
                        sw.WriteLine("\"" + dr["AY"].ToString() + "\",\"" + encode(dr["programmeTypeDisp"].ToString()) + "\",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" + dr["programmeStartDate"].ToString() + "\",\"" + dr["programmeCompletionDate"].ToString() + "\""
                            + ",\"" + encode(dr["courseCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\",\"" + dr["hc"].ToString() + "\",\"" + dr["trainingPlc"].ToString() + "\"");
                        sn++;
                    }
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genQPODetails()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public DataTable genALLSSummary(int yr)
        {
            return dbRep.ALLSSummary(yr);
        }

        public Tuple<bool, string> genALLSDetails(int yr)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "ALLSDetails_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.ALLSDetails(yr);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                Cryptography decrypt = new Cryptography();
                int psn = 1, msn = 1;
                int prevProg = (int)dt.Rows[0]["programmeId"];
                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    sw.WriteLine("\"No.\",\"Qual\",\"Title of Course in Full\",\"Course Code\",\"#\",\"Module\",\"Training Hrs Per Run\",\"Unique Headcounts (for entire programme)\",\"Actual Training Places (in modular units)\",\"Actual Training Places (in MC/PDC)\""
                        + ",\"Actual Total No. of Training Hours (based on modular units)\",\"No. of New Course Runs\",\"No. of Course Runs (incl. Crossovers)\"");

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (prevProg != (int)dr["programmeId"])
                        {
                            prevProg = (int)dr["programmeId"];
                            msn = 1;
                            psn++;
                        }

                        sw.WriteLine("\"" + psn + "\",\"" + encode(dr["programmeTypeDisp"].ToString()) + "\",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" + encode(dr["courseCode"].ToString()) + "\",\"" + msn + "\",\"" + encode(dr["moduleTitle"].ToString()) + "\""
                            + ",\"" + dr["moduleTrainingHour"].ToString() + "\",\"" + dr["uniqueHC"].ToString() + "\",\"" + dr["trainPlcMod"].ToString() + "\",\"" + dr["trainPlcPDC"].ToString() + "\",\"" + dr["totalTrainingHour"].ToString() + "\""
                            + ",\"" + dr["newRun"].ToString() + "\",\"" + dr["inclCrossRun"].ToString() + "\"");

                        msn++;
                    }
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genALLSDetails()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public Tuple<bool, string> genSFCDisbursement(int yr, string subsidyName)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "SFCDisbursement_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.SFCDisbursement(yr, subsidyName);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                Cryptography decrypt = new Cryptography();
                int sn = 1;
                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    sw.WriteLine("\"SN\",\"Name\",\"NRIC\",\"Citizenship\",\"Programme\",\"Course code\",\"Project code\",\"Class\",\"Course Start Date\",\"Course End Date\""
                        + ",\"Net Amount\",\"GST Amount\",\"" + encode(subsidyName) + "\",\"Claim ID\",\"Payment through Nets / Cheque (Course Fee w/o GST)\",\"Amount to be disbursed (with GST)\"");

                    foreach (DataRow dr in dt.Rows)
                    {
                        sw.WriteLine("\"" + sn + "\",\"" + encode(dr["fullName"].ToString()) + "\",\"" + encode(decrypt.decryptInfo(dr["idNumber"].ToString())) + "\",\"" + encode(dr["idTypeDisp"].ToString()) + "\""
                            + ",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" + encode(dr["courseCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\",\"" + encode(dr["batchCode"].ToString()) + "\""
                            + ",\"" + dr["programmeStartDate"].ToString() + "\",\"" + dr["programmeCompletionDate"].ToString() + "\",\"" + dr["programmetNettAmount"].ToString() + "\""
                            + ",\"" + dr["gst"].ToString() + "\",\"" + dr["subsidyAmt"].ToString() + "\",\"" + encode(dr["referenceNumber"].ToString()) + "\",\"" + dr["othAmount"].ToString() + "\""
                            + ",\"" + dr["SFCAmount"].ToString() + "\"");
                        sn++;
                    }
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genSFCDisbursement()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public Tuple<bool, string> genFullQualQuarter(int mth, int yr)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string folderName = "FullQualQuarter_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            DirectoryInfo di = null;
            StreamWriter sw = null;

            try
            {
                DataTable dt = dbRep.FullQualQuarter(DateTime.ParseExact(yr + "-" + mth + "-" + DateTime.DaysInMonth(yr, mth), "yyyy-M-d", CultureInfo.InvariantCulture));
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                if (Directory.Exists(_path + folderName)) return new Tuple<bool, string>(false, "Duplicate folder. Unable to generate report.");
                di = Directory.CreateDirectory(_path + folderName);

                string prevCat = dt.Rows[0]["programmeCategory"].ToString();
                int progId = (int)dt.Rows[0]["programmeId"];
                string prevClass = dt.Rows[0]["batchCode"].ToString();
                decimal prevFee = (decimal)dt.Rows[0]["programmeNettAmount"];
                int prevSub = (dt.Rows[0]["subsidyId"] == DBNull.Value ? -1 : (int)dt.Rows[0]["subsidyId"]);
                int pax = (int)dt.Rows[0]["pax"];
                decimal totalRev = (decimal)dt.Rows[0]["totalRevFee"], totalDef = (decimal)dt.Rows[0]["totalDefFee"];

                sw = File.CreateText(di.FullName + Path.DirectorySeparatorChar + prevCat + "_" + yr + mth + ".csv");
                //display header
                sw.WriteLine("\"Subsidy\",\"Programme\",\"Module\",\"Module Fee\",\"Deferred Module Fee\",\"Nett Course Fee\",\"Class Code\",\"Course Code\",\"Project Code\""
                    + ",\"Start Date\",\"End Date\",\"No. of Pax\",\"No. of Module\",\"Total Rev Collected (Calculated)\",\"Total deferred Module fee (Calculated)\""
                    + ",\"No of Modules to defer\",\"As at 31-Mar-" + yr + "\",\"From 01-Apr-" + yr + " to 30-Jun-" + yr + "\",\"From 01-Jul-" + yr + " to 30-Sep-" + yr + "\""
                    + ",\"From 01-Oct-" + yr + " to 31-Dec-" + yr + "\",\"From 01-Jan-" + (yr + 1) + " to 31-Mar-" + (yr + 1) + "\",\"Total No. of Modules completed\"");

                foreach (DataRow dr in dt.Rows)
                {
                    if (prevCat != dr["programmeCategory"].ToString())
                    {
                        //display the subtotal
                        sw.WriteLine("\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"Sub Total\",\"" + pax + "\",\"\",\"" + totalRev + "\",\"" + totalDef + "\"");

                        sw.Close();
                        sw.Dispose();

                        //reset var
                        prevCat = dr["programmeCategory"].ToString();
                        pax = (int)dr["pax"];
                        totalRev = (decimal)dr["totalRevFee"];
                        totalDef = (decimal)dr["totalDefFee"];

                        prevClass = dr["batchCode"].ToString();
                        prevFee = (decimal)dr["programmeNettAmount"];
                        prevSub = (dr["subsidyId"] == DBNull.Value ? -1 : (int)dr["subsidyId"]);

                        //create new csv
                        sw = File.CreateText(di.FullName + Path.DirectorySeparatorChar + prevCat + "_" + yr + mth + ".csv");

                        //display header
                        sw.WriteLine("\"Subsidy\",\"Programme\",\"Module\",\"Module Fee\",\"Deferred Module Fee\",\"Nett Course Fee\",\"Class Code\",\"Course Code\",\"Project Code\""
                            + ",\"Start Date\",\"End Date\",\"No. of Pax\",\"No. of Module\",\"Total Rev Collected (Calculated)\",\"Total deferred Module fee (Calculated)\""
                            + ",\"No of Modules to defer\",\"As at 31-Mar-" + yr + "\",\"From 01-Apr-" + yr + " to 30-Jun-" + yr + "\",\"From 01-Jul-" + yr + " to 30-Sep-" + yr + "\""
                            + ",\"From 01-Oct-" + yr + " to 31-Dec-" + yr + "\",\"From 01-Jan-" + (yr + 1) + " to 31-Mar-" + (yr + 1) + "\",\"Total No. of Modules completed\"");
                    }

                    if (prevClass != dr["batchCode"].ToString())
                    {
                        //display the subtotals                        
                        sw.WriteLine("\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"Sub Total\",\"" + pax + "\",\"\",\"" + totalRev + "\",\"" + totalDef + "\"");
                        sw.Write(Environment.NewLine);

                        //reset var
                        pax = (int)dr["pax"];
                        totalRev = (decimal)dr["totalRevFee"];
                        totalDef = (decimal)dr["totalDefFee"];

                        prevClass = dr["batchCode"].ToString();
                        prevFee = (decimal)dr["programmeNettAmount"];
                        prevSub = (dr["subsidyId"] == DBNull.Value ? -1 : (int)dr["subsidyId"]);
                    }

                    //determine should add the values as the values are duplicated across rows
                    if (prevClass != dr["batchCode"].ToString() || prevFee != (decimal)dr["programmeNettAmount"] || prevSub != (dr["subsidyId"] == DBNull.Value ? -1 : (int)dr["subsidyId"]))
                    {
                        pax += (int)dr["pax"];
                        totalRev += (decimal)dr["totalRevFee"];
                        totalDef += (decimal)dr["totalDefFee"];

                        prevClass = dr["batchCode"].ToString();
                        prevFee = (decimal)dr["programmeNettAmount"];
                        prevSub = (dr["subsidyId"] == DBNull.Value ? -1 : (int)dr["subsidyId"]);
                    }

                    sw.WriteLine("\"" + encode(dr["subsidyScheme"].ToString()) + "\",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" + encode(dr["moduleTitle"].ToString()) + "\",\"" + dr["nettModCost"].ToString() + "\",\"" + dr["defModFee"].ToString() + "\""
                        + ",\"" + dr["programmeNettAmount"].ToString() + "\",\"" + encode(dr["batchCode"].ToString()) + "\",\"" + encode(dr["courseCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\""
                        + ",\"" + dr["programmeStartDate"].ToString() + "\",\"" + dr["programmeCompletionDate"].ToString() + "\",\"" + dr["pax"].ToString() + "\",\"" + dr["noOfMod"].ToString() + "\""
                        + ",\"" + dr["totalRevFee"].ToString() + "\",\"" + dr["totalDefFee"].ToString() + "\",\"" + dr["noDefMod"].ToString() + "\",\"" + dr["beforeFY"].ToString() + "\""
                        + ",\"" + dr["fyQ1"].ToString() + "\",\"" + dr["fyQ2"].ToString() + "\",\"" + dr["fyQ3"].ToString() + "\",\"" + dr["fyQ4"].ToString() + "\",\"" + dr["noCompleteMod"].ToString() + "\"");
                }
                //display the subtotal
                sw.WriteLine("\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"Sub Total\",\"" + pax + "\",\"\",\"" + totalRev + "\",\"" + totalDef + "\"");

                sw.Close();
                sw.Dispose();

                //create the zip file
                ZipFile.CreateFromDirectory(di.FullName, _path + folderName + ".zip");
                //delete the directory
                di.Delete(true);

                return new Tuple<bool, string>(true, folderName + ".zip");
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genFullQualQuarter()", ex.Message, -1);

                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (di != null) di.Delete(true);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public Tuple<bool, string> genWTSDisbursement(int yr, string subsidyName)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "WTSDisbursement_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.WTSDisbursement(yr, subsidyName);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                Cryptography decrypt = new Cryptography();
                int sn = 1;
                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    sw.WriteLine("\"SN\",\"Name\",\"NRIC\",\"Citizenship\",\"Programme\",\"Course code\",\"Project code\",\"Class\",\"Course Start Date\",\"Course End Date\"");

                    foreach (DataRow dr in dt.Rows)
                    {
                        sw.WriteLine("\"" + sn + "\",\"" + encode(dr["fullName"].ToString()) + "\",\"" + encode(decrypt.decryptInfo(dr["idNumber"].ToString())) + "\",\"" + encode(dr["idTypeDisp"].ToString()) + "\""
                            + ",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" + encode(dr["courseCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\",\"" + encode(dr["batchCode"].ToString()) + "\""
                            + ",\"" + dr["programmeStartDate"].ToString() + "\",\"" + dr["programmeCompletionDate"].ToString() + "\"");
                        sn++;
                    }
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genWTSDisbursement()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public Tuple<bool, string> genCseFeeDrawnDown(int mth, int yr, string subsidyName)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "CourseFeeDrawnDown_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.cseFeeGrantDrawDown(mth, yr, subsidyName);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                string mthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(mth);
                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    sw.WriteLine("\"Programme Type\",\"Programme Level\",\"Programmes\",\"Assessment Completion Date\",\"Course Code\",\"Project Code\",\"Module Name\",\"Total Headcounts Attending / Completed\""
                        + ",\"Total Headcounts Attending / Completed (No Subsidy)\",\"Course Fee Grant Per Module\",\"Grant Drawdown as at " + mthName + "\" " + yr + "");

                    foreach (DataRow dr in dt.Rows)
                    {
                        sw.WriteLine("\"" + encode(dr["programmeTypeDisp"].ToString()) + "\",\"" + encode(dr["programmeLevelDisp"].ToString()) + "\",\"" + encode(dr["programmeTitle"].ToString()) + "\",\"" + dr["assessmentDate"].ToString() + "\""
                            + ",\"" + encode(dr["courseCode"].ToString()) + "\",\"" + encode(dr["projectCode"].ToString()) + "\",\"" + encode(dr["moduleTitle"].ToString()) + "\",\"" + dr["hc"].ToString() + "\""
                            + ",\"" + dr["nonSubsidyHC"].ToString() + "\",\"" + dr["feeGrant"].ToString() + "\",\"" + dr["totalFeeGrant"].ToString() + "\"");
                    }
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genCseFeeDrawnDown()", ex.Message, -1);

                if (File.Exists(_path + fileName)) File.Delete(_path + fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public DataTable genSSGKPIAchievement(int fyStart, int fyEnd)
        {
            return dbRep.SSGKPIAchievement(fyStart, fyEnd);
        }

        public Tuple<bool, string> genACIMonthlyDetails(DateTime dtStart, DateTime dtEnd)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string folderName = "ACIMonthlyReportDetails_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            DirectoryInfo di = null;
            Cryptography decrypt = new Cryptography();

            try
            {
                DataTable dt = dbRep.ACIMonthlyDetails(dtStart, dtEnd);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                if (Directory.Exists(_path + folderName)) return new Tuple<bool, string>(false, "Duplicate folder. Unable to generate report.");
                di = Directory.CreateDirectory(_path + folderName);

                DataView dv = new DataView(dt);
                //find distinct programmes
                DataTable dtProg = dv.ToTable(true, "programmeId", "courseCode", "programmeTitle");
                //find distinct nett amount of the programme
                DataTable dtFee = dv.ToTable(true, "programmeId", "programmeNettAmount");
                //find distinct subsidy
                DataTable dtSub = dv.ToTable(true, "programmeId", "subsidyId", "subsidyScheme");

                int sn, hc, soa, completeHc, allHc, allSOA, allCompleteHc;
                decimal total, allTotal;
                foreach (DataRow drProg in dtProg.Rows)
                {
                    using (StreamWriter sw = File.CreateText(di.FullName + Path.DirectorySeparatorChar + drProg["courseCode"].ToString() + ".csv"))
                    {
                        sw.WriteLine("\"" + encode(drProg["programmeTitle"].ToString()) + "\"");
                        sw.Write("\"S/N\",\"Course Fee(S$)\",\"Course Code\",\"Project Code\",\"Program Start\",\"Program End\",\"Nos of RFI\",\"Bill To Company OR MOE\",\"RFI No. of HC(A)\""
                            + ",\"RFI Amount\",\"Invoice(s) No.\",\"Invoice(s) date\"");

                        //for each subsidy of the programme, display the header column
                        List<string> subCols = new List<string>();
                        foreach (DataRow drSub in dtSub.Select("programmeId=" + drProg["programmeId"].ToString()))
                        {
                            sw.Write(",\"" + (drSub["subsidyScheme"] == DBNull.Value ? "Subsidy N/A" : encode(drSub["subsidyScheme"].ToString())) + "\"");
                            subCols.Add(drSub["subsidyId"]==DBNull.Value ? "NULL" : drSub["subsidyId"].ToString());
                        }

                        sw.WriteLine(",\"Amount\",\"Total HC(A) + (B) + (C)\",\"Total Income (S$)\",\"SOA\",\"Total HC Completed\"");

                        sn = 1; allHc = 0; allSOA = 0; allCompleteHc = 0; allTotal = 0;
                        foreach (DataRow drFee in dtFee.Select("programmeId=" + drProg["programmeId"].ToString()))
                        {
                            DataRow[] drsTemp; DataTable dtTemp;
                            //get the rows for the selected nett amount and programme where rfi col are empty
                            drsTemp = dt.Select("programmeId=" + drFee["programmeId"].ToString() + " and programmeNettAmount=" + drFee["programmeNettAmount"].ToString() + " and noOfRFI is null");
                            if (drsTemp.Length > 0)
                            {
                                dtTemp = drsTemp.CopyToDataTable();
                                //create only 1 row for all the subsidy of the programme
                                sw.Write("\"" + sn + "\",\"" + dtTemp.Rows[0]["programmeNettAmount"].ToString() + "\",\"" + encode(dtTemp.Rows[0]["courseCode"].ToString()) + "\",\"" + encode(dtTemp.Rows[0]["projectCode"].ToString()) + "\""
                                    + ",\"" + dtTemp.Rows[0]["programmeStartDateDisp"].ToString() + "\",\"" + dtTemp.Rows[0]["programmeCompletionDateDisp"].ToString() + "\",\"\",\"\",\"\",\"\",\"\",\"\"");

                                hc = 0; completeHc = 0; soa = 0; total = 0;
                                //for all the subsidy for the programme, search through the current rows to find the corresponding headcount
                                foreach (string sub in subCols)
                                {
                                    drsTemp = dtTemp.Select("subsidyId " + (sub == "NULL" ? "is null" : "=" + sub));
                                    if (drsTemp.Length == 0) sw.Write(",\"\"");
                                    else
                                    {
                                        sw.Write(",\"" + drsTemp[0]["nonRFI_HC"].ToString() + "\"");
                                        hc += (int)drsTemp[0]["nonRFI_HC"];
                                        completeHc += (int)drsTemp[0]["completeHC"];
                                        soa += (int)drsTemp[0]["soa"];
                                        total += (decimal)drsTemp[0]["totalAmt"];

                                        allHc += hc; allCompleteHc += completeHc; allSOA += soa; allTotal += total;
                                    }
                                }
                                sw.WriteLine(",\"" + total + "\",\"" + hc + "\",\"" + total + "\",\"" + soa + "\",\"" + completeHc + "\"");
                                sn++;
                            }

                            //get the rows for the selected nett amount and programme where rfi col are NOT empty
                            drsTemp = dt.Select("programmeId=" + drFee["programmeId"].ToString() + " and programmeNettAmount=" + drFee["programmeNettAmount"].ToString() + " and noOfRFI is not null");
                            if (drsTemp.Length == 0) continue;

                            dtTemp = drsTemp.CopyToDataTable();
                            foreach (DataRow drTemp in dtTemp.Rows)
                            {
                                sw.Write("\"" + sn + "\",\"" + drTemp["programmeNettAmount"].ToString() + "\",\"" + encode(drTemp["courseCode"].ToString()) + "\",\"" + encode(drTemp["projectCode"].ToString()) + "\""
                                    + ",\"" + drTemp["programmeStartDateDisp"].ToString() + "\",\"" + drTemp["programmeCompletionDateDisp"].ToString() + "\",\"" + drTemp["noOfRFI"].ToString() + "\""
                                    + ",\"" + encode(drTemp["RFICompany"].ToString()) + "\",\"" + drTemp["RFI_HC"].ToString() + "\",\"" + drTemp["RFIAmount"].ToString() + "\",\"" + encode(drTemp["RFIInv"].ToString()) + "\""
                                    + ",\"" + drTemp["RFIInvDate"].ToString() + "\"");
                                //for all the subsidy for the programme, find the corresponding headcount in the current row
                                foreach (string sub in subCols)
                                {
                                    if (sub == (drTemp["subsidyId"] == DBNull.Value ? "NULL" : drTemp["subsidyId"].ToString())) sw.Write(",\"" + drTemp["nonRFI_HC"].ToString() + "\"");
                                    else sw.Write(",\"\"");
                                }

                                sw.WriteLine(",\"" + drTemp["nonRFIAmt"].ToString() + "\",\"" + drTemp["totalHC"].ToString() + "\",\"" + drTemp["totalAmt"].ToString() + "\",\"" + drTemp["soa"].ToString() + "\""
                                    + ",\"" + drTemp["completeHC"].ToString() + "\"");

                                allHc += (int)drTemp["totalHC"];
                                allCompleteHc += (int)drTemp["completeHC"];
                                allSOA += (int)drTemp["soa"];
                                allTotal += (decimal)drTemp["totalAmt"];
                                sn++;
                            }
                        }

                        for (int i = 0; i < subCols.Count + 12; i++) sw.Write("\"\",");
                        sw.WriteLine("\"Total\",\"" + allHc + "\",\"" + allTotal + "\",\"" + allSOA + "\",\"" + allCompleteHc + "\"");
                    }
                }

                //create the zip file
                ZipFile.CreateFromDirectory(di.FullName, _path + folderName + ".zip");
                //delete the directory
                di.Delete(true);

                return new Tuple<bool, string>(true, folderName + ".zip");
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genACIMonthlyDetails()", ex.Message, -1);

                if (di != null) di.Delete(true);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

        public Tuple<bool, string> genACIMonthlySummary(int fyStart, int fyEnd)
        {
            if (_path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "ACIMonthlyReportSummary_" + _userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            try
            {
                DataTable dt = dbRep.ACIMonthlySummary(fyStart, fyEnd);
                if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");

                string enrolStr = "", completeStr = "", soaStr = "";

                using (StreamWriter sw = File.CreateText(_path + fileName))
                {
                    //display headers
                    sw.Write("\"Income Collected and Headcounts/SOAs Achievement from FY " + fyStart + " to " + fyEnd + "\"");
                    sw.Write(Environment.NewLine);
                    sw.Write(Environment.NewLine);

                    sw.Write("\"Programme Type\",\"Programme Level\",\"Programme Code\",\"Programmes\"");
                    for (int i = 0; i < fyEnd - fyStart + 1; i++)
                    {
                        enrolStr += ",\"Headcounts Enrolled in FY " + (fyStart + i).ToString().Substring(2) + "\"";
                        completeStr += ",\"Headcounts Completed FY " + (fyStart + i).ToString().Substring(2) + "\"";
                        soaStr += ",\"SOAs Achieved FY " + (fyStart + i).ToString().Substring(2) + "\"";
                    }
                    sw.Write(enrolStr + ",\"Total Headcounts Enrolled\"");
                    sw.Write(completeStr + ",\"Total Headcounts Completed\"");
                    sw.Write(enrolStr + ",\"Total SOAs Achieved\"");
                    sw.Write(Environment.NewLine);

                    string prevPgType = dt.Rows[0]["programmeType"].ToString();
                    string prevPgLvl = dt.Rows[0]["programmeLevel"].ToString();
                    string prevPgLvlDisp = dt.Rows[0]["programmeLevelDisp"].ToString();
                    int[] totalLvl = new int[(fyEnd - fyStart + 2) * 3];
                    int[] totalType = new int[(fyEnd - fyStart + 2) * 3];
                    foreach (DataRow dr in dt.Rows)
                    {
                        enrolStr = ""; completeStr = ""; soaStr = "";

                        if (dr["programmeLevel"].ToString() != prevPgLvl || dr["programmeType"].ToString() != prevPgType)
                        {
                            //subtotal
                            sw.Write("\"\",\"\",\"\",\"" + encode(prevPgLvlDisp) + "\"");
                            for (int i = 0; i < fyEnd - fyStart + 1; i++)
                            {
                                enrolStr += ",\"" + totalLvl[i] + "\"";
                                completeStr += ",\"" + totalLvl[fyEnd - fyStart + 2 + i] + "\"";
                                soaStr += ",\"" + totalLvl[(fyEnd - fyStart + 2) * 2 + i] + "\"";
                            }
                            sw.Write(enrolStr + ",\"" + totalLvl[fyEnd - fyStart + 1] + "\"" + completeStr + ",\"" + totalLvl[(fyEnd - fyStart) * 2 + 3] + "\"" + soaStr + ",\"" + totalLvl[(fyEnd - fyStart) * 3 + 5] + "\"");
                            sw.Write(Environment.NewLine);

                            //reset var
                            enrolStr = ""; completeStr = ""; soaStr = "";
                            prevPgLvl = dr["programmeLevel"].ToString();
                            prevPgLvlDisp = dr["programmeLevelDisp"].ToString();
                            for (int i = 0; i < totalLvl.Length; i++) totalLvl[i] = 0;
                        }

                        if (dr["programmeType"].ToString() != prevPgType)
                        {
                            //total and new section
                            sw.Write("\"\",\"\",\"\",\"Total\"");
                            for (int i = 0; i < fyEnd - fyStart + 1; i++)
                            {
                                enrolStr += ",\"" + totalType[i] + "\"";
                                completeStr += ",\"" + totalType[fyEnd - fyStart + 2 + i] + "\"";
                                soaStr += ",\"" + totalType[(fyEnd - fyStart + 2) * 2 + i] + "\"";
                            }
                            sw.Write(enrolStr + ",\"" + totalType[fyEnd - fyStart + 1] + "\"" + completeStr + ",\"" + totalType[(fyEnd - fyStart) * 2 + 3] + "\"" + soaStr + ",\"" + totalType[(fyEnd - fyStart) * 3 + 5] + "\"");
                            sw.Write(Environment.NewLine);
                            sw.Write(Environment.NewLine);

                            enrolStr = ""; completeStr = ""; soaStr = "";
                            sw.Write("\"Programme Type\",\"Programme Level\",\"Programme Code\",\"Programmes\"");
                            for (int i = 0; i < fyEnd - fyStart + 1; i++)
                            {
                                enrolStr += ",\"Headcounts Enrolled in FY " + (fyStart + i).ToString().Substring(2) + "\"";
                                completeStr += ",\"Headcounts Completed FY " + (fyStart + i).ToString().Substring(2) + "\"";
                                soaStr += ",\"SOAs Achieved FY " + (fyStart + i).ToString().Substring(2) + "\"";
                            }
                            sw.Write(enrolStr + ",\"Total Headcounts Enrolled\"");
                            sw.Write(completeStr + ",\"Total Headcounts Completed\"");
                            sw.Write(enrolStr + ",\"Total SOAs Achieved\"");
                            sw.Write(Environment.NewLine);

                            //reset var
                            enrolStr = ""; completeStr = ""; soaStr = "";
                            prevPgType = dr["programmeType"].ToString();
                            for (int i = 0; i < totalType.Length; i++) totalType[i] = 0;
                        }

                        sw.Write("\"" + encode(dr["programmeTypeDisp"].ToString()) + "\",\"" + encode(dr["programmeLevelDisp"].ToString()) + "\",\"" + encode(dr["programmeCode"].ToString()) + "\",\"" + encode(dr["programmeTitle"].ToString()) + "\"");
                        for (int i = 0; i < fyEnd - fyStart + 1; i++)
                        {
                            enrolStr += ",\"" + dr["Enrolled_FY" + (i + 1)].ToString() + "\"";
                            completeStr += ",\"" + dr["Complete_FY" + (i + 1)].ToString() + "\"";
                            soaStr += ",\"" + dr["SOA_FY" + (i + 1)].ToString() + "\"";

                            totalLvl[i] += (int)dr["Enrolled_FY" + (i + 1)];
                            totalType[i] += (int)dr["Enrolled_FY" + (i + 1)];
                            totalLvl[fyEnd - fyStart + 2 + i] += (int)dr["Complete_FY" + (i + 1)];
                            totalType[fyEnd - fyStart + 2 + i] += (int)dr["Complete_FY" + (i + 1)];
                            totalLvl[(fyEnd - fyStart + 2) * 2 + i] += (int)dr["SOA_FY" + (i + 1)];
                            totalType[(fyEnd - fyStart + 2) * 2 + i] += (int)dr["SOA_FY" + (i + 1)];
                        }
                        sw.Write(enrolStr + ",\"" + dr["Enrolled_Total"].ToString() + "\"" + completeStr + ",\"" + dr["Complete_Total"].ToString() + "\"" + soaStr + ",\"" + dr["SOA_Total"].ToString() + "\"");
                        totalLvl[fyEnd - fyStart + 1] += (int)dr["Enrolled_Total"];
                        totalType[fyEnd - fyStart + 1] += (int)dr["Enrolled_Total"];
                        totalLvl[(fyEnd - fyStart) * 2 + 3] += (int)dr["Complete_Total"];
                        totalType[(fyEnd - fyStart) * 2 + 3] += (int)dr["Complete_Total"];
                        totalLvl[(fyEnd - fyStart) * 3 + 5] += (int)dr["SOA_Total"];
                        totalType[(fyEnd - fyStart) * 3 + 5] += (int)dr["SOA_Total"];
                        sw.Write(Environment.NewLine);
                    }

                    //after the loop ends, write the final subtotal and total
                    enrolStr = ""; completeStr = ""; soaStr = "";
                    sw.Write("\"\",\"\",\"\",\"" + prevPgLvlDisp + "\"");
                    for (int i = 0; i < fyEnd - fyStart + 1; i++)
                    {
                        enrolStr += ",\"" + totalLvl[i] + "\"";
                        completeStr += ",\"" + totalLvl[fyEnd - fyStart + 2 + i] + "\"";
                        soaStr += ",\"" + totalLvl[(fyEnd - fyStart + 2) * 2 + i] + "\"";
                    }
                    sw.Write(enrolStr + ",\"" + totalLvl[fyEnd - fyStart + 1] + "\"" + completeStr + ",\"" + totalLvl[(fyEnd - fyStart) * 2 + 3] + "\"" + soaStr + ",\"" + totalLvl[(fyEnd - fyStart) * 3 + 5] + "\"");
                    sw.Write(Environment.NewLine);
                    enrolStr = ""; completeStr = ""; soaStr = "";
                    sw.Write("\"\",\"\",\"\",\"Total\"");
                    for (int i = 0; i < fyEnd - fyStart + 1; i++)
                    {
                        enrolStr += ",\"" + totalType[i] + "\"";
                        completeStr += ",\"" + totalType[fyEnd - fyStart + 2 + i] + "\"";
                        soaStr += ",\"" + totalType[(fyEnd - fyStart + 2) * 2 + i] + "\"";
                    }
                    sw.Write(enrolStr + ",\"" + totalType[fyEnd - fyStart + 1] + "\"" + completeStr + ",\"" + totalType[(fyEnd - fyStart) * 2 + 3] + "\"" + soaStr + ",\"" + totalType[(fyEnd - fyStart) * 3 + 5] + "\"");
                }

                return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Report_Management.cs", "genACIMonthlySummary()", ex.Message, -1);

                if (File.Exists(_path+fileName)) File.Delete(_path+fileName);

                return new Tuple<bool, string>(false, "Error generating report.");
            }
        }

    }
}