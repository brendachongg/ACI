﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace LogicLayer
{
    public class Trainee_Management
    {
        private DB_Trainee dbTrainee = new DB_Trainee();

        public DataTable getCodeReferenceValues(string codeType)
        {
            return dbTrainee.getCodeReferenceValues(codeType);
        }

        public DataTable getOtherLanguageCodeReference(string codeType)
        {
            return dbTrainee.getOtherLanguages(codeType);
        }

        public DataTable getIdentificationTypeCodeReference(string codeType)
        {
            return dbTrainee.getCodeReferenceValues(codeType);
        }

        public DataTable getNationalityCodeReference(string codeType)
        {
            return dbTrainee.getCodeReferenceValues(codeType);
        }

        public DataTable getRaceCodeReference(string codeType)
        {
            return dbTrainee.getCodeReferenceValues(codeType);
        }

        public DataTable getEduLevelReference(string codeType)
        {
            return dbTrainee.getCodeReferenceValues(codeType);
        }

        public Tuple<bool, string> withdrawTrainee(string traineeId, int userId, string reason)
        {
            //currently only allow trainee to withdraw if class have not started, control via the aspx
            if (dbTrainee.withdrawTrainee(traineeId, userId, reason))
                return new Tuple<bool, string>(true, "Trainee is withdrawn successfully.");
            else
                return new Tuple<bool, string>(false, "Error withdrawing trainee.");
        }

        public Tuple<bool, string> updateTraineeDetails(string applicantId, string fullName, string identification, string identityType, string nationality, string gender, string contactNumber1, string contactNumber2,
               string emailAddress, string race, DateTime birthDate, string addrLine1, string postalCode, string highestEducation, string highestEducationRemarks, string spokenLanguage, string writtenLanguage, int updatedBy)
        {
            Cryptography cryp = new Cryptography();
            identification = cryp.encryptInfo(identification);
            if (!contactNumber1.Equals(""))
                contactNumber1 = cryp.encryptInfo(contactNumber1);

            if (!contactNumber2.Equals(""))
                contactNumber2 = cryp.encryptInfo(contactNumber2);

            if (!emailAddress.Equals(""))
                emailAddress = cryp.encryptInfo(emailAddress);

            addrLine1 = cryp.encryptInfo(addrLine1);
            postalCode = cryp.encryptInfo(postalCode);
            bool success = dbTrainee.updateTraineeParticular(applicantId, fullName, identification, identityType, nationality, gender, contactNumber1, contactNumber2,
                emailAddress, race, birthDate, addrLine1, postalCode, highestEducation, highestEducationRemarks, spokenLanguage, writtenLanguage, updatedBy);

            if (success)
                return new Tuple<bool, string>(true, "Trainee's updated successfully");
            else
                return new Tuple<bool, string>(false, "Error updating trainee");
        }

        public DataTable getTraineeDetailsByPayment(int paymentId)
        {
            return dbTrainee.getTraineeDetailsByPayment(paymentId);
        }

        public DataTable getTraineeDetailsForPayment(string traineeId)
        {
            return dbTrainee.getTraineeDetailsForPayment(traineeId);
        }

        public DataTable getTraineeExemMod(string traineeId)
        {
            return dbTrainee.getTraineeExemMod(traineeId);
        }

        public Tuple<string, string> getEnrollmentLetter(string traineeId)
        {
            DataTable dtTraineeDetails = dbTrainee.getTraineeBriefDetails(traineeId);
            if (dtTraineeDetails == null || dtTraineeDetails.Rows.Count == 0) return null;

            int batchId = (int)dtTraineeDetails.Rows[0]["programmeBatchId"];

            DataTable dtBatchDetails = (new DB_Batch_Session()).getBatchDetails(batchId);
            if (dtBatchDetails == null || dtBatchDetails.Rows.Count == 0) return null;

            if (dtBatchDetails.Rows[0]["enrollmentLetter"] == DBNull.Value) return new Tuple<string, string>("", "");

            Cryptography decrypt = new Cryptography();
            dtTraineeDetails.Rows[0]["idNumber"] = decrypt.decryptInfo(dtTraineeDetails.Rows[0]["idNumber"].ToString());
            if (dtTraineeDetails.Rows[0]["emailAddress"] != DBNull.Value) dtTraineeDetails.Rows[0]["emailAddress"] = decrypt.decryptInfo(dtTraineeDetails.Rows[0]["emailAddress"].ToString());

            string letter = dtBatchDetails.Rows[0]["enrollmentLetter"].ToString();

            letter = letter.Replace("@programme_title", dtBatchDetails.Rows[0]["programmeTitle"].ToString());
            letter = letter.Replace("@current_date", (new DateTime()).ToString("dd MMM yyyy"));
            letter = letter.Replace("@class_start_date", dtBatchDetails.Rows[0]["programmeStartDateDisp"].ToString());
            letter = letter.Replace("@class_end_date", dtBatchDetails.Rows[0]["programmeCompletionDateDisp"].ToString());
            letter = letter.Replace("@name", dtTraineeDetails.Rows[0]["fullName"].ToString());
            letter = letter.Replace("@idNumber", dtTraineeDetails.Rows[0]["idNumber"].ToString());

            return new Tuple<string, string>(dtTraineeDetails.Rows[0]["emailAddress"] != DBNull.Value ? dtTraineeDetails.Rows[0]["emailAddress"].ToString() : null, letter);
        }

        public Tuple<bool, string> emailEnrollmentLetter(string[] traineeIds, int userId)
        {
            DataTable dt = dbTrainee.getTraineeEnrollmentLetter(traineeIds);
            if (dt == null || dt.Rows.Count == 0) return new Tuple<bool, string>(false, "Error retrieving trainee details.");

            List<string> ids = new List<string>();
            string noLetter = "Unable to email enrollment letter to the following trainees as enrollment letter has not been set up for the class: ";
            string noEmail = "Unable to email enrollment letter to the following trainees as they have no email: ";
            string errEmail = "Error sending enrollment letter to the following trainees: ";

            string sender = ConfigurationManager.AppSettings["EnrollmentEmail"];
            Cryptography decrypt = new Cryptography();
            Email_Handler eh = new Email_Handler();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["emailAddress"] == DBNull.Value)
                {
                    noEmail += "<li>" + dr["traineeId"].ToString() + ", " + dr["fullName"].ToString() + "</li>";
                    continue;
                }

                dr["idNumber"] = decrypt.decryptInfo(dr["idNumber"].ToString());
                dr["emailAddress"] = decrypt.decryptInfo(dr["emailAddress"].ToString());

                if (dr["enrollmentLetter"] == DBNull.Value)
                {
                    noLetter += "<li>" + dr["traineeId"].ToString() + ", " + dr["fullName"].ToString() + "</li>";
                    continue;
                }

                string letter = dr["enrollmentLetter"].ToString();
                letter = letter.Replace("@programme_title", dr["programmeTitle"].ToString());
                letter = letter.Replace("@current_date", (new DateTime()).ToString("dd MMM yyyy"));
                letter = letter.Replace("@class_start_date", dr["programmeStartDateDisp"].ToString());
                letter = letter.Replace("@class_end_date", dr["programmeCompletionDateDisp"].ToString());
                letter = letter.Replace("@name", dr["fullName"].ToString());
                letter = letter.Replace("@idNumber", dr["idNumber"].ToString());

                string[] bcc = new string[1];
                bcc[0] = ConfigurationManager.AppSettings["EnrollmentEmail"].ToString();

                if (!eh.SendEmail(sender, dr["emailAddress"].ToString(), null, bcc, "Successful Enrollment to " + dr["programmeTitle"].ToString(), letter))
                    errEmail += "<li>" + dr["traineeId"].ToString() + ", " + dr["fullName"].ToString() + "</li>";
                else
                    ids.Add(dr["traineeId"].ToString());
            }

            noLetter += "<br/>";

            bool update = true;
            if (ids.Count > 0) update = dbTrainee.updateEnrollmentLetterSend(ids.ToArray(), userId);

            bool success = update && noLetter.IndexOf("<li>") == -1 && errEmail.IndexOf("<li>") == -1 && noEmail.IndexOf("<li>") == -1;
            if (success) return new Tuple<bool, string>(true, "Enrollment letter emailed successfully.");
            else return new Tuple<bool, string>(false, (noLetter.IndexOf("<li>") != -1 ? noLetter : "") + (noEmail.IndexOf("<li>") != -1 ? noEmail : "") + (errEmail.IndexOf("<li>") != -1 ? errEmail : "") +
                (update ? "" : "Error updating send enrollment letter status."));
        }

        public Tuple<int, string, string> enrollApplicant(string applicantId, int userId, bool skipCheck = false)
        {
            DataTable dtProgDetails = (new DB_Applicant()).getApplicantProgBatchDetails(applicantId);
            if (dtProgDetails == null || dtProgDetails.Rows.Count == 0)
                return new Tuple<int, string, string>(-1, null, "Unable to retrieve applicant details");

            if (!skipCheck)
            {
                //check if programme has already started            
                if (DateTime.Now.CompareTo(((DateTime)dtProgDetails.Rows[0]["programmeStartDate"])) > 0)
                    return new Tuple<int, string, string>(-1, null, "Unable to enroll applicant as class has started");

                //if programme is full qual, must see interview status         
                if (dtProgDetails.Rows[0]["programmeType"].ToString() == ProgrammeType.FQ.ToString()
                    && !(dtProgDetails.Rows[0]["interviewStatus"].ToString() == InterviewStatus.PASSED.ToString() || dtProgDetails.Rows[0]["interviewStatus"].ToString() == InterviewStatus.NREQ.ToString()))
                    return new Tuple<int, string, string>(-1, null, "Unable to enroll applicant as applicant has not pass interview.");

                //check if course payment fully made
                if (!(new Finance_Management()).isCseFullPayment(applicantId)) return new Tuple<int, string, string>(-1, null, "Unable to enroll applicant as full programme payment has not been made.");

                //check if within batch capacity        
                int capacity = (new DB_Batch_Session()).getBatchCapacity((int)dtProgDetails.Rows[0]["programmeBatchId"]);
                int enrolled = (new DB_Attendance()).getMaxBatchSessionsEnrollment((int)dtProgDetails.Rows[0]["programmeBatchId"]);
                if (enrolled + 1 > capacity) return new Tuple<int, string, string>(-1, null, "Unable to enroll applicant as class capacity has been reached.");
            }

            string traineeId = dbTrainee.enrollApplicant(applicantId, dtProgDetails.Rows[0]["applicantExemModule"] == DBNull.Value ? null : dtProgDetails.Rows[0]["applicantExemModule"].ToString().Replace(";", ","), userId);
            if (traineeId == null) return new Tuple<int, string, string>(-1, null, "Unable to enroll applicant due to database error.");
            else return new Tuple<int, string, string>(0, traineeId, "Applicant enrolled successfully.");
            //{
            //    Tuple<string, string> enrollmentLetter = getEnrollmentLetter(traineeId);

            //    if (enrollmentLetter == null) return new Tuple<int, string, string>(1, traineeId, "Applicant enrolled successfully but unable to generate enrollment letter.");
            //    else if (ConfigurationManager.AppSettings["EnrollmentEmail"] == "1" || ConfigurationManager.AppSettings["EnrollmentEmail"].ToLower()=="true")
            //    {
            //        if ((new Email_Handler()).SendEmail(ConfigurationManager.AppSettings["EnrollmentEmail"], enrollmentLetter.Item1, null, "Successful Enrollment to " + dtProgDetails.Rows[0]["programmeTitle"].ToString(), enrollmentLetter.Item2))
            //            return new Tuple<int, string, string>(0, traineeId, "Applicant enrolled successfully.");
            //        else return new Tuple<int, string, string>(2, traineeId, "Applicant enrolled successfully but unable to send enrollment letter.");
            //    }
            //    else return new Tuple<int, string, string>(3, traineeId, "Applicant enrolled successfully but email enrollment letter option has been turned off.");
            //}   
        }



        public DataTable searchTraineeByValue(string value)
        {
            Cryptography cryp = new Cryptography();

            DataTable dtListOfTrainee = dbTrainee.getTraineeByValue(value, cryp.encryptInfo(value), value);

            if (dtListOfTrainee == null) return null;

            if (dtListOfTrainee.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();


                foreach (DataRow dr in dtListOfTrainee.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                    if (dr["contactNumber1"] != DBNull.Value && !dr["contactNumber1"].ToString().Equals("")) 
                        dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());
                }
            }

            return dtListOfTrainee;
        }

        public DataTable searchTrainee(string searchBy, string searchValue)
        {
            string condition = "";
            SqlParameter p = null;
            if (searchBy == "ID")
            {
                condition = "UPPER(t.traineeId) like @v";
                p = new SqlParameter("@v", "%" + searchValue.ToUpper() + "%");
            }
            else if (searchBy == "NAME")
            {
                condition = "UPPER(t.fullName) like @v";
                p = new SqlParameter("@v", "%" + searchValue.ToUpper() + "%");
            }
            else if (searchBy == "NRICPIN")
            {
                condition = "UPPER(t.idNumber) = @v";
                p = new SqlParameter("@v", (new Cryptography()).encryptInfo(searchValue.ToUpper()));
            }
            else if (searchBy == "BTC")
            {
                condition = "UPPER(pb.batchCode) LIKE @v";
                p = new SqlParameter("@v", "%" + searchValue.ToUpper() + "%");
            }
            else if (searchBy == "PJC")
            {
                condition = "UPPER(pb.projectCode) = @v";
                p = new SqlParameter("@v", "%" + searchValue.ToUpper() + "%");
            }

            DataTable dt = dbTrainee.searchTrainee(condition, p);
            if (dt == null) return null;

            Cryptography decryp = new Cryptography();
            foreach (DataRow dr in dt.Rows)
                dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

            return dt;
        }

        public Tuple<bool, string> addSitIn(string traineeId, int programmeBatchId, int moduleId, int userId)
        {
            //check if trainee has already register for this batch module before
            if (dbTrainee.isTraineeInModule(traineeId, programmeBatchId, moduleId))
            {
                return new Tuple<bool, string>(false, "Trainee has already been registered to this module.");
            }

            //check if the selected repeat module clash with any of the trainee existing enroll programme or makeup or repeat
            DataTable dtClash = (new DB_Assessment()).getClashSession(traineeId, "select 1 from batchModule_session s inner join batch_module bm on bm.batchModuleId=s.batchModuleId and bm.programmeBatchId=@bid and bm.moduleId=@mid "
                + "where s.sessionDate=t.sessionDate and s.sessionPeriod=t.sessionPeriod", new SqlParameter[] { new SqlParameter("@bid", programmeBatchId), new SqlParameter("@mid", moduleId) });

            if (dtClash == null) return new Tuple<bool, string>(false, "Error checking trainee's details.");
            if (dtClash.Rows.Count > 0)
            {
                string errMsg = "Selected module's session clashes with the existing session(s) that trainee has been enrolled for:";
                string type;
                foreach (DataRow dr in dtClash.Rows)
                {
                    if (dr["type"].ToString() == "MAIN") type = "Enrolled session for module " + dr["moduleCode"].ToString() + ", class " + dr["batchCode"].ToString() + " on ";
                    else if (dr["type"].ToString() == "MAKEUP") type = "Make-up session of module " + dr["moduleCode"].ToString() + ", class " + dr["batchCode"].ToString() + " on ";
                    else type = "Re-assessment session of module " + dr["moduleCode"].ToString() + ", class " + dr["batchCode"].ToString() + " on ";

                    errMsg += "<li>" + type + " " + dr["sessionDate"].ToString() + " " + dr["sessionPeriodDisp"].ToString() + "</li>";
                }

                return new Tuple<bool, string>(false, errMsg);
            }

            //check if exceed the all the batch session capacity
            DB_Batch_Session dbBatch = new DB_Batch_Session();
            int batchModuleId = dbBatch.getBatchModuleId(programmeBatchId, moduleId);
            int bCapacity = dbBatch.getBatchCapacityByBatchModule(batchModuleId);
            if (bCapacity == -1) return new Tuple<bool, string>(false, "Error validating sit-in module details.");
            int currCapacity = (new DB_Attendance()).getMaxBatchSessionsEnrollment(programmeBatchId);
            if (currCapacity == -1) return new Tuple<bool, string>(false, "Error validating sit-in module details.");
            if (currCapacity + 1 > bCapacity) return new Tuple<bool, string>(false, "Exceeded selected class capacity.");

            if (dbTrainee.addSitIn(traineeId, programmeBatchId, batchModuleId, moduleId, userId))
                return new Tuple<bool, string>(true, "Sit-In added succesfully.");
            else
                return new Tuple<bool, string>(false, "Error adding sit-in.");
        }

        public bool removeSitIn(string traineeId, int batchModuleId, int userId)
        {
            return dbTrainee.removeSitIn(traineeId, batchModuleId, userId);
        }

        public DataTable getSitInDetails(string traineeId, int batchModuleId)
        {
            return dbTrainee.getSitInDetails(traineeId, batchModuleId);
        }

        public int getTotalCount()
        {
            return dbTrainee.getTotalCountOfTrainees();
        }

        public DataTable searchSitIn(string search, string value)
        {
            string condition = null;
            SqlParameter p = null;

            if (search != null)
            {
                if (search == "T")
                {
                    condition = "UPPER(tm.traineeId) like @t or UPPER(t.fullName) like @t";
                    p = new SqlParameter("@t", "%" + value.ToUpper() + "%");
                }
                else if (search == "M")
                {
                    condition = "UPPER(m.moduleCode) like @m or UPPER(m.moduleTitle) like @m";
                    p = new SqlParameter("@m", "%" + value.ToUpper() + "%");
                }
            }

            DataTable dt = dbTrainee.searchSitIn(condition, p);
            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
                dr["idNumber"] = decryptContent.decryptInfo(dr["idNumber"].ToString());

            return dt;
        }

        //retrieve List of Trainee
        public DataTable getListOfTrainee(int pageIdx)
        {
            //DataTable dtListOfTrainee = dbTrainee.getListOfTrainee(start, maxR);
            DataTable dtListOfTrainee = dbTrainee.getListOfTrainee(pageIdx);

            if (dtListOfTrainee.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();

                //foreach (DataRow dr in dtListOfTrainee.Rows)
                //{
                //    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                //    dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());

                //    if (!dr["contactNumber2"].ToString().Equals(""))
                //    {
                //        dr["contactNumber2"] = decryp.decryptInfo(dr["contactNumber2"].ToString());
                //    }

                //    dr["emailAddress"] = decryp.decryptInfo(dr["emailAddress"].ToString());

                //    dr["addressLine"] = decryp.decryptInfo(dr["addressLine"].ToString());

                //    dr["postalCode"] = decryp.decryptInfo(dr["postalCode"].ToString());

                //}
            }

            return dtListOfTrainee;
        }

        public DataTable getListOfTraineeByValue(string value)
        {
            Cryptography cryp = new Cryptography();

            DataTable dtListOfTrainee = dbTrainee.getListOfTraineeByValue(value, cryp.encryptInfo(value), value);


            if (dtListOfTrainee.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();


                //foreach (DataRow dr in dtListOfTrainee.Rows)
                //{
                //    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                //    dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());

                //    if (!dr["contactNumber2"].ToString().Equals(""))
                //    {
                //        dr["contactNumber2"] = decryp.decryptInfo(dr["contactNumber2"].ToString());
                //    }

                //    dr["emailAddress"] = decryp.decryptInfo(dr["emailAddress"].ToString());

                //    dr["addressLine"] = decryp.decryptInfo(dr["addressLine"].ToString());

                //    dr["postalCode"] = decryp.decryptInfo(dr["postalCode"].ToString());

                //}
            }

            return dtListOfTrainee;

            //return new Tuple<DataTable, int>(dtListOfTrainee, int.Parse(dtListOfTrainee.Rows[0]["tRank"].ToString()));
        }

        public DataTable getTraineeDetailsByTraineeNRIC(string idNumber)
        {
            Cryptography crypt = new Cryptography();

            DataTable dtTraineeDetails = dbTrainee.getTraineeDetailsByTraineeNRIC(crypt.encryptInfo(idNumber));

            if (dtTraineeDetails == null) return null;

            dtTraineeDetails.Columns.Add("birthDateDisplay");

            if (dtTraineeDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTraineeDetails.Rows)
                {
                    dr["idNumber"] = crypt.decryptInfo(dr["idNumber"].ToString());

                    if (!dr["contactNumber1"].ToString().Equals(""))
                        dr["contactNumber1"] = crypt.decryptInfo(dr["contactNumber1"].ToString());

                    if (!dr["contactNumber2"].ToString().Equals(""))
                        dr["contactNumber2"] = crypt.decryptInfo(dr["contactNumber2"].ToString());


                    if (!dr["emailAddress"].ToString().Equals(""))
                        dr["emailAddress"] = crypt.decryptInfo(dr["emailAddress"].ToString());


                    dr["addressLine"] = crypt.decryptInfo(dr["addressLine"].ToString());

                    dr["postalCode"] = crypt.decryptInfo(dr["postalCode"].ToString());

                    dr["birthDateDisplay"] = Convert.ToDateTime(dr["birthDate"].ToString()).Date.ToString("dd MMM yyyy");

                }

                return dtTraineeDetails;
            }
            else
            {
                return null;
            }
        }



        public DataTable getTraineeProgrammeInfo(string traineeId)
        {
            DataTable dt = dbTrainee.getTraineeProgrammeInfo(traineeId);
            return dt;
        }

        public Tuple<DataTable, DataTable> getTraineeDetailsByTraineeId(string traineeId)
        {
            DataSet dtTraineeDetails = dbTrainee.getTraineeDetailsByTraineeId(traineeId);

            if (dtTraineeDetails == null) return null;

            DataTable dtTraineeDetail = dtTraineeDetails.Tables[0];
            dtTraineeDetail.Columns.Add("birthDateDisplay", typeof(String));
            if (dtTraineeDetail.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();

                foreach (DataRow dr in dtTraineeDetail.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                    if (!dr["contactNumber1"].ToString().Equals(""))
                        dr["contactNumber1"] = decryp.decryptInfo(dr["contactNumber1"].ToString());

                    if (!dr["contactNumber2"].ToString().Equals(""))
                    {
                        dr["contactNumber2"] = decryp.decryptInfo(dr["contactNumber2"].ToString());
                    }

                    if (!dr["emailAddress"].ToString().Equals(""))
                    {
                        dr["emailAddress"] = decryp.decryptInfo(dr["emailAddress"].ToString());
                    }

                    dr["addressLine"] = decryp.decryptInfo(dr["addressLine"].ToString());

                    dr["postalCode"] = decryp.decryptInfo(dr["postalCode"].ToString());

                    dr["birthDateDisplay"] = Convert.ToDateTime(dr["birthDate"].ToString()).Date.ToString("dd MMM yyyy");

                }
            }
            DataTable dtTraineeEmployeeDetails = dtTraineeDetails.Tables[1];
            dtTraineeEmployeeDetails.Columns.Add("employmentStartDateDisplay", typeof(String));
            dtTraineeEmployeeDetails.Columns.Add("employmentEndDateDisplay", typeof(String));
            if (dtTraineeEmployeeDetails.Rows.Count > 0)
            {

                foreach (DataRow dr in dtTraineeEmployeeDetails.Rows)
                {
                    dr["employmentStartDateDisplay"] = Convert.ToDateTime(dr["employmentStartDate"].ToString()).Date.ToString("dd MMM yyyy");

                    if (dr["employmentEndDate"] != DBNull.Value) dr["employmentEndDateDisplay"] = Convert.ToDateTime(dr["employmentEndDate"].ToString()).Date.ToString("dd MMM yyyy");
                }

            }

            return new Tuple<DataTable, DataTable>(dtTraineeDetail, dtTraineeEmployeeDetails);

        }

    }
}
