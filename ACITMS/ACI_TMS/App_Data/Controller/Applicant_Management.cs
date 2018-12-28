using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;

namespace LogicLayer
{

    public struct LanguageProficiency
    {
        public string lang;
        public Proficiency spoken;
        public Proficiency written;
    }

    public struct EmploymentRecord
    {
        public string companyName;
        public string dept;
        public string designation;
        public EmplStatus status;
        public EmplOccupation occupationType;
        public decimal salary;
        public DateTime dtStart;
        public DateTime dtEnd;
    }

    public struct EmploymentHistory
    {
        public string companyName;
        public string dept;
        public string designation;
        public string status;
        public string occupationType;
        public decimal salary;
        public DateTime dtStart;
        public DateTime dtEnd;
        public string current;
    }

    public class Applicant_Management
    {
        private DB_Applicant dbApplicant = new DB_Applicant();

        public DataTable getSponsorship()
        {
            return dbApplicant.getSponsorship();
        }

        public DataTable getEmploymentJob()
        {
            return dbApplicant.getEmploymentJob();
        }

        public DataTable getEmploymentStatus()
        {
            return dbApplicant.getEmploymentStatus();
        }

        public DataTable getApplicantDetailsForPayment(string applicantId)
        {
            return dbApplicant.getApplicantDetailsForPayment(applicantId);
        }

        public DataTable getCodeValueDisplay(string codeValue, string codeType)
        {
            return dbApplicant.getCodeValueDisplay(codeValue, codeType);
        }

        public DataTable getIdentificationTypeCodeReference()
        {
            return dbApplicant.getIdentificationTypeCodeReference();
        }

        public DataTable getNationalityCodeReference()
        {
            return dbApplicant.getNationalityCodeReference();
        }

        public DataTable getRaceCodeReference()
        {
            return dbApplicant.getRaceCodeReference();
        }

        public DataTable getOccupationCodeReference()
        {
            return dbApplicant.getOccupationCodeReference();
        }

        public DataTable getAllEducationCodeReference()
        {
            return dbApplicant.getAllEducationCodeReference();
        }

        public DataTable getAllLanguageProficiencyCodeReference()
        {
            return dbApplicant.getAllLanguageProficiencyCodeReference();
        }

        public DataTable getAllLanguageCodeReference()
        {
            return dbApplicant.getAllLanguageCodeReference();
        }

        public DataTable getInterviewStatus()
        {
            return dbApplicant.getInterviewStatus();
        }

        public DataTable getAllGetToKnowChannelCodeReference()
        {
            return dbApplicant.getAllGetToKnowChannelCodeReference();
        }


        public DataTable getOtherLanguageCodeReference()
        {
            return dbApplicant.getOtherLanguages();
        }

        public bool checkIfApplicantDetailsIsUpdated(string applicantId)
        {
            return dbApplicant.checkIfApplicantDetailsIsUpdated(applicantId);
        }

       


        public DataTable searchApplicantByValue(string value)
        {
            Cryptography cryp = new Cryptography();

            DataTable dtListOfApplicant = dbApplicant.getApplicantByValue(cryp.encryptInfo(value), value, value);

            if (dtListOfApplicant == null) return null;

            if (dtListOfApplicant.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();

                foreach (DataRow dr in dtListOfApplicant.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                    if (dr["contactNumber1"] != DBNull.Value && !dr["contactNumber1"].ToString().Equals("")) dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());
                }
            }

            return dtListOfApplicant;
        }

        //Get total number of unprocessed applications
        public int getUnprocessedApplicationCount()
        {
            return dbApplicant.getUnprocessedApplicationCount();
        }

        public bool updateApplicantStatusReject(string applicantId, int userId)
        {

            bool success = dbApplicant.updateApplicantStatusReject(applicantId, userId);

            return success;
        }

        public Tuple<string, byte[]> getApplicantSignature(string applicantid)
        {
            return dbApplicant.getApplicantSignature(applicantid);
        }

        public List<Tuple<string, string>> getApplicantDocuments(string applicantid)
        {
            return dbApplicant.getApplicantDocuments(applicantid);
        }

        //Get a list of applicant, return 2 datatables, 2 counts
        public Tuple<DataTable, DataTable, int, int> getListOfApplicant()
        {
            DateTime todayDate = DateTime.Now.Date;

            DataTable dtListOfApplicant = dbApplicant.getListOfApplicant();
            if (dtListOfApplicant == null) return null;

            DataTable dtListOfApplicantToday = new DataTable();
            dtListOfApplicantToday = dtListOfApplicant.Clone();
            int todayCount = 0;
            int othercount = 0;

            if (dtListOfApplicant.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();

                foreach (DataRow dr in dtListOfApplicant.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());


                    if (dr["contactNumber1"] != DBNull.Value && !dr["contactNumber1"].ToString().Equals(""))
                    {
                        dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());
                    }
                    else
                    {
                        dr["contactNumber1"] = "";
                    }


                    if (dr["contactNumber2"].ToString() != "")
                    {
                        dr["contactNumber2"] = decryp.decryptInfo(dr["contactNumber2"].ToString());
                    }
                    else
                    {
                        dr["contactNumber2"] = "";
                    }

                    if (dr["emailAddress"].ToString() != "")
                    {
                        dr["emailAddress"] = decryp.decryptInfo(dr["emailAddress"].ToString());
                    }
                    else
                    {
                        dr["emailAddress"] = "";
                    }

                    if (dr["addressLine"].ToString() != "")
                    {
                        dr["addressLine"] = decryp.decryptInfo(dr["addressLine"].ToString());
                    }
                    else
                    {
                        dr["addressLine"] = "";
                    }

                    if (dr["postalCode"].ToString() != "")
                    {
                        dr["postalCode"] = decryp.decryptInfo(dr["postalCode"].ToString());
                    }
                    else
                    {
                        dr["postalCode"] = "";
                    }
                    DateTime applicationDate = Convert.ToDateTime(dr["applicationDate"].ToString()).Date;

                    int compareDateResult = DateTime.Compare(applicationDate, todayDate);

                    if (compareDateResult == 0)
                    {
                        todayCount++;
                        dtListOfApplicantToday.ImportRow(dr);
                        dr.Delete();
                    }
                    else
                    {
                        othercount++;
                    }
                }
            }

            return new Tuple<DataTable, DataTable, int, int>(dtListOfApplicant, dtListOfApplicantToday, todayCount, othercount);
        }

        //Search applicant by NRIC or name
        public DataTable getListOfApplicantByValue(string value)
        {
            Cryptography cryp = new Cryptography();

            DataTable dtListOfApplicant = dbApplicant.getListOfApplicantByValue(cryp.encryptInfo(value), value);

            if (dtListOfApplicant == null) return null;

            if (dtListOfApplicant.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();


                foreach (DataRow dr in dtListOfApplicant.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                    if (!dr["contactNumber1"].ToString().Equals(""))
                    {
                        dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());
                    }

                    if (!dr["contactNumber2"].ToString().Equals(""))
                    {
                        dr["contactNumber2"] = decryp.decryptInfo(dr["contactNumber2"].ToString());
                    }

                    if (!dr["emailAddress"].ToString().Equals(""))
                    {
                        dr["emailAddress"] = decryp.decryptInfo(dr["emailAddress"].ToString());
                    }

                    if (!dr["addressLine"].ToString().Equals(""))
                    {
                        dr["addressLine"] = decryp.decryptInfo(dr["addressLine"].ToString());
                    }

                    if (!dr["postalCode"].ToString().Equals("")) 
                    { 
                        dr["postalCode"] = decryp.decryptInfo(dr["postalCode"].ToString());
                    }

                }
            }

            return dtListOfApplicant;
        }

        //Get applicant's exempted module
        public List<string> getApplicantExemptedModule(string applicantId)
        {
            DataTable dtExemptedModule = dbApplicant.getApplicantExemptedModule(applicantId);
            if (dtExemptedModule == null) return null;

            if (dtExemptedModule.Rows.Count > 0)
            {
                if (!dtExemptedModule.Rows[0]["applicantExemModule"].ToString().Equals(""))
                {
                    string[] exemptedModule = dtExemptedModule.Rows[0]["applicantExemModule"].ToString().Split(';');
                    return exemptedModule.ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //Get application details by applicantId
        public Tuple<DataTable, DataTable, DataTable, DataTable, DataTable> getApplicationDetailsByApplicantId(string applicantId)
        {
            DataSet dtApplicationDetails = dbApplicant.getApplicationDetailsByApplicantId(applicantId);
            if (dtApplicationDetails == null) return null;

            //Includes exempted modules
            DataTable dtApplicantDetails = dtApplicationDetails.Tables[0];
            dtApplicantDetails.Columns.Add("birthDateDisplay", typeof(String));
            dtApplicantDetails.Columns.Add("programmeStartDateDisplay", typeof(String));
            dtApplicantDetails.Columns.Add("programmeCompletionDateDisplay", typeof(String));

            //May consist 0 to 2 records
            DataTable dtEmploymentDetails = dtApplicationDetails.Tables[1];
            dtEmploymentDetails.Columns.Add("employmentStartDateDisplay", typeof(String));
            dtEmploymentDetails.Columns.Add("employmentEndDateDisplay", typeof(String));

            //Interview schedule and remarks
            DataTable dtInterviewDetails = dtApplicationDetails.Tables[2];
            dtInterviewDetails.Columns.Add("interviewDateDisplay", typeof(String));

            DataTable dtExemptedModule = new DataTable();
            dtExemptedModule.Columns.Add("moduleId", typeof(String));
            dtExemptedModule.Columns.Add("WSQCompetencyCode", typeof(String));
            dtExemptedModule.Columns.Add("moduleTitle", typeof(String));

            List<string> listOfExemptedModule = new List<string>();

            //string packageCode = "";

            string programmeBatchId = "";

            //Decryption before passing back to interface
            if (dtApplicantDetails.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();


                foreach (DataRow dr in dtApplicantDetails.Rows)
                {
                    programmeBatchId = dr["programmeBatchId"].ToString();



                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                    if (dr["contactNumber1"].ToString() != "")
                    {
                        dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());
                    }
                    else
                    {
                        dr["contactNumber1"] = "";
                    }

                    if (dr["contactNumber2"].ToString() != "")
                    {
                        dr["contactNumber2"] = decryp.decryptInfo(dr["contactNumber2"].ToString());
                    }
                    else
                    {
                        dr["contactNumber2"] = "";
                    }

                    if (dr["emailAddress"].ToString() != "")
                    {
                        dr["emailAddress"] = decryp.decryptInfo(dr["emailAddress"].ToString());
                    }
                    else
                    {
                        dr["emailAddress"] = "";
                    }

                    if (dr["addressLine"].ToString() != "")
                    {
                        dr["addressLine"] = decryp.decryptInfo(dr["addressLine"].ToString());
                    }
                    else
                    {
                        dr["addressLine"] = "";
                    }

                    if (dr["postalCode"].ToString() != "")
                    {
                        dr["postalCode"] = decryp.decryptInfo(dr["postalCode"].ToString());
                    }
                    else
                    {
                        dr["postalCode"] = "";
                    }

                    dr["birthDateDisplay"] = Convert.ToDateTime(dr["birthDate"].ToString()).Date.ToString("dd MMM yyyy");

                    dr["programmeStartDateDisplay"] = Convert.ToDateTime(dr["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");

                    dr["programmeCompletionDateDisplay"] = Convert.ToDateTime(dr["programmeCompletionDate"].ToString()).ToString("dd MMM yyyy");

                    //packageCode = dr["packageCode"].ToString();

                    if (!dr["applicantExemModule"].ToString().Equals(""))
                    {
                        string[] exemptedModule = dr["applicantExemModule"].ToString().Split(';');

                        listOfExemptedModule = exemptedModule.ToList();
                    }
                }

                Session_Handler sh = new Session_Handler();
                sh.setApplicationDetails(dtApplicantDetails);
            }

            //if (!packageCode.Equals(""))
            //{
            //    if (listOfExemptedModule != null || listOfExemptedModule.Count > 0)
            //    {
            //        Bundle_Management pm = new Bundle_Management();
            //        DataTable dtPackageModule = pm.getPackageModule(packageCode);

            //        if (dtPackageModule.Rows.Count > 0)
            //        {
            //            foreach (DataRow dr in dtPackageModule.Rows)
            //            {
            //                if (listOfExemptedModule.Contains(dr["moduleCode"].ToString()))
            //                {
            //                    DataRow row = dtExemptedModule.NewRow();
            //                    row["moduleCode"] = dr["moduleCode"].ToString();
            //                    row["moduleTitle"] = dr["moduleTitle"].ToString();
            //                    dtExemptedModule.Rows.Add(row);
            //                }
            //            }
            //        }
            //    }
            //}
            if (!programmeBatchId.Equals(""))
            {
                if (listOfExemptedModule != null || listOfExemptedModule.Count > 0)
                {
                    Batch_Session_Management bsm = new Batch_Session_Management();
                    DataTable dtBatchModule = bsm.getBatchModuleByProgrammeBatchId(programmeBatchId);

                    if (dtBatchModule.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtBatchModule.Rows)
                        {
                            if (listOfExemptedModule.Contains(dr["moduleId"].ToString()))
                            {
                                DataRow row = dtExemptedModule.NewRow();
                                row["moduleId"] = dr["moduleId"].ToString();
                                row["WSQCompetencyCode"] = dr["WSQCompetencyCode"].ToString();
                                row["moduleTitle"] = dr["moduleTitle"].ToString();
                                dtExemptedModule.Rows.Add(row);
                            }
                        }
                    }
                }
            }

            //Decryption before passing back to interface
            if (dtEmploymentDetails.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();

                foreach (DataRow dr in dtEmploymentDetails.Rows)
                {
                    dr["employmentStartDateDisplay"] = Convert.ToDateTime(dr["employmentStartDate"].ToString()).Date.ToString("dd MMM yyyy");

                    if (dr["employmentEndDate"] != DBNull.Value) dr["employmentEndDateDisplay"] = Convert.ToDateTime(dr["employmentEndDate"].ToString()).Date.ToString("dd MMM yyyy");
                }

            }

            //Convert datetime format for display
            if (dtInterviewDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtInterviewDetails.Rows)
                {
                    if (dr["interviewDate"].ToString() != "")
                        dr["interviewDateDisplay"] = Convert.ToDateTime(dr["interviewDate"].ToString()).Date.ToString("dd MMM yyyy");
                }
            }


            Finance_Management fm = new Finance_Management();
            DataTable dtPaymentHistory = fm.getPaymentHistoryByApplicantId(applicantId);


            return new Tuple<DataTable, DataTable, DataTable, DataTable, DataTable>(dtApplicantDetails, dtEmploymentDetails, dtInterviewDetails, dtPaymentHistory, dtExemptedModule);
        }

        
        public Tuple<bool, string> registerApplicantFull(int programmeId, int programmeBatchId, Sponsorship spon, string fullName, IDType idType, string id, string nationality, string race, string gender, DateTime dtDOB
            , string email, string contact1, string contact2, string addr, string postal, LanguageProficiency[] lang, string highEdulvl, EmploymentRecord[] empl, GetToKnowChannel[] knowChannel, int userId, bool isOnline = false, string preferredModeOfPayment = "")
        {
            Cryptography encrypt = new Cryptography();

            //determine if interview is needed
            DataTable dt = (new DB_Programme()).getProgrammeDetails(programmeId.ToString());
            if (dt == null || dt.Rows.Count == 0) return new Tuple<bool, string>(false, "Unable to register due to error retriving programme details.");
            InterviewStatus intStatus = (dt.Rows[0]["programmeType"].ToString() == ProgrammeType.FQ.ToString()) ? InterviewStatus.NYD : InterviewStatus.NREQ;

            //check if batch capacity reached
            int batchCapacity = (new DB_Batch_Session()).getBatchCapacity(programmeBatchId);
            int enrolled = (new DB_Attendance()).getMaxBatchSessionsEnrollment(programmeBatchId);
            if (enrolled + 1 > batchCapacity) return new Tuple<bool, string>(false, "Unable to register due to full class.");

            string appId = dbApplicant.registerApplicantFull(programmeBatchId, intStatus, spon, fullName, idType, encrypt.encryptInfo(id), nationality, race, gender, dtDOB, encrypt.encryptInfo(email), encrypt.encryptInfo(contact1),
                contact2 == null ? contact2 : encrypt.encryptInfo(contact2), encrypt.encryptInfo(addr), encrypt.encryptInfo(postal), lang, highEdulvl, empl, knowChannel, userId, isOnline, preferredModeOfPayment);
            if (appId != null) return new Tuple<bool, string>(true, appId);
            else return new Tuple<bool, string>(false, "Error with registration.");
        }

        public Tuple<bool, string> registerApplicantQuick(string fullName, DateTime dtDOB, string id, IDType idType, Sponsorship spon, int programmeId, int programmeBatchId, int userId)
        {
            Cryptography encrypt = new Cryptography();

            //determine if interview is needed
            DataTable dt = (new  DB_Programme()).getProgrammeDetails(programmeId.ToString());
            if (dt == null || dt.Rows.Count == 0) return new Tuple<bool, string>(false, "Unable to register applicant due to error retriving programme details.");
            InterviewStatus intStatus = (dt.Rows[0]["programmeType"].ToString() == ProgrammeType.FQ.ToString()) ? InterviewStatus.NYD : InterviewStatus.NREQ;

            //check if batch capacity reached
            int batchCapacity = (new DB_Batch_Session()).getBatchCapacity(programmeBatchId);
            int enrolled = (new DB_Attendance()).getMaxBatchSessionsEnrollment(programmeBatchId);
            if (enrolled + 1 > batchCapacity) return new Tuple<bool, string>(false, "Unable to register applicant due to full class.");

            if (dbApplicant.registerApplicantQuick(fullName, dtDOB, encrypt.encryptInfo(id), idType, spon, programmeBatchId, intStatus, userId))
                return new Tuple<bool, string>(true, "Applicant registered successfully.");
            else return new Tuple<bool, string>(false, "Error registering applicant.");
        }

        public bool updateApplicantDetails(string applicantId, string fullName, string identification, string identityType, string nationality, string gender, string contactNumber1, string contactNumber2,
                string emailAddress, string race, DateTime birthDate, string addrLine1, string postalCode, string highestEducation, string highestEducationRemarks, string spokenLanguage, string writtenLanguage, string channel, string sponsorship, int updatedBy, string applicantRemarks)
        {
            Cryptography cryp = new Cryptography();

            identification = cryp.encryptInfo(identification);
            if (!contactNumber1.Equals(""))
                contactNumber1 = cryp.encryptInfo(contactNumber1);

            if (!contactNumber2.Equals(""))
            {
                contactNumber2 = cryp.encryptInfo(contactNumber2);
            }

            if (!emailAddress.Equals(""))
            {
                emailAddress = cryp.encryptInfo(emailAddress);
            }

            addrLine1 = cryp.encryptInfo(addrLine1);

            postalCode = cryp.encryptInfo(postalCode);

            bool success = dbApplicant.updateApplicantParticular(applicantId, fullName, identification, identityType, nationality, gender, contactNumber1, contactNumber2,
                emailAddress, race, birthDate, addrLine1, postalCode, highestEducation, highestEducationRemarks, spokenLanguage, writtenLanguage, channel, sponsorship, updatedBy, applicantRemarks);

            return success;
        }

        //Update employment details
        public bool updateEmploymentDetails(string applicantId, int employmentId, string employerName, string position,
            DateTime startEmployment, DateTime endEmployment, decimal salary, string currentEmployment, string employmentStatus, int userId, string occupationCode, string companyDept)
        {
            Cryptography cryp = new Cryptography();

            bool success = dbApplicant.updateApplicantEmployment(applicantId, employmentId, employerName,
                position, startEmployment, endEmployment, salary, currentEmployment, employmentStatus, userId, occupationCode, companyDept);

            return success;
        }

        //Update applicant's remarks
        public bool updateApplicantRemarks(string applicantId, string remarks, int userId)
        {
            bool success = dbApplicant.updateApplicantRemarks(applicantId, remarks, userId);

            return success;
        }

        //Change applicant's course and project codes
        public bool updateApplicantCourseProjectCode(string applicantId, string programmBatchId)
        {
            bool success = dbApplicant.updateApplicantCourseProjectCode(applicantId, programmBatchId);

            return success;
        }

        //Update applicant's exempted modules
        public bool updateApplicantExemptedModule(string applicantId, string exemptedModules, decimal totalModuleCost, int userId)
        {
            //decimal programmePayableAmount = totalModuleCost + (totalModuleCost * General_Constance.GST_RATE);

            bool success = dbApplicant.updateApplicantModuleExemption(applicantId, exemptedModules, totalModuleCost, userId);

            return success;
        }

        //Update applicant's interview status and schedule
        public bool updateApplicantInterviewDetails(string applicantId, int interviewerId, string interviewStatus, string shortlistStatus, DateTime interviewDate, string interviewRemarks, int userId)
        {
            bool success = dbApplicant.updateApplicantInterviewDetails(applicantId, interviewerId, interviewStatus, shortlistStatus, interviewDate, interviewRemarks, userId);

            return success;
        }
    }
}
