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
    public class Blacklist_Management
    {
        private DB_Blacklist dbBlacklist = new DB_Blacklist();


        public DataTable getSuspensionListWithinPeriod()
        {
            DataTable dtSuspendedList = dbBlacklist.getSuspensionListWithinPeriod();

            if (dtSuspendedList == null) return null;

            return dtSuspendedList;
        }

        public bool searchBlackListRecordByNRICSuspensionDate(string nric, DateTime startDate, DateTime endDate)
        {
            Cryptography cryp = new Cryptography();

            string encryptedIdNumber = cryp.encryptInfo(nric);

            bool success = dbBlacklist.searchBlackListRecordByNRICSuspensionDate(encryptedIdNumber, startDate, endDate);

            return success;
        }
        //Search for blacklist by NRIC
        public string searchBlacklistedRecordByNRIC(string identity)
        {
            string result = "";

            bool exist = dbBlacklist.searchBlacklistedRecordByNRIC(identity);

            if (exist)
            {
                result = General_Constance.STATUS_YES;
            }
            else
            {
                result = General_Constance.STATUS_NO;

            }

            return result;
        }

        //Get suspended/black list
        public DataTable getSuspendedList()
        {
            DataTable dtSuspendedList = dbBlacklist.getSuspendedList();

            if (dtSuspendedList == null) return null;

            if (dtSuspendedList.Rows.Count > 0)
            {
                Cryptography decryp = new Cryptography();


                foreach (DataRow dr in dtSuspendedList.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());

                }
            }
            
            return dtSuspendedList;
        }

        //Get suspended/black list by NRIC or Name
        public DataTable getSuspendedListByValue(string value)
        {
            Cryptography cryp = new Cryptography();

            DataTable dtSuspendedListByValue = dbBlacklist.getSuspendedListByValue(cryp.encryptInfo(value), value);

            if (dtSuspendedListByValue == null) return null;

            if (dtSuspendedListByValue.Rows.Count > 0)
            {
                foreach (DataRow dr in dtSuspendedListByValue.Rows)
                {
                    dr["idNumber"] = cryp.decryptInfo(dr["idNumber"].ToString());

                }
            }

            return dtSuspendedListByValue;
        }

        //Remove from blacklist/suspension
        public bool removeFromSuspension(string supsendedid)
        {

            bool success = dbBlacklist.removefromSuspension(supsendedid);

            return success;
        }

        //Add entry to blacklist/suspension
        public bool addSuspension(string idNumber, string fullName, DateTime startDate, DateTime endDate, string type, string remarks, int userId)
        {
            Cryptography cryp = new Cryptography();

            string encryptedIdNumber = cryp.encryptInfo(idNumber);

            bool success = dbBlacklist.addSuspension(encryptedIdNumber, fullName, startDate, endDate, type, remarks, userId);

            return success;
        }
    }

    
}
