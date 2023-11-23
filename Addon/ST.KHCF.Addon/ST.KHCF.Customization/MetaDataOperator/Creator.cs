using SAPbobsCOM;
using UserFields = ST.Helper.MetaDataOperater.UserFields;
using UserTable = ST.Helper.MetaDataOperater.UserTable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST.KHCF.Customization.Logic;

namespace ST.KHCF.Customization.MetaDataOperator
{
    public class Creator
    {
        static Company company;

        public static void CreateAll(Company _company)
        {
            company = _company;
            Logic.General_Logic.Initialize();

            ConfigurationTable();
            Frequencies();
            Frequency_Repetition();
            Frequency_Repetition_Mapping();

            Create_All_UDTs();

            Create_All_UDO_Tables();

            #region Roles
            Role();
            ApplicationStatus();
            RolePermission();
            #endregion
            Prefix();

            Coverage_Fields();
            Coverage_Groups_RulesTable();

            Invoice_Account_Mapping();

            Sub_Channel_Fields();
            Sub_Status_Fields();
            Sub_Sector_Fields();

            Field_Change_Log();

            Config_Value_Fields();

            Donation_Accounts();

            //Waiting_List();

            Job_Title_Fields();
            City_Area_Fields();
            Activity_Sub_Fields();
            Team_Member_Fields();
            Cancer_Type_Fields();

            Program_Level1_Fields();
            Program_Tool_Fields();

            Donation_Cost_Center_Level4_Fields();

            Support_Type_Fields();

            Goodwill_Funds_Type();
            
            Waiting_List();

            System_UDFs();

            Relations_Objects();

            Set_Default_Data();

            Filtered_Sectores();
            DreamsSubType();
           // 

        }

        private static void Invoice_Account_Mapping()
        {
            if (!UserTable.UserTableExist("ST_INV_ACCOUNT_MAPP", company))
                UserTable.CreateUserTable("ST_INV_ACCOUNT_MAPP", "Invoice Account Mapping", company);

            UserFields.createUserField("ST_COVERAGE", "Coverage", BoFieldTypes.db_Alpha, 254, "@ST_INV_ACCOUNT_MAPP", company, "ST_COVERAGE");
            UserFields.createUserField("ST_CUSTOMER_GROUP_CODE", "Customer Group Code", BoFieldTypes.db_Numeric, 11, "@ST_INV_ACCOUNT_MAPP", company);
            UserFields.createUserField("ST_CUSTOMER_GROUP_NAME", "Customer Group Name", BoFieldTypes.db_Alpha, 100, "@ST_INV_ACCOUNT_MAPP", company);
            //   UserFields.createUserField("ST_ACCOUNT", "Account", BoFieldTypes.db_Alpha, 20, "@ST_INV_ACCOUNT_MAPP", company);
            UserFields.createUserField("ST_UNEARNED_REVENUE_ACCOUNT", "Unearned Revenue Account", BoFieldTypes.db_Alpha, 254, "@ST_INV_ACCOUNT_MAPP", company, "");
            UserFields.createUserField("ST_REVENUE_ACCOUNT", "Revenue Account", BoFieldTypes.db_Alpha, 254, "@ST_INV_ACCOUNT_MAPP", company, "");

        }

        private static void Filtered_Sectores()
        {
           // UserFields.createUserField("ST_SECTOR", "Sector", BoFieldTypes.db_Alpha, 254, "@ST_FILTERED_SECTS", company, "ST_SECTOR");
            UserFields.createUserField("ST_SUB_SECTOR", "Sub Sector", BoFieldTypes.db_Alpha, 254, "@ST_FILTERED_SECTS", company, "ST_SUB_SECTOR");
        }



        private static void Sub_Sector_Fields()
        {
            UserFields.createUserField("ST_SECTOR", "Sector", BoFieldTypes.db_Alpha, 254, "@ST_SUB_SECTOR", company, "ST_SECTOR");

        }
        private static void DreamsSubType()
        {
            UserFields.createUserField("ST_DREAM_TYPE", "Type", BoFieldTypes.db_Alpha, 254, "@ST_DREAMS_SUB_TYPE", company,"ST_DREAMS_TYPE");

        }

        private static void Goodwill_Funds_Type()
        {
            UserFields.createUserField("ST_REVENUE_ACCOUNT_CODE", "Revenue Account", BoFieldTypes.db_Alpha, 254, "@ST_GODWIL_FUNDS_TYP", company);
        }

        private static void Support_Type_Fields()
        {
            UserFields.createUserField("ST_REVENUE_ACCOUNT_CODE", "Revenue Account", BoFieldTypes.db_Alpha, 254, "@ST_SUPPORT_TYPE", company);
            UserFields.createUserField("ST_EXPENSE_ACCOUNT_CODE", "Expense Account", BoFieldTypes.db_Alpha, 254, "@ST_SUPPORT_TYPE", company);
        }

        private static void City_Area_Fields()
        {
            UserFields.createUserField("ST_NAME_IN_ENGLISH", "Name in English", BoFieldTypes.db_Alpha, 100, "@ST_CITY_AREA", company, ""); 
        }

        private static void Job_Title_Fields()
        {
            UserFields.createUserField("ST_ARABIC_NAME", "Arabic Name", BoFieldTypes.db_Alpha, 100, "@ST_JOB_TITLE", company, "");
        }

        private static void Activity_Sub_Fields()
        {
            UserFields.createUserField("ST_ACTIVITY_TYPE", "Activity Type",BoFieldTypes.db_Alpha,254,"@ST_ACT_SUB_TYPE",company, Linked_Table: "ST_ACTIVITY_TYPE");
        }
         private static void Team_Member_Fields()
        {
            UserFields.createUserField("ST_DEPARTMENT", "Department", BoFieldTypes.db_Alpha, 254, "@ST_TEAM_MEMBER", company, "");
        }

        private static void Cancer_Type_Fields()
        {
            UserFields.createUserField("ST_AVERAGE_COST_From", "Average Cost/From", BoFieldTypes.db_Float, 254, "@ST_CANCER_TYPE", company, "", BoFldSubTypes.st_Sum);
            UserFields.createUserField("ST_AVERAGE_COST_To", "Average CostTO", BoFieldTypes.db_Float, 254, "@ST_CANCER_TYPE", company, "", BoFldSubTypes.st_Sum);
        }
        private static void Program_Level1_Fields()
        {
            UserFields.createUserField("ST_GL_ACCOUNT", "GL Account", BoFieldTypes.db_Alpha, 254, "@ST_PROGRAM_LEVEL1", company);
            UserFields.createUserField("ST_VISIBLE_UDFS", "Visible UDFs", BoFieldTypes.db_Memo, 254, "@ST_PROGRAM_LEVEL1", company);
        }
        private static void Program_Tool_Fields()
        {
        //  UserFields.createUserField("ST_VISIBLE_UDFS", "Visible UDFs", BoFieldTypes.db_Memo, 254, "@ST_PROGRAM_TOOL", company);
        }
        private static void Donation_Cost_Center_Level4_Fields()
        {
            UserFields.createUserField("ST_COST_CENTER", "Cost Center", BoFieldTypes.db_Alpha, 254, "@ST_DON_CST_CTR_LVL4", company);
        }
        private static void Set_Default_Data()
        {
           Dictionary<string, string> Support_Types = new Dictionary<string, string>();
            Support_Types.Add("D", "Diagnosis");
            Support_Types.Add("A", "Accommodation");
            Support_Types.Add("T", "Treatment");
            Support_Types.Add("P", "Transportation");
            Support_Types.Add("F", "Food Support");
            Support_Types.Add("S", "Scalp Cooling, Rehabilitation & Others Support");

            Add_Lines_To_UDT(Support_Types, "ST_SUPPORT_TYPE");
        }

        private static void Add_Lines_To_UDT(Dictionary<string, string> Code_Names_List, string UDT_Table_Name)
        {
            SAPbobsCOM.UserTable UDT = company.UserTables.Item(UDT_Table_Name);
            foreach (var item in Code_Names_List)
            {
                string SQL = $@"Select Count(*) from ""@{UDT_Table_Name}"" WHERE ""Code"" = '{item.Key}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if ((int)RC.Fields.Item(0).Value == 0)
                {
                    UDT.Code = item.Key;
                    UDT.Name = item.Value;

                    if (UDT.Add() != 0)
                    {
                        throw new Logic.Custom_Exception($"Error during add the Row[{item.Key}][{company.GetLastErrorDescription()}]");
                    }
                }

            }

        }

        private static void Relations_Objects()
        {
            if (!UserTable.UserTableExist("ST_REL_OBJECTS", company))
                UserTable.CreateUserTable("ST_REL_OBJECTS", "Relations Objects", company);

            UserFields.createUserField("ST_KHCF_OBJECT_TYPE", "KHCF Object Type", BoFieldTypes.db_Alpha, 254, "@ST_REL_OBJECTS", company);
            UserFields.createUserField("ST_KHCF_OBJECT_CODE", "KHCF Object Code", BoFieldTypes.db_Alpha, 254, "@ST_REL_OBJECTS", company);

            UserFields.createUserField("ST_KHCF_TABLE_NAME", "KHCF Table Name", BoFieldTypes.db_Alpha, 50, "@ST_REL_OBJECTS", company);
            UserFields.createUserField("ST_KHCF_TABLE_CODE", "KHCF Code in Table", BoFieldTypes.db_Alpha, 50, "@ST_REL_OBJECTS", company);
            UserFields.createUserField("ST_PERCENTAGE", "Percentage", BoFieldTypes.db_Float, 50, "@ST_REL_OBJECTS", company, "", BoFldSubTypes.st_Percentage);
            UserFields.createUserField("ST_AMOUNT", "Amount", BoFieldTypes.db_Float, 50, "@ST_REL_OBJECTS", company, "", BoFldSubTypes.st_Sum);

        }

        private static void Frequency_Repetition_Mapping()
        {
            if (!UserTable.UserTableExist("ST_FREQ_REPET_MAPP", company))
                UserTable.CreateUserTable("ST_FREQ_REPET_MAPP", "Frequency Repetition Mapping", company);
      
            UserFields.createUserField("ST_FREQUENCY", "Frequency", BoFieldTypes.db_Alpha, 254, "@ST_FREQ_REPET_MAPP", company, "ST_FREQUENCIES");
            UserFields.createUserField("ST_FREQ_REPETITION", "Frequency Repetition", BoFieldTypes.db_Alpha, 254, "@ST_FREQ_REPET_MAPP", company, "ST_FREQ_REPETITION");
        
        }

        private static void Frequency_Repetition()
        {
            if (!UserTable.UserTableExist("ST_FREQ_REPETITION", company))
                UserTable.CreateUserTable("ST_FREQ_REPETITION", "Frequency Repetition", company);
        }

        private static void Frequencies()
        {
            if (!UserTable.UserTableExist("ST_FREQUENCIES", company))
                UserTable.CreateUserTable("ST_FREQUENCIES", "Frequencies", company);


        }
        private static void Prefix() 
        {
            List<string> values = new List<string>();
            values.Add("M");
            values.Add("F");
            List<string> descriptions = new List<string>();
            descriptions.Add("Male");
            descriptions.Add("Female");
            UserFields.createUserField("ST_GENDER", "Gender", BoFieldTypes.db_Alpha, 1, "@ST_PREFIX", company, values, descriptions, "M");
            UserFields.createUserField("ST_NAME_IN_ENGLISH", "English Name", BoFieldTypes.db_Alpha, 100, "@ST_PREFIX", company);
        }
        private static void Donation_Accounts()
        {
            if (!UserTable.UserTableExist("ST_DONATION_ACC", company))
                UserTable.CreateUserTable("ST_DONATION_ACC", "Donation Accounts", company);

            UserFields.createUserField("ST_DONATION_PURPOSE_LEVEL2", "Donation Purpose level2", BoFieldTypes.db_Alpha, 254, "@ST_DONATION_ACC", company, "ST_DON_PURPOSE_LVL2");
            UserFields.createUserField("ST_DONATION_SUB_PURPOSE_LEVEL3", "Donation Sub Purpose level3", BoFieldTypes.db_Alpha, 254, "@ST_DONATION_ACC", company, "ST_DON_SUB_PRP_LVL3");
            UserFields.createUserField("GL_ACCOUNT", "GL Account", BoFieldTypes.db_Alpha, 20, "@ST_DONATION_ACC", company);


        }

        private static void Sub_Channel_Fields()
        {
            UserFields.createUserField("ST_CHANNEL", "Channel", BoFieldTypes.db_Alpha, 254, "@ST_SUB_CHANNEL", company, "ST_SUB_CHANNEL");

        }
        private static void Sub_Status_Fields()
        {
            UserFields.createUserField("ST_STATUS", "Status", BoFieldTypes.db_Alpha, 254, "@ST_COMM_SUB_STATUS", company, "ST_COMM_STATUS");

        }

        private static void Field_Change_Log()
        {
            if (!UserTable.UserTableExist("ST_FIELD_CHANGE_LOG", company))
                UserTable.CreateUserTable("ST_FIELD_CHANGE_LOG", "Fields Change Log", company);

            UserFields.createUserField("ST_TABLE_NAME", "Table Name", BoFieldTypes.db_Alpha, 254, "@ST_FIELD_CHANGE_LOG", company);
            UserFields.createUserField("ST_FIELD_NAME", "Field Name", BoFieldTypes.db_Alpha, 254, "@ST_FIELD_CHANGE_LOG", company);
            UserFields.createUserField("ST_PREVIOUS_VALUE", "Previous Value", BoFieldTypes.db_Alpha, 254, "@ST_FIELD_CHANGE_LOG", company);
            UserFields.createUserField("ST_NEW_VALUE", "New Value", BoFieldTypes.db_Alpha, 254, "@ST_FIELD_CHANGE_LOG", company);
            UserFields.createUserField("ST_DATE", "Date", BoFieldTypes.db_Date, 254, "@ST_FIELD_CHANGE_LOG", company);
            UserFields.createUserField("ST_USER", "User", BoFieldTypes.db_Alpha, 254, "@ST_FIELD_CHANGE_LOG", company);


        }

        private static void Coverage_Fields()
        {
            UserFields.createUserField("ST_TREATMENT_LIMIT", "Treatment Limit", BoFieldTypes.db_Float, 254, "@ST_COVERAGE", company, "", BoFldSubTypes.st_Sum);
            UserFields.createUserField("ST_TRANSPORTATION_LIMIT", "Transportation Limit", BoFieldTypes.db_Float, 254, "@ST_COVERAGE", company, "", BoFldSubTypes.st_Sum);
            UserFields.createUserField("ST_UNEARNED_REVENUE_ACCOUNT", "Unearned Revenue Account", BoFieldTypes.db_Alpha, 254, "@ST_COVERAGE", company, "");
            UserFields.createUserField("ST_REVENUE_ACCOUNT", "Revenue Account", BoFieldTypes.db_Alpha, 254, "@ST_COVERAGE", company, "");
        }

        private static void Role()
        {
            if (!Helper.MetaDataOperater.UserTable.UserTableExist("ST_ROLE", company))
                Helper.MetaDataOperater.UserTable.CreateUserTable("ST_ROLE", "Roles", company);

        }

        private static void RolePermission()
        {
            if (!UserTable.UserTableExist("ST_ROLE_PERMISSION", company))
                UserTable.CreateUserTable("ST_ROLE_PERMISSION", "Roles Permission", company);

            UserFields.createUserField("ST_ROLE", "Role", BoFieldTypes.db_Alpha, 50, "@ST_ROLE_PERMISSION", company, "ST_ROLE");

            // UserFields.createUserField("ST_APPLICATION_TYPE", "Application Type", BoFieldTypes.db_Alpha, 2, "@ST_ROLE_PERMISSION", company, "ST_WORKFLOW", BoFldSubTypes.st_None);
            UserFields.createUserField("ST_STATUS", "Status", BoFieldTypes.db_Alpha, 50, "@ST_ROLE_PERMISSION", company, "ST_APP_STATUS");
            UserFields.createUserField("ST_ITEM_IDS", "Items IDs", BoFieldTypes.db_Memo, 254, "@ST_ROLE_PERMISSION", company);
            UserFields.createUserField("ST_COLUMN_IDS", "Column IDs", BoFieldTypes.db_Memo, 254, "@ST_ROLE_PERMISSION", company);

            List<string> values = new List<string>();
            values.Add("1");
            values.Add("2");
            values.Add("3");
            values.Add("4");
            List<string> descriptions = new List<string>();
            descriptions.Add("Hide");
            descriptions.Add("Disable");
            descriptions.Add("Enable");
            descriptions.Add("Mandatory");
            UserFields.createUserField("ST_ATTR", "Attribute Type", BoFieldTypes.db_Alpha, 1, "@ST_ROLE_PERMISSION", company, values, descriptions, "1");

            values = new List<string>();
            values.Add("Y");
            values.Add("N");
            descriptions = new List<string>();
            descriptions.Add("Yes");
            descriptions.Add("No");
            UserFields.createUserField("ST_IS_TAB", "Is Tab", BoFieldTypes.db_Alpha, 1, "@ST_ROLE_PERMISSION", company, values, descriptions, "N");
            UserFields.createUserField("ST_NOTE", "Note", BoFieldTypes.db_Alpha, 254, "@ST_ROLE_PERMISSION", company);
            UserFields.createUserField("ST_FORM_TYPE", "Form Type", BoFieldTypes.db_Alpha, 254, "@ST_ROLE_PERMISSION", company);


        }
        private static void ApplicationStatus()
        {
            if (!UserTable.UserTableExist("ST_APP_STATUS", company))
                UserTable.CreateUserTable("ST_APP_STATUS", "Application Status", company);

            //UserFields.createUserField("ST_STATUS_TEXT", "Status Text", BoFieldTypes.db_Alpha, 100, "@ST_APP_STATUS", company);
            //List<string> values = new List<string>();
            //values.Add("Y");
            //values.Add("N");
            //List<string> descriptions = new List<string>();
            //descriptions.Add("Yes");
            //descriptions.Add("No");
            //UserFields.createUserField("ST_SEND_SMS", "Send SMS", BoFieldTypes.db_Alpha, 1, "@ST_APP_STATUS", company, values, descriptions, "N");
        }

        private static void System_UDFs()
        {
            List<string> values = new List<string>() { "I", "C" };
            List<string> description = new List<string>() { "Individual", "Corporate" };
            UserFields.createUserField("ST_TYPE", "Type", BoFieldTypes.db_Alpha, 1, "OCRG", company, values, description, "I");
            values = new List<string>() { "N", "C", "F", "P", "O" };
            description = new List<string>() { "None", "CCI", "Fundraising", "Patient", "Other" };
            UserFields.createUserField("ST_CUSTOMER_TYPE", "Customer Type", BoFieldTypes.db_Alpha, 1, "OCRG", company, values, description, "N");
            UserFields.createUserField("ST_GL_ACCOUNT", "GL Account", BoFieldTypes.db_Alpha, 20, "OCRG", company);
            UserFields.createUserField("ST_SERIES_ID", "Series ID", BoFieldTypes.db_Numeric, 6, "OCRG", company);
            UserFields.createUserField("ST_ENTITY_TYPE", "Entity Type", BoFieldTypes.db_Alpha, 50, "OCRG", company);

            UserFields.createUserField("ST_ROLE", "Role", BoFieldTypes.db_Alpha, 50, "OUSR", company, "ST_ROLE");
            values = new List<string>() { "Y", "N" };
            description = new List<string>() { "Yes", "No" };
            UserFields.createUserField("ST_CAN_ACTIVE_MEMBERSHIP", "Can Active Membership", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_APPROVE_MEMBERSHIP", "Can Approve Membership", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_APPROVE_MEMBERCARD", "Can Approve Member Card", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_CONVERT_LEAD_TO_CUSTOMER", "Can Convert Lead to Customer", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_STOP_CARD", "Can Stop Card", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_APPROVE_EXP_DONATION", "Can Approve Expected Donation", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_STOP_MEMBERSHIP", "Can Stop Membership", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_CANCEL_MEMBERSHIP", "Can Cancel Membership", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_CHANG_EMP_LEAD", "Can change sales employee on Lead", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_OVERWRITE_DATA", "Can Overwrite Data", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_REMOVE_PATIENT", "Can Remove Patient", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_REMOVE_SOCIAL_STUDY", "Can Remove Social Study", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_COMPLETE_PATIENT_SOCIAL_STUDY", "Can Complete Patient Social Study", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_REJECT_PATIENT_SOCIAL_STUDY", "Can Reject Patient Social Study", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_HAVE_PATIENT_FLAG_ACCESS", "Have Patient Flag Access", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_CANCEL_COVERAGE_REQUEST", "Can Cancel Coverage Request", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_PATIENT_ALLOW_CCI_MEMBERSHIP", "Can Patient Allow CCI Membership", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_EXTEND_RESERVATION", "Can Extend Reservation", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_APPROVE_DREAM", "can approve dreams come true", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");
            UserFields.createUserField("ST_CAN_CONFIRM_EXPECTED_DONATION", "can confirm expected donation", BoFieldTypes.db_Alpha, 1, "OUSR", company, values, description, "N");

            //UserFields.createUserField("ST_JOIN_DATE", "Join Date", BoFieldTypes.db_Date, 1, "OBSL ", company);
            //UserFields.createUserField("ST_DONOR_CODE", "Donor Code", BoFieldTypes.db_Alpha, 1, "OBSL ", company);

            //Finicial Project Fields
            UserFields.createUserField("ST_FROM_DATE", "Trip From Date", BoFieldTypes.db_Date, 254, "OPRJ", company);
            UserFields.createUserField("ST_TO_DATE", "Trip To Date", BoFieldTypes.db_Date, 254, "OPRJ", company);


            //Marketing Doucment Scholarship UDFs
            //OINV
            UserFields.createUserField("ST_SEMESTER", "Scholarship Semester", BoFieldTypes.db_Alpha, 50, "OINV", company);
            UserFields.createUserField("ST_FACULTY", "Scholarship Faculity", BoFieldTypes.db_Alpha, 50, "OINV", company);
            UserFields.createUserField("ST_GPA", "Scholarship GPA", BoFieldTypes.db_Numeric, 10, "OINV", company);
            //ORCT
            UserFields.createUserField("ST_SEMESTER", "Scholarship Semester", BoFieldTypes.db_Alpha, 50, "ORCT", company);
            UserFields.createUserField("ST_FACULTY", "Scholarship Faculity", BoFieldTypes.db_Alpha, 50, "ORCT", company);
            UserFields.createUserField("ST_GPA", "Scholarship GPA", BoFieldTypes.db_Numeric, 10, "ORCT", company);
            //OPCH
            UserFields.createUserField("ST_SEMESTER", "Scholarship Semester", BoFieldTypes.db_Alpha, 50, "OPCH", company);
            UserFields.createUserField("ST_FACULTY", "Scholarship Faculity", BoFieldTypes.db_Alpha, 50, "OPCH", company);
            UserFields.createUserField("ST_GPA", "Scholarship GPA", BoFieldTypes.db_Numeric, 10, "OPCH", company);
            //OIGE
            UserFields.createUserField("ST_SEMESTER", "Scholarship Semester", BoFieldTypes.db_Alpha, 50, "OIGE", company);
            UserFields.createUserField("ST_FACULTY", "Scholarship Faculity", BoFieldTypes.db_Alpha, 50, "OIGE", company);
            UserFields.createUserField("ST_GPA", "Scholarship GPA", BoFieldTypes.db_Numeric, 10, "OIGE", company);
            //OQUT
            UserFields.createUserField("ST_SEMESTER", "Scholarship Semester", BoFieldTypes.db_Alpha, 50, "OQUT", company);
            UserFields.createUserField("ST_FACULTY", "Scholarship Faculity", BoFieldTypes.db_Alpha, 50, "OQUT", company);
            UserFields.createUserField("ST_GPA", "Scholarship GPA", BoFieldTypes.db_Numeric, 10, "OQUT", company);
            //OPOR
            UserFields.createUserField("ST_SEMESTER", "Scholarship Semester", BoFieldTypes.db_Alpha, 50, "OPOR", company);
            UserFields.createUserField("ST_FACULTY", "Scholarship Faculity", BoFieldTypes.db_Alpha, 50, "OPOR", company);
            UserFields.createUserField("ST_GPA", "Scholarship GPA", BoFieldTypes.db_Numeric, 10, "OPOR", company);


            UserFields.createUserField("ST_MEMBERSHIP_CODE", "Membership Code", BoFieldTypes.db_Alpha, 20, "OINV", company);
            UserFields.createUserField("ST_MEMBERSHIP_CODE", "Membership Code", BoFieldTypes.db_Alpha, 20, "ORCT", company);
            UserFields.createUserField("ST_ACTUAL_DONATION_CODE", "Actual Donation Code", BoFieldTypes.db_Alpha, 20, "OINV", company);
            UserFields.createUserField("ST_ACTUAL_DONATION_CODE", "Actual Donation Code", BoFieldTypes.db_Alpha, 20, "ORCT", company);
            values = new List<string>() { "I", "C" };
            description = new List<string>() { "Individual", "Corporate" };
            UserFields.createUserField("ST_MEMBERSHIP_TYPE", "Membership Type", BoFieldTypes.db_Alpha, 1, "OINV", company, values, description, "I");
            UserFields.createUserField("ST_MEMBERSHIP_TYPE", "Membership Type", BoFieldTypes.db_Alpha, 1, "ORCT", company, values, description, "I");
            UserFields.createUserField("ST_PAYMENT_METHOD", "Payment Method", BoFieldTypes.db_Alpha, 50, "ORCT", company, "ST_PAYMENT_METHOD");
            UserFields.createUserField("ST_REVENUE_REALIZATION_JE", "Revenue Realization JE", BoFieldTypes.db_Numeric, 11, "INV1", company);
            UserFields.createUserField("ST_CANCEL_REVENUE_REALIZATION_JE", "Cancellation Revenue Realization JE", BoFieldTypes.db_Numeric, 11, "INV1", company);


            UserFields.createUserField("ST_ITM_TYP", "Item Type", BoFieldTypes.db_Alpha, 50, "OITM", company, "ST_ITEM_TYPE");
            UserFields.createUserField("ST_ITM_DET", "Item Details", BoFieldTypes.db_Alpha, 50, "OITM", company, "ST_ITEM_DETAILS");
            UserFields.createUserField("ST_ITM_THM", "Item Theme", BoFieldTypes.db_Alpha, 50, "OITM", company, "ST_ITEM_THEME");


            UserFields.createUserField("ST_COVERAGE", "Coverage", BoFieldTypes.db_Alpha, 254, @"QUT1", company, "ST_COVERAGE");
            UserFields.createUserField("ST_NUMBER_OF_EMPLOYEES_UNDER_60", "Number of Employees under 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_EMPLOYEES_ABOVE_60", "Number of Employees above 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_FAMILIES_UNDER_60", "Number of Families under 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_FAMILIES_ABOVE_60", "Number of Families above 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_RETIRED_UNDER_60", "Number of Retired under 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_RETIRED_ABOVE_60", "Number of Retired above 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_RETIRED_FAMILIES_UNDER_60", "Number of Retired Families under 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_RETIRED_FAMILIES_ABOVE_60", "Number of Retired Families above 60", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_NUMBER_OF_STUDENTS", "Number of Students", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");
            UserFields.createUserField("ST_TOTAL_NUMBER_OF_EXPECTED_MEMBERS", "Total Number of Expected Members", BoFieldTypes.db_Numeric, 11, @"QUT1", company, "");

            #region OCRD

            UserFields.createUserField("ST_NUMBER_OF_EMPLOYEES_UNDER_60", "Number of Employees under 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMBER_OF_EMPLOYEES_ABOVE_60", "Number of Employees above 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMBER_OF_FAMILIES_UNDER_60", "Number of Families under 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMBER_OF_FAMILIES_ABOVE_60", "Number of Families above 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMBER_OF_RETIRED_UNDER_60", "Number of Retired under 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMBER_OF_RETIRED_ABOVE_60", "Number of Retired above 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMB_RET_FAMILIES_UNDER_60", "Number of Retired Families under 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMB_RET_FAMILIES_ABOVE_60", "Number of Retired Families above 60", BoFieldTypes.db_Numeric, 11, "OCRD", company);
            UserFields.createUserField("ST_NUMBER_OF_STUDENTS", "Number of Students", BoFieldTypes.db_Numeric, 11, "OCRD", company);

            UserFields.createUserField("ST_CHANNEL", "Channel", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_CHANNEL");
            UserFields.createUserField("ST_SUB_CHANNEL", "Sub Channel", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_SUB_CHANNEL");
            UserFields.createUserField("ST_BROKER1", "Broker1", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_DATA_SOURCE", "Data Source", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_DATA_SOURCE");
            UserFields.createUserField("ST_MAIN_SECTOR", "Main Sector", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_MAIN_SECTOR");
            UserFields.createUserField("ST_SUB_SECTOR", "Sub Sector", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_SUB_SECTOR");
            UserFields.createUserField("ST_FILTERED_SECTORS", "Filtered Sectors", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_FILTERED_SECTORES");
            UserFields.createUserField("ST_CORPORATE_NATIONAL_ID", "Corporate National ID", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            // UserFields.createUserField("ST_NATIONAL_ID", "National ID ", BoFieldTypes.db_Numeric, 11, @"OCRD", company, "");
            UserFields.createUserField("ST_PERSONAL_ID", "Personal ID ", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_PASSPORT_ID", "Passport ID", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_NATIONAL_ID", "National ID ", BoFieldTypes.db_Alpha, 10, @"OCRD", company, "");
            UserFields.createUserField("ST_MEMBER_CARD", "Member Card", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");

            UserFields.createUserField("ST_FIRST_NAME_AR", "First Name AR", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_FATHER_NAME_AR", "Father Name AR", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_MIDDLE_NAME_AR", "Middle Name AR", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_SURNAME_AR", "Surname AR", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_FIRST_NAME_EN", "First Name EN", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_FATHER_NAME_EN", "Father Name EN", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_MIDDLE_NAME_EN", "Middle Name EN", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_SURNAME_EN", "Surname EN", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_FULL_NAME_EN", "Full Name EN", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_DATE_OF_BIRTH", "Date of Birth ", BoFieldTypes.db_Date, 254, @"OCRD", company, "");
            values = new List<string>() { "M", "F" };
            description = new List<string>() { "Male", "Female" };
            UserFields.createUserField("ST_GENDER", "Gender", BoFieldTypes.db_Alpha, 254, @"OCRD", company, values, description, "M");
            UserFields.createUserField("ST_NATIONALITY", "Nationality", BoFieldTypes.db_Alpha, 254, @"OCRD", company);
            UserFields.createUserField("ST_ACCOUNT_MANAGER", "Account Manager", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            //UserFields.createUserField("ST_SECTOR", "Sector ", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_SECTOR");
            UserFields.createUserField("ST_GENERAL_MANAGER", "General Manager", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "");
            UserFields.createUserField("ST_JOB_TITLE", "Job Title", BoFieldTypes.db_Alpha, 254, @"OCRD", company, "ST_JOB_TITLE");
            //UserFields.createUserField("ST_BROKER1_CODE", "Broker1 Code", BoFieldTypes.db_Alpha, 254, "OCRD", company);
            UserFields.createUserField("ST_BROKER1_NAME", "Broker1 Name", BoFieldTypes.db_Alpha, 254, "OCRD", company);

            values = new List<string>() { "N", "C", "F", "P", "O" };
            description = new List<string>() { "None", "CCI", "Fundraising", "Patient", "Other" };
            UserFields.createUserField("ST_CUSTOMER_TYPE", "Customer Type", BoFieldTypes.db_Alpha, 1, "OCRD", company, values, description, "N");

            #endregion
            #region Employee Master Data
            UserFields.createUserField("ST_GRANDFATHER_NAME", "Grandfather Name", BoFieldTypes.db_Alpha, 254, @"OHEM", company, "");
            UserFields.createUserField("ST_SHIFT", "Shift", BoFieldTypes.db_Alpha, 254, @"OHEM", company, "");
            UserFields.createUserField("ST_SALARY", "Salary", BoFieldTypes.db_Alpha, 254, @"OHEM", company, "");
            UserFields.createUserField("ST_COMMISSION", "Commission", BoFieldTypes.db_Alpha, 254, @"OHEM", company, "");
            UserFields.createUserField("ST_BOOTH_NAME", "Booth Name", BoFieldTypes.db_Alpha, 254, @"OHEM", company, "ST_SUB_LOCATION");
            values = new List<string>() { "-", "B", "V", "T","R" };
            description = new List<string>() { "", "Booth", "Volunteer", "Teacher","Regular"};
            UserFields.createUserField("ST_EMPLOYEE_TYPE", "Employee Type", BoFieldTypes.db_Alpha, 1, "OHEM", company, values, description, "-");

            #endregion
            #region Purchase Order
            UserFields.createUserField("ST_PATIENT_ID", "Patient ID", BoFieldTypes.db_Alpha, 254, @"OPOR", company, Linked_UDO:"ST_PATIENTS_CARD");
            UserFields.createUserField("ST_DREAMS_ID", "Dreams Come True ID", BoFieldTypes.db_Alpha, 254, @"OPOR", company, Linked_UDO:"ST_DREAMS_COME_TRUE");
            //UserFields.createUserField("ST_DREAMS_TYPE", "Dreams Type", BoFieldTypes.db_Alpha, 254, @"OPOR", company, "ST_DREAMS_TYPE");
            #endregion
            #region Goods Issue
            UserFields.createUserField("ST_PATIENT_ID", "Patient ID", BoFieldTypes.db_Alpha, 254, @"OIGE", company, Linked_UDO: "ST_PATIENTS_CARD");
            UserFields.createUserField("ST_DREAMS_ID", "Dreams Come True ID", BoFieldTypes.db_Alpha, 254, @"OIGE", company, Linked_UDO: "ST_DREAMS_COME_TRUE");
            //UserFields.createUserField("ST_DREAMS_TYPE", "Dreams Type", BoFieldTypes.db_Alpha, 254, @"OIGE", company, "ST_DREAMS_TYPE");
            UserFields.createUserField("ST_RECOGNITION_ID", "Recognition ID", BoFieldTypes.db_Alpha, 254, @"OIGE", company, Linked_UDO: "ST_TRACKING_REC");

            #endregion
            UserFields.createUserField("ST_OPPORTUNITY_TYPES", "Opportunity Types", BoFieldTypes.db_Alpha, 254, @"OOPR", company, "ST_OPPORT_TYPES");
            UserFields.createUserField("ST_COMPETITOR_NAME", "Competitor Name", BoFieldTypes.db_Alpha, 254, @"OPR3", company, "");
            #region Campaign Lines
            UserFields.createUserField("ST_RESPONSIBLE_TWO", "responsible 2", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_APPROACH_METHOD", "approach method", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_OVERALL_STATUS", "overall status", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_PREVIOUS_Y_COMM", "previous year comments", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_COMMENTS", "comments", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_REFERRAL", "referral", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_DONATION_RECEIVED", "donation received", BoFieldTypes.db_Alpha, 50, @"CPN1", company,new List<string> { "Y","N"}, new List<string> { "yes", "No" }, "");
            UserFields.createUserField("ST_REASON", "reason", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_DONATION_METHOD", "donation method", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_RECOGNITION", "recognition ", BoFieldTypes.db_Alpha, 254, @"CPN1", company, Linked_Table : "ST_GIVEN_RECOGNIT");
            UserFields.createUserField("ST_RECOGNITION_STATUS", "recognition status", BoFieldTypes.db_Alpha, 254, @"CPN1", company, new List<string> { "P", "N", "D", "I" }, new List<string> { "pending", "no need", "done", "in progress" }, "");
            UserFields.createUserField("ST_SERIAL", "Serial", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_NEXT_CAM_AMOUNT", "next campaign amount", BoFieldTypes.db_Alpha, 254, @"CPN1", company, "");
            UserFields.createUserField("ST_REQ_FOLLOW_UP", "requires follow up", BoFieldTypes.db_Alpha, 254, @"CPN1", company, Linked_Table : "ST_REQ_FOLL");
            UserFields.createUserField("ST_MIN_GR", "Minimum Guaranteed Donation", BoFieldTypes.db_Numeric, 11, @"CPN1", company, "");
            #endregion
            #region Time sheet Lines
            UserFields.createUserField("ST_TAUGHT_PATIENT", "Taught Patients", BoFieldTypes.db_Alpha, 254, @"TSH1", company,"");
            #endregion

            #region Activity

            values = new List<string>();
            description = new List<string>();
            foreach (var OneObj in Objects_Logic.All_UDO_Definition.Where(O => O.Type == Logic.Classes.Object_Type.UDO_Main))
            {
                values.Add(((int)OneObj.KHCF_Object).ToString());
                description.Add(OneObj.Title);
            }
            List<string> status_values = new List<string>();
            List<string> status_desc = new List<string>();
            status_values.Add("T");
            status_desc.Add("tentative");
            status_values.Add("C");
            status_desc.Add("confirmed");
            status_values.Add("O");
            status_desc.Add("open");
            status_values.Add("R");
            status_desc.Add("canceled");
             status_values.Add("P");
            status_desc.Add("postponed");

            UserFields.createUserField("ST_KHCF_OBJECT_TYPE", "KHCF Object Type", BoFieldTypes.db_Alpha, 3, "OCLG", company, values, description, "");
            UserFields.createUserField("ST_KHCF_OBJECT_CODE", "KHCF Object Code", BoFieldTypes.db_Alpha, 15, "OCLG", company);
            UserFields.createUserField("ST_EXP_DON_CODE", "Expected Donation Code", BoFieldTypes.db_Alpha, 15, "OCLG", company,Linked_UDO: "ST_EXPEC_DONATION");
            UserFields.createUserField("ST_PROGRAM_TOOL", "Program/Tool", BoFieldTypes.db_Alpha, 254, "OCLG", company, Linked_Table: "ST_PROGRAM_LEVEL1");
            UserFields.createUserField("ST_MEETING_STATUS", "Meeting status", BoFieldTypes.db_Alpha, 50, "OCLG", company, status_values,status_desc,"" );
            UserFields.createUserField("ST_ATTIRE", "Attire", BoFieldTypes.db_Alpha, 50, "OCLG", company, new List<string> {"B","S","C"},new List<string> { "Business", "Smart Casual", "Casual" },"" );
            UserFields.createUserField("ST_CONTACT_TYPE", "Contact Type", BoFieldTypes.db_Alpha, 15, "OCLG", company , new List<string> { "I","C"}, new List<string> {"Individual","Corporate" },"");
            UserFields.createUserField("ST_CONTACT_CARD", "Contact Card", BoFieldTypes.db_Alpha, 254, "OCLG", company);
            UserFields.createUserField("ST_OUTCOME", "Outcome", BoFieldTypes.db_Alpha, 254, "OCLG", company);
            UserFields.createUserField("ST_ACTION_ITEM", "Action Item", BoFieldTypes.db_Alpha, 254, "OCLG", company);
            UserFields.createUserField("ST_GIFTS", "gifts", BoFieldTypes.db_Alpha, 254, "OCLG", company);
            UserFields.createUserField("ST_MATERIAL", "material ", BoFieldTypes.db_Alpha, 254, "OCLG", company);
            UserFields.createUserField("ST_OBJECTIVE", "Objective", BoFieldTypes.db_Alpha, 254, "OCLG", company);
            UserFields.createUserField("ST_BACKGROUND", "Background", BoFieldTypes.db_Alpha, 254, "OCLG", company);
            UserFields.createUserField("ST_FOLLOW_STATUS", "Follow up status", BoFieldTypes.db_Alpha, 15, "OCLG", company, new List<string> { "P", "I" , "D" , "N" }, new List<string> { "pending","in progress", "done", "no longer required" }, "");


            #endregion


            #region AP Invoice

            UserFields.createUserField("ST_PATIENT_CODE", "Patient Code", BoFieldTypes.db_Alpha, 50, "OPCH", company);
            values = new List<string>() { "-", "I", "O", "D" };
            description = new List<string>() { "", "In", "Out", "Day Case" };
            UserFields.createUserField("ST_INVOICE_TYPE", "Invoice Type", BoFieldTypes.db_Alpha, 1, "OPCH", company, values, description, "-");
            UserFields.createUserField("ST_INVOICE_SOURCE", "Invoice Source", BoFieldTypes.db_Alpha, 50, "OPCH", company, "ST_INVOICE_SOURCE");
            UserFields.createUserField("ST_INVOICE_DISCOUNT", "Invoice Discount", BoFieldTypes.db_Float, 50, "OPCH", company, "", BoFldSubTypes.st_Sum);
            UserFields.createUserField("ST_COVERAGE_TRANSACTION_CODE", "Coverage Transaction", BoFieldTypes.db_Alpha, 50, "OPCH", company);
            UserFields.createUserField("ST_TARGET_JE_ID", "Target JE ID", BoFieldTypes.db_Numeric, 11, "OPCH", company);
            UserFields.createUserField("ST_RECOGNITION_ID", "Recognition ID", BoFieldTypes.db_Alpha, 254, @"OPCH", company, Linked_UDO: "ST_TRACKING_REC");


            #endregion

            UserFields.createUserField("ST_COVERAGE_RULE", "Coverage Rule", BoFieldTypes.db_Alpha, 10, "OQUT", company, "", BoFldSubTypes.st_None, null, 0, "ST_COVERAGE_RULES");

            #region Quotation

            #endregion
            UserFields.createUserField("ST_TEL_COUNTRY_CODE", "Tel Country Code", BoFieldTypes.db_Alpha, 4, "OCRY", company, "");
            #region Opportunity
            UserFields.createUserField("ST_SUBMISSION_DEADLINE", "Submission Deadline", BoFieldTypes.db_Date, 254, "OOPR", company);
            UserFields.createUserField("ST_ANNOUNCEMENT_DEADLINE", "Announcement Deadline", BoFieldTypes.db_Date, 254, "OOPR", company);
            UserFields.createUserField("ST_GRANT_REQUIREMENT", "Grant Requirements", BoFieldTypes.db_Alpha, 254, "OOPR", company);
            UserFields.createUserField("ST_SUBMISSION_NOTES", "Submission Notes", BoFieldTypes.db_Alpha, 254, "OOPR", company);
            values = new List<string>() { "LID", "LCD", "IDD" };
            description = new List<string>() { "LID", "LCD", "IDD" };
            UserFields.createUserField("ST_RESPONSIBLE_DEPARTMENT", "Responsible Department", BoFieldTypes.db_Alpha, 3, "OOPR", company, values, description, "LID");
            values = new List<string>() { "Y", "N" };
            description = new List<string>() { "Yes", "No" };
            UserFields.createUserField("ST_SUBMITTED", "Submitted", BoFieldTypes.db_Alpha, 1, "OOPR", company, values, description, "N");
            values = new List<string>() { "O", "W", "L" };
            description = new List<string>() { "Open", "Won", "Lost" };
            UserFields.createUserField("ST_STATUS", "Status", BoFieldTypes.db_Alpha, 1, "OOPR", company, values, description, "O");
            UserFields.createUserField("ST_CO_DEPARTMENT", "CO-Department", BoFieldTypes.db_Alpha, 5, "OOPR", company);
            UserFields.createUserField("ST_SUB_ACC_MANAGER", "Sub Account Manager", BoFieldTypes.db_Alpha, 254, "OOPR", company);
            UserFields.createUserField("ST_PROGRAM", "Program", BoFieldTypes.db_Alpha, 10, "OPR4", company, "ST_PROGRAM_LEVEL1");
            values = new List<string>() { "Y", "N", };
            description = new List<string>() { "Yes", "No" };
            UserFields.createUserField("ST_CLOSING_DATE", "Closing Date", BoFieldTypes.db_Date, 254, "OPR4", company);
            UserFields.createUserField("ST_INTERESTED", "Interested", BoFieldTypes.db_Alpha, 1, "OPR4", company, values, description, "N");

            UserFields.createUserField("ST_HP_SHOP", "Hope Shop", BoFieldTypes.db_Alpha, 1, "OITB", company, values, description, "N");

            #endregion



            #region Campaign
            UserFields.createUserField("ST_PROGRAM", "Program", BoFieldTypes.db_Alpha, 10, @"OCPN", company, "ST_PROGRAM_LEVEL1");
            UserFields.createUserField("ST_OCC", "Occasion", BoFieldTypes.db_Alpha, 10, @"OCPN", company, Linked_Table : "ST_OCCASION");
            #endregion

            #region Responses
            UserFields.createUserField("ST_PROGRAM", "Program", BoFieldTypes.db_Alpha, 10, @"ORPT", company, "ST_PROGRAM_TOOL");
            #endregion

            #region Bin Location Master Data
            UserFields.createUserField("ST_IN_DATE", "Installation Date", BoFieldTypes.db_Date, 20, @"OBIN", company, "");
            #endregion


            #region Opportunity 

            values = new List<string>() { "Y", "N", };
            description = new List<string>() { "Yes", "No" };
            UserFields.createUserField("ST_HAVE_CANCER_INS", "Already Have Cancer Insurance", BoFieldTypes.db_Alpha, 1, "OPR3", company, values, description, "N");
            UserFields.createUserField("ST_INSURANCE_END_DATE", "Insurance End Date", BoFieldTypes.db_Date, 1, "OPR3", company);


            #endregion

            #region Journal Entry
            values = new List<string>() { "-", "R", "CR" };
            description = new List<string>() { "", "Revenue Realization", "Cancel Revenue Realization" };
            UserFields.createUserField("ST_TYPE", "Type", BoFieldTypes.db_Alpha, 2, "OJDT", company, values, description, "-");
            UserFields.createUserField("ST_REVENUE_REALIZATION_YEAR", "Revenue Realization Year", BoFieldTypes.db_Numeric, 6, "OJDT", company);
            UserFields.createUserField("ST_REVENUE_REALIZATION_MONTH", "Revenue Realization Month", BoFieldTypes.db_Numeric, 6, "OJDT", company);
            UserFields.createUserField("ST_REVENUE_REALIZATION_INV_NUM", "Revenue Realization Invoice No.", BoFieldTypes.db_Numeric, 11, "OJDT", company);

            #endregion

            #region Time Sheet

            UserFields.createUserField("ST_PROGRAM", "Program", BoFieldTypes.db_Alpha, 10, "OTSH", company, "ST_PROGRAM_LEVEL1");

            #endregion



        }

        private static void ConfigurationTable()
        {
            if (!Helper.MetaDataOperater.UserTable.UserTableExist("ST_KHCF_CONFIG", company))
                Helper.MetaDataOperater.UserTable.CreateUserTable("ST_KHCF_CONFIG", "KHCF Configuration", company);

            Helper.MetaDataOperater.UserFields.createUserField("ST_CONFIG_CODE", "Configuration Code", BoFieldTypes.db_Alpha, 254, "@ST_KHCF_CONFIG", company);
            Helper.MetaDataOperater.UserFields.createUserField("ST_CONFIG_DISC", "Configuration Description", BoFieldTypes.db_Memo, 254, "@ST_KHCF_CONFIG", company);
            Helper.MetaDataOperater.UserFields.createUserField("ST_CONFIG_VALUE", "Configuration Value", BoFieldTypes.db_Memo, 254, "@ST_KHCF_CONFIG", company);
            Helper.MetaDataOperater.UserFields.createUserField("ST_CONFIG_DEFAULT", "Configuration Default Value", BoFieldTypes.db_Memo, 254, "@ST_KHCF_CONFIG", company);

        }

        private static void Coverage_Groups_RulesTable()
        {
            if (!Helper.MetaDataOperater.UserTable.UserTableExist("ST_COV_GR_RULES", company))
                Helper.MetaDataOperater.UserTable.CreateUserTable("ST_COV_GR_RULES", "Coverage Groups Rules", company);

            Helper.MetaDataOperater.UserFields.createUserField("ST_GROUP_CODE", "Group Code", BoFieldTypes.db_Alpha, 254, "@ST_COV_GR_RULES", company);
            Helper.MetaDataOperater.UserFields.createUserField("ST_GROUP_NAME", "Customer Group Name", BoFieldTypes.db_Alpha, 254, "@ST_COV_GR_RULES", company);
            UserFields.createUserField("ST_COVERAGE", "Coverage", BoFieldTypes.db_Alpha, 254, "@ST_COV_GR_RULES", company, "ST_COVERAGE");
            Helper.MetaDataOperater.UserFields.createUserField("ST_AGE_UNDER_60", "Age Under 60", BoFieldTypes.db_Float, 254, "@ST_COV_GR_RULES", company, "", BoFldSubTypes.st_Price);
            Helper.MetaDataOperater.UserFields.createUserField("ST_AGE_ABOVE_60", "Age Above 60", BoFieldTypes.db_Float, 254, "@ST_COV_GR_RULES", company, "", BoFldSubTypes.st_Price);
            Helper.MetaDataOperater.UserFields.createUserField("ST_STUDENT", "Student", BoFieldTypes.db_Float, 254, "@ST_COV_GR_RULES", company, "", BoFldSubTypes.st_Price);

        }

        private static void Create_All_UDTs()
        {
            foreach (var OneUDT in Logic.Objects_Logic.All_UDT_Definition)
            {
                if (!ST.Helper.MetaDataOperater.UserTable.UserTableExist(OneUDT.Table_Name, company))
                {
                    Helper.MetaDataOperater.UserTable.CreateUserTable(OneUDT.Table_Name, OneUDT.Title, company);
                }
            }
        }


        private static void Create_All_UDO_Tables()
        {
            foreach (var OneUDO in Logic.Objects_Logic.All_UDO_Definition)
            //foreach (var OneUDO in Logic.Objects_Logic.All_UDO_Definition.Where(O => O.KHCF_Object == Logic.Classes.KHCF_Objects.CCI_Corporate_Member_Card
            //|| O.KHCF_Object == Logic.Classes.KHCF_Objects.CCI_Member_Card || O.KHCF_Object == Logic.Classes.KHCF_Objects.Fundraising_Corporate_Card
            //|| O.KHCF_Object == Logic.Classes.KHCF_Objects.Fundraising_Individual_Card || O.KHCF_Object == Logic.Classes.KHCF_Objects.Individual_Membership
            //|| O.KHCF_Object == Logic.Classes.KHCF_Objects.Coverage_Rules || O.KHCF_Object == Logic.Classes.KHCF_Objects.Communication_Log
            //|| O.KHCF_Object == Logic.Classes.KHCF_Objects.Corporate_Membership || O.KHCF_Object == Logic.Classes.KHCF_Objects.Sales_Target
            //|| O.KHCF_Object == Logic.Classes.KHCF_Objects.Commission_Rules
            //|| O.KHCF_Object == Logic.Classes.KHCF_Objects.Expected_Donations || O.KHCF_Object == Logic.Classes.KHCF_Objects.Actual_Donations))
            {
                if (OneUDO.Type == Logic.Classes.Object_Type.UDO_Main)
                {
                    if (!ST.Helper.MetaDataOperater.UserTable.UserTableExist(OneUDO.Table_Name, company))
                    {
                        Helper.MetaDataOperater.UserTable.CreateUserTable(OneUDO.Table_Name, OneUDO.Title, company, BoUTBTableType.bott_MasterData);
                    }

                    var Fields_list = Logic.Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == OneUDO.KHCF_Object).ToArray();
                    foreach (var OneField in Fields_list)
                    {
                        Create_One_Field(company, OneField, "@" + OneUDO.Table_Name);
                    }

                    List<string> Child_Table_List = new List<string>();
                    if (OneUDO.Childs != null)
                    {
                        foreach (var OneChild in OneUDO.Childs)
                        {
                           var X = Logic.Objects_Logic.All_UDO_Definition.Where(O => O.KHCF_Object == OneChild).ToArray();
                            var Child_Obj = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == OneChild);
                            if (Child_Obj == null)
                            {
                                throw new Logic.Custom_Exception($"There is no UDO Definition for [{OneChild}] for the UDO[{OneUDO.Title}]");
                            }

                            if (!ST.Helper.MetaDataOperater.UserTable.UserTableExist(Child_Obj.Table_Name, company))
                            {
                                Helper.MetaDataOperater.UserTable.CreateUserTable(Child_Obj.Table_Name, Child_Obj.Title, company, BoUTBTableType.bott_MasterDataLines);
                            }
                            var Line_Fields_list = Logic.Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == Child_Obj.KHCF_Object).ToArray();
                            foreach (var OneField in Line_Fields_list)
                            {
                                Create_One_Field(company, OneField, "@" + Child_Obj.Table_Name);
                            }
                            Child_Table_List.Add(Child_Obj.Table_Name);
                        }

                    }
                    UserTable.CreateUserObject(OneUDO.Table_Name, OneUDO.Table_Name, OneUDO.Title.Replace(" ", "_"), company, BoUDOObjType.boud_MasterData, Child_Table_List.ToArray());
                }
            }

           // Create_BP_Fields(company);
        }

        private static void Create_BP_Fields()
        {
            Logic.Classes.KHCF_Objects[] BP_Objects = new Logic.Classes.KHCF_Objects[] { Logic.Classes.KHCF_Objects.Fundraising_Individual_Card };

            foreach (Logic.Classes.KHCF_Objects OneObj in BP_Objects)
            {
                Logic.Classes.Field_Definition[] Obj_Fields = Logic.Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == OneObj).ToArray();

                foreach (Logic.Classes.Field_Definition One_Field in Obj_Fields)
                {
                    if (!One_Field.Is_Original_System_Field)
                    {
                        Create_One_Field(company, One_Field, "OCRD");
                    }

                }

            }

        }

        private static void Waiting_List()
        {
            UserFields.createUserField("DocEntry", "DocEntry", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_CREATION_DATE", "Creation Date", BoFieldTypes.db_Date, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_M_NUMBER", "Medical Number", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_P_NAME", "Full Arabic Name", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_NATIONAL", "Nationality", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_AGE", "Date OF Birth", BoFieldTypes.db_Date, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_C_TYPE", "Cancer Type", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_P_STATUS", "Patient Status", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_R_SUPPORT", "Support Amount", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_T_AMOUNT", "Total Requested for the whole period", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_R_AMOUNT", "Coverage", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_A_AMOUNT", "Coverage CCI", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            //UserFields.createUserField("ST_R_Amount", "Coverage CCI", BoFieldTypes.db_Float, 254, "@ST_WAITING_LIST", company, "", BoFldSubTypes.st_Sum);
            UserFields.createUserField("ST_A_COST", "Average Cost", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_R_SUPPORT", "Requested Support", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_PREVIOUS_DIAGNOSIS_PLACE", "Previous Diagnosis Place", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_P_PARTY", "Previous Coverage Party", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_P_AMOUNT", "Previous Coverage Amount", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_REQUEST_D", "Request Date", BoFieldTypes.db_Date, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_USER1", "UserSign1", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_GOOD1", "Good Fund", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_AMOUNT1", "Amount", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_REMARK1", "Remarks", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_USER2", "UserSign2", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_GOOD2", "Good Fund", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_AMOUNT2", "Amount", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_REMARK2", "Remarks", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_USER3", "UserSign3", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_GOOD3", "Good Fund", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_AMOUNT3", "Amount", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_REMARK3", "Remarks", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_USER4", "UserSign4", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_GOOD4", "Good Fund", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_AMOUNT4", "Amount", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_REMARK4", "Remarks", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_USER5", "UserSign4", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_GOOD5", "Good Fund", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_AMOUNT5", "Amount", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
            UserFields.createUserField("ST_REMARK5", "Remarks", BoFieldTypes.db_Alpha, 254, "@ST_WAITING_LIST", company, "");
        }

        private static void Config_Value_Fields()
        {
            //UserFields.createUserField("ST_CONFIG_VALUE", "Configration Limit", BoFieldTypes.db_Alpha, 254, "@ST_CONFIG_VALUES", company, "", BoFldSubTypes.st_Sum);

            UserFields.createUserField("ST_CONFIG_CODE", "Configuration Code", BoFieldTypes.db_Alpha, 254, "@ST_CONFIG_VALUES", company);
            UserFields.createUserField("ST_CONFIG_DISC", "Configuration Description", BoFieldTypes.db_Memo, 254, "@ST_CONFIG_VALUES", company);
            UserFields.createUserField("ST_CONFIG_VALUE", "Configuration Value", BoFieldTypes.db_Memo, 254, "@ST_CONFIG_VALUES", company);
            UserFields.createUserField("ST_CONFIG_DEFAULT", "Configuration Default Value", BoFieldTypes.db_Memo, 254, "@ST_CONFIG_VALUES", company);
            
        }
        private static void Create_One_Field(Company company, Logic.Classes.Field_Definition OneField, string Table_Name)
        {
            //string Default_Value = "";
            if (string.IsNullOrEmpty(OneField.Valid_Values_Text) == false && OneField.Valid_Values_Text.Contains(",") == true)
            {
                int Length = 0;
                List<string> Values = new List<string>();
                List<string> Descriptions = new List<string>();
                if (OneField.Valid_Values_Text.Contains("|"))
                {
                    string[] Val_Desc = OneField.Valid_Values_Text.Split("|".ToCharArray());
                    foreach (string One_Val_Desc in Val_Desc)
                    {
                        if (One_Val_Desc.Contains(",") == false)
                        {
                            throw new Exception($"The Valid Values[{OneField.Valid_Values_Text}] for the Field[{OneField.Field_Name}], Table[{Table_Name}] is not valid as Valid Values");
                        }
                        string[] One_Vale_Desc_Array = One_Val_Desc.Split(",".ToCharArray());
                        Values.Add(One_Vale_Desc_Array[0]);
                        Descriptions.Add(One_Vale_Desc_Array[1]);
                    }
                }
                else
                {
                    string[] Values_Text = OneField.Valid_Values_Text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(S => S.Trim()).ToArray();
                    if (Values_Text.Length == 2 && (Values_Text.Contains("No") && Values_Text.Contains("Yes")))
                    {
                        Values = new List<string>() { Values_Text[0].Substring(0, 1), Values_Text[1].Substring(0, 1) };
                        Descriptions = new List<string>() { Values_Text[0], Values_Text[1] };
                        Length = 1;
                    }
                    else
                    {
                        Values.AddRange(Values_Text);
                        Descriptions.AddRange(Values_Text);
                    }
                }
                Helper.MetaDataOperater.UserFields.createUserField(OneField.Field_Name, OneField.Field_Title, OneField.Data_Type, Logic.Fields_Logic.Get_Field_Size(OneField, Length), Table_Name, company, Values, Descriptions, Values[0]);
            }
            else
            {
                Helper.MetaDataOperater.UserFields.createUserField(OneField.Field_Name, OneField.Field_Title, OneField.Data_Type, Logic.Fields_Logic.Get_Field_Size(OneField, 0), Table_Name, company, OneField.Linked_Table, Logic.Fields_Logic.Get_Field_Sub_Type(OneField));
            }
        }

    }
}
