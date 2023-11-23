using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST.KHCF.Customization.Logic.Classes;

namespace ST.KHCF.Customization.Logic
{
    public class Objects_Logic
    {
        public static UDO_Definition[] All_UDO_Definition;
        public static UDO_Definition[] All_UDT_Definition;

        internal static string Get_Can_Approve_UDF_Name(UDO_Definition UDO_Info)
        {
            string Result;
            switch (UDO_Info.KHCF_Object)
            {
                case KHCF_Objects.CCI_Corporate_Member_Card:
                case KHCF_Objects.CCI_Member_Card:
                    Result = "U_ST_CAN_APPROVE_MEMBERCARD";
                    break;
                case KHCF_Objects.Individual_Membership:
                case KHCF_Objects.Corporate_Membership:
                    Result = "U_ST_CAN_APPROVE_MEMBERSHIP";
                    break;
                case KHCF_Objects.Expected_Donations:
                    Result = "U_ST_CAN_APPROVE_EXP_DONATION";
                    break;
                default:
                    throw new Logic.Custom_Exception($"We there is no can Approve UDF for the Object[{UDO_Info.KHCF_Object}] ");
            }

            return Result;
        }

        internal static void Initialize()
        {
            if (All_UDO_Definition != null && All_UDO_Definition.Length > 0)
            {
                return;
            }

            List<UDO_Definition> Result = new List<UDO_Definition>();

            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Individual_Card, Table_Name = "ST_FUND_INDIV_CARD", Type = Object_Type.UDO_Main, Title = "Individual Contact Card", Has_Draft = false, UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Fundraising_Individual_Address, KHCF_Objects.Fundraising_Individual_Contacts, KHCF_Objects.Fundraising_Member_Card_Attachment }  , Has_BP = true});
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Individual_Address, Table_Name = "ST_FUND_INDIV_ADDR", Type = Object_Type.UDO_Child, Title = "Fundraising Individual Address", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Individual_Contacts, Table_Name = "ST_FUND_INDIV_CONT", Type = Object_Type.UDO_Child, Title = "Fund Individual Contacts", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Member_Card_Attachment, Table_Name = "ST_FUND_INDIV_ATT", Type = Object_Type.UDO_Child, Title = "Fund Individual Attachments", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Corporate_Card, Table_Name = "ST_FUND_CORP_CARD", Type = Object_Type.UDO_Main, Title = "Corporate Contact Card", Has_Draft = false, UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Fundraising_Corporate_Address, KHCF_Objects.Fundraising_Corporate_Contacts, KHCF_Objects.Fundraising_Corporate_Member_Card_Attachment } , Has_BP = true});
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Corporate_Address, Table_Name = "ST_FUND_CORP_ADDR", Type = Object_Type.UDO_Child, Title = "Fundraising Corporate Address", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Corporate_Contacts, Table_Name = "ST_FUND_CORP_CONT", Type = Object_Type.UDO_Child, Title = "Fundraising Corporate Contacts", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fundraising_Corporate_Member_Card_Attachment, Table_Name = "ST_FUND_CORP_ATT", Type = Object_Type.UDO_Child, Title = "Fund Corporate Attachment", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Patients_Card, Table_Name = "ST_PATIENTS_CARD", Type = Object_Type.UDO_Main, Title = "Patients Card", Has_Draft = false, UDO_Modules = KHCF_Modules.Patient, Childs = new KHCF_Objects[] { KHCF_Objects.Patients_Address, KHCF_Objects.Patients_Card_Attachment } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Patients_Address, Table_Name = "ST_PATIENTS_ADDRESS", Type = Object_Type.UDO_Child, Title = "Patients Address", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Patients_Card_Attachment, Table_Name = "ST_PATIENTS_CRD_ATT", Type = Object_Type.UDO_Child, Title = "Patients Card Attachments", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Social_Study, Table_Name = "ST_SOCIAL_STUDY", Type = Object_Type.UDO_Main, Title = "Social Study", Has_Draft = false, UDO_Modules = KHCF_Modules.Patient, Childs = new KHCF_Objects[] { KHCF_Objects.Social_Study_Address, KHCF_Objects.Social_Study_Attachment, KHCF_Objects.Social_Study_Health_Issue, KHCF_Objects.Social_Study_Special_Needs, KHCF_Objects.Social_Previous_Support } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Social_Study_Address, Table_Name = "ST_SOCIAL_STUDY_ADD", Type = Object_Type.UDO_Child, Title = "Social Study Address", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Social_Study_Attachment, Table_Name = "ST_SOCIAL_STUDY_ATT", Type = Object_Type.UDO_Child, Title = "Social Study Attachments", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Social_Study_Special_Needs, Table_Name = "ST_SOCIAL_SPEC_NED", Type = Object_Type.UDO_Child, Title = "Social Special Needs", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Social_Study_Health_Issue, Table_Name = "ST_SOCIAL_HLTH_ISU", Type = Object_Type.UDO_Child, Title = "Social Health Issue", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Social_Previous_Support, Table_Name = "ST_SOCIAL_PREV_SUP", Type = Object_Type.UDO_Child, Title = "Social Previous Support", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition()
            {
                KHCF_Object = KHCF_Objects.CCI_Member_Card,
                Table_Name = "ST_CCI_INDIV_CARD",
                Type = Object_Type.UDO_Main,
                Title = "Individual Member Card",
                Has_Draft = false,
                UDO_Modules = KHCF_Modules.CCI,
                Childs = new KHCF_Objects[] { KHCF_Objects.CCI_Member_Card_Address, KHCF_Objects.CCI_Member_Card_Attachment, KHCF_Objects.CCI_Individual_Member_Card_Contacts },
                //External_Key = "U_ST_NATIONAL_ID",
                Has_BP = true
            });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Member_Card_Address, Table_Name = "ST_CCI_INDIV_ADDR", Type = Object_Type.UDO_Child, Title = "CCI Individual Address", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Individual_Member_Card_Contacts, Table_Name = "ST_CCI_INDIV_CONT", Type = Object_Type.UDO_Child, Title = "CCI Individual Contacts", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Member_Card_Attachment, Table_Name = "ST_CCI_INDIV_ATT", Type = Object_Type.UDO_Child, Title = "CCI Individual Attachments", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Individual_Membership_Attachment, Table_Name = "ST_CCI_IND_SHP_ATT", Type = Object_Type.UDO_Child, Title = "CCI Individual Membership Att", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Corporate_Member_Card, Table_Name = "ST_CCI_CORP_CARD", Type = Object_Type.UDO_Main, Title = "Corporate Member Card", Has_Draft = false, UDO_Modules = KHCF_Modules.CCI, Has_BP = true, Childs = new KHCF_Objects[] { KHCF_Objects.CCI_Corporate_Member_Card_Address, KHCF_Objects.CCI_Corporate_Member_Card_Contacts, KHCF_Objects.CCI_Corporate_Member_Card_Attachment } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Corporate_Member_Card_Address, Table_Name = "ST_CCI_CORP_ADDR", Type = Object_Type.UDO_Child, Title = "CCI Corporate Address", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Corporate_Member_Card_Contacts, Table_Name = "ST_CCI_CORP_CONT", Type = Object_Type.UDO_Child, Title = "CCI Corporate Contacts", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Corporate_Member_Card_Attachment, Table_Name = "ST_CCI_CORP_ATT", Type = Object_Type.UDO_Child, Title = "CCI Corporate Attachments", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Coverage_Request, Table_Name = "ST_COVERAGE_REQUEST", Type = Object_Type.UDO_Main, Title = "Coverage Request", Has_Draft = false, UDO_Modules = KHCF_Modules.Patient, Childs = new KHCF_Objects[] { KHCF_Objects.Coverage_Request_Attachment , KHCF_Objects.Coverage_Request_Treatment} });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Coverage_Request_Attachment, Table_Name = "ST_COVER_REQ_ATT", Type = Object_Type.UDO_Child, Title = "Coverage Request Attachments", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Coverage_Request_Treatment, Table_Name = "ST_COVER_REQ_TREAT", Type = Object_Type.UDO_Child, Title = "Coverage Request Treatment", UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Coverage_Transaction, Table_Name = "ST_COVERAGE_TRANS", Type = Object_Type.UDO_Main, Title = "Coverage Transaction", Has_Draft = false, UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Accommodation, Table_Name = "ST_ACCOMMODATION", Type = Object_Type.UDO_Main, Title = "Accommodation", Has_Draft = false, UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Treatment_Plan_Details, Table_Name = "ST_TREATM_PLAN_DET", Type = Object_Type.UDO_Main, Title = "Treatment Plan Details", Has_Draft = false, UDO_Modules = KHCF_Modules.Patient });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Areas_and_Transportation_Types, Table_Name = "ST_AREAS_TRANS_TYPS", Type = Object_Type.UDO_Main, Title = "Areas & Transportation Types", Has_Draft = false, UDO_Modules = KHCF_Modules.Patient });
            //Result.Add(new UDO_Definition() { KHCF_Object = HCF_Objects.CCI_Individual_Member_Attachment, Table_Name = "ST_INDIV_ATT", Type = Object_Type.UDO_Child, Title = "CCI Individual Membership Attachments", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition()
            {
                KHCF_Object = KHCF_Objects.Individual_Membership,
                Table_Name = "ST_INDIV_MEMBERSHIP",
                
                Type = Object_Type.UDO_Main,
                Title = "Individual Membership",
                UDO_Modules = KHCF_Modules.CCI,
                External_Key = "", 
                Childs = new KHCF_Objects[] { KHCF_Objects.CCI_Individual_Membership_Attachment },
                SQL_Existing_Query = @"SELECT T0.* FROM ""@ST_INDIV_MEMBERSHIP""  T0 INNER JOIN ""@ST_CCI_INDIV_CARD""  T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" WHERE T1.U_ST_NATIONAL_ID = '{0}' AND  T0.U_ST_ACTIVE = 'Y'"
            });
            Result.Add(new UDO_Definition()
            {
                KHCF_Object = KHCF_Objects.Corporate_Membership,
                Table_Name = "ST_CORP_MEMBERSHIP",
                Type = Object_Type.UDO_Main,
                Title = "Corporate Membership",
                UDO_Modules = KHCF_Modules.CCI,
                External_Key = "U_ST_CORPORATE_NATIONAL_ID", Childs = new KHCF_Objects[] { KHCF_Objects.CCI_Corporate_Membership_Attachment },
                SQL_Existing_Query = @"SELECT T0.* FROM ""@ST_CORP_MEMBERSHIP""  T0 INNER JOIN ""@ST_CCI_CORP_CARD""  T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" WHERE T1.U_ST_NATIONAL_ID = '{0}' AND  T0.U_ST_ACTIVE = 'Y'"
            });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.CCI_Corporate_Membership_Attachment, Table_Name = "ST_CCI_COR_SHP_ATT", Type = Object_Type.UDO_Child, Title = "CCI Individual Membership Att", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Coverage_Rules, Table_Name = "ST_COVERAGE_RULES", Type = Object_Type.UDO_Main, Title = "Coverage Rules", UDO_Modules = KHCF_Modules.CCI, Childs = new KHCF_Objects[] { KHCF_Objects.Coverage_Rules_Lines } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Coverage_Rules_Lines, Table_Name = "ST_COVERAGE_RULES_L", Type = Object_Type.UDO_Child, Title = "Coverage Rules Lines", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Sales_Target, Table_Name = "ST_SALES_TARGET", Type = Object_Type.UDO_Main, Title = "Sales Target", UDO_Modules = KHCF_Modules.CCI, Childs = new KHCF_Objects[] { KHCF_Objects.Sales_Target_Lines } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Sales_Target_Lines, Table_Name = "ST_SALES_TARGET_L", Type = Object_Type.UDO_Child, Title = "Sales Target Lines", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Commission_Rules, Table_Name = "ST_COMMISSION_RULES", Type = Object_Type.UDO_Main, Title = "Commission Rules", UDO_Modules = KHCF_Modules.CCI, Childs = new KHCF_Objects[] { KHCF_Objects.Commission_Rules_Lines } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Commission_Rules_Lines, Table_Name = "ST_COMMISS_RULES_L", Type = Object_Type.UDO_Child, Title = "Commission Rules Lines", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Expected_Donations_Naming_Lines, Table_Name = "ST_FUND_EXP_DON_NAM", Type = Object_Type.UDO_Child, Title = "Expected Donations Naming", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Expected_Donations_Machinery_Lines, Table_Name = "ST_FUND_EXP_DON_MAC", Type = Object_Type.UDO_Child, Title = "Expected Donations Machinery", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Expected_Donations_Patients_Lines, Table_Name = "ST_FUND_EXP_DON_PAT", Type = Object_Type.UDO_Child, Title = "Expected Donations Patients", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Expected_Donations_Orphans_Lines, Table_Name = "ST_FUND_EXP_DON_ORP", Type = Object_Type.UDO_Child, Title = "Expected Donations Orphans", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Expected_Donations_Department_Lines, Table_Name = "ST_FUND_EXP_DON_DEP", Type = Object_Type.UDO_Child, Title = "Expected Donations Departments", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Expected_Donations_Payment_Lines, Table_Name = "ST_FUND_EXP_DON_PAY", Type = Object_Type.UDO_Child, Title = "Expected Donations Payments", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Expected_Donations, Table_Name = "ST_EXPEC_DONATION", Type = Object_Type.UDO_Main, Title = "Expected Donation", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] {KHCF_Objects.Expected_Donations_Payment_Lines } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Actual_Donations_Naming_Lines, Table_Name = "ST_FUND_ACT_DON_NAM", Type = Object_Type.UDO_Child, Title = "Actual Donations Naming", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Actual_Donations_Machinery_Lines, Table_Name = "ST_FUND_ACT_DON_MAC", Type = Object_Type.UDO_Child, Title = "Actual Donations Machinery", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Actual_Donations_Patients_Lines, Table_Name = "ST_FUND_ACT_DON_PAT", Type = Object_Type.UDO_Child, Title = "Actual Donations Patients", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Actual_Donations_Orphans_Lines, Table_Name = "ST_FUND_ACT_DON_ORP", Type = Object_Type.UDO_Child, Title = "Actual Donations Orphans", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Actual_Donations, Table_Name = "ST_ACTUAL_DONATIONS", Type = Object_Type.UDO_Main, Title = "Actual Donation", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Actual_Donations_Patients_Lines, KHCF_Objects.Actual_Donations_Orphans_Lines,  KHCF_Objects.Actual_Donations_Naming_Lines, KHCF_Objects.Actual_Donations_Machinery_Lines } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Booth_Commission, Table_Name = "ST_BOOTH_COMMISSION", Type = Object_Type.UDO_Main, Title = "Booth Commission", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Plaque_Wall, Table_Name = "ST_PLAQUE_WALL", Type = Object_Type.UDO_Main, Title = "Plaque Wall", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Plaque_Wall_Lines } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Plaque_Wall_Lines, Table_Name = "ST_PLAQUE_LINES", Type = Object_Type.UDO_Child, Title = "Plaque Wall Lines", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Tracking_Recognitions, Table_Name = "ST_TRACKING_REC", Type = Object_Type.UDO_Main, Title = "Recognitions", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Tracking_Recognitions_Attachment } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Tracking_Recognitions_Attachment, Table_Name = "ST_TRACKING_REC_ATT", Type = Object_Type.UDO_Child, Title = "Tracking Recognitions Attach", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Recommending_Recognitions, Table_Name = "ST_RECOMMEND_REC", Type = Object_Type.UDO_Main, Title = "Recognition Criteria", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Recommending_Recognitions_Types } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Recommending_Recognitions_Types, Table_Name = "ST_RECOM_REC_TYPE", Type = Object_Type.UDO_Child, Title = "Recommending Recognitions Type", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Naming, Table_Name = "ST_NAMING", Type = Object_Type.UDO_Main, Title = "Areas", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Naming_Attachment } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Naming_Attachment, Table_Name = "ST_NAMING_ATT", Type = Object_Type.UDO_Child, Title = "NAMING Attachment", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Machinery, Table_Name = "ST_MACHINERY", Type = Object_Type.UDO_Main, Title = "Machines", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Machinery_Attachment } }); //KHCF_Objects.Machinery_Installation_Info,
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Machinery_Installation_Det, Table_Name = "ST_MACHIN_DET", Type = Object_Type.UDO_Main, Title = "Machine Details", UDO_Modules = KHCF_Modules.Fundraising , Childs = new KHCF_Objects[] { KHCF_Objects.Machinery_Installation_L } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Machinery_Installation_L, Table_Name = "ST_MACHIN_DET_L", Type = Object_Type.UDO_Child, Title = "Machine Detials Lines", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Machinery_Installation_Info, Table_Name = "ST_MACHIN_INST_INFO", Type = Object_Type.UDO_Child, Title = "Machinery Installation Info", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Machinery_Attachment, Table_Name = "ST_MACHIN_ATT", Type = Object_Type.UDO_Child, Title = "Machinery Attachment", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Monthly_Giving, Table_Name = "ST_MONTHLY_GIVING", Type = Object_Type.UDO_Main, Title = "Recurring Donations", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Monthly_Giving_Accounting } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Monthly_Giving_Accounting, Table_Name = "ST_MONTH_GIVING_ACC", Type = Object_Type.UDO_Child, Title = "Monthly Giving Accounting", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds, Table_Name = "ST_GOOD_WILL_FUNDS", Type = Object_Type.UDO_Main, Title = "Goodwill Funds", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Goodwill_Funds_Attachment } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Attachment, Table_Name = "ST_GOOD_WILL_ATT", Type = Object_Type.UDO_Child, Title = "Goodwill Funds Attachment", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Participants, Table_Name = "ST_GOOD_WILL_PARTI", Type = Object_Type.UDO_Child, Title = "Goodwill Pledge Participants", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Departments, Table_Name = "ST_GOOD_WILL_DEPS", Type = Object_Type.UDO_Child, Title = "Goodwill Departments", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Co_Departments, Table_Name = "ST_GOODWILL_CODEP", Type = Object_Type.UDO_Child, Title = "Goodwill Co Departments", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Report, Table_Name = "ST_GOODWILL_REPORT", Type = Object_Type.UDO_Child, Title = "Goodwill Report Requirements", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Nationality, Table_Name = "ST_GOODWILL_NAT", Type = Object_Type.UDO_Child, Title = "Goodwill Nationality", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Sub_Nationality, Table_Name = "ST_GW_SUB_NAT", Type = Object_Type.UDO_Child, Title = "Goodwill Sub Nationality", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Cancer_Type, Table_Name = "ST_GW_CAN_TYPE", Type = Object_Type.UDO_Child, Title = "Goodwill Cancer Type", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Goodwill_Funds_Support_Type, Table_Name = "ST_GW_SUP_TYPE", Type = Object_Type.UDO_Child, Title = "Goodwill Support Type", UDO_Modules = KHCF_Modules.Fundraising });

            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Grants, Table_Name = "ST_GRANTS", Type = Object_Type.UDO_Main, Title = "Grants", UDO_Modules = KHCF_Modules.Fundraising , Childs = new KHCF_Objects[] { KHCF_Objects.Grants_Lines } });
            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Grants_Lines, Table_Name = "ST_GRANTS_LINES", Type = Object_Type.UDO_Child, Title = "Grants Lines", UDO_Modules = KHCF_Modules.Fundraising });
            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Potential_Grants, Table_Name = "ST_POTENTIAL_GRANTS", Type = Object_Type.UDO_Main, Title = "Potential Grants", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants, Table_Name = "ST_WON_GRANTS", Type = Object_Type.UDO_Main, Title = "Grants", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] {  KHCF_Objects.Won_Grants_Donation_Amount, KHCF_Objects.Won_Grants_Post_Dates, KHCF_Objects.Won_Grants_Reports_Req } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Target, Table_Name = "ST_FUND_TARGET", Type = Object_Type.UDO_Main, Title = "Allocation", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] {KHCF_Objects.Fund_Target_Wishes, KHCF_Objects.Fund_Target_Orph, KHCF_Objects.Fund_Target_Pats,KHCF_Objects.Fund_Target_Machines, KHCF_Objects.Fund_Target_Areas } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Target_Machines, Table_Name = "ST_TARGET_MACHINES", Type = Object_Type.UDO_Child, Title = "Fund Target Machines", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Target_Areas, Table_Name = "ST_TARGET_AREAS", Type = Object_Type.UDO_Child, Title = "Fund Target Areas", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Target_Wishes, Table_Name = "ST_TARGET_WISH", Type = Object_Type.UDO_Child, Title = "Fund Target Wishes", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Target_Orph, Table_Name = "ST_TARGET_ORPH", Type = Object_Type.UDO_Child, Title = "Fund Target Orphans", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Target_Pats, Table_Name = "ST_TARGET_PATS", Type = Object_Type.UDO_Child, Title = "Fund Target Patients", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Rules, Table_Name = "ST_FUND_RULES", Type = Object_Type.UDO_Main, Title = "Fund Rules", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Fund_Rules_Attachment, KHCF_Objects.Goodwill_Funds_Support_Type, KHCF_Objects.Goodwill_Funds_Cancer_Type,KHCF_Objects.Goodwill_Funds_Report, KHCF_Objects.Goodwill_Funds_Nationality, KHCF_Objects.Goodwill_Funds_Sub_Nationality,KHCF_Objects.Fund_Rules_Age } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Rules_Attachment, Table_Name = "ST_FUND_RULE_ATT", Type = Object_Type.UDO_Child, Title = "Fund Rule Attachment", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Fund_Rules_Age, Table_Name = "ST_FUND_RULE_AGE", Type = Object_Type.UDO_Child, Title = "Fund Rule Age", UDO_Modules = KHCF_Modules.Fundraising });

            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Grants, Table_Name = "ST_GRANTS", Type = Object_Type.UDO_Main, Title = "Grants", UDO_Modules = KHCF_Modules.Fundraising , Childs = new KHCF_Objects[] { KHCF_Objects.Grants_Lines } });
            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Grants_Lines, Table_Name = "ST_GRANTS_LINES", Type = Object_Type.UDO_Child, Title = "Grants Lines", UDO_Modules = KHCF_Modules.Fundraising });
            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Potential_Grants, Table_Name = "ST_POTENTIAL_GRANTS", Type = Object_Type.UDO_Main, Title = "Potential Grants", UDO_Modules = KHCF_Modules.Fundraising });
            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants, Table_Name = "ST_WON_GRANTS", Type = Object_Type.UDO_Main, Title = "Won Grants", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Won_Grants_Wishes,KHCF_Objects.Won_Grants_Orph, KHCF_Objects.Won_Grants_Donation_Amount } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants_Wishes, Table_Name = "ST_WON_GRANT_WISH", Type = Object_Type.UDO_Child, Title = "Won Grants Wishes", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants_Orph, Table_Name = "ST_WON_GRANT_ORPH", Type = Object_Type.UDO_Child, Title = "Won Grants Orphans", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants_Patients, Table_Name = "ST_WON_GRANT_PATS", Type = Object_Type.UDO_Child, Title = "Won Grants Patients", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants_Donation_Amount, Table_Name = "ST_WON_GRANT_AMOUNT", Type = Object_Type.UDO_Child, Title = "Won Grants Amount", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants_Reports_Req, Table_Name = "ST_GRANT_REPORTS", Type = Object_Type.UDO_Child, Title = "Won Grants Reports Requirments", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Won_Grants_Post_Dates, Table_Name = "ST_WON_GRANT_POS_DT", Type = Object_Type.UDO_Child, Title = "Won_Grants_Post_Dates", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Pledges, Table_Name = "ST_PLEDGES", Type = Object_Type.UDO_Main, Title = "Pledges", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Pledges_Patients, KHCF_Objects.Pledges_Machinery_Lines, KHCF_Objects.Pledges_Participant_Lines } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Pledges_Patients, Table_Name = "ST_PLEDGES_PATIENTS", Type = Object_Type.UDO_Child, Title = "Pledges Patients", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Pledges_Machinery_Lines, Table_Name = "ST_PLEDGES_MAC", Type = Object_Type.UDO_Child, Title = "Pledges Machinery", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Pledges_Participant_Lines, Table_Name = "ST_PLEDGES_PART", Type = Object_Type.UDO_Child, Title = "Pledges Participant", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Dreams_Come_True, Table_Name = "ST_DREAMS_COME_TRUE", Type = Object_Type.UDO_Main, Title = "Dreams Come True", UDO_Modules = KHCF_Modules.Fundraising , Childs = new KHCF_Objects[] { KHCF_Objects.Dreams_Come_True_Attachment } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Dreams_Come_True_Attachment, Table_Name = "ST_DREAMS_COME_ATT", Type = Object_Type.UDO_Child, Title = "Goodwill Funds Attachment", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Patient_Activity, Table_Name = "ST_PATIENT_ACTIVITY", Type = Object_Type.UDO_Main, Title = "Patient Activities", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Patient_Activity_Patients_Lines , KHCF_Objects.Patient_Activity_Attachment} });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Patient_Activity_Patients_Lines, Table_Name = "ST_PATIENT_ACTV_PAT", Type = Object_Type.UDO_Child, Title = "Patient Activity Patients", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Patient_Activity_Attachment, Table_Name = "ST_PATIENT_ACTV_ATT", Type = Object_Type.UDO_Child, Title = "Patient Activity Attachment", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Communication_Log, Table_Name = "ST_COMMUNICAT_LOG", Type = Object_Type.UDO_Main, Title = "Communication Log", UDO_Modules = KHCF_Modules.CCI });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Schools_Universites, Table_Name = "ST_SCHOOL_UNI", Type = Object_Type.UDO_Main, Title = "Schools & Universites Activity", UDO_Modules = KHCF_Modules.Fundraising, Childs = new KHCF_Objects[] { KHCF_Objects.Schools_And_Universites_Details, KHCF_Objects.Schools_And_Universites_Activity, KHCF_Objects.Schools_And_Universites_Item, KHCF_Objects.Schools_And_Universites_Grade } });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Schools_And_Universites_Details, Table_Name = "ST_SCHOOL_UNI_DET", Type = Object_Type.UDO_Child, Title = "School/Unversity Details", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Schools_And_Universites_Activity, Table_Name = "ST_SCHOOL_UNI_ACT", Type = Object_Type.UDO_Child, Title = "School/Unversity Activities", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Schools_And_Universites_Item, Table_Name = "ST_SCHOOL_UNI_ITM", Type = Object_Type.UDO_Child, Title = "School/Unversity Items", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Schools_And_Universites_Grade, Table_Name = "ST_SCHOOL_GRADE", Type = Object_Type.UDO_Child, Title = "School/Unversity Grade", UDO_Modules = KHCF_Modules.Fundraising });
            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Schools_And_Universites_Details, Table_Name = "ST_SCHOOL_UNI_DET", Type = Object_Type.UDO_Child, Title = "School/Unversity Details", UDO_Modules = KHCF_Modules.Fundraising });
            Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Ambassador, Table_Name = "ST_AMBASSADOR", Type = Object_Type.UDO_Main, Title = "Ambassador Activity", UDO_Modules = KHCF_Modules.Fundraising });
            //Result.Add(new UDO_Definition() { KHCF_Object = KHCF_Objects.Ambassador_Team_Members, Table_Name = "ST_AMB_TEAM_MEMBER", Type = Object_Type.UDO_Child, Title = "Ambassador Team Member", UDO_Modules = KHCF_Modules.Fundraising });
            All_UDO_Definition = Result.ToArray();

            Result = new List<UDO_Definition>();


            //Result.Add(new UDO_Definition() { Table_Name = "ST_TITLE", Type = Object_Type.UDT, Title = "Title" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_ACTIVITY_TYPE", Type = Object_Type.UDT, Title = "Activity Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_ACT_SUB_TYPE", Type = Object_Type.UDT, Title = "Activity Sub Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_TEAM_MEMBER", Type = Object_Type.UDT, Title = "Team Member" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_RELATION_FATHER", Type = Object_Type.UDT, Title = "Relationship Father" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_BRANCH", Type = Object_Type.UDT, Title = "Branch" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_LOCATION", Type = Object_Type.UDT, Title = "Location" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SUB_LOCATION", Type = Object_Type.UDT, Title = "Sub-Location" });
           // Result.Add(new UDO_Definition() { Table_Name = "ST_RECOGNI_REASON", Type = Object_Type.UDT, Title = "Recognition Reason" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_RECOGNITION_TYPE", Type = Object_Type.UDT, Title = "Recognition Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_AREA_TYPE", Type = Object_Type.UDT, Title = "Area Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_KHCC_DEPAR_AR", Type = Object_Type.UDT, Title = "KHCC department AR" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_STREAM", Type = Object_Type.UDT, Title = "Stream" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PROGRAM_TOOL", Type = Object_Type.UDT, Title = "Program-Tool" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DONATION_PURPOSE", Type = Object_Type.UDT, Title = "Donation Purpose" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DONA_SUB_PURPOSE", Type = Object_Type.UDT, Title = "Donation Sub-Purpose" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_GRANTS_PORTALS", Type = Object_Type.UDT, Title = "Grants Portals" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_EXPEC_POST_DATES", Type = Object_Type.UDT, Title = "Expected Post Dates" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_AREA_OF_INTEREST", Type = Object_Type.UDT, Title = "Area of Interest" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_ALERT_MODE", Type = Object_Type.UDT, Title = "Alert Mode" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SEARCH_FREQUENCY", Type = Object_Type.UDT, Title = "Search Frequency" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_ATTACHMENT_TYPE", Type = Object_Type.UDT, Title = "Attachment Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_INCOME_SOURCE", Type = Object_Type.UDT, Title = "Income Source" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_RESIDENCE_TYPE", Type = Object_Type.UDT, Title = "Residence Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_RESID_OWNERSHIP", Type = Object_Type.UDT, Title = "Residence Ownership" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PREV_COVER_PARTY", Type = Object_Type.UDT, Title = "Previous Coverage Party" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_CANCER_TYPE", Type = Object_Type.UDT, Title = "Cancer Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_RELIGION", Type = Object_Type.UDT, Title = "Religion" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SUB_NATIONALITY", Type = Object_Type.UDT, Title = "Sub Nationality" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DIAGNOSIS_STATUS", Type = Object_Type.UDT, Title = "Diagnosis Status" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_TREATMENT_PLAN", Type = Object_Type.UDT, Title = "Treatment Plan" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_CITY_AREA", Type = Object_Type.UDT, Title = "City_Area" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_TRANSPORT_TYPE", Type = Object_Type.UDT, Title = "Transportation Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_CHANNEL", Type = Object_Type.UDT, Title = "Channel" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SUB_CHANNEL", Type = Object_Type.UDT, Title = "Sub Channel" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_JOB_TITLE", Type = Object_Type.UDT, Title = "Job Title " });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SECTOR", Type = Object_Type.UDT, Title = "Sector" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_COVERAGE", Type = Object_Type.UDT, Title = "Coverage" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_VIP_RELATED_TO", Type = Object_Type.UDT, Title = "VIP Related to" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PAYMENT_METHOD", Type = Object_Type.UDT, Title = "Payment Method" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_COMM_STATUS", Type = Object_Type.UDT, Title = "Communication Status" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_COMM_SUB_STATUS", Type = Object_Type.UDT, Title = "Communication Sub Status" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_COMM_TYPE", Type = Object_Type.UDT, Title = "Communication Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_COMM_SOURCE", Type = Object_Type.UDT, Title = "Communication Source" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DATA_SOURCE", Type = Object_Type.UDT, Title = "Data Source" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_MAIN_SECTOR", Type = Object_Type.UDT, Title = "Main Sector" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SUB_SECTOR", Type = Object_Type.UDT, Title = "Sub Sector" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PREFIX", Type = Object_Type.UDT, Title = "Prefix" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_OPPORT_TYPES", Type = Object_Type.UDT, Title = "Opportunity Types" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_QUOTA_SERVICE", Type = Object_Type.UDT, Title = "Quotation Service" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_QUOTA_SERVICE", Type = Object_Type.UDT, Title = "Quotation Service" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PROGRAM_LEVEL1", Type = Object_Type.UDT, Title = "Program level1" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DON_PURPOSE_LVL2", Type = Object_Type.UDT, Title = "Donation Purpose level2" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DON_SUB_PRP_LVL3", Type = Object_Type.UDT, Title = "Donation Sub Purpose level3" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DON_CST_CTR_LVL4", Type = Object_Type.UDT, Title = "Donation Cost Center level4" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_TRIP_NAME", Type = Object_Type.UDT, Title = "Trip Name" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_GIVEN_RECOGNIT", Type = Object_Type.UDT, Title = "Given Recognition" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_RECOGNIT_REASON", Type = Object_Type.UDT, Title = "Recognition Reason" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_RECOGNITION_TYPE", Type = Object_Type.UDT, Title = "Recognition Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PATIENT_STRY_TYP", Type = Object_Type.UDT, Title = "Patient Story Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PATIENT_STRY_USE", Type = Object_Type.UDT, Title = "Patient Story Use" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_REPORTS_FREQUEN", Type = Object_Type.UDT, Title = "Reports Frequency" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SUPPORT_TYPE", Type = Object_Type.UDT, Title = "Support Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_REPORTS_REQUIRE", Type = Object_Type.UDT, Title = "Reports Requirements" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PLAY_THERAPIST", Type = Object_Type.UDT, Title = "Play Therapist" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DREAMS_TYPE", Type = Object_Type.UDT, Title = "Dreams Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_DREAMS_SUB_TYPE", Type = Object_Type.UDT, Title = "Dreams Sub Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PATIENT_ACTV_TYP", Type = Object_Type.UDT, Title = "Patient Activity Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SCHOOLS_UNIVERS", Type = Object_Type.UDT, Title = "Schools Universities " });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PARTICIPA_GRADES", Type = Object_Type.UDT, Title = "Participating Grades " });
            Result.Add(new UDO_Definition() { Table_Name = "ST_TREATMENT_TYPE", Type = Object_Type.UDT, Title = "Treatment Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_INVOICE_SOURCE", Type = Object_Type.UDT, Title = "Invoice Source" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_GODWIL_FUNDS_TYP", Type = Object_Type.UDT, Title = "Goodwill Funds Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_STEER_COMM_USERS", Type = Object_Type.UDT, Title = "Steering Committee Users" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_Waiting_List", Type = Object_Type.UDT, Title = "Waiting List Users" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_CONFIG_VALUES", Type = Object_Type.UDT, Title = "Configuration Values" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_FILTERED_SECTS", Type = Object_Type.UDT, Title = "Filtered Sectores" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_INDIV_ATT", Type = Object_Type.UDT, Title = "CCI Individual Attachments" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_CORP_ATT", Type = Object_Type.UDT, Title = "CCI Corporate Attachments" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_COV_GR_RULES", Type = Object_Type.UDT, Title = "Covergae  Groups Rule" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_AREA_BUILD_NAME", Type = Object_Type.UDT, Title = "Area Building Name" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_AREA_FL_NUM", Type = Object_Type.UDT, Title = "Area Floor Number" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_AREA_FL_FUNC", Type = Object_Type.UDT, Title = "Area Floor Function" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SEASON_GREETING", Type = Object_Type.UDT, Title = "Season Greeting" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_GREETING_METHOD", Type = Object_Type.UDT, Title = "Greeting Method" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_PAT_ACT_SUPPORT", Type = Object_Type.UDT, Title = "Patient Activity Support" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_INITIATIVE", Type = Object_Type.UDT, Title = "School Initiative" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_ITEM_TYPE", Type = Object_Type.UDT, Title = "Item Type" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_ITEM_DETAILS", Type = Object_Type.UDT, Title = "Item Details " });
            Result.Add(new UDO_Definition() { Table_Name = "ST_ITEM_THEME", Type = Object_Type.UDT, Title = "Item Theme " });
            Result.Add(new UDO_Definition() { Table_Name = "ST_REQ_FOLL", Type = Object_Type.UDT, Title = "requires follow up" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_OCCASION", Type = Object_Type.UDT, Title = "occasion" });
            Result.Add(new UDO_Definition() { Table_Name = "ST_SUBJECT", Type = Object_Type.UDT, Title = "Subject" });

            All_UDT_Definition = Result.ToArray();


        }
    }
}
