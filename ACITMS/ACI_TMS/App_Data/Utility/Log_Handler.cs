using DataLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GeneralLayer
{
    public class Log_Handler
    {
        public void WriteLog(string systemErrorMsg, string programName, string methodName, string description, int userID, bool logToDB = true)
        {
            StringBuilder sBuilder = new StringBuilder();

            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "log" + Path.DirectorySeparatorChar.ToString() + "Error.log";

            sBuilder.Append(Environment.NewLine + DateTime.Now.ToString());
            sBuilder.Append(" | ");
            sBuilder.Append(userID);
            sBuilder.Append(" | ");
            sBuilder.Append(programName);
            sBuilder.Append(" | ");
            sBuilder.Append(methodName);
            sBuilder.Append(" | ");
            sBuilder.Append(description);
            sBuilder.Append(" | ");
            sBuilder.Append(systemErrorMsg);

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(sBuilder.ToString());
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(sBuilder.ToString());
                }
            }

            if(logToDB) 
                //(new DB_Caselog()).createErrorCaseLog((CaseLogCategory)Enum.Parse(typeof(CaseLogCategory), CaseLogCategory.SYSERR.ToString()), DateTime.Now, systemErrorMsg, 
                //    "Error happen at " + programName + " " + methodName + ". " + description, General_Constance.SYS_USR);
                (new CaseLog_Management()).createCaseLog(CaseLogCategory.SYSERR, DateTime.Now, systemErrorMsg.Replace("\r", "").Replace("\n", ""),
                    "Error happen at " + programName + " " + methodName + ". " + description, null, null, null, General_Constance.SYS_USR); 
        }

        public void WriteLog(Exception systemErr, string programName, string methodName, string description, int userID, bool logToDB = true)
        {
            StringBuilder sBuilder = new StringBuilder();

            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "log" + Path.DirectorySeparatorChar.ToString() + "Error.log";

            sBuilder.Append(Environment.NewLine + DateTime.Now.ToString());
            sBuilder.Append(" | ");
            sBuilder.Append(userID);
            sBuilder.Append(" | ");
            sBuilder.Append(programName);
            sBuilder.Append(" | ");
            sBuilder.Append(methodName);
            sBuilder.Append(" | ");
            sBuilder.Append(description);
            sBuilder.Append(" | ");
            sBuilder.Append(systemErr.StackTrace);

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(sBuilder.ToString());
                }
            }
            else { 
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(sBuilder.ToString());
                }
            }

            if (logToDB) 
                //(new DB_Caselog()).createErrorCaseLog((CaseLogCategory)Enum.Parse(typeof(CaseLogCategory), CaseLogCategory.SYSERR.ToString()), DateTime.Now, systemErr.Message, 
                //"Error happen at " + programName + " " + methodName + ". " + systemErr.StackTrace, General_Constance.SYS_USR); 
                (new CaseLog_Management()).createCaseLog(CaseLogCategory.SYSERR, DateTime.Now, systemErr.Message.Replace("\r", "").Replace("\n", ""),
                    "Error happen at " + programName + " " + methodName + ". " + systemErr.StackTrace, null, null, null, General_Constance.SYS_USR); 
        }
    }
}