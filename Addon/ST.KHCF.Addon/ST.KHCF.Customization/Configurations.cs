using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization
{
    internal class Configurations
    {
        internal static string Get_CCI_Department(Company company, bool With_Error = true)
        {
           string Result = Utility.Get_Configuration(company, "CCI_Department", "CCI Department", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the CCI Department in the configuration table");
            }

            return Result;
        }
        internal static string Get_KHCF_MemberCard(Company company, bool With_Error = true)
        {
           string Result = Utility.Get_Configuration(company, "KHCF_MemberCard", "KHCF MemberCard", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the KHCF MemberCard in the configuration table");
            }

            return Result;
        }

        internal static string Get_Fundraising_Department(Company company, bool With_Error = true)
        {
           string Result = Utility.Get_Configuration(company, "Fundraising_Department", "Fundraising Department", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Fundraising Department in the configuration table");
            }

            return Result;
        }

        internal static string Get_Broker_Vendor_Group(Company company, bool With_Error = true)
        {
           string Result = Utility.Get_Configuration(company, "Broker_Vendor_Group", "Broker Vendor Group", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Broker Vendor Group in the configuration table");
            }

            return Result;
        }

        internal static string Get_Diagnosed_Cancer_Patient_Code(Company company, bool With_Error = true)
        {
           string Result = Utility.Get_Configuration(company, "Diagnosed_Cancer_Patient_Code", "Diagnosed Cancer Patient Code", "006");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Diagnosed Cancer Patient Code in the configuration table");
            }

            return Result;
        }


        internal static string Get_Individual_Membership_Invoice_Report(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Individual_Membership_Invoice_Report", "Individual Membership Invoice Report", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Individual Membership Invoice Report Path in the configuration table");
            }

            return Result;
        }
        internal static string Get_Patient_Legal_Report_RPT_File_Path(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Patient_Legal_Report_RPT_File_Path", "Patient Legal Report RPT File Path", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please  the Patient Legal Report RPT File Path in the configuration table");
            }

            return Result;
        }
        internal static string Get_Patient_National_Cancer_Registry_Report_RPT_File_Path(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Patient_National_Cancer_Registry_Report_RPT_File_Path", "Patient National Cancer Registry Report RPT File Path", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please  the Patient National Cancer Registry Report RPT File Path in the configuration table");
            }

            return Result;
        }

        internal static string Get_Corporate_Membership_Invoice_Report(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Corporate_Membership_Invoice_Report", "Corporate Membership Invoice Report", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Corporate Membership Invoice Report Path in the configuration table");
            }

            return Result;
        }
        internal static string Get_Fundraising_Certificate_Report(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Fundraising_Certificate_Report", "Fundraising Certificate Report", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Fundraising Certificate Report Path in the configuration table");
            }

            return Result;
        }
        internal static string Get_Fundraising_Label_Report(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Fundraising_Label_Report", "Fundraising Label Report", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Fundraising Label Report Path in the configuration table");
            }

            return Result;
        }
        internal static string Get_Report_Output_Folder_path(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Report_Output_Folder_Path", "Report Output Folder Path", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Report Output Folder Path in the configuration table");
            }

            return Result;
        }
        internal static string Get_Attachment_Folder(Company company, bool With_Error = true)
        {
           string Result = Utility.Get_Configuration(company, "Attachment_Folder", "Attachment Folder", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Attachments");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Attachment folder in the configuration table");
            }

            return Result;
        }
        internal static string Get_Unearned_Revenue(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Unearned_Revenue", "Unearned Revenue", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Unearned Revenue in the configuration table");
            }

            return Result;
        }
        internal static string Get_Down_Payment(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "GL_Down_Payment", "GL Account for Down payment", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Unearned Revenue in the configuration table");
            }

            return Result;
        }
        internal static string Get_GL_Account_For_Discount(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "GL_Account_For_Discount", "GL Account For Discount", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the GL Account For Discount in the configuration table");
            }

            return Result;
        }
        internal static string Get_Treatment_Revenue_Account(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Treatment_Revenue_Account", "Treatment Revenue Account", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Treatment Revenue Account in the configuration table");
            }

            return Result;
        }
        internal static string Get_Other_CCI_Patient_Clearing_Account(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Other_CCI_Patient_Clearing_Account", "Other CCI Patient Clearing Account, we use it for creating KFCC Invoice Journal Entry", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Other CCI Patient Clearing Account in the configuration table");
            }

            return Result;
        }

        internal static string Get_Creator_Role_Code(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Creator_Role_Code", "Creator Role Code", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Creator Role Code in the configuration table");
            }

            return Result;
        }
        //internal static string Get_Authorizer_Role_Code(Company company, bool With_Error = true)
        //{
        //    string Result = Utility.Get_Configuration(company, "Authorizer_Role_Code", "Authorizer Role Code", "");

        //    if (Result == "" && With_Error == true)
        //    {
        //        throw new  Logic.Custom_Exception("Please set the Authorizer Role Code in the configuration table");
        //    }

        //    return Result;
        //}
        internal static string Get_Allowance_Account(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Allowance_GL_Account", "Allowance GL Account", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Allowance GL Account in the configuration table");
            }

            return Result;
        }   

        internal static int Get_Default_CCI_Individual_Customer_Group(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Default_CCI_Individual_Customer_Group", "Default CCI Individual Customer Group", "");

            int Int_Result;
            if (!int.TryParse(Result, out Int_Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Default CCI Individual Customer Group in the configuration table");
            }

            return Int_Result;
        }
        internal static int Get_Default_Fundraising_Individual_Customer_Group(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Default_Fundraising_Individual_Customer_Group", "Default Fundraising Individual Customer Group", "");

            int Int_Result;
            if (!int.TryParse(Result, out Int_Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Default Fundraising Individual Customer Group in the configuration table");
            }

            return Int_Result;
        }

        internal static int Get_CCI_Patient_Vendor_Group(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "CCI_Patient_Vendor_Group", "CCI Patient Vendor Group", "121");

            int Int_Result;
            if (!int.TryParse(Result, out Int_Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the CCI Patient Vendor Group in the configuration table");
            }

            return Int_Result;
        }   
        internal static int Get_Other_CCI_Patient_Vendor_Group(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Other_CCI_Patient_Vendor_Group", "Other CCI Patient Vendor Group", "122");

            int Int_Result;
            if (!int.TryParse(Result, out Int_Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Other CCI Patient Vendor Group in the configuration table");
            }

            return Int_Result;
        }   

        internal static int Get_Other_Goodwill_Vendor_Group(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Other_Goodwill_Vendor_Group", "Other Goodwill Vendor Group", "120");

            int Int_Result;
            if (!int.TryParse(Result, out Int_Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Other Goodwill Group in the configuration table");
            }

            return Int_Result;
        }
        internal static int Get_Other_CCI_Companies_Vendor_Group(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Other_CCI_Companies_Vendor_Group", "Other CCI Companies Vendor Group", "107");

            int Int_Result;
            if (!int.TryParse(Result, out Int_Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Other CCI Companies Vendor Group in the configuration table");
            }

            return Int_Result;
        }


        internal static int GEt_Revenue_Realization_JE_Series(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Revenue_Realization_JE_Series", "Revenue Realization JE Series", "");


            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Revenue Realization JE Series in the configuration table");
            }
            if (Result == "")
            {
                return 0;
            }

            return int.Parse(Result);
        }           

        internal static int Get_Renewal_Days_for_Start_Date(Company company, bool With_Error = true)
        {
            string Config_Value = Utility.Get_Configuration(company, "Renewal_Days_for_Start_Date", "Renewal Days for Start Date", "1");

            int Result=0;
            

            if (!int.TryParse(Config_Value, out Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Renewal Days for Start Date in the configuration table");
            }
            //if (Config_Value == "")
            //{
            //    return 0;
            //}

            return Result;
        }

        internal static int Get_Past_Allowance_Months(Company company, bool With_Error = true)
        {
            string Config_Value = Utility.Get_Configuration(company, "Past_Allowance_Months", "Past due allowance months", "3");

            int Result = 0;


            if (!int.TryParse(Config_Value, out Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Past Due Allowance Months value in the configuration table");
            }
            //if (Config_Value == "")
            //{
            //    return 0;
            //}

            return Result;
        }

        internal static int Get_Renewal_Month_for_End_Date(Company company, bool With_Error = true)
        {
            string Config_Value = Utility.Get_Configuration(company, "Renewal_Month_for_End_Date", "Renewal Month for End Date", "12");

            int Result;

            if (!int.TryParse(Config_Value, out Result) && With_Error)
            {
                throw new Logic.Custom_Exception("Please set Renewal Month for End Date in the configuration table");
            }
            //if (Config_Value == "")
            //{
            //    return 0;
            //}

            return Result;
        }

        //internal static string[] Get_Corporate_Card_Fields_For_Approval(Company company)
        //{
        //    string Result = Utility.Get_Configuration(company, "Corporate_Card_Fields_For_Approval", "Corporate Card Fields list For Approval with(,) separator", "");

        //    if (Result == "")
        //    {
        //        return new string[] { };
        //    }
        //    else
        //    {
        //        return Result.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //    }
        //}

        internal static string[] Get_Individual_Card_Fields_For_Approval(Company company)
        {
            string Result = Utility.Get_Configuration(company, "Individual_Card_Fields_For_Approval", "Individual Card Fields list For Approval with(,) separator", "");

            if (Result == "")
            {
                return new string[] { };
            }
            else
            {
                return Result.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

        internal static double Get_Allowance_Rate(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Allowance_Rate", "Allowance Rate", "");
           
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Allowance Rate in the configuration table");
            }
            if (Result == "")
            {
                return 0;
            }

            return double.Parse(Result);
        }

        internal static string[] Get_Corporate_Card_Fields_For_Approval_Before_Update(Company company)
        {
            string Result = Utility.Get_Configuration(company, "Corporate_Card_Fields_For_Approval", "Corporate Card Fields list For Approval with(,) separator", "");
            if (Result == "")
            {
                return new string[] { };
            }
            else
            {
                return Result.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

        internal static string Get_KFCH_Vendor_Code(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "KFCH_Vendor_Code", "KFCH Vendor Code", "");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the KFCH Vendor Code in the configuration table");
            }

            return Result;
        }

        internal static string Get_Aramex_Payment_Method_Code(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Aramex_Payment_Method_Code", "Aramex Payment Method Code", "008");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Aramex Payment Method Code in the configuration table");
            }
            return Result;
        }
        internal static string Get_OnLine_Payment_Method_Code(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "On_line_Payment_Method_Code", "On line Payment Method Code", "004");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the On line Payment Method Code in the configuration table");
            }
            return Result;
        }
        internal static string Get_Smart_Link_Payment_Method_Code(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Smart_Link_Payment_Method_Code", "Smart Link Payment Method Code", "010");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Smart Link Payment Method Code in the configuration table");
            }
            return Result;
        }

        internal static string Get_Cash_Payment_Method_Code(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Current_Payment_Method_Code", "Current Payment Method Code", "02");

            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Cash Payment Method Code in the configuration table");
            }

            return Result;
        }

        internal static string Get_Grant_Alert_Period(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "Grant_Donation_Alert_Period", "Grant Donation Alert Period", "");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the Grant Donation Alert Period in the configuration table");
            }

            return Result;
        }

        internal static void Set_Default_Configuration(Company company)
        {
            //Utility.Get_Configuration(company, "CCI_Department", "CCI Department", "");
            //Utility.Get_Configuration(company, "Broker_Vendor_Group", "Broker Vendor Group", "");
            //Utility.Get_Configuration(company, "Attachment_Folder", "Attachment Folder", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Attachments");
            //Utility.Get_Configuration(company, "Get_Unearned_Revenue_For_DownPayment", "Get Unearned Revenue for Down Payment", "");

            Get_CCI_Department(company, false);
            Get_Fundraising_Department(company, false);
            Get_Broker_Vendor_Group(company, false);
            Get_Attachment_Folder(company, false);
            Get_Unearned_Revenue(company, false);
            Get_GL_Account_For_Discount(company, false);
            Get_Individual_Card_Fields_For_Approval(company);
            Get_Corporate_Card_Fields_For_Approval_Before_Update(company);
            Get_Allowance_Rate(company, false);
            Get_Allowance_Account(company, false);
            GEt_Revenue_Realization_JE_Series(company, false);
            //Get_Authorizer_Role_Code(company, false);
            Get_Creator_Role_Code(company, false);
            Get_File_Delimiter(company, false);
            Get_Renewal_Days_for_Start_Date(company, false);
            Get_Renewal_Month_for_End_Date(company, false);
            Get_Default_CCI_Individual_Customer_Group(company, false);
            Get_Default_Fundraising_Individual_Customer_Group(company, false);
            Get_CCI_Patient_Vendor_Group(company, false);
            Get_Other_CCI_Patient_Vendor_Group(company, false);
            Get_Other_Goodwill_Vendor_Group(company, false);
            Get_Other_CCI_Companies_Vendor_Group(company, false);
            Get_Treatment_Revenue_Account(company, false);
            Get_Other_CCI_Patient_Clearing_Account(company, false);
            Get_KFCH_Vendor_Code(company, false);
            Get_Patient_Legal_Report_RPT_File_Path(company, false);
            Get_Patient_National_Cancer_Registry_Report_RPT_File_Path(company, false);
            Get_Aramex_Payment_Method_Code(company, false);
            Get_KHCF_MemberCard(company, false);
            Get_Diagnosed_Cancer_Patient_Code(company, false);
            Get_Grant_Alert_Period(company, false);


        }

        internal static string Get_File_Delimiter(Company company, bool With_Error = true)
        {
            string Result = Utility.Get_Configuration(company, "File_Delimiter", "File Delimiter", ",");
            if (Result == "" && With_Error)
            {
                throw new Logic.Custom_Exception("Please set the File Delimiter in the configuration table");
            }

            return Result;

        }
    }
}
