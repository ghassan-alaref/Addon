using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ST.KHCF.Customization.Logic.Membership;
using static ST.KHCF.Customization.Logic.Membership;
using Company = SAPbobsCOM.Company;
using Form = SAPbouiCOM.Form;

namespace ST.KHCF.Customization.Forms.CCI
{
    public class Frm_Corporate_Membership : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static SAPbouiCOM.Application SBO_Application;
        internal static Parent_Form Form_Obj;


        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);

            Matrix Mat_Att = (Matrix)form.Items.Item("500").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            Mat_Att.Columns.Item("SELECTED").AffectsFormMode = false;
            Mat_Att.AutoResizeColumns();

            string SQL_PaymentTerms = $@"SELECT T0.""GroupNum"" AS ""Code"",T0.""PymntGroup"" AS ""Name"" FROM OCTG T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "194", SQL_PaymentTerms, true);

            string CCI_Department_ID = Configurations.Get_CCI_Department(company);
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" = {CCI_Department_ID}";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "161", SQL_Account_Manager, true);

            Grid Grd_Membership = (Grid)form.Items.Item("600").Specific;
            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE"" order By ""Code""";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_COVERAGE", SQL_Cov);
            string SQL_Memeber_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND U_ST_CUSTOMER_TYPE = 'C'";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_CUSTOMER_GROUP", SQL_Memeber_Customer_Group, true);

            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_CUSTOMER_GROUP")).DisplayType = BoComboDisplayType.cdt_Description;

            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("N","New");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("R","Renewed");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("C","Canceled");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("S","Stopped");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("P","Past to Renew");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).DisplayType = BoComboDisplayType.cdt_Description;
            ((ComboBoxColumn)Grd_Membership.Columns.Item("ParentType")).ValidValues.Add("C","Corporate");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("ParentType")).ValidValues.Add("I","Individual");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("ParentType")).DisplayType = BoComboDisplayType.cdt_Description;

            Grd_Membership.AutoResizeColumns();

            Grid Grd_Summery = (Grid)form.Items.Item("700").Specific;
            Grd_Summery.AutoResizeColumns();

            Frm_Individual_Membership.Fill_ComboButton_Values(form, "CORP");
            
            form.Items.Item("19").AffectsFormMode = false;
            form.Items.Item("198").AffectsFormMode = false;
            form.Items.Item("175").AffectsFormMode = false;
            form.Items.Item("145").AffectsFormMode = false;
            form.Items.Item("153").AffectsFormMode = false;
            form.Items.Item("500").AffectsFormMode = false;
            form.Items.Item("502").AffectsFormMode = false;
            form.Items.Item("503").AffectsFormMode = false;
            form.Items.Item("504").AffectsFormMode = false;
            form.Items.Item("175").Enabled = false;
            form.Items.Item("145").Enabled = false;
            form.Items.Item("153").Enabled = false;
            form.Items.Item("161").Visible = true;
            form.Items.Item("666").Click();
            form.Items.Item("669").AffectsFormMode = false;
        }

        internal static bool SBO_Application_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo)
        {
            bool BubbleEvent = true;
            
            try
            {
                if (Form_Obj == null || BusinessObjectInfo.FormTypeEx != Form_Obj.Form_Type)
                {
                    return BubbleEvent;
                }

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {
                    Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
                }

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    // ("U_ST_CORPORATE_NAME", 0, RC_Card.Fields.Item("U_ST_CORPORATE_ARABIC_NAME").Value.ToString());
                    ADD_Update_UDO(BusinessObjectInfo);
                    Frm_Individual_Membership.Create_Child_Memberships(BusinessObjectInfo,"C");
                }

                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }

                Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                if (!BusinessObjectInfo.BeforeAction && Form_Obj.Set_ReadOnly(form, KHCF_Object))
                {
                    // return;
                }
                

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            SBO_Application.SetStatusBarMessage("Loading", BoMessageTime.bmt_Short, false);

            Frm_Individual_Membership.Fill_ComboButton_Values(form, "CORP");

            Set_Data_Load_Items_Enabled(form);
            string MemberCard_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            string MembershipCode = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string U_ST_APPROVAL_STATUS = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);

           
            string End_Date_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0);
            DateTime EndDate = DateTime.ParseExact(End_Date_Text, "yyyyMMdd", null);
            string isActive = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACTIVE", 0);
            if (DateTime.Now < EndDate)
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACTIVE", 0, "Y");
            else
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACTIVE", 0, "N");

            string U_ST_APPROVAL_STATUS1 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
            string SQL = $@"SELECT SUM(T0.U_ST_DISCOUNT_VALUE) AS ""SUM_DISCOUNT_VALUE"", SUM(T0.U_ST_PREMIUM) AS ""SUM_PREMIUM"" 
FROM ""@ST_INDIV_MEMBERSHIP"" T0 
WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{MembershipCode}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C'  ";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            double Sum_Prem = (double)RC.Fields.Item("SUM_PREMIUM").Value;
            double Sum_Disc = (double)RC.Fields.Item("SUM_DISCOUNT_VALUE").Value;

            form.DataSources.UserDataSources.Item("27").Value = Sum_Prem.ToString();
            form.DataSources.UserDataSources.Item("172").Value = Sum_Disc.ToString();

            string subscriberCount = "0";
          //  string SubscribersSQL = $@"SELECT COUNT(T0.""Code"") AS ""Subscribers"" FROM ""@ST_CCI_INDIV_CARD"" T0 WHERE T0.""U_ST_PARENT_ID"" = '{MemberCard_Code}'";
            string SubscribersSQL = $@"SELECT  COUNT(T0.""Code"") AS ""Subscribers""  FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{MembershipCode}' And T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" ='C'";
            Recordset SubscribersRC = Helper.Utility.Execute_Recordset_Query(company, SubscribersSQL);
            subscriberCount = SubscribersRC.Fields.Item("Subscribers").Value.ToString();
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SUBSCRIBERS", 0, subscriberCount);

            string SQL_Invoice_Unpaid = $@"SELECT Sum(T0.""DocTotal"" - T0.""PaidToDate"") FROM OINV T0 
WHERE T0.""CANCELED""='N' AND (T0.""U_ST_MEMBERSHIP_CODE"" in (Select T1.""Code"" From ""@ST_INDIV_MEMBERSHIP"" T1 where T1.""U_ST_PARENT_MEMBERSHIP_ID"" = '{MembershipCode}' AND U_ST_PARENT_MEMBERSHIP_TYPE = 'C') )";
            Recordset RC_Invoice_Unpaid = Helper.Utility.Execute_Recordset_Query(company, SQL_Invoice_Unpaid);
            string SQL_DownPayment_Unpaid = $@"SELECT Sum(T0.""DocTotal"" - T0.""PaidToDate"") FROM ODPI T0 
WHERE T0.""CANCELED""='N' AND (T0.""U_ST_MEMBERSHIP_CODE"" in (Select T1.""Code"" From ""@ST_INDIV_MEMBERSHIP"" T1 where T1.""U_ST_PARENT_MEMBERSHIP_ID"" = '{MembershipCode}' AND U_ST_PARENT_MEMBERSHIP_TYPE = 'C') )";
            Recordset RC_DownPayment_Unpaid = Helper.Utility.Execute_Recordset_Query(company, SQL_DownPayment_Unpaid);
            double Total_Due = 0;

                Total_Due = (double)RC_Invoice_Unpaid.Fields.Item(0).Value + (double)RC_DownPayment_Unpaid.Fields.Item(0).Value;
            
            form.DataSources.UserDataSources.Item("251").Value = Total_Due.ToString();
            if (form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INSTALLMENT_TYPE", 0) == "-1")
            {
                form.DataSources.UserDataSources.Item("198").Value = form.DataSources.UserDataSources.Item("251").Value;
            }
            else
            {
                form.DataSources.UserDataSources.Item("198").Value = "0";
            }


            Load_Memberships(form, MembershipCode);
            Load_Summery(form, MemberCard_Code);

            form.DataSources.UserDataSources.Item("OLD_COVER").Value = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_COVERAGE", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "181", SQL_Invoice, false);

            //string SQL_Payment = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM ORCT Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PAYMENT_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "183", SQL_Payment, false);

            //string SQL_JE = $@"Select ""TransId""  AS ""Code"", ""Number"" AS ""Name"" FROM OJDT Where ""TransId"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_JE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "185", SQL_JE, false);

        }

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            string UDO_Member_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);

            if (businessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
            {
                string Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBERSHIP_STATUS", 0);
                if ((Status == "R" || Status == "P"))
                {
                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);
                    string MemberCard_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBER_CARD", 0);
                    Membership.Renewal_Children(company, MemberCard_Code, UDO_Code, UDO_Info);
                }

            }
            if (businessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE)
            {
                string Old_Coverage = form.DataSources.UserDataSources.Item("OLD_COVER").Value;
                string New_Coverate = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_COVERAGE", 0);
                if (Old_Coverage != New_Coverate)
                {
                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);
                    UDO_Definition UDO_Indiv_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);

                    string SQL = $@"SELECT T0.""Code"" FROM ""@ST_INDIV_MEMBERSHIP""  T0 
WHERE T0.U_ST_PARENT_MEMBERSHIP_TYPE = 'C' AND  T0.""U_ST_PARENT_MEMBERSHIP_ID"" ='{UDO_Code}'
AND IFNULL(U_ST_INVOICE_NUMBER,0) = 0 ";
                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                    List<string> children = new List<string>();
                    for (int i = 0; i < RC.RecordCount; i++)
                    {
                        children.Add(RC.Fields.Item("Code").Value.ToString());

                        RC.MoveNext();
                    }
                    //string[] Children_Card_Codes = KHCF_Logic_Utility.Get_Children_Membership_Codes(company, UDO_Code, UDO_Info);
                    Field_Data Fld_New_Coverage = new Field_Data() { Field_Name = "U_ST_COVERAGE", Data_Type = BoFieldTypes.db_Alpha, Value = New_Coverate };
                    Field_Data[] Updated_Fields = new Field_Data[] { Fld_New_Coverage };
                    foreach (string One_Child_Code in children)
                    {
                        try
                        {
                            Utility.Update_UDO(company, UDO_Indiv_Info, One_Child_Code, Updated_Fields);
                            SBO_Application.StatusBar.SetText($"The Membership[{One_Child_Code}]Coverage has been updated successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        }
                        catch (Exception ex)
                        {
                            SBO_Application.StatusBar.SetText($"Error during Update the Membership[{One_Child_Code}] Coverage[{ex.Message}]", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                        }
                    }
                    form.DataSources.UserDataSources.Item("OLD_COVER").Value = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_COVERAGE", 0);
                }
            }

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) == "P")
            {
                Form_Obj.Send_Alert_For_Approve(UDO_Code);
            }
        }

        internal static void SBO_Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            
            BubbleEvent = true;
           

            if (pVal.BeforeAction)
                return;

            
            if (Form_Obj == null || SBO_Application.Forms.ActiveForm.TypeEx != Form_Obj.Form_Type)
            {
                return;
            }
            Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");

            if (pVal.MenuUID == "1282" || pVal.MenuUID == "1281")//Add || Find
            {
                Form form = SBO_Application.Forms.ActiveForm;
                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info);
                form.DataSources.UserDataSources.Item("27").Value = "0";
                form.DataSources.UserDataSources.Item("172").Value = "0";
                //DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                //DT_Members.Rows.Clear();
                //Helper.Utility.Clear_ComboBox(form, new string[] { "181"});
                Set_Data_Load_Items_Enabled(form);
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
               
            }
            else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE && Form_Obj !=null)
            {
                if(SBO_Application.Forms.ActiveForm.TypeEx == Form_Obj.Form_Type)
                 Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            KHCF_BP BP = new KHCF_BP();
            
            //BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
            //BP.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);
            //string BP_Code = Utility.Create_BP(company, form, BP);

            Set_Default_Value_Befoe_Adding(form);


        }

        private static void Set_Default_Value_Befoe_Adding(Form form)
        {
            //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATION_DATE", 0, DateTime.Today.ToString("yyyyMMdd"));
                // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "I");
            }

        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            if (ItemUID == "15")// Start Date
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string StartDate_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0);
                if (StartDate_Text == "")
                {
                    return;
                }
                DateTime StartDate = DateTime.ParseExact(StartDate_Text, "yyyyMMdd", null);
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_END_DATE", 0, StartDate.AddYears(1).AddDays(-1).ToString("yyyyMMdd"));
            }
        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string s = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COVERAGE", 0);
            string member_card = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            string SQL_Card = $@"SELECT U_ST_CHANNEL 
FROM ""@ST_CCI_CORP_CARD""  T0 
WHERE T0.""Code"" = '{member_card}'";
            Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);

            //if (RC_Card.Fields.Item("U_ST_CHANNEL").Value.ToString() != s)
            //{
            //    Loader.New_Msg = "The Coverage Should be like the Corporate";
            //    return false;
            //}

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }

            return true;
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            

            if (Form_Obj == null || pVal.FormTypeEx != Form_Obj.Form_Type)
            {
                return;
            }
            try
            {
                
                //if (pVal.ItemUID == "192" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Calculate_Premium(pVal);
                //}
                //if (pVal.ItemUID == "191" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Create_Invoice(pVal);
                //}
                if (pVal.ItemUID == "189" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Payment(pVal);
                }
                if (pVal.ItemUID == "200" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Invoice(pVal,true);
                }
                if (pVal.ItemUID == "150" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Print_Invoice(pVal, company);
                }
                if (pVal.ItemUID == "24" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Approve(pVal);
                }
                if (pVal.ItemUID == "25" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Reject(pVal);
                }
              
                if (pVal.ItemUID == "300" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Member_Card_Choose_From_List(pVal);
                }
                if (pVal.ItemUID == "19" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Actions(pVal);
                    
                }

                if (pVal.ItemUID == "502" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                    SBO_Application.Menus.Item("1304").Activate();
                }
                if (pVal.ItemUID == "503" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                    SBO_Application.Menus.Item("1304").Activate();
                }
                if (pVal.ItemUID == "504" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "Item_2" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Invoice(pVal, false);
                }

                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        public static string Print_Invoice(ItemEvent pval, Company oCompany)
        {
            //List<Helper.Utility.Crystal_Report_Parameter> Result = new List<Helper.Utility.Crystal_Report_Parameter>();
            //string Rpt_File = Utility.Get_Configuration(oCompany, "Corp_Member_Print_Path", "Corporate Membership Print Path", "");
            //string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(oCompany, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Utility.Get_Configuration(oCompany, "Report_Output_Folder_Path", "Report Output Folder Path", ""));

            List<Helper.Utility.Crystal_Report_Parameter> Result = new List<Helper.Utility.Crystal_Report_Parameter>();
            Result.Add(new Helper.Utility.Crystal_Report_Parameter() { Name = "DocKey@", Type = Helper.Utility.Crystal_Report_Parameter.DataType.Integer, Value = 2 });
            Result.Add(new Helper.Utility.Crystal_Report_Parameter() { Name = "ObjectId@", Type = Helper.Utility.Crystal_Report_Parameter.DataType.Integer, Value = 13 });
            string Rpt_File = @"C:\Users\ahlwani\Desktop\Temp\Test_Sales_Order_Layout.rpt";
            //string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(company, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "Server=NDB@ns3204322:30013;Database=KHCF_UAT;User Id=SYSTEM;Password=Skytech@1234", @"C:\Users\ahlwani\Desktop\Temp");
            string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(oCompany, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "Server=ns3204322:30015;Database=KHCF_UAT;User Id=SYSTEM;Password=Skytech@1234", @"C:\Users\ahlwani\Desktop\Temp");

             SBO_Application.StatusBar.SetText("Report has been Created Successfully at " + Pdf_File_Name, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Success);
            return Pdf_File_Name;
        }

        private static void Reject(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == Form_Obj.KHCF_Object);

            Logic.Membership.Reject(company, UDO_Code, UDO_Info);

            string SQL_Membership =
$@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_MEMBERSHIP_STATUS"" 
FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" = 'P'
And T0.""U_ST_APPROVAL_STATUS"" ='P' AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{UDO_Code}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' ";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);

            for (int i = 0; i < RC.RecordCount; i++)
            {
                string Code = RC.Fields.Item("Code").Value.ToString();
                Logic.Membership.Reject(company, Code, UDO_Info);
                RC.MoveNext();
            }
            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            SBO_Application.Menus.Item("1304").Activate();

        }

        private static void Approve(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == Form_Obj.KHCF_Object);

            Logic.Membership.Approve(company, UDO_Code, UDO_Info);

            string SQL_Membership =
$@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_MEMBERSHIP_STATUS"" 
FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" = 'P'
And T0.""U_ST_APPROVAL_STATUS"" ='P' AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{UDO_Code}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' ";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);

            for (int i = 0; i < RC.RecordCount; i++)
            {
                string Code = RC.Fields.Item("Code").Value.ToString();
                Logic.Membership.Approve(company, Code, UDO_Info);
                RC.MoveNext();
            }

            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            SBO_Application.Menus.Item("1304").Activate();

        }

        private static void Run_Actions(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE && form.Mode != BoFormMode.fm_UPDATE_MODE)
            {
                throw new Logic.Custom_Exception("This action can run in OK Mode onlny.");
            }
            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("19").Specific;
            string Title = "";
            string Action_ID = form.DataSources.UserDataSources.Item("19").Value;
            if (Action_ID == "-")
                throw new Logic.Custom_Exception("Please select an action");
            else
                Title = Utility.Get_Field_Configuration(company, form.TypeEx.Replace("ST", "Frm") + "_" + Action_ID, "", "");

            if (Title == "" || string.IsNullOrEmpty(Title))
                throw new Logic.Custom_Exception($"This Action [{Action_ID}] is not supported");

            if (Title.ToLower() == "stop".ToLower())
            {
                string Stop_Date_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_DATE", 0);
                string Stop_Reason = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_NOTE", 0);
                if (string.IsNullOrEmpty(Stop_Date_Text) || string.IsNullOrEmpty(Stop_Reason))
                {
                    form.Items.Item("204").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    form.Items.Item("206").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    if (Stop_Date_Text == "")
                    {
                        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_STOP_DATE", 0, (DateTime.Today).ToString("yyyyMMdd"));
                    }
                    SBO_Application.MessageBox("Please confirm the Stop Date and the Stop Reason and try again");
                    SBO_Application.StatusBar.SetText("Please confirm the Stop Date and the Stop Reason and try again", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);
                    return;
                }
            }

            if (SBO_Application.MessageBox($"Are you sure want to {Title} the Membership?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);

            switch (Action_ID)
            {
                case "C"://Cancel
                    Membership.Cancel_Individual_Membership(company, UDO_Code, UDO_Info);
                    if (Membership.Check_Membership_Children(company,UDO_Code,"C"))
                    {
                        if (SBO_Application.MessageBox($@"Are you sure you want to cancel the related memberships?", 1, "Yes", "No") == 1)
                        {
                            Form Cancel_Form1 = Frm_Stop_Cancel_Children.Create_Form();
                            Cancel_Form1.DataSources.UserDataSources.Item("Type").Value = "C";
                            Cancel_Form1.DataSources.UserDataSources.Item("UD_2").Value = UDO_Code;
                            Frm_Stop_Cancel_Children.FillData(Cancel_Form1);
                            Cancel_Form1.Visible = true;
                        }
                    }
                    break;
                case "L"://Close
                    Membership.Close_Individual_Membership(company, UDO_Code, UDO_Info);
                    if (Membership.Check_Membership_Children(company,UDO_Code,"C"))
                    {
                        if (SBO_Application.MessageBox($@"Do you want to close related Membership?", 1, "Yes", "No") == 1)
                        {
                            Form Cancel_Form1 = Frm_Stop_Cancel_Children.Create_Form();
                            Cancel_Form1.DataSources.UserDataSources.Item("Type").Value = "L";
                            Cancel_Form1.DataSources.UserDataSources.Item("UD_2").Value = UDO_Code;
                            Frm_Stop_Cancel_Children.FillData(Cancel_Form1);
                            Cancel_Form1.Visible = true;
                        }
                    }
                    break;
                case "RE"://Renewal
                    Renewal_Form(form,true);
                    break;
                case "P"://Past to Renew
                    Convert_Past_to_Renew(form);
                    break;
                case "S"://Stop
                    DateTime StopDate = DateTime.ParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_DATE", 0), "yyyyMMdd", null);
                    string Stop_Note = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_NOTE", 0);
                    //Membership.Stop_Individual_Membership(company, UDO_Code, UDO_Info, StopDate, Stop_Note);
                    if (Membership.Check_Membership_Children(company, UDO_Code, "C"))
                    {
                        if (SBO_Application.MessageBox($@"Are you sure you want to stop the related memberships?", 1, "Yes", "No") == 1)
                        {
                            Form Stop_Form = Frm_Stop_Cancel_Children.Create_Form();
                            Stop_Form.DataSources.UserDataSources.Item("Type").Value = "S";
                            Stop_Form.DataSources.UserDataSources.Item("UD_2").Value = UDO_Code;
                            Stop_Form.DataSources.UserDataSources.Item("Stop").Value = StopDate.ToString("yyyyMMdd");
                            Stop_Form.DataSources.UserDataSources.Item("Note").Value = Stop_Note;
                            Frm_Stop_Cancel_Children.FillData(Stop_Form);
                            Stop_Form.Visible = true;
                        }
                        Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = "S" };
                        Field_Data Fld_Active = new Field_Data() { Field_Name = "U_ST_ACTIVE", Value = "N" };
                        Field_Data Fld_StopDate = new Field_Data() { Field_Name = "U_ST_STOP_DATE", Value = StopDate };
                        Field_Data Fld_Stop_Note = new Field_Data() { Field_Name = "U_ST_STOP_NOTE", Value = Stop_Note };

                        Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld_Status, Fld_Active, Fld_StopDate, Fld_Stop_Note });
                    }
                    break;
                case "R"://Remove
                    Membership.Remove(company, UDO_Code, UDO_Info);
                    if (Membership.Check_Membership_Children(company, UDO_Code, "C"))
                    {
                        if (SBO_Application.MessageBox($@"Are you sure you want to remove the related memberships?", 1, "Yes", "No") == 1)
                        {
                            Form Cancel_Form1 = Frm_Stop_Cancel_Children.Create_Form();
                            Cancel_Form1.DataSources.UserDataSources.Item("Type").Value = "R";
                            Cancel_Form1.DataSources.UserDataSources.Item("UD_2").Value = UDO_Code;
                            Frm_Stop_Cancel_Children.FillData(Cancel_Form1);
                            Cancel_Form1.Visible = true;
                        }
                    }

                    form.Mode = BoFormMode.fm_FIND_MODE;
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                    break;

                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            }
        }

        private static void Convert_Past_to_Renew(Form form)
        {
            string Status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS", 0);
            if (Status != "P")
            {
                throw new Exception("The Membership status is not Past Membership");
            }

            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == Form_Obj.KHCF_Object);
            Membership.Convert_Past_to_Renew(company, Membership_Code, Form_Obj);

            if (Membership.Check_Membership_Children(company, Membership_Code, "C"))
            {
                if (SBO_Application.MessageBox($@"Are you sure you want to renew the related past memberships?", 1, "Yes", "No") == 1)
                {
                    string SQL_Membership =
$@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_MEMBERSHIP_STATUS"" 
FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" = 'P'
And T0.""U_ST_APPROVAL_STATUS"" ='A' AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Membership_Code}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' ";

                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);

                    for (int i = 0; i < RC.RecordCount; i++)
                    {
                        string Code = RC.Fields.Item("Code").Value.ToString();
                        Membership.Convert_Past_to_Renew(company, Code, Form_Obj, UDO_Info);
                        RC.MoveNext();
                    }

                }
            }

        }

        private static void Set_Data_Load_Items_Enabled(Form form)
        {
            string[] Premium_Items = new string[] { "15", "143", "194" };
            bool Must_Enabled;
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                if (form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INVOICE_NUMBER", 0) == "")
                {
                    Must_Enabled = true;
                }
                else
                {
                    Must_Enabled = false;
                }
            }
            else
            {
                Must_Enabled = true;
            }
            foreach (string OneItem in Premium_Items)
            {
                if (Must_Enabled)
                {
                    form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                }
                else
                {
                    form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                }
            }

        }

        private static void Renewal_Form(Form form, bool isNewForm)
        {
            string Original_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string MemberCard_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            
            DateTime Old_End_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0), "yyyyMMdd", null);
            bool Is_Past;
            DateTime New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Old_End_Date, out Is_Past);
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS", 0) == "S")
            {
                DateTime Stop_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_STOP_DATE", 0), "yyyyMMdd", null);
                New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Stop_Date, out Is_Past);
            }

            UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == Form_Obj.KHCF_Object);
            Form Renewal_Form = null;
            if (isNewForm)
            {
                Renewal_Form = Loader.Open_UDO_Form(KHCF_Object.KHCF_Object);
                Renewal_Form.Mode = BoFormMode.fm_ADD_MODE;
            }
            else
            {
                Renewal_Form = form;
            }
            Field_Definition[] Fields = Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == Form_Obj.KHCF_Object).ToArray();
            try
            {
                Renewal_Form.Freeze(true);
                foreach (Field_Definition One_Field in Fields)
                {
                    Renewal_Form.DataSources.DBDataSources.Item(0).SetValue(One_Field.Column_Name_In_DB, 0, form.DataSources.DBDataSources.Item(0).GetValue(One_Field.Column_Name_In_DB, 0));
                }
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, "");
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_START_DATE", 0, New_Renewal_StartDate.ToString("yyyyMMdd"));
                Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_END_DATE", 0, New_Renewal_StartDate.AddMonths(Configurations.Get_Renewal_Month_for_End_Date(company, true)).AddDays(-1).ToString("yyyyMMdd"));
                if (Is_Past)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_STATUS", 0, "P");
                }
                else
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_STATUS", 0, "R");
                }
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PREVIOUS_MEMBERSHIP_CODE", 0, Original_Code);
                Form_Obj.Set_Fields(Renewal_Form);
                if (isNewForm)
                    form.Close();
            }
            finally
            {
                Renewal_Form.Freeze(false);
            }

        }

        private static void Create_Payment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Member_Card = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            double Payment_Amount = double.Parse(form.DataSources.UserDataSources.Item("198").Value);
            if (Payment_Amount == 0)
            {
                throw new Logic.Custom_Exception("Please set the Payment Amount");
            }
            //string Card_Number = DT_Result.GetValue("CARD_ID", i).ToString();
            //string Client_Number = DT_Result.GetValue("CLIENT_NO", i).ToString();
            //string Card_Status = DT_Result.GetValue("CARD_STATUS", i).ToString();
            //if (Card_Status.ToLower() == "closed" || Card_Status.ToLower() == "closed to lawyer")
            //{
            //    SBO_Application.StatusBar.SetText($"The Card Status is [{Card_Status}], You can't Add a Payment for this Card");
            //    continue;
            //}

            //string CardCode = Utility.Get_CardCode_By_CardNumber(company, Card_Number);

            SBO_Application.ActivateMenuItem("2817");
            Form Frm_Payment = SBO_Application.Forms.ActiveForm;

            Form UDF_Form = SBO_Application.Forms.Item(Frm_Payment.UDFFormUID);
            Matrix Mat_Payment_Lines = (Matrix)Frm_Payment.Items.Item("71").Specific;
            System.Threading.Thread.Sleep(1000);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
            string BP_Code = Utility.Get_BP_Code(company, Member_Card, UDO_Info);

            ((EditText)Frm_Payment.Items.Item("5").Specific).Value = BP_Code;
            //System.Threading.Thread.Sleep(2000);
            // UDF_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_CODE", 0, Membership_Code);
            Utility.Set_UDF_Value_on_Form(Membership_Code, UDF_Form, "U_ST_MEMBERSHIP_CODE", true);
            Utility.Set_UDF_Value_on_Form("C", UDF_Form, "U_ST_MEMBERSHIP_TYPE", false);
            // ((ComboBox)UDF_Form.Items.Item("U_ST_MEMBERSHIP_TYPE").Specific).Select("C");
            Frm_Payment.Items.Item("37").Click();
            // Frm_Payment.Items.Item("37").Click();
            ((EditText)Frm_Payment.Items.Item("13").Specific).Value = Payment_Amount.ToString();
            System.Threading.Thread.Sleep(1000);
            Frm_Payment.Items.Item("14").Click();
            Frm_Payment.Items.Item("5").Enabled = false;
            Frm_Payment.Items.Item("10").Enabled = false;
            Frm_Payment.Items.Item("37").Enabled = false;
            UDF_Form.Items.Item("U_ST_MEMBERSHIP_CODE").Enabled = false;
            //return;
            //Frm_Payment.Items.Item("5").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            //Frm_Payment.Items.Item("10").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            //Frm_Payment.Items.Item("37").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            //UDF_Form.Items.Item("U_ST_MEMBERSHIP_CODE").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            ////System_Forms.Display_Payment_Form(SBO_Application, company, CardCode, Card_Number, Frm_Payment, Payment_Type, Client_Number);

        }

        private static void Member_Card_Choose_From_List(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            if (Code == "")
            {
                return;
            }

            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Select_MemberCard(Code, form);
        }

        private static void Select_MemberCard(string MemberCard_Code, Form form)
        {
            string SQL_Card = $@"SELECT U_ST_CORPORATE_ARABIC_NAME , U_ST_CHANNEL ,U_ST_ACCOUNT_MANAGER,U_ST_SUB_CHANNEL
FROM ""@ST_CCI_CORP_CARD""  T0 
WHERE T0.""Code"" = '{MemberCard_Code}'";
            Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CORPORATE_NAME", 0, RC_Card.Fields.Item("U_ST_CORPORATE_ARABIC_NAME").Value.ToString());
            try
            {
                form.Freeze(true);

                string StartDate_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0);
                if (StartDate_Text != "")
                {
                    DateTime StartDate = DateTime.ParseExact(StartDate_Text, "yyyyMMdd", null);
                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_END_DATE", 0, StartDate.AddYears(1).ToString("yyyyMMdd"));

                }

                string CCI_Department_ID = Configurations.Get_CCI_Department(company);
                string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" = {CCI_Department_ID} And T0.""SlpCode""='{RC_Card.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString()}'";
                Recordset RC_Manager = Helper.Utility.Execute_Recordset_Query(company, SQL_Account_Manager);

                string SQL_Status = $@"SELECT TOP 1 ""U_ST_MEMBERSHIP_STATUS"",""U_ST_START_DATE"",""U_ST_END_DATE"", ""U_ST_STOP_DATE"", 
""U_ST_COVERAGE"", ""U_ST_INSTALLMENT_TYPE"",""U_ST_APPROVAL_STATUS"" FROM ""@ST_CORP_MEMBERSHIP"" 
WHERE ""U_ST_MEMBER_CARD""='{MemberCard_Code}' AND ""U_ST_MEMBERSHIP_STATUS"" IN ('N','R','P','S') ORDER BY ""U_ST_START_DATE"" DESC ";
                Recordset RC_Status = Helper.Utility.Execute_Recordset_Query(company, SQL_Status);

                if (RC_Status.RecordCount == 0)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_STATUS", 0, "N");
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CORPORATE_NAME", 0, RC_Card.Fields.Item("U_ST_CORPORATE_ARABIC_NAME").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, RC_Card.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString());

                }

                else if (RC_Status.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value.ToString() != "C")
                {
                    DateTime StartDate = Convert.ToDateTime(RC_Status.Fields.Item("U_ST_START_DATE").Value.ToString());
                    DateTime EndDate = Convert.ToDateTime(RC_Status.Fields.Item("U_ST_END_DATE").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_START_DATE", 0, StartDate.ToString("yyyyMMdd"));
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_END_DATE", 0, EndDate.ToString("yyyyMMdd"));
                    if (RC_Status.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value.ToString() == "S")
                    {
                        DateTime StopDate = Convert.ToDateTime(RC_Status.Fields.Item("U_ST_STOP_DATE").Value.ToString());
                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_END_DATE", 0, StopDate.ToString("yyyyMMdd"));
                    }
                    Renewal_Form(form, false);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COVERAGE", 0, RC_Status.Fields.Item("U_ST_COVERAGE").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_CARD", 0, MemberCard_Code);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, RC_Card.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_INSTALLMENT_TYPE", 0, RC_Status.Fields.Item("U_ST_INSTALLMENT_TYPE").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "A");
                }
                if (RC_Manager.RecordCount > 0)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, RC_Manager.Fields.Item("Name").Value.ToString());
                }
                Form_Obj.Set_Fields(form);
            }
            finally
            {
                form.Freeze(false);
            }
        }

        internal static void Create_Membership_for_MemberCard(string MemberCard_Code)
        {
            Form Renewal_Form = Loader.Open_UDO_Form(KHCF_Objects.Corporate_Membership);
            Renewal_Form.Mode = BoFormMode.fm_ADD_MODE;
            Form_Obj.Set_Fields(Renewal_Form);
            Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_CARD", 0, MemberCard_Code);
            Renewal_Form.Items.Item("161").Visible = true;
            Select_MemberCard(MemberCard_Code, Renewal_Form);
        }

        private static void Create_Invoice(ItemEvent pVal,bool Create_Invoice)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("This action can run in OK mode only.");
            }
            string CurrentMembershipCode = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string relatedIndividualMembershipsSQL = $@"SELECT T0.""Code"" FROM  ""@ST_INDIV_MEMBERSHIP"" T0 
 WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{CurrentMembershipCode}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' 
 AND ( T0.""U_ST_INVOICE_NUMBER"" IS NULL OR T0.""U_ST_INVOICE_NUMBER"" = '' ) ";
            Recordset membershipsRC = Helper.Utility.Execute_Recordset_Query(company, relatedIndividualMembershipsSQL);
            if (membershipsRC.RecordCount > 0)
            {
                for (int i = 0; i < membershipsRC.RecordCount; i++)
                {
                    Inoice_Data Inv_Data = new Inoice_Data();

                    Premium_Data Premium = Membership.Calculate_Premium(company, membershipsRC.Fields.Item("Code").Value.ToString());
                    Inv_Data.Source_Code = membershipsRC.Fields.Item("Code").Value.ToString();
                    Inv_Data.Premium_Amount = Premium.Premium_Amount;
                    Inv_Data.Discount_Value = Premium.Discount_Value;
                    Inv_Data.Discount_Percentage = Premium.Discount_Percentage;
                    Inv_Data.Waiting_Period = Premium.Waiting_Period;
                    Inv_Data.Is_One_Installment = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INSTALLMENT_TYPE", 0) == "O";
                    Inv_Data.Payment_Terms = Convert.ToInt32(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INSTALLMENT_TYPE", 0));

                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                    List<Field_Data> Premium_Field_Data = new List<Field_Data>();
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_PREMIUM", Value = Premium.Premium_Amount });
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_PERCENTAGE", Value = Premium.Discount_Percentage });
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_VALUE", Value = Premium.Discount_Value });
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Premium.Waiting_Period });
                    Utility.Update_UDO(company, UDO_Info, membershipsRC.Fields.Item("Code").Value.ToString(), Premium_Field_Data.ToArray());
                    string type = string.Empty;
                    if (Create_Invoice)
                    {
                        int NewEntry = Membership.Create_Invoice(company, Inv_Data, UDO_Info, out type);
                        SBO_Application.StatusBar.SetText($"New {type}[{NewEntry}] has been created", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    }
                    else
                    {
                        SBO_Application.StatusBar.SetText($"Premium has been calculated for {membershipsRC.Fields.Item("Code").Value.ToString()} related membership", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    }
                    membershipsRC.MoveNext();
                }
            }
            else
            {
                throw new Logic.Custom_Exception("No available memberships to generate invoices to.");
            }
        }

        private static void Add_Attachment(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            // BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            if (BF.FileName.Split('.').Length >= 2)
            {
                int index = BF.FileName.Split('.').Length;
                if (BF.FileName.Split('.')[index - 1] == "exe")
                    return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_CCI_COR_SHP_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("500").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Attachment.Size;
            if (Count == 1)
            {
                if (DS_Attachment.GetValue("U_ST_FILE_NAME", Count - 1) != "")
                {
                    DS_Attachment.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Attachment.InsertRecord(Count);
            }

            DS_Attachment.SetValue("U_ST_FILE_NAME", Count, BF.FileName);
            if (form.Mode == BoFormMode.fm_OK_MODE && Count != 0)
            {
                int Line_id = Convert.ToInt32(DS_Attachment.GetValue("LineId", Count - 1).ToString()) + 1;
                DS_Attachment.SetValue("LineId", Count, Line_id.ToString());
            }
            else
                DS_Attachment.SetValue("LineId", Count, (Count + 1).ToString());

            //DS_Address.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                Update_Attachment_Matrix(form, UDO_Code, "Update");
                Matrix Mat = (Matrix)form.Items.Item("500").Specific;

                Mat.FlushToDataSource();
                Mat.LoadFromDataSource();
                Mat_Add.AutoResizeColumns();
            }




        }

        private static void Remove_Attachment(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
            Form_Obj.Remove_Matrix_Row(form, "500");
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                Update_Attachment_Matrix(form, UDO_Code, "Remove");

                Matrix Mat = (Matrix)form.Items.Item("500").Specific;
                Mat.FlushToDataSource();
                Mat.LoadFromDataSource();
                Mat.AutoResizeColumns();
            }



        }

        private static void Open_Attachment(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);


            Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            for (int i = 0; i < Mat.RowCount; i++)
            {
                SAPbouiCOM.CheckBox Chk_Selected = (SAPbouiCOM.CheckBox)Mat.GetCellSpecific("SELECTED", i + 1);
                if (Chk_Selected.Checked)
                {
                    EditText Txt_FileName = (EditText)Mat.GetCellSpecific("FileName", i + 1);
                    System.Diagnostics.Process.Start(Txt_FileName.Value);
                }
            }

        }

        internal static void Update_Attachment_Matrix(SAPbouiCOM.Form form, string UDO_Code, string Case)
        {
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_CCI_COR_SHP_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("500").Specific;
            int Count = DS_Attachment.Size;
            UserTable UDT_Rel = company.UserTables.Item("ST_CCI_COR_SHP_ATT");
            string SQL_Att = $@"SELECT *  FROM ""@ST_CCI_IND_SHP_ATT""  T0 INNER JOIN  ""@ST_CORP_MEMBERSHIP""  T1 on T0.""Code""= '{UDO_Code}'";
            UDO_Definition UDO_Info_Att = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Membership_Attachment);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);

            Field_Definition[] Address_Fields = Logic.Fields_Logic.All_Field_Definition.Where(U => U.KHCF_Object == KHCF_Objects.CCI_Individual_Membership_Attachment).ToArray();

            CompanyService oCmpSrv = company.GetCompanyService();
            GeneralService oGeneralService = oCmpSrv.GetGeneralService(Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
            GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
            oGeneralParams.SetProperty("Code", UDO_Code);
            GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
            SAPbobsCOM.GeneralDataCollection Attatchment_Children = oGeneralData.Child("ST_CCI_COR_SHP_ATT");

            for (int J = Count - 1; J < form.DataSources.DBDataSources.Item("@ST_CCI_COR_SHP_ATT").Size; J++)
            {
                if (Case == "Remove")
                    Attatchment_Children.Remove(J);
                else
                {
                    SAPbobsCOM.GeneralData oChild = Attatchment_Children.Add();
                    foreach (var One_Add_Field in Address_Fields)
                    {
                        if (One_Add_Field.Is_Temp == true)
                        {
                            continue;
                        }

                        oChild.SetProperty(One_Add_Field.Column_Name_In_DB, form.DataSources.DBDataSources.Item("@ST_CCI_COR_SHP_ATT").GetValue(One_Add_Field.Column_Name_In_DB, J));
                    }
                }
            }
            oGeneralService.Update(oGeneralData);
        }

        private static void Load_Memberships(SAPbouiCOM.Form form, string Card_ID)
        {
            DataTable DT_Membership = form.DataSources.DataTables.Item("MEMBERSHIP");
            DT_Membership.Rows.Clear();
            string SQL_Membership = $@"SELECT T0.""Code"", T0.""U_ST_MEMBER_CARD"",T1.""U_ST_FULL_NAME_AR"", T0.""U_ST_MEMBERSHIP_STATUS"" , T0.""U_ST_CREATION_DATE""
, T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE,T0.U_ST_AGE,T0.U_ST_PREMIUM,T1.U_ST_CUSTOMER_GROUP ,
(Select T2.""U_ST_CORPORATE_NAME"" from ""@ST_CORP_MEMBERSHIP"" T2 WHERE T2.""Code"" = T0.""U_ST_PARENT_MEMBERSHIP_ID"") As CorporateName , T0.""U_ST_PARENT_MEMBERSHIP_ID"" , 'C' As ""ParentType""
FROM ""@ST_INDIV_MEMBERSHIP""  T0
JOIN ""@ST_CCI_INDIV_CARD"" T1 ON T1.""Code"" = T0.""U_ST_MEMBER_CARD""
WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" ='{Card_ID}' And T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' ";
            Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            DT_Membership.Rows.Add(RC_Membership.RecordCount);

            for (int i = 0; i < RC_Membership.RecordCount; i++)
            {
                for (int J = 0; J < DT_Membership.Columns.Count; J++)
                {
                    string Col_Name = DT_Membership.Columns.Item(J).Name;
                    string UDF_Name;

                    UDF_Name = Col_Name;
                    if (Col_Name == "U_ST_PREMIUM")
                    {
                        double Premium = Convert.ToDouble(RC_Membership.Fields.Item(UDF_Name).Value.ToString());
                        DT_Membership.SetValue(Col_Name, i, Premium.ToString("N03"));
                    }
                    else
                        DT_Membership.SetValue(Col_Name, i, RC_Membership.Fields.Item(UDF_Name).Value);
                }
                RC_Membership.MoveNext();
            }
            Grid Grd_Membership = (Grid)form.Items.Item("600").Specific;

            Grd_Membership.AutoResizeColumns();

        }

        private static void Load_Summery(Form form, string Card_ID)
        {
            DataTable DT_Summery = form.DataSources.DataTables.Item("Summery");
            DT_Summery.Rows.Clear();
            //string SQL_Membership = $@"SELECT T0.""Code"" FROM ""@ST_CORP_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBER_CARD"" ='{Card_ID}'";
            //Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            //string Membership_Code = RC_Membership.Fields.Item("Code").Value.ToString();
            //string SQL_Call_Procedure = $@"call ST_MEMBERSHIP_SUMMARY('{Membership_Code}')";
            //            string SQL_Membership = $@"SELECT T0.""Code"", T0.""U_ST_MEMBER_CARD"", T0.""U_ST_CREATION_DATE""
            //, T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE,T1.U_ST_CUSTOMER_GROUP
            //FROM ""@ST_CORP_MEMBERSHIP""  T0
            //JOIN ""@ST_CCI_CORP_CARD"" T1 ON T1.""Code"" = T0.""U_ST_MEMBER_CARD""
            //WHERE T0.""U_ST_MEMBER_CARD"" ='{Card_ID}'";

            string SQL_Membership = $@"SELECT T0.""Code"",  (Select Count(*) From   ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Members"",  (Select Sum(U_ST_PREMIUM) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Total_Premiums"",  (Select Count(*) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}'
And T2.""U_ST_ACTIVE""='Y' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Active_Members"", (Select Sum(U_ST_PREMIUM) From   ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' And T2.""U_ST_ACTIVE""='Y') As ""Total_Premiums_For_Active_Members"",  (Select Count(*) From  ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
And T2.""U_ST_MEMBERSHIP_STATUS""='S' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Stopped_Members"", (Select Sum(U_ST_PREMIUM) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' And T2.""U_ST_MEMBERSHIP_STATUS""='S') As ""Total_Premiums_for_Stopped_Members"",  (Select Count(*) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' And T2.""U_ST_MEMBERSHIP_STATUS""='C' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Canceled_Members"", (Select Sum(U_ST_PREMIUM) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' And T2.""U_ST_MEMBERSHIP_STATUS""='C') As ""Total_Premiums_for_Canceled_Members"", 0 As ""Total_Number_of_Additions"" , 0 as ""Total_Additions_Premiums"" , 0 as ""Total_Net_Premiums"", T0.""U_ST_MEMBER_CARD"", T0.""U_ST_CREATION_DATE""
, T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE,T1.U_ST_CUSTOMER_GROUP
FROM ""@ST_CORP_MEMBERSHIP""  T0
JOIN ""@ST_CCI_CORP_CARD"" T1 ON T1.""Code"" = T0.""U_ST_MEMBER_CARD""
WHERE T0.""U_ST_MEMBER_CARD"" = '{Card_ID}'";

            Recordset RC_Membership_Procedure = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            DT_Summery.Rows.Add(RC_Membership_Procedure.RecordCount);

            for (int i = 0; i < RC_Membership_Procedure.RecordCount; i++)
            {
                for (int J = 0; J < DT_Summery.Columns.Count; J++)
                {
                    string Col_Name = string.Empty;
                    string UDF_Name = string.Empty;
                    try
                    {
                        Col_Name = DT_Summery.Columns.Item(J).Name;
                        UDF_Name = Col_Name;
                        double result = -1;
                        double.TryParse(RC_Membership_Procedure.Fields.Item(UDF_Name).Value.ToString(), out result);

                        if (result != -1)
                        {
                            DT_Summery.SetValue(Col_Name, i, result.ToString("N03"));
                        }
                        else
                        {
                            DT_Summery.SetValue(Col_Name, i, RC_Membership_Procedure.Fields.Item(UDF_Name).Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Col_Name + UDF_Name);
                    }
                }
                RC_Membership_Procedure.MoveNext();
            }
            Grid Grd_Membership = (Grid)form.Items.Item("700").Specific;

            Grd_Membership.AutoResizeColumns();

        }


        //private static void Calculate_Premium(ItemEvent pVal)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    if (form.Mode != BoFormMode.fm_OK_MODE)
        //    {
        //        throw new Exception("We can calculate the Premium if the form in OK Mode only");
        //    }
        //    string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
        //    Premium_Data Premium = Membership.Calculate_Premium(company, Membership_Code);
        //    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PREMIUM", 0, Premium.Premium_Amount.ToString());
        //    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DISCOUNT_PERCENTAGE", 0, Premium.Discount_Percentage.ToString());
        //    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DISCOUNT_VALUE", 0, Premium.Discount_Value.ToString());
        //    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WAITING_PERIOD", 0, Premium.Waiting_Period.ToString());

        //    form.Mode = BoFormMode.fm_UPDATE_MODE;
        //}
    }
}
