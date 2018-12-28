using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Globalization;

namespace LogicLayer
{
    public class Finance_Management
    {
        private DB_Finance dbFinance = new DB_Finance();

        public string getReceiptNumber()
        {
            return dbFinance.getReceiptNumber();
        }
        public Tuple<DataTable, DataTable> getMakeupPaymentDetails(int paymentId)
        {
            return new Tuple<DataTable, DataTable>(dbFinance.getMakeupPaymentDetails(paymentId), dbFinance.getMakeupPaymentModules(paymentId));
        }

        public DataTable getMakeupPaymentHistory(int absentId)
        {
            return dbFinance.getMakeupPaymentHistory(absentId);
        }

        public Tuple<bool, string> clearMakeupPayment(int paymentId, int userId, DateTime dt)
        {
            if (dbFinance.updateMakeupPaymentStatus(paymentId, PaymentStatus.PAID, dt, userId))
                return new Tuple<bool, string>(true, "Payment updated successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating payment.");
        }

        public Tuple<bool, string> voidMakeupPayment(int paymentId, DateTime dt, string reason, int userId)
        {
            if(dbFinance.voidMakeupPayment(paymentId, dt, reason, userId))
                return new Tuple<bool, string>(true, "Payment void successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating payment.");
        }

        public Tuple<int, string> addMakeupPayment(string traineeId, int batchId, int[] sessions, DateTime dt, PaymentMode mode, string refNum, decimal amt, string remarks, int userId)
        {
            PaymentStatus status = mode == PaymentMode.CHEQ ? PaymentStatus.PEND : PaymentStatus.PAID;
            int paymentId = dbFinance.addMakeupPayment(traineeId, batchId, sessions, dt, mode, refNum, amt, remarks, status, userId);
            if (paymentId != -1)
            {
                if (status == PaymentStatus.PAID) return new Tuple<int, string>(1, paymentId.ToString()); 
                else return new Tuple<int, string>(0, "Payment saved successfully.");
            }
            else return new Tuple<int, string>(-1, "Error saving payment.");
        }

        public DataTable getTraineeAbsentPaymentDetails(string traineeId)
        {
            return dbFinance.getTraineeAbsentPaymentDetails(traineeId);
        }

        public bool isCseFullPayment(string applicantId)
        {
            DataTable dtPayTypes = dbFinance.getApplnClassPaymentTypes(applicantId);
            decimal paid = dbFinance.getApplnCsePaymentMade(applicantId);
            DataTable dtPayDetails = (new DB_Applicant()).getApplicantDetailsForPayment(applicantId);

            if (dtPayDetails == null || dtPayDetails == null || dtPayDetails.Rows.Count == 0 || dtPayDetails.Rows.Count == 0) return false;

            decimal progFee = (decimal)dtPayDetails.Rows[0]["programmePayableAmount"];
            decimal subsidy = dtPayDetails.Rows[0]["subsidyAmt"] == DBNull.Value ? 0 : (decimal)dtPayDetails.Rows[0]["subsidyAmt"];
            decimal regFee = dtPayDetails.Rows[0]["registrationFee"] == DBNull.Value ? 0 : (decimal)dtPayDetails.Rows[0]["registrationFee"];
            decimal progOrCombinedGST = dtPayDetails.Rows[0]["GSTPayableAmount"] == DBNull.Value ? 0 : (decimal)dtPayDetails.Rows[0]["GSTPayableAmount"];
            decimal regGST = dtPayTypes.Select("paymentType='" + PaymentType.REG.ToString() + "'").Length > 0 ? Math.Round(regFee * General_Constance.GST_RATE, 2) : 0;

            decimal nett = progFee + regFee + progOrCombinedGST + regGST - subsidy;

            return nett <= paid;
        }

        public DataTable getAllSubsidy()
        {
            return dbFinance.getAllSubsidy();
        }

        public DataTable getSubsidyTypes()
        {
            return dbFinance.getSubsidyTypes();
        }

        public Tuple<bool, string> addSubsidy(string scheme, int progId, string type, decimal value, DateTime effDt, int userId)
        {
            if (dbFinance.isExistingSubsidy(scheme, progId))
                return new Tuple<bool, string>(false, "Subsidy already exist.");

            if (dbFinance.addSubsidy(scheme, progId, type, value, effDt, userId))
                return new Tuple<bool, string>(true, "Subsidy saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error saving subsidy.");
        }

        public Tuple<bool, string> updateSubsidy(int subId, string type, decimal value, DateTime effDt, int userId)
        {
            if (dbFinance.updateSubsidy(subId, type, value, effDt, userId))
                return new Tuple<bool, string>(true, "Subsidy saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error saving subsidy.");
        }

        public Tuple<bool, string> delSubsidy(int subId, int userId)
        {
            if (dbFinance.delSubsidy(subId, userId))
                return new Tuple<bool, string>(true, "Subsidy deleted successfully.");
            else
                return new Tuple<bool, string>(false, "Error deleting subsidy.");
        }

        public Tuple<bool, string> addRegFee(DateTime dt, decimal fee, int userId)
        {
            if (!dbFinance.checkExistingRegFee(dt))
                return new Tuple<bool, string>(false, "Registration fee for the effective date already exist.");

            if (dbFinance.addRegFee(dt, fee, userId))
                return new Tuple<bool, string>(true, "Registration fee saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error saving registration fee");
        }

        public DataTable getAllRegFee()
        {
            return dbFinance.getAllRegFee();
        }

        public Tuple<bool, string> updateRegFee(int feeId, decimal fee, int userId)
        {
            if (dbFinance.updateRegFee(feeId, fee, userId))
                return new Tuple<bool, string>(true, "Registration fee saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error saving registration fee");
        }

        public Tuple<bool, string> delRegFee(int feeId, int userId)
        {
            if (dbFinance.delRegFee(feeId, userId))
                return new Tuple<bool, string>(true, "Registration fee deleted successfully.");
            else
                return new Tuple<bool, string>(false, "Error deleting registration fee");
        }

        public DataTable getAllApplnClassPaymentDetails(string applicantId, PaymentType pymType)
        {
            return dbFinance.getApplnClassPaymentDetails(applicantId, pymType, true);
        }

        public DataTable getAllTraineeClassPaymentDetails(string traineeId, PaymentType pymType)
        {
            return dbFinance.getTraineeClassPaymentDetails(traineeId, pymType, true);
        }

        public DataTable getClassPaymentDetail(int paymentId)
        {
            return dbFinance.getClassPaymentDetails(paymentId);
        }

        public DataTable getApplnClassPaymentTypes(string applicantId)
        {
            return dbFinance.getApplnClassPaymentTypes(applicantId);
        }

        public DataTable getTraineeClassPaymentTypes(string traineeId)
        {
            return dbFinance.getTraineeClassPaymentTypes(traineeId);
        }

        public int getNoOfOutstandingClassPayments()
        {
            return dbFinance.getNoOfOutstandingClassPayments();
        }

        public DataTable searchPayments(string searchType, string searchValue)
        {
            if (searchType == "PD") return dbFinance.searchPaymentsByDate(DateTime.ParseExact(searchValue, "dd MMM yyyy", CultureInfo.InvariantCulture));
            else if (searchType == "APP") return dbFinance.searchPaymentsByApplicant(searchValue);
            else if (searchType == "TR") return dbFinance.searchPaymentsByTrainee(searchValue);
            else if (searchType == "REF") return dbFinance.searchPaymentsByRef(searchValue);
            else if (searchType == "OUT") return dbFinance.getOutstandingClassPayments();
            else return null;
        }

        public void updateApplSubsidy(string applicantId,
            decimal regFee, decimal progFee, int subsidyId, decimal subFee, decimal gst, int userId)
        {
            if (dbFinance.updateApplSubsidy(applicantId, regFee, subsidyId, subFee, gst, userId))
            {
               
            }
        }

        public bool insertOnlineApplnPayment(string applicantid, decimal paymentAmt, string paymentType, string billReferenceNo)
        {
            return dbFinance.insertOnlineApplnPayment(applicantid, paymentAmt, paymentType, billReferenceNo);
        }

        public Tuple<bool, string> updateApplnClassPayment(string applicantId, int programmeBatchId, string idNumber, 
            decimal regFee, decimal progFee, int subsidyId, decimal subFee, decimal gst, DataTable dt, int userId)
        {
            if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No payment to process.");

            //check if amount paid equal to net amt
            decimal nett = (regFee == -1 ? 0 : regFee) + (progFee == -1 ? 0 : progFee) - (subsidyId == -1 ? 0 : subFee) + gst;
            decimal paid = 0;
            foreach (DataRow dr in dt.Rows)
                if (dr["paymentStatus"].ToString() == PaymentStatus.PAID.ToString()) paid += (decimal)dr["paymentAmount"];

            if (paid > nett) return new Tuple<bool, string>(false, "Paid amount is more than net total.");

            if (dbFinance.updateApplnClassPayment(applicantId, programmeBatchId, idNumber, regFee, subsidyId, subFee, gst, dt, userId))
                return new Tuple<bool, string>(true, "Payment updated successfully.");
            else return new Tuple<bool, string>(false, "Error updating payment.");
        }

        public Tuple<bool, string> updateTraineeClassPayment(string traineeId, int programmeBatchId, string idNumber,
            decimal regFee, decimal progFee, int subsidyId, decimal subFee, decimal gst, DataTable dt, int userId)
        {
            if (dt.Rows.Count == 0) return new Tuple<bool, string>(false, "No payment to process.");

            //check if amount paid equal to net amt
            decimal nett = (regFee == -1 ? 0 : regFee) + (progFee == -1 ? 0 : progFee) - (subsidyId == -1 ? 0 : subFee) + gst;
            decimal paid = 0;
            foreach (DataRow dr in dt.Rows)
                if (dr["paymentStatus"].ToString() == PaymentStatus.PAID.ToString()) paid += (decimal)dr["paymentAmount"];

            if (paid > nett) return new Tuple<bool, string>(false, "Paid amount is more than net total.");

            if (dbFinance.updateTraineeClassPayment(traineeId, programmeBatchId, idNumber, regFee, subsidyId, subFee, gst, dt, userId))
                return new Tuple<bool, string>(true, "Payment updated successfully.");
            else return new Tuple<bool, string>(false, "Error updating payment.");
        }

        public Tuple<bool, string> delMakeupPayment(int paymentId, int userId)
        {
            if(dbFinance.delMakeupPayment(paymentId, userId))
                return new Tuple<bool, string>(true, "Payments deleted successfully.");
            else
                return new Tuple<bool, string>(false, "Error deleting payments.");
        } 

        public Tuple<bool, string> delApplnClassPayment(string applicantId, PaymentType pymType, int userId)
        {
            if (dbFinance.delApplnClassPayment(applicantId, pymType, userId))
                return new Tuple<bool, string>(true, "Payments deleted successfully.");
            else
                return new Tuple<bool, string>(false, "Error deleting payments.");
        }

        public DataTable getAvailableSubsidy(int programmeId)
        {
            return dbFinance.getAvailableSubsidy(programmeId);
        }

        public DataTable getPaymentModes()
        {
            return dbFinance.getPaymentModes();
        }

        public decimal getCurrentRegFee()
        {
            return dbFinance.getCurrentRegFee();
        }

        //Get payment history details related to application ID 
        public DataTable getPaymentHistoryByApplicantId(string applicantId)
        {
            DataTable dtPaymentHistory = dbFinance.getPaymentHistoryByApplicantId(applicantId);

            return dtPaymentHistory;

        }

        #region Yong Xiang Codes

        // retrieve void receipt number by applicant Id this is used once course receipt is voided.
        //this will be used in applicant details. aspx
        public string getReptNumberCSE(string applicantId)
        {
            return dbFinance.getReptNumberCSE(applicantId);
        }

        // retrieve void receipt number by applicant Id this is used once course receipt is voided.
        //this will be used in applicant details. aspx (for full payment)
        public string getReptNumberFULLCSEPayment(string applicantId)
        {
            return dbFinance.getReptNumberFULLCSEPayment(applicantId);
        }

        // retrieve void receipt number by applicant Id this is used once registration receipt is voided.
        //this will be used in applicant details. aspx
        public string getReptNumberREG(string applicantId)
        {
            return dbFinance.getReptNumberREG(applicantId);
        }

        #endregion

    }


}
