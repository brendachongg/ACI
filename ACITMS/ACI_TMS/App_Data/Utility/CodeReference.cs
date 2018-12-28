using System;

namespace GeneralLayer
{
    //this file contains the codes used in the code_reference table that are also being used in the .cs files
    //so as to facilitate maintainence

      public enum ApplicantDocumentType
    {
        CERT, 
        WTS, 
        ID
    }

    public enum DailySettlementStatus
    {
        P, //Pending
        A,  //Approved
        R,  //Reject
        V //Verified
    }

    public enum DayPeriod
    {
        AM,
        PM,
        EVE,    //Evening
        FD      //full day, not found in database, only use in UI to indicate both AM and PMs
    }

    public enum ClassType
    {
        SAT_D,  //Day - Every Saturday
        SAT_E,  //Evening - Every Saturday
        SUN_D,  //Day - Every Sunday
        SUN_E,  //Evening - Every Sunday
        WDY_D,  //Day - Monday to Friday
        WDY_E,  //Evening -  Monday to Friday
        WEN_D,  //Day - Saturday to Sunday
        WEN_E,  //Evening -  Saturday to Sunday
    }

    public enum PaymentType
    {
        REG,
        PROG,
        BOTH,
        MAKEUP
    }

    public enum PaymentMode
    {
        CHEQ,
        NETS,
        AXS,
        SFC, //SkillsFuture Credit
        PSEA,
        RFI //Request for invoice (company sponsored)
    }



    public enum PaymentStatus
    {
        PAID,
        PEND,   //Pending clearence
        WAIVED,
        VOID
    }

    public enum SubsidyType
    {
        AMT,
        RATE
    }

    public enum ModuleResult
    {
        C,      //Competent
        NYC,    //Not Yet Competent
        PA,     //Pending Assessment/Attendance
        EXEM    //Exempted
    }

    public enum StaffEmploymentType
    {
        FT,     //Full Time
        ADJ,    //Adjuct
        TMP,    //Temporary
        INT,    //Intern
        CON     //Contract
    }

    public enum SOAStatus
    {
        NYA,    //Not Yet Attain
        PROC,   //Processing
        RSOA    //Receieved SOA
    }

    public enum ProgrammeType
    {
        FQ,     //Full Qualification
        SCWSQ,  //Short Course WSQ
        SCNWSQ  //Short Course Non WSQ
    }

    public enum InterviewStatus
    {
        PASSED,     //Passed
        FAILED,     //Failed
        PD,         //Pending
        NYD,        //Not Yet Done
        NREQ        //Not Required (for Short Courses)
    }

    public enum TraineeStatus
    {
        CC,     //Program Cancelled
        C,      //Completed
        E,      //Enrolled
        W       //Withdraw
    }

    public enum Sponsorship
    {
        SELF,   //self sponsored
        COMP    //company sponsored
    }

    public enum ApplicantStatus
    {
        PD,     //Pending (Yet To Call)
        WD,     //Withdraw
        WL,     //Waiting List
        NEW     //New
    }

    public enum IDType
    {
        NRIC = 1,
        FIN = 2,
        Oth = 3
    }

    public enum Salutation
    {
        Dr = 0,
        Mr = 1,
        Mrs = 2,
        Ms = 3,
        Mdm = 4,
        Prof = 5
    }

    public enum Proficiency
    {
        None = 0,
        Fair = 1,
        Good = 2,
        Excellent = 3
    }

    public enum EmplStatus
    {
        UNEM,   //Unemployed
        EM,     //Employed
        SELF,   //Self-employed
        EN,     //Employed in non-F&B
        SN      //Self-employed in Non-F&B
    }

    public enum EmplOccupation
    {
        Legislators_SeniorOfficials_Managers = 01,
        Professionals = 02,
        AssociateProfessionals_Technicians = 03,
        ClericalWorkers = 04,
        Service_Sales_Workers = 05,
        Agricultural_Fishery_Workers = 06,
        Production_Craftsmen_Workers = 07,
        Machine_Operators_Assemblers = 08,
        Cleaners_Labourer = 09,
        Not_Classified = 10
    }

    //public enum EmplOccupation
    //{
    //    Legislators_SeniorOfficials_Managers = "01",
    //    Professionals = "02",
    //    AssociateProfessionals_Technicians = "03",
    //    ClericalWorkers = "04",
    //    Service_Sales_Workers = "05",
    //    Agricultural_Fishery_Workers = "06",
    //    Production_Craftsmen_Workers = "07",
    //    Machine_Operators_Assemblers = "08",
    //    Cleaners_Labourers = "09",
    //    Not_Classified = "10"
    //}


    public enum GetToKnowChannel
    {
        NEWSP,  //Newspaper
        MAG,    //Magazine
        FB,     //Facebook
        WOM,     //Word of Mouth
        OTH     //Others
    }

    public enum CaseLogCategory
    {
        SYSERR    //System Error
    }

    public enum CaseLogStatus
    {
        RS,     //Resolving
        NEW,    //New 
        C,      //Closed
        UA      //Unattended (not stored in db table)
    }
}