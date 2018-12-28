using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Globalization;


namespace LogicLayer
{

    public class Result
    {
        public string resultTxt { get; set; }
        public bool result { get; set; }
    }


    /// <summary>
    /// Summary description for ACITMS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ACITMS : System.Web.Services.WebService
    {
        private Programme_Management progManagement = new Programme_Management();
        private Bundle_Management bundleManagement = new Bundle_Management();
        private Applicant_Management am = new Applicant_Management();
        private Finance_Management fm = new Finance_Management();

        public ACITMS()
        {
        }

        [WebMethod]
        public string gerReceiptNumber()
        {
            return fm.getReceiptNumber();
        }

        [WebMethod]
        public bool insertOnlinePayment(string applicantid, decimal paymentAmt, string paymentType, string billReferenceNo)
        {
            return (new Finance_Management()).insertOnlineApplnPayment(applicantid, paymentAmt, paymentType, billReferenceNo);
        }


        [WebMethod]
        public Result registerApplicantFull(int programmeId, int programmeBatchId, string spon, string fullName, string idType, string id, string nationality, string race, string gender, DateTime dtDOB
            , string email, string contact1, string contact2, string addr, string postal, string lang, string highEdulvl, string empl, string knowChannel, int userId, bool isOnline, string preferredModeOfPayment)
        {

            List<LanguageProficiency> langProf = new List<LanguageProficiency>();
            List<EmploymentRecord> emplRec = new List<EmploymentRecord>();

            dynamic o = JsonConvert.DeserializeObject(lang);

            foreach (var obj in o)
            {
                LanguageProficiency proficiency = new LanguageProficiency()
                {
                    lang = obj["lang"].ToString(),
                    spoken = (Proficiency)Enum.Parse(typeof(Proficiency), obj["spoken"].ToString()),
                    written = (Proficiency)Enum.Parse(typeof(Proficiency), obj["written"].ToString())
                };
                langProf.Add(proficiency);
            }


            List<GetToKnowChannel> kChannels = new List<GetToKnowChannel>();
            o = JsonConvert.DeserializeObject(knowChannel);
            foreach (var obj in o)
            {
                kChannels.Add((GetToKnowChannel)Enum.Parse(typeof(GetToKnowChannel), obj.ToString()));
            }

            o = JsonConvert.DeserializeObject(empl);
            foreach (var obj in o)
            {
                DateTime dtStart = DateTime.ParseExact(obj["dtStart"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture);
                DateTime dtEnd = obj["dtEnd"].ToString() == "" ? DateTime.MaxValue : DateTime.ParseExact(obj["dtEnd"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture);

                EmploymentRecord rec =

                new EmploymentRecord()
                {
                    companyName = obj["companyName"].ToString(),
                    dept = obj["dept"].ToString(),
                    designation = obj["designation"].ToString(),
                    status = (EmplStatus)Enum.Parse(typeof(EmplStatus), obj["status"].ToString()),
                    occupationType = (EmplOccupation)int.Parse(obj["occupationType"].ToString()),
                    salary = decimal.Parse(obj["salary"].ToString()),
                    dtStart = dtStart,
                    dtEnd = dtEnd
                };

                emplRec.Add(rec);

            }

            
            Result rs = new Result();
            try {
                Tuple<bool, string> registerResult = am.registerApplicantFull(programmeId, programmeBatchId, (Sponsorship)Enum.Parse(typeof(Sponsorship), spon), fullName, (IDType)int.Parse(idType), id, nationality, race, gender, dtDOB
                , email, contact1, contact2, addr, postal, langProf.ToArray(), highEdulvl, emplRec.ToArray(), kChannels.ToArray(), userId, isOnline, preferredModeOfPayment);
                            rs.result = registerResult.Item1;
                rs.resultTxt = registerResult.Item2;
           }catch(Exception ex){
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "ACITMS.asmx", "registerApplicantFull()", ex.ToString(), userId, false);
           }




            return rs;
        }

        [WebMethod]
        public DataSet loadEmploymentParameters()
        {
            DataSet ds = new DataSet();
            DataTable dt = am.getEmploymentJob();
            ds.Tables.Add(dt);
            dt = am.getEmploymentStatus();
            ds.Tables.Add(dt);

            return ds;
        }

        [WebMethod]
        public DataSet loadPaymentMode()
        {
            DataSet ds = new DataSet();
            DataTable dt = (new Finance_Management()).getPaymentModes();
            ds.Tables.Add(dt);
            return ds;
        }
        [WebMethod]
        public DataSet loadEducationLanguageParameters()
        {
            DataSet ds = new DataSet();
            DataTable dt = am.getAllLanguageProficiencyCodeReference();
            ds.Tables.Add(dt);
            dt = am.getAllEducationCodeReference();
            ds.Tables.Add(dt);
            return ds;

        }

        [WebMethod]
        public DataSet loadGetToKnowParameters()
        {
            DataSet ds = new DataSet();
            DataTable dt = am.getAllGetToKnowChannelCodeReference();
            ds.Tables.Add(dt);

            return ds;
        }

        [WebMethod]
        public DataSet loadApplicantParameters()
        {
            DataSet ds = new DataSet();

            DataTable dt = am.getNationalityCodeReference();

            ds.Tables.Add(dt);


            dt = am.getIdentificationTypeCodeReference();
            ds.Tables.Add(dt);



            dt = am.getRaceCodeReference();
            ds.Tables.Add(dt);

            return ds;

        }

        [WebMethod]
        public DataSet getSponsorship()
        {
            DataSet ds = new DataSet();
            DataTable dt = (new Applicant_Management()).getSponsorship();
            ds.Tables.Add(dt);
            return ds;
        }

        [WebMethod]
        public DataSet getProgrammeCategory()
        {
            DataSet ds = new DataSet();
            DataTable dt = (new Programme_Management()).getProgrammeCategory();
            ds.Tables.Add(dt);
            return ds;
        }

        [WebMethod]
        public DataSet getAvailableProgrammeForReg(string progCat)
        {
            DataSet ds = new DataSet();
            DataTable dt = (new Programme_Management()).getAvailableProgrammeForReg(progCat);
            ds.Tables.Add(dt);
            return ds;
        }

        [WebMethod]
        public DataSet getAvailableProgrammesForReg(int programmeid)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(progManagement.getAvailableProgrammeDateForReg(programmeid));
            return ds;
        }

        [WebMethod]
        public DataSet getProgrammesRelatedInfo()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(progManagement.getProgrammeCategory());
            ds.Tables.Add(progManagement.getProgrammeLevel());
            ds.Tables.Add(progManagement.getProgrammeType());


            return ds;


        }

        [WebMethod]
        public DataSet retrieveProgrammesById(string pid, string bid)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(progManagement.getProgrammeById(pid));
            ds.Tables.Add(progManagement.getProgrammesModules(bid));
            if (bundleManagement.getBundle(int.Parse(bid)) == null) return null;

            decimal totalCost = bundleManagement.getBundle(int.Parse(bid), true).Item5;

            DataTable dataTable = new DataTable();
            DataRow dr = dataTable.NewRow();
            dataTable.Columns.Add("TotalCostWithGST");

            dr["TotalCostWithGST"] = totalCost + (totalCost * General_Constance.GST_RATE);
            dataTable.Rows.InsertAt(dr, 0);
            ds.Tables.Add(dataTable);

            return ds;
        }

        [WebMethod]
        public DataSet retrieveProgrammes(string progCat)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(progManagement.getAllAvailableProgrammesWS(progCat));
            return ds;
        }

        public DataSet retrieveProgrammesClassByCategory(string category)
        {
            DataSet ds = new DataSet();
            return ds;
        }


        [WebMethod]
        public DataSet retrieveModules(string bundleid)
        {

            DataSet ds = new DataSet();
            DataTable dt = progManagement.getProgrammesModules(bundleid);
            if (dt.Rows.Count > 0)
                ds.Tables.Add(dt);
            else
                return null;
            return ds;
        }

    }
}

