using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using GeneralLayer;
using Excel = Microsoft.Office.Interop.Excel;
using System.Configuration;
using System.Data;
using System.IO;

namespace LogicLayer
{
    public struct empRec
    {
        public string companyName;
        public string dept;
        public string designation;
        public EmplStatus status;
        public int occupationType;
        public decimal salary;
        public DateTime dtStart;
        public DateTime dtEnd;
        public bool current;
    }

    public struct InterviewDetails
    {
        public int interviewerId;
        public DateTime interviewedDate;
        public string interviewRemarks;

    }

    public struct ApplicantRec
    {
        public string Name;
        public string idNumber;
        public string idType;
        public string nationality;
        public string race;
        public string contactNo1;
        public string contactNo2;
        public string emailAddress;
        public string gender;
        public DateTime dob;
        public string postalCode;
        public string address;
        public string highestEdu;
        public string highedEduRemarks;
        public string spokenLang;
        public string writtenLang;
        public decimal registrationFees;
        public DateTime applDate;
        public string selfSpon;
        public int programmeBatchId;
        public decimal programmedPayable;
        public int subsidyId;
        public string subsidyType;
        public decimal subsidyAmt;
        public string interviewStatus;
        public string shortlistStatus;
        public string blacklistStatus;
        public string rejectStatus;
        public string exempMod;
        public string applicantRemarks;
        public string knowChannel;
        public decimal exemptedCost;

        public string sno;

        public InterviewDetails interviewDetails;
        public List<empRec> employmentRecords;

    }

    public struct TraineeS
    {
        public string applicantId;
        public int programmeBatchId;
        public DateTime PaymentDate;
        public decimal paymentAmt;
        public string paymentMode;
        public decimal gstPayableAmt;
        public string ReferenceNo;
        public string BankInDate;
        public string paymentFor;
        public bool convertBankInDate;

    }

    public class Data_Migration_Management
    {
        DB_Data_Migration dataMig = new DB_Data_Migration();

        public Tuple<bool, string> enrollTrainee(DataRow row, int userId)
        {

            string applicant = row["Applicant"].ToString();
            DateTime EnrollmentDate;


            string[] details = applicant.Split('|');
            string exempMod = "";
            string applicantId = "";
            if (details.Length == 5)
                exempMod = details[4];
            applicantId = details[0];


            if (!DateTime.TryParse(row["EnrolledDate"].ToString(), out EnrollmentDate))
                return new Tuple<bool, string>(false, applicantId + " - Enrollment Date is invalid!");

            try
            {
                if (!(new Finance_Management()).isCseFullPayment(applicantId))
                    return new Tuple<bool, string>(false, applicantId + " -Unable to enroll applicant as full programme payment has not been made.");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, applicantId + " - Unable to enroll applicant as full programme payment has not been made.");
            }

            string r = dataMig.enrollTrainee(applicantId, exempMod, EnrollmentDate, userId);
            if (r == "")
                return new Tuple<bool, string>(false, applicantId + " - Unable to enroll the applicant");
            else
                return new Tuple<bool, string>(true, r);
            //string applicantId, string exemMod, DateTime enrollmentDate,

        }

        public Tuple<bool, string> insertPayment(DataRow row, int userId)
        {
            //List<TraineeS> t = new List<TraineeS>();
            TraineeS traineePayment;

            string[] applicantIdProgrammeBatchId = row["Applicant"].ToString().Split('|');
            string applicant = applicantIdProgrammeBatchId[0];
            int programmeBatchId = int.Parse(applicantIdProgrammeBatchId[2]);
            string mode = row["Mode"].ToString();
            DateTime BankInDate;
            bool bankindateC = false;
            bool paymentdateC = false;
            bool amountC = false;

            if (mode == GeneralLayer.PaymentMode.CHEQ.ToString())
            {
                if (row["BankInDate"].ToString() == "")
                    return new Tuple<bool, string>(false, "Bank In Date is empty. For cheque payment, Bank In Date is needed");
                else
                    bankindateC = DateTime.TryParse(row["BankInDate"].ToString(), out BankInDate);
            }

            string PaymentDate = row["PaymentDate"].ToString();
            DateTime paymentD;

            if (PaymentDate == "")
                return new Tuple<bool, string>(false, "Payment Date is empty");
            else
            {
                paymentdateC = DateTime.TryParse(PaymentDate, out paymentD);
                if (!paymentdateC)
                    return new Tuple<bool, string>(false, "Invalid Payment Date");

            }
            decimal amount = -1;

            string strAmount = row["Amount"].ToString();

            if (strAmount == "")
                return new Tuple<bool, string>(false, "Amount is empty");
            else
            {
                amountC = Decimal.TryParse(row["Amount"].ToString(), out amount);
                if (!amountC)
                    return new Tuple<bool, string>(false, "Amount is invalid");
            }


            decimal GSTPayable = -1;

            string strGstPayable = row["GSTPayableAmt"].ToString();

            if (strGstPayable == "")
                return new Tuple<bool, string>(false, "Amount is empty");
            else
            {
                if (!Decimal.TryParse(row["GSTPayableAmt"].ToString(), out GSTPayable))
                    return new Tuple<bool, string>(false, "Amount is invalid");
            }


            string referenceNum = row["ReferenceNumber"].ToString();
            if (referenceNum.Trim() == "")
                return new Tuple<bool, string>(false, "Reference Number is empty");
            string paymentFor = row["PaymentFor"].ToString();

            if (paymentFor.Trim() == "")
                return new Tuple<bool, string>(false, "Payment For is empty");

            if (paymentFor != GeneralLayer.PaymentType.BOTH.ToString() && paymentFor != GeneralLayer.PaymentType.REG.ToString() &&
                paymentFor != GeneralLayer.PaymentType.PROG.ToString() && paymentFor != GeneralLayer.PaymentType.MAKEUP.ToString())
                return new Tuple<bool, string>(false, "Payment For is Invalid");


            traineePayment = new TraineeS()
            {
                applicantId = applicant,
                programmeBatchId = programmeBatchId,
                PaymentDate = paymentD,
                paymentAmt = amount,
                paymentMode = mode,
                ReferenceNo = referenceNum,
                gstPayableAmt = GSTPayable,
                BankInDate = row["BankInDate"].ToString(),
                paymentFor = paymentFor,
                convertBankInDate = bankindateC

            };

            return dataMig.insertPayment(traineePayment, userId);




        }
        public Tuple<bool, string> insertSubsidy(DataRow row, int userId)
        {
            //Find the programme id from programme structure 
            int version = -1;
            try
            {
                version = int.Parse(row["Version"].ToString());

            }
            catch (Exception ex)
            {

                return new Tuple<bool, string>(false, "Invalid Version No.");
            }

            string programmeCode = row["ProgrammeCode"].ToString();
            string CourseCode = row["CourseCode"].ToString();
            string scheme = row["Scheme"].ToString();
            string subType = row["Type"].ToString();
            decimal value;

            DateTime effectiveDate;

            if (!DateTime.TryParse(row["EffectiveDate"].ToString(), out effectiveDate))
                return new Tuple<bool, string>(false, "Invalid Effective Date");

            if (!Decimal.TryParse(row["Value"].ToString(), out value))
                return new Tuple<bool, string>(false, "Invalid Value");


            int programmeId = dataMig.findProgramme(version, programmeCode, CourseCode);



            if (programmeId == -1)
                return new Tuple<bool, string>(false, "Unable to find the Programme Id");
            else
            {
                if ((new DB_Finance()).isExistingSubsidy(scheme, programmeId))
                    return new Tuple<bool, string>(false, scheme + " is an Existing Scheme!");
                else
                {
                    if (dataMig.insertSubsidy(programmeId, scheme, effectiveDate, subType, value, 1))
                        return new Tuple<bool, string>(true, scheme + " inserted successfully");
                    else
                        return new Tuple<bool, string>(false, scheme + " unable to insert successfully");

                }
            }
        }
        public Tuple<bool, string> processSOA(DataRow row, int userId)
        {
            string assessmentCompleted = row["assessmentCompleted"].ToString();
            int ProgrammeBatchId = int.Parse(row["ProgrammeBatchId"].ToString());


            string moduleResult = row["ModuleResult"].ToString();

            if (moduleResult != GeneralLayer.ModuleResult.C.ToString() && moduleResult != GeneralLayer.ModuleResult.NYC.ToString() && moduleResult != GeneralLayer.ModuleResult.PA.ToString())
                return new Tuple<bool, string>(false, "Module Result is not valid.");

            string firstAssessmentDate = row["FirstAssessmentDate"].ToString();
            if (firstAssessmentDate.Trim().Length == 0)
                return new Tuple<bool, string>(false, "First Assessment Date is Missing");

            string accessorName = row["AccessorName"].ToString();
            int accessorId = -1;

            if (dataMig.getUserId(accessorName) == -1)
                return new Tuple<bool, string>(false, "Accessor Not Found");
            else
                accessorId = dataMig.getUserId(accessorName);

            string reAssessmentRequired = row["ReAssessmentRequired"].ToString();
            //string ReTakeRequired = row["ReTakeRequired"].ToString();
            //string RetakeModuleCode = row["RetakeModuleCode"].ToString();

            //int reTakeModuleCodeId = -1;

            //if(dataMig.getModuleId(RetakeModuleCode) == -1)
            //    return new Tuple<bool, string>(false, "Retake Module Code Not Found");
            //else

            string FinalAccessorName = row["FinalAccessorName"].ToString();

            string FinalAssessmentDate = row["FinalAssessmentDate"].ToString();
            int finalAccessorId = -1;

            if (reAssessmentRequired == General_Constance.STATUS_YES.ToString())
            {
                if (FinalAssessmentDate.Trim().Length == 0)
                    return new Tuple<bool, string>(false, "Reassessment is required. Final Assessment Date is Required");

                if (FinalAccessorName.Trim().Length == 0)
                    return new Tuple<bool, string>(false, "Reassessment is required. Final Assessor is Required");

                if (dataMig.getUserId(FinalAccessorName) == -1)
                    return new Tuple<bool, string>(false, " Final Accessor Not Found");
                else
                    finalAccessorId = dataMig.getUserId(FinalAccessorName);
            }

            string soaModStatus = row["SOAStatus"].ToString();

            if (soaModStatus != GeneralLayer.SOAStatus.NYA.ToString() && soaModStatus != GeneralLayer.SOAStatus.PROC.ToString() && soaModStatus != GeneralLayer.SOAStatus.RSOA.ToString())
                return new Tuple<bool, string>(false, "SOA Status can only be NYA, PROC or RSOA");
            string SOAProcessedDate = row["SOAProcessedDate"].ToString();
            string SOAReceivedDate = row["SOAReceivedDate"].ToString();
            if (soaModStatus == GeneralLayer.SOAStatus.RSOA.ToString() && (SOAReceivedDate.Trim().Length == 0 || SOAProcessedDate.Trim().Length == 0))
                return new Tuple<bool, string>(false, "SOA Received Date or Processed Date is Empty");

            if (soaModStatus == GeneralLayer.SOAStatus.PROC.ToString() && SOAProcessedDate.Trim().Length == 0)
                return new Tuple<bool, string>(false, "SOA Processed Date is Empty");


            string TraineeID = row["TraineeID"].ToString();
            string ClassCode = row["ClassCode"].ToString();
            int ModuleId = int.Parse(row["ModuleId"].ToString());
            int BatchModuleId = int.Parse(row["BatchModuleId"].ToString());


            bool result = dataMig.updateSOA(assessmentCompleted, moduleResult, firstAssessmentDate, accessorId,
                finalAccessorId, soaModStatus, SOAProcessedDate, SOAReceivedDate, TraineeID, ClassCode, ModuleId, BatchModuleId, FinalAssessmentDate, ProgrammeBatchId, reAssessmentRequired, userId);


            if (result)
                return new Tuple<bool, string>(true, "SOA Successfully Processed for " + TraineeID);
            else
                return new Tuple<bool, string>(false, "Unable to Process SOA for " + TraineeID);

        }

        public Tuple<bool, string> generateSOAExcel(string path, List<String> lsTraineeId, int userId)
        {

            if (path == null) return new Tuple<bool, string>(false, "Path is not specified.");
            string fileName = "DataMigration_SOA_" + userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";

            Cryptography crypt = new Cryptography();
            using (StreamWriter sw = File.CreateText(path + fileName))
            {
                sw.WriteLine("\"Full Name\",\"TraineeID\",\"IDNumber\",\"ClassCode\",\"ModuleId\",\"BatchModuleId\",\"ModuleTitle\",\"ModuleCode\",\"ModuleResult\",\"ProgrammeBatchId\"," +
                                "\"assessmentCompleted\",\"FirstAssessmentDate\",\"AccessorName\",\"ReAssessmentRequired\",\"ReTakeRequired\",\"RetakeModuleCode\",\"RetakeClassCode\",\"FinalAssessmentDate\"," +
                                "\"FinalAccessorName\",\"SOAStatus\",\"SOAProcessedDate\",\"SOAReceivedDate\"");
                foreach (string tId in lsTraineeId)
                {

                    //select t.fullName, t.idNumber, pb.batchCode, tm.traineeid, tm.programmeBatchId, tm.moduleid, tm.batchModuleId, ms.moduleTitle, ms.modulecode, 
                    //                      tm.moduleResult
                    try
                    {
                        DataTable dt = dataMig.generateSOAExcel(tId);
                        if (dt == null) return new Tuple<bool, string>(false, "Error retrieving data.");
                        if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No records retrieved.");



                        foreach (DataRow dr in dt.Rows)
                        {
                            //sw.WriteLine("\"" + crypt.decryptInfo(dr["fullName"].ToString()) + "\",\"" + dr["traineeid"].ToString() + " \",\"" + crypt.decryptInfo(dr["idNumber"].ToString()) + "\",\"" + dr["batchCode"].ToString()
                            //            + "\", \"" + dr["moduleid"].ToString() + "\", \"" + dr["batchModuleId"].ToString() + "\", \"" + dr["moduleTitle"].ToString() + "\", \"" + dr["modulecode"].ToString() + "\", \"" 
                            //            + dr["courseCode"].ToString() + "\"," + "\"" + "" + "\", \"" + dr["courseCode"].ToString() + "\", \"" + dr["courseCode"].ToString() + "\", \""
                            //            + dr["moduleResult"].ToString() + "\", \"" + dr["courseCode"].ToString() + "\", \"" + dr["courseCode"].ToString() + "\", \"" + dr["courseCode"].ToString() + "\", " 
                            //            + "\"" + dr["courseCode"].ToString() + "\", \"" + dr["courseCode"].ToString() + "\", \"" + dr["courseCode"].ToString() + "\",\"" + dr["courseCode"].ToString() + "\"");

                            sw.WriteLine("\"" + dr["fullName"].ToString() + "\",\"" + dr["traineeid"].ToString() + "\",\"" + crypt.decryptInfo(dr["idNumber"].ToString()) + "\",\"" + dr["batchCode"].ToString()
                                        + "\",\"" + dr["moduleid"].ToString() + "\",\"" + dr["batchModuleId"].ToString() + "\",\"" + dr["moduleTitle"].ToString() + "\",\"" + dr["modulecode"].ToString() + "\",\""
                                        + dr["moduleResult"].ToString() + "\",\"" + dr["programmeBatchId"].ToString() + "\"");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log_Handler lh = new Log_Handler();
                        lh.WriteLog(ex, "Report_Management.cs", "genCourseFeeReceived()", ex.Message, -1);

                        if (File.Exists(path + fileName)) File.Delete(path + fileName);

                        return new Tuple<bool, string>(false, "Error generating report.");
                    }


                }
            }


            return new Tuple<bool, string>(true, fileName);

        }

        public Tuple<bool, string, string> insertApplicant(DataRow dtApplicant, int userId)
        {

            string sno = dtApplicant["Sno"].ToString().Trim();
            string nric = dtApplicant["ICNumber"].ToString().Trim();
            string name = dtApplicant["ApplicantName"].ToString().Trim();
            string idtype = dtApplicant["IDType"].ToString().Trim();

            string idTypeNo = dataMig.getCodeValue("IDTYPE", idtype);

            if (idTypeNo == "" || idTypeNo == null)
                return new Tuple<bool, string, string>(false, sno + "- Invalid ID Type", "");

            if (nric == "" || nric == null)
                return new Tuple<bool, string, string>(false, sno + "- Nric Is empty", "");

            string contactNum1 = dtApplicant["ContactNumber1"].ToString().Trim();

            if (contactNum1 == "" || contactNum1 == null)
                return new Tuple<bool, string, string>(false, sno + "- Contact Number 1 Not Specified", "");

            string contactNum2 = dtApplicant["ContactNumber2"].ToString().Trim();

            string nationality = dataMig.getCodeValue("NATION", dtApplicant["Nationality"].ToString().Trim());
            string race = dataMig.getCodeValue("RACE", dtApplicant["Race"].ToString().Trim());
            string highestEdu = dataMig.getCodeValue("EDU", dtApplicant["HighestEducation"].ToString().Trim());
            string engSpoken = dataMig.getCodeValue("LANGPR", dtApplicant["EnglishSpoken"].ToString().Trim());
            string chnSpoken = dataMig.getCodeValue("LANGPR", dtApplicant["ChineseSpoken"].ToString().Trim());
            string engWritten = dataMig.getCodeValue("LANGPR", dtApplicant["EnglishWritten"].ToString().Trim());
            string chnWritten = dataMig.getCodeValue("LANGPR", dtApplicant["ChineseWritten"].ToString().Trim());

            if (nationality == "" || race == "" || highestEdu == "" || engSpoken == "" || chnSpoken == "" || engWritten == "" || engSpoken == "")
                return new Tuple<bool, string, string>(false, sno + "- Something wrong with the data! Please checked.", "");

            string spokenPro = "ENG:" + engSpoken + ";CHIN:" + chnSpoken;
            string writtenPro = "ENG:" + engWritten + ";CHIN:" + chnWritten;
            string gender = dtApplicant["Gender"].ToString().Trim();
            string address = dtApplicant["Address"].ToString().Trim();
            string postalCode = dtApplicant["PostalCode"].ToString().Trim();

            if (postalCode == "" || address == "")
                return new Tuple<bool, string, string>(false, sno + "- Something wrong with address", "");
            decimal regFees = -1;
            if (dtApplicant["RegistrationFees"].ToString() != "")
                Decimal.TryParse(dtApplicant["RegistrationFees"].ToString(), out regFees);

            string classCode = dtApplicant["ClassCode"].ToString().Trim();
            string subsidyScheme = dtApplicant["SubsidyScheme"].ToString().Trim();
            string interviewStatus = dtApplicant["InterviewStatus"].ToString().Trim();
            string applicationRemarks = dtApplicant["ApplicationRemarks"].ToString().Trim();
            string highestEduRemarks = dtApplicant["HighestEducationRemarks"].ToString().Trim();
            string emailAddress = dtApplicant["EmailAddress"].ToString().Trim();
            string getToKnowChannel = dtApplicant["GetToKnowChannel"].ToString().Trim();
            int programmeBatchId = -1;

            string emailReg = "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";
            if (emailAddress != "")
            {
                if (!Regex.IsMatch(emailAddress, emailReg))
                    return new Tuple<bool, string, string>(false, sno + " - Invalid Email", "");
            }
            int subId = -1;
            string subsidyType = "";


            decimal subsidyVal = 0;

            Tuple<string, int> programmeRs = dataMig.getProgrammeId(classCode);
            if (programmeRs.Item2 == -1)
                return new Tuple<bool, string, string>(false, sno + " - " + programmeRs.Item1, "");
            else
                programmeBatchId = programmeRs.Item2;

            if (subsidyScheme.Trim().Length > 0)
            {
                Tuple<bool, string, decimal, int> result = dataMig.getSubsidy(subsidyScheme);
                if (result.Item1)
                {
                    subId = result.Item4;
                    subsidyVal = result.Item3;
                    subsidyType = result.Item2;
                }
                else
                    return new Tuple<bool, string, string>(false, result.Item2, "");
            }


            InterviewDetails intDetails = new InterviewDetails();

            if (interviewStatus == GeneralLayer.InterviewStatus.FAILED.ToString() || interviewStatus == GeneralLayer.InterviewStatus.PASSED.ToString())
            {
                string interviewerName = dtApplicant["InterviewerName"].ToString();

                DateTime interviewedDate = Convert.ToDateTime(dtApplicant["InterviewerDate"].ToString());

                string interviewRemarks = dtApplicant["InterviewRemarks"].ToString();
                int interviewerId = -1;

                if (interviewerName == "")
                    return new Tuple<bool, string, string>(false, sno + "- Interviewer Name is empty", "");

                if (dataMig.getUserId(interviewerName) == -1)
                    return new Tuple<bool, string, string>(false, sno + "- Interviewer Not Found in Database", "");

                interviewerId = dataMig.getUserId(interviewerName);

                intDetails = new InterviewDetails()
                {
                    interviewerId = interviewerId,
                    interviewedDate = interviewedDate,
                    interviewRemarks = interviewRemarks
                };
            }

            string shortListed = dtApplicant["ShortListed"].ToString();
            string rejected = dtApplicant["Rejected"].ToString();

            string exemptedModule = dtApplicant["ExemptedModuleCode"].ToString();

            string[] allExemModule = exemptedModule.Split(';');
            string exmpModulesId = "";
            decimal exemptedCost = 0;

            foreach (string mod in allExemModule)
            {
                if (mod.Trim().Length > 0)
                {
                    Tuple<bool, string, int, decimal> modId = dataMig.getModuleId(classCode, mod);

                    if (modId.Item1)
                    {
                        exmpModulesId += modId.Item3 + ";";
                        exemptedCost += modId.Item4;
                    }
                    else
                        return new Tuple<bool, string, string>(false, modId.Item2, "");
                }
            }

            if (exmpModulesId.Length > 0)
                //remove the last ";"
                exmpModulesId = exmpModulesId.Substring(0, exmpModulesId.Length - 1);

            string sponsored = dtApplicant["Sponsored"].ToString();



            List<empRec> lsEmploymentRecord = new List<empRec>();
            if (sponsored == GeneralLayer.Sponsorship.COMP.ToString())
            {

                //empRec PrevRecord;
                empRec CurrRecord;

                string companyName = dtApplicant["CurrCompanyName"].ToString();
                string companyDept = dtApplicant["CurrCompanyDepartment"].ToString();
                decimal salaryAmt = dtApplicant["CurrSalaryAmount"].ToString() == "" ? 0 : Convert.ToDecimal(dtApplicant["CurrSalaryAmount"].ToString());
                string empStatusRaw = dtApplicant["CurrEmploymentStatus"].ToString();



                if (dtApplicant["CurrStartDate"].ToString() == "")
                    return new Tuple<bool, string, string>(false, sno + "- Applicant's Current Employment Start Date is missing.", "");
                DateTime startDate = Convert.ToDateTime(dtApplicant["CurrStartDate"].ToString());
                DateTime endDate = DateTime.MaxValue;
                string occpationTypeRaw = dtApplicant["CurrOccupationType"].ToString().Trim();
                string designation = dtApplicant["CurrDesignation"].ToString();

                if (companyName == "" || companyDept == "" || salaryAmt == 0 || empStatusRaw == "" || occpationTypeRaw == "" || designation == "")
                    return new Tuple<bool, string, string>(false, sno + "- Applicant's Current Employment Information is missing.", "");

                string empStatus = dataMig.getCodeValue("EMSTAT", empStatusRaw.Trim());
                string occpationType = dataMig.getCodeValue("EMJOB", occpationTypeRaw.Trim());

                //string PrevcompanyName = dtApplicant["PrevCompanyName"].ToString();

                //if (PrevcompanyName.Length != 0)
                //{
                //    string PrevcompanyDept = dtApplicant["PrevCompanyDepartment"].ToString();
                //    decimal PrevsalaryAmt = dtApplicant["PrevSalaryAmount"].ToString() == "" ? 0 : Convert.ToDecimal(dtApplicant["PrevSalaryAmount"].ToString());
                //    string PrevempStatus = dtApplicant["PrevEmploymentStatus"].ToString();

                //    if (dtApplicant["PrevStartDate"].ToString() == "")
                //        return new Tuple<bool, string>(false, "Applicant's Previous Employment Start Date is missing.");

                //    if (dtApplicant["PrevEndDate"].ToString() == "")
                //        return new Tuple<bool, string>(false, "Applicant's Previous Employment End Date is missing.");

                //    DateTime PrevstartDate = Convert.ToDateTime(dtApplicant["PrevStartDate"].ToString());
                //    DateTime PrevendDate = Convert.ToDateTime(dtApplicant["PrevEndDate"].ToString());
                //    string PrevoccpationType = dtApplicant["PrevOccupationType"].ToString();
                //    string Prevdesignation = dtApplicant["PrevDesignation"].ToString();

                //    if (PrevcompanyName == "" || PrevcompanyDept == "" || PrevsalaryAmt == 0 || PrevempStatus == "" || PrevoccpationType == "" || Prevdesignation == "")
                //        return new Tuple<bool, string>(false, "Applicant Previous Employment Details cannot be empty");

                //    if (PrevstartDate > PrevendDate)
                //        return new Tuple<bool, string>(false, "Applicant Previous Employment Start Date is later than Previous Employment End Date");

                //    if (PrevendDate > startDate)
                //        return new Tuple<bool, string>(false, "Applicant Previous Employment End Date is later than Previous Current Employment Start Date");

                //    PrevRecord = new empRec()
                //    {
                //        companyName = PrevcompanyName,
                //        dept = PrevcompanyDept,
                //        designation = Prevdesignation,
                //        status = (EmplStatus)Enum.Parse(typeof(EmplStatus), PrevempStatus),
                //        occupationType = int.Parse(PrevoccpationType),//int.Parse(dataMig.getCodeValue("EMJOB", PrevoccpationType)),
                //        salary = PrevsalaryAmt,
                //        dtStart = PrevstartDate,
                //        dtEnd = PrevendDate,
                //        current = false
                //    };

                //    lsEmploymentRecord.Add(PrevRecord);
                //}

                CurrRecord = new empRec()
                {
                    companyName = companyName,
                    dept = companyDept,
                    designation = designation,
                    status = (EmplStatus)Enum.Parse(typeof(EmplStatus), empStatus),
                    occupationType = int.Parse(occpationType),//int.Parse(dataMig.getCodeValue("EMJOB", occpationType)),
                    salary = salaryAmt,
                    dtStart = startDate,
                    dtEnd = endDate,
                    current = true
                };



                lsEmploymentRecord.Add(CurrRecord);


            }

            ;
            DateTime applicantionDate;
            DateTime dob;
            //DateTime enrollmentDate;



            try
            {
                dob = Convert.ToDateTime(dtApplicant["BirthDate"].ToString());
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string, string>(false, sno + " - Date Of Birth Is invalid", "");
            }
            try
            {
                applicantionDate = Convert.ToDateTime(dtApplicant["ApplicationDate"].ToString());
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string, string>(false, sno + " - Application Date Is invalid", "");
            }

            //try
            //{
            //    enrollmentDate = Convert.ToDateTime(dtApplicant["EnrollmentDate"].ToString());
            //}
            //catch (Exception ex)
            //{
            //    return new Tuple<bool, string>(false, sno + " - Enrollment Date Is invalid");

            //}

            if (!IsNRICValid(nric, idtype))
            {
                return new Tuple<bool, string, string>(false, sno + " - " + nric + " is an invalid NRIC/FIN", "");
            }

            Cryptography crypt = new Cryptography();
            ApplicantRec applicant = new ApplicantRec()
            {
                Name = name,
                idNumber = crypt.encryptInfo(nric),
                idType = idTypeNo,
                nationality = nationality,
                race = race,
                contactNo1 = crypt.encryptInfo(contactNum1),
                contactNo2 = contactNum2 == "" ? "" : crypt.encryptInfo(contactNum2),
                emailAddress = emailAddress == "" ? "" : crypt.encryptInfo(emailAddress),
                gender = gender,
                dob = dob,
                postalCode = crypt.encryptInfo(postalCode),
                address = crypt.encryptInfo(address),
                highestEdu = highestEdu,
                highedEduRemarks = highestEduRemarks,
                spokenLang = spokenPro,
                writtenLang = writtenPro,
                registrationFees = regFees,
                applDate = applicantionDate,
                selfSpon = sponsored,
                programmeBatchId = programmeBatchId,
                programmedPayable = 0,
                subsidyId = subId,
                subsidyAmt = subsidyVal,
                subsidyType = subsidyType,
                interviewStatus = interviewStatus,
                shortlistStatus = shortListed,
                blacklistStatus = "N",
                rejectStatus = rejected,
                exempMod = exmpModulesId,
                applicantRemarks = applicationRemarks,
                interviewDetails = intDetails,
                employmentRecords = lsEmploymentRecord,
                knowChannel = getToKnowChannel,
                exemptedCost = exemptedCost,
                sno = sno
            };
            Tuple<bool, string> regR = new Tuple<bool,string>(false, "");
            Tuple<bool, string> paymentR = new Tuple<bool,string>(false, "");

            Tuple<bool, applicantResult> insertR = dataMig.insertApplicant(applicant, userId);
            if (insertR.Item1)
            {
                string mode = dtApplicant["Mode"].ToString();
                DateTime BankInDate;
                bool bankindateC = false;
                bool paymentdateC = false;
                bool amountC = false;

                if (mode == GeneralLayer.PaymentMode.CHEQ.ToString())
                {
                    if (dtApplicant["BankInDate"].ToString() == "")
                        return new Tuple<bool, string, string>(false, applicant.sno + " - Bank In Date is empty. For cheque payment, Bank In Date is needed", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                    else
                        bankindateC = DateTime.TryParse(dtApplicant["BankInDate"].ToString(), out BankInDate);
                }

                string PaymentDate = dtApplicant["PaymentDate"].ToString();
                DateTime paymentD;

                if (PaymentDate == "")
                    return new Tuple<bool, string, string>(false, applicant.sno + " - Payment Date is empty", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                else
                {
                    paymentdateC = DateTime.TryParse(PaymentDate, out paymentD);
                    if (!paymentdateC)
                        return new Tuple<bool, string, string>(false, applicant.sno + " - Invalid Payment Date", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));

                }
                decimal amount = -1;

                string strAmount = dtApplicant["Amount"].ToString();

                if (strAmount == "")
                    return new Tuple<bool, string, string>(false, applicant.sno + " - Amount is empty", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                else
                {
                    amountC = Decimal.TryParse(dtApplicant["Amount"].ToString(), out amount);
                    if (!amountC)
                        return new Tuple<bool, string, string>(false, applicant.sno + " - Amount is invalid", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                }


                decimal GSTPayable = -1;

                string strGstPayable = dtApplicant["GSTPayableAmt"].ToString();

                if (strGstPayable == "")
                    return new Tuple<bool, string, string>(false, applicant.sno + " - Amount is empty", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                else
                {
                    if (!Decimal.TryParse(dtApplicant["GSTPayableAmt"].ToString(), out GSTPayable))
                        return new Tuple<bool, string, string>(false, applicant.sno + " - Amount is invalid", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                }


                string referenceNum = dtApplicant["ReferenceNumber"].ToString();
                if (referenceNum.Trim() == "")
                    return new Tuple<bool, string, string>(false, applicant.sno + " - Reference Number is empty",(insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                string paymentFor = dtApplicant["PaymentFor"].ToString();

                if (paymentFor.Trim() == "")
                    return new Tuple<bool, string, string>(false, applicant + " - Payment For is empty", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));

                if (paymentFor != GeneralLayer.PaymentType.BOTH.ToString() && paymentFor != GeneralLayer.PaymentType.REG.ToString() &&
                    paymentFor != GeneralLayer.PaymentType.PROG.ToString() && paymentFor != GeneralLayer.PaymentType.MAKEUP.ToString())
                    return new Tuple<bool, string, string>(false, applicant.sno + " - Payment For is Invalid",(insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));


                TraineeS traineePayment = new TraineeS()
                 {
                     applicantId = insertR.Item2.applicantId,
                     programmeBatchId = applicant.programmeBatchId,
                     PaymentDate = paymentD,
                     paymentAmt = amount,
                     paymentMode = mode,
                     ReferenceNo = referenceNum,
                     gstPayableAmt = GSTPayable,
                     BankInDate = dtApplicant["BankInDate"].ToString(),
                     paymentFor = paymentFor,
                     convertBankInDate = bankindateC

                 };

                paymentR = dataMig.insertPayment(traineePayment, userId);

                if (dtApplicant["RegAmt"].ToString() != "")
                {
                    decimal regAmt = 0;
                    DateTime regDate;
                    if (Decimal.TryParse(dtApplicant["RegAmt"].ToString(), out regAmt))
                    {
                        return new Tuple<bool, string, string>(false, "Registration Amount is not decimal", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                    }
                    if (DateTime.TryParse(dtApplicant["RegDate"].ToString(), out regDate))
                    {
                        return new Tuple<bool, string, string>(false, "Registration Date is not Wrong", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                    }
                    if (dtApplicant["RegMode"].ToString() == "")
                    {
                        return new Tuple<bool, string, string>(false, "Registration Mode is not empty", (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod));
                    }

                    TraineeS tReg = new TraineeS()
                    {
                        applicantId = insertR.Item2.applicantId,
                        programmeBatchId = applicant.programmeBatchId,
                        PaymentDate = regDate,
                        paymentAmt = regAmt,
                        paymentMode = dtApplicant["RegMode"].ToString(),
                        ReferenceNo = referenceNum,
                        BankInDate = dtApplicant["BankInDate"].ToString(),
                        paymentFor = "REG",
                        convertBankInDate = bankindateC

                    };

                   regR = dataMig.insertPayment(tReg, userId);

                }
                return new Tuple<bool, string, string>(insertR.Item1, (insertR.Item2.applicantId + "|" + applicant.Name + "|" + applicant.programmeBatchId + "|" + classCode + "|" + insertR.Item2.exemMod), paymentR.Item2.ToString() + " " + regR.Item2.ToString());
            }
            else
                return new Tuple<bool, string, string>(insertR.Item1, insertR.Item2.msg, paymentR.Item2.ToString() + " " + regR.Item2.ToString());
        }
        public Tuple<bool, string, string>[] InsertIntoModules(DataTable dtModule, int userId)
        {
            Tuple<bool, string, string>[] rs = new Tuple<bool, string, string>[dtModule.Rows.Count];

            for (int i = 0; i < dtModule.Rows.Count; i++)
            {
                string moduleCode = dtModule.Rows[i]["Code"].ToString();
                string moduleTitle = dtModule.Rows[i]["Title"].ToString();
                int moduleVersion = int.Parse(dtModule.Rows[i]["Version"].ToString());
                string moduleLevel = dtModule.Rows[i]["Level"].ToString();
                string moduleDescription = dtModule.Rows[i]["Description"].ToString();
                int moduleCredit = int.Parse(dtModule.Rows[i]["Credit"].ToString());
                decimal moduleCost = decimal.Parse(dtModule.Rows[i]["Cost"].ToString());
                DateTime moduleEffectDate = Convert.ToDateTime(dtModule.Rows[i]["EffectiveDate"].ToString());
                decimal moduleTrainingHour = decimal.Parse(dtModule.Rows[i]["TrainingHours"].ToString());
                string WSQCompetencyCode = dtModule.Rows[i]["WSQCompetencyCode"].ToString();

                rs[i] = dataMig.insertIntoModule(moduleCode, moduleVersion, moduleLevel, moduleTitle, moduleDescription, moduleCredit, moduleCost, moduleEffectDate, moduleTrainingHour, WSQCompetencyCode, 1);
            }

            return rs;
        }

        public Tuple<bool, string> insertBundle(string bundleCode, string bundleType, DataTable dtBundleMod, int userId)
        {
            return dataMig.createBundle(bundleCode, bundleType, dtBundleMod, userId);
        }


        public Tuple<bool, string, int> insertProgrammeBatch(DataRow dtProgrammeBatch, int userId)
        {
            if (!dataMig.checkClassValue(dtProgrammeBatch["LessonType"].ToString()))
            {
                return new Tuple<bool, string, int>(false, "Wrong Class Type for " + dtProgrammeBatch["ProgrammeCode"].ToString(), -1);
            }
            if (!(new DB_Programme()).checkProgrammeCodeExist(dtProgrammeBatch["ProgrammeCode"].ToString()))
            {
                return new Tuple<bool, string, int>(false, "Programme Code Not Found. Unable to insert for " + dtProgrammeBatch["ProgrammeCode"].ToString(), -1);
            }

            string classCode = dtProgrammeBatch["ClassCode"].ToString();
            string lessonType = dtProgrammeBatch["LessonType"].ToString().ToUpper();

            if ((new DB_Programme()).checkBatchCodeExist(classCode, lessonType))
                return new Tuple<bool, string, int>(false, "Batch Code exists. -> " + classCode, -1);



            return dataMig.createProgrammeBatch(dtProgrammeBatch, userId);
        }

        public Tuple<bool, string> insertBatchModule(DataTable dtBatchModule, DataTable dtSessionDetails, int userId, int programmeBatchId)
        {

            foreach (DataRow drBatchMod in dtBatchModule.Rows)
            {
                string modCode = drBatchMod["ModuleCode"].ToString();
                string day = drBatchMod["Day"].ToString();
                DateTime startDate = Convert.ToDateTime(drBatchMod["StartDate"].ToString());
                DateTime endDate = Convert.ToDateTime(drBatchMod["EndDate"].ToString());

                int trainer1 = -1;
                int trainer2 = -1;
                int assessor = -1;

                if (drBatchMod["Trainer1"].ToString().Trim() != "")
                {
                    trainer1 = dataMig.getUserId(drBatchMod["Trainer1"].ToString());
                    if (trainer1 == -1)
                        return new Tuple<bool, string>(false, "Unable to find trainer1");
                }
                if (drBatchMod["Trainer2"].ToString().Trim() != "")
                {
                    trainer2 = dataMig.getUserId(drBatchMod["Trainer2"].ToString().Trim());
                    if (trainer2 == -1)
                        return new Tuple<bool, string>(false, "Unable to find trainer2");
                }
                if (drBatchMod["Accessor"].ToString().Trim() != "")
                {
                    assessor = dataMig.getUserId(drBatchMod["Accessor"].ToString().Trim());
                    if (assessor == -1)
                        return new Tuple<bool, string>(false, "Unable to find assessor");
                }


                string convertedDay = "";

                string[] words = day.Split(',');
                foreach (string word in words)
                {
                    string[] spiltW = word.Split(';');
                    if (spiltW[0].ToUpper() == "MON")
                        convertedDay += "1/" + spiltW[1] + ";";
                    if (spiltW[0].ToUpper() == "TUE")
                        convertedDay += "2/" + spiltW[1] + ";";
                    if (spiltW[0].ToUpper() == "WED")
                        convertedDay += "3/" + spiltW[1] + ";";
                    if (spiltW[0].ToUpper() == "THU")
                        convertedDay += "4/" + spiltW[1] + ";";
                    if (spiltW[0].ToUpper() == "FRI")
                        convertedDay += "5/" + spiltW[1] + ";";
                    if (spiltW[0].ToUpper() == "SAT")
                        convertedDay += "6/" + spiltW[1] + ";";
                    if (spiltW[0].ToUpper() == "SUN")
                        convertedDay += "7/" + spiltW[1] + ";";
                }

                if (convertedDay.Trim().Length <= 0)
                    return new Tuple<bool, string>(false, "Day is wrong.");

                convertedDay = convertedDay.Remove(convertedDay.Length - 1);
                DataRow[] sessiondetails = dtSessionDetails.Select("ModuleCode='" + modCode + "'");
                dataMig.createBatchModule(modCode, convertedDay, startDate, endDate, userId, programmeBatchId, trainer1, trainer2, assessor, sessiondetails);

                //if (dataMig.createBatchModule(modCode, convertedDay, startDate, endDate, userId, programmeBatchId, trainer1, trainer2, assessor, sessiondetails).Item1)
                //    return new Tuple<bool, string>(true, "Success");
                //else
                //    return new Tuple<bool, string>(false, "Not successful");
            }

            return new Tuple<bool, string>(false, "Not successful");

        }

        public Tuple<bool, string> insertProgramme(DataRow dtProgrammeRow, int userId)
        {
            string programmeCode = dtProgrammeRow["ProgrammeCode"].ToString();
            string courseCode = dtProgrammeRow["CourseCode"].ToString();
            int programmeVersion = int.Parse(dtProgrammeRow["Version"].ToString());
            string programmeLevel = dtProgrammeRow["Level"].ToString();
            string programmeTitle = dtProgrammeRow["Title"].ToString();
            string programmeDescription = dtProgrammeRow["Description"].ToString();
            int numOfSOA = int.Parse(dtProgrammeRow["NumOfSOA"].ToString());
            string SSGRefNum = dtProgrammeRow["SSGReferenceNum"].ToString();
            string programmeType = dtProgrammeRow["Type"].ToString();
            string bundleCode = dtProgrammeRow["BundleCode"].ToString();
            string programmeCategory = dtProgrammeRow["Category"].ToString();

            if (!(new DB_Programme().checkProgrammeCodeExist(programmeCode)))
            {
                if (dataMig.createProgramme(programmeCode, courseCode, programmeVersion, programmeLevel, programmeCategory, programmeTitle, programmeDescription, numOfSOA, SSGRefNum, bundleCode, programmeType, userId))
                    return new Tuple<bool, string>(true, "Programme Code: " + programmeCode + " Inserted successfully");
                else
                    return new Tuple<bool, string>(false, ("Programme Code: " + programmeCode + " not inserted."));

            }

            else
                return new Tuple<bool, string>(false, "Programme Code: " + programmeCode + " already exist.");

        }

        protected bool IsNRICValid(string nric, string idType)
        {

            if (idType == GeneralLayer.IDType.Oth.ToString())
            {
                return true;
            }
            else
            {


                string ic = nric;
                if (ic.Length != 9)
                {
                    return false;

                }
                else
                {


                    int[] icArray = new int[7];
                    string firstChar = ic[0].ToString();
                    string lastChar = ic[8].ToString();

                    for (int i = 0; i < 7; i++)
                    {
                        try
                        {
                            icArray[i] = int.Parse(ic[i + 1].ToString());
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }

                    icArray[0] *= 2;
                    icArray[1] *= 7;
                    icArray[2] *= 6;
                    icArray[3] *= 5;
                    icArray[4] *= 4;
                    icArray[5] *= 3;
                    icArray[6] *= 2;

                    int weight = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        weight += icArray[j];
                    }


                    int offset = (firstChar == "T" || firstChar == "G") ? 4 : 0;
                    int temp = (offset + weight) % 11;
                    string[] st = new string[] { "J", "Z", "I", "H", "G", "F", "E", "D", "C", "B", "A" };
                    string[] fg = new string[] { "X", "W", "U", "T", "R", "Q", "P", "N", "M", "L", "K" };
                    string theAlpha = "";
                    if (firstChar == "S" || firstChar == "T")
                    {
                        theAlpha = st[temp];
                    }
                    else if (firstChar == "F" || firstChar == "G")
                    {
                        theAlpha = fg[temp];
                    }

                    if (lastChar != theAlpha)
                    {
                        return false;
                    }
                    else
                    {

                        return true;
                    }
                }
            }

        }




    }
}
