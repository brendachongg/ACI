using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer;
using System.Collections;
using System.Data;


namespace LogicLayer
{
    [Serializable]
    public struct DailySettlementDetails
    {
        public string paymentMode { get; set; }
        public decimal feesCollected { get; set; }
        public decimal lessSubsidy { get; set; }
    }
    [Serializable]
    public struct DailySettlementRecords
    {
        public string applicantname { get; set; }
        public string applicantnric { get; set; }
        public string programmeName { get; set; }
        public DateTime progStartDate { get; set; }
        public DateTime progEndDate { get; set; }
        public string courseCode { get; set; }
        public string projectCode { get; set; }
        public decimal adminFeesWOGst { get; set; }
        public decimal adminFeesGst { get; set; }
        public decimal adminFeesWGst { get; set; }
        public decimal courseFeesWOGst { get; set; }
        public decimal courseFessGst { get; set; }
        public decimal courseFeesWGst { get; set; }
        public decimal lessScheme { get; set; }
        public decimal totalCourseFees { get; set; }
        public decimal totalFeesCollected { get; set; }
        public string remarks { get; set; }

        public string paymentMode { get; set; }


    }
    public class Daily_Settlement_Management
    {
        private DB_Daily_Settlement dbDailySett = new DB_Daily_Settlement();

        public void rejectSettlement(int loginid, int settlementid, string remarks, bool isACI = true)
        {
            dbDailySett.rejectSettlement(loginid, settlementid, remarks);
        }

        public void confirmSettlement(int loginid, int settlementid)
        {
            dbDailySett.confirmSettlement(loginid, settlementid);
        }

        public DataTable getSettlementRecords(int dailysettlementid)
        {
            return dbDailySett.getDailySettlementRecords(dailysettlementid);
        }

        public DataTable getPendingSettlementRecords()
        {
            return dbDailySett.getDailySettlment();
        }
        public void insertSettlementRecords(int loginid, DateTime settlementdate, List<DailySettlementRecords> settlementRecords, List<DailySettlementDetails> settlementDetails)
        {
            dbDailySett.insertSettlementRecords(loginid, settlementdate, settlementRecords, settlementDetails);
        }
    }
}