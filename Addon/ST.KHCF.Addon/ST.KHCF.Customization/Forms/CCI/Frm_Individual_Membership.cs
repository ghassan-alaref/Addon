using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ST.KHCF.Customization.Logic.Membership;
using Application = SAPbouiCOM.Application;
using Company = SAPbobsCOM.Company;
using Form = SAPbouiCOM.Form;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_Individual_Membership : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string isInvoiced = string.Empty;
        static string[] Aramex_ITems = new string[] { "450", "451", "452", "453", "454", "455" };
        static string[] Editable_Item_For_Add_Only = new string[] { "Item_17", "Item_16", "300" };


        internal static void Create_Membership_for_MemberCard(string MemberCard_Code)
        {
            Form Renewal_Form = Loader.Open_UDO_Form(KHCF_Objects.Individual_Membership);
            Renewal_Form.Mode = BoFormMode.fm_ADD_MODE;
            Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_CARD", 0, MemberCard_Code);
            Form_Obj.Set_Fields(Renewal_Form);
            Select_MemberCard(MemberCard_Code, Renewal_Form);
        }

        internal override Depends_List[] Get_Depends_List_List()
        {
            List<Depends_List> Result = new List<Depends_List>();
            Result.AddRange(base.Get_Depends_List_List());
            Result.Add(new Depends_List() { Item_ID = "142", Parent_Item_ID = "139", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '{{0}}'" });
            

            return Result.ToArray();
        }

        internal override void Initialize_Form(SAPbouiCOM.Form form)
        {
            base.Initialize_Form(form);
            //return;
            string SQL_Currancies = $@"SELECT T0.""CurrCode"" AS ""Code"",T0.""CurrCode"" AS ""Name"" FROM OCRN T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "161", SQL_Currancies, true);
            SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)form.Items.Item("161").Specific;

            string CCI_Department_ID = Configurations.Get_CCI_Department(company);
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" = {CCI_Department_ID}";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "251", SQL_Account_Manager, true);
            string SQL_Both = $@"SELECT T0.""empID"" AS ""Code"", (T0.""firstName"" || ' ' || T0.""lastName"") AS ""Name"" FROM OHEM T0 Where U_ST_EMPLOYEE_TYPE = 'B'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "17", SQL_Both, true);

            string SQL_PaymentTerms = $@"SELECT T0.""GroupNum"" AS ""Code"",T0.""PymntGroup"" AS ""Name"" FROM OCTG T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "194", SQL_PaymentTerms, true);

            SAPbouiCOM.ComboBox comboBox_type = (SAPbouiCOM.ComboBox)form.Items.Item("Item_17").Specific;
            comboBox_type.ValidValues.Add("N", "None");
            comboBox_type.ValidValues.Add("I", "Individual Membership");
            comboBox_type.ValidValues.Add("C", "Corporate Membership");

            SAPbouiCOM.ChooseFromList CHS_Member_Cards = form.ChooseFromLists.Item("CHS_Member_Cards");
            
            Conditions CHS_Member_Cards_Cons = CHS_Member_Cards.GetConditions();
            Condition CHS_Member_Cards_Con = CHS_Member_Cards_Cons.Add();
            CHS_Member_Cards_Con.Alias = "U_ST_APPROVAL_STATUS";
            CHS_Member_Cards_Con.Operation = BoConditionOperation.co_EQUAL;
            CHS_Member_Cards_Con.CondVal = "A";
            CHS_Member_Cards.SetConditions(CHS_Member_Cards_Cons);

            Fill_ComboButton_Values(form,"INDIV");
            
            Matrix Mat_Att = (Matrix)form.Items.Item("500").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            Mat_Att.Columns.Item("SELECTED").AffectsFormMode = false;
            Mat_Att.AutoResizeColumns();

            Grid Grd_Membership = (Grid)form.Items.Item("600").Specific;
            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE"" order By ""Code""";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_COVERAGE", SQL_Cov);
            string SQL_Memeber_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND U_ST_CUSTOMER_TYPE = 'C'";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_CUSTOMER_GROUP", SQL_Memeber_Customer_Group, true);

            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_CUSTOMER_GROUP")).DisplayType = BoComboDisplayType.cdt_Description;

            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("N","New");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("R", "Renew");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("C", "Canceled");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("S", "Stopped");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("P", "Past to Renew");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).DisplayType = BoComboDisplayType.cdt_Description;
            ((ComboBoxColumn)Grd_Membership.Columns.Item("ParentType")).ValidValues.Add("I", "Individual");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("ParentType")).ValidValues.Add("C", "Corporate");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("ParentType")).DisplayType = BoComboDisplayType.cdt_Description;
            Grd_Membership.AutoResizeColumns();

            form.Items.Item("19").AffectsFormMode = false;
            form.Items.Item("198").AffectsFormMode = false;
            form.Items.Item("203").AffectsFormMode = false;
            form.Items.Item("205").AffectsFormMode = false;
            form.Items.Item("500").AffectsFormMode = false;
            form.Items.Item("502").AffectsFormMode = false;
            form.Items.Item("503").AffectsFormMode = false;
            form.Items.Item("504").AffectsFormMode = false;
            Form_Obj.Load_Depends_Items(form);
         
            form.Items.Item("203").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
            form.Items.Item("24").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_False);
            //form.Items.Item("24").Visible = false;
            form.Items.Item("198").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            foreach (string OneItem in Editable_Item_For_Add_Only)
            {
                form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
            }

            form.Items.Item("Item_4").Click();
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
                    && BusinessObjectInfo.BeforeAction )
                {
                    
                    Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Form frm = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    if (!string.IsNullOrEmpty(frm.DataSources.DBDataSources.Item(0).GetValue("U_ST_PREMIUM", 0)) && !string.IsNullOrEmpty(frm.DataSources.DBDataSources.Item(0).GetValue("U_ST_DISCOUNT_VALUE", 0)))
                    {
                        double premium = Convert.ToDouble(frm.DataSources.DBDataSources.Item(0).GetValue("U_ST_PREMIUM", 0));
                        double discount = Convert.ToDouble(frm.DataSources.DBDataSources.Item(0).GetValue("U_ST_DISCOUNT_VALUE", 0));
                        frm.DataSources.UserDataSources.Item("253").Value = (premium - discount).ToString();
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
                }
                //if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                // && !BusinessObjectInfo.BeforeAction)
                //{
                //    Calculate_Premium(BusinessObjectInfo);
                //}

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                   
                    ADD_Update_UDO(BusinessObjectInfo);
                    Create_Child_Memberships(BusinessObjectInfo,"I");
                }

                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo, out isInvoiced);
                    if (isInvoiced == "Y")
                    {
                        return BubbleEvent;
                    }
                }
                else
                {
                    string d = "";
                }
                Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                form.Items.Item("500").AffectsFormMode = false;
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                //if (!BusinessObjectInfo.BeforeAction && Form_Obj.Set_ReadOnly(form, KHCF_Object))
                //{
                //    // return;
                //}
                //if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                //{
                //    //Set_Data_Load_Items_Enabled(form);
                //}

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo, out string isInvoiced)
        {

            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string U_ST_APPROVAL_STATUS = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
            string Parent_Type = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_MEMBERSHIP_TYPE", 0);
            string ParentID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_MEMBERSHIP_ID", 0);
            string TableName = string.Empty, FieldName = string.Empty;
            Load_Memberships(form, Membership_Code);
            LinkedButton Parent_Link = (LinkedButton)form.Items.Item("Item_18").Specific;
            if (Parent_Type == "I")
            {
                Parent_Link.LinkedObjectType = "ST_INDIV_MEMBERSHIP";
                TableName = "ST_INDIV_MEMBERSHIP";
                FieldName = "U_ST_MEMBER_NAME";
            }
            else if (Parent_Type == "C")
            {
                Parent_Link.LinkedObjectType = "ST_CORP_MEMBERSHIP";
                TableName = "ST_CORP_MEMBERSHIP";
                FieldName = "U_ST_CORPORATE_NAME";
            }
            if (!string.IsNullOrEmpty(TableName))
            {
                string SQL = $@"Select T0.""{FieldName}"" From ""@{TableName}"" T0 Where T0.""Code""='{ParentID}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.RecordCount > 0)
                {
                    form.DataSources.UserDataSources.Item("UD_9").Value = RC.Fields.Item(0).Value.ToString();
                }
            }
            else
            {
                form.DataSources.UserDataSources.Item("UD_9").Value = "";
            }
            Form_Obj.Load_Depends_Items(form);
            Fill_ComboButton_Values(form, "INDIV");
            Form_Obj.Set_Fields(form);

            string SQL_Payment = $@"Select top 1 T0.""DocEntry"", T0.""DocDate"" FROM ORCT T0 Where T0.""U_ST_MEMBERSHIP_CODE"" = '{Membership_Code}' Order By T0.""DocEntry"" DESC";
            Recordset RC_paymnet = Helper.Utility.Execute_Recordset_Query(company, SQL_Payment);

            double Due_Amount = 0;
            if (RC_paymnet.RecordCount != 0)
            {
                string payment_value = RC_paymnet.Fields.Item(0).Value.ToString();
                if (!string.IsNullOrEmpty(payment_value))
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RECEIPT_VOUCHER_NUMBER", 0, payment_value);
                string payment_date = RC_paymnet.Fields.Item(1).Value.ToString();
                if (!string.IsNullOrEmpty(payment_date))
                {
                    DateTime d;
                    bool result = DateTime.TryParse(payment_date, out d);
                    if (result)
                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COLLECTION_DATE", 0, d.ToString("yyyyMMdd"));
                }
            }
            else
            {
                form.DataSources.UserDataSources.Item("253").Value = "0";
            }

            string SQL_P = $@"Select sum(T0.""DocTotal"") FROM ORCT T0 Where T0.""U_ST_MEMBERSHIP_CODE"" = '{Membership_Code}'";
            Recordset RC_P = Helper.Utility.Execute_Recordset_Query(company, SQL_P);
            double Incoming_Payment = Convert.ToDouble(RC_P.Fields.Item(0).Value.ToString());
            double Discount_Value = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DISCOUNT_VALUE", 0).ToString());
            double Premium = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PREMIUM", 0).ToString());
            double Member_Amount = Premium - Discount_Value - Incoming_Payment;
            form.DataSources.UserDataSources.Item("253").Value = Math.Round(Member_Amount, 3).ToString();
            /*double total = Member_Amount;
            if (Membership.Check_Membership_Children(company, Membership_Code, "I"))
            {
                string SQL_Membership = $@"SELECT T0.""Code"",T0.""U_ST_PREMIUM"", T0.""U_ST_DISCOUNT_VALUE"" FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" in ('N','R','P') And T0.""U_ST_APPROVAL_STATUS"" ='A'
            AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Membership_Code}'";
                Recordset RC_P_Child = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
                for (int i = 0; i < RC_P_Child.RecordCount; i++)
                {
                    string Code = RC_P_Child.Fields.Item("Code").Value.ToString();
                    SQL_P = $@"Select sum(T0.""DocTotal"") FROM ORCT T0 Where T0.""U_ST_MEMBERSHIP_CODE"" = '{Code}'";
                    Recordset RC_P2 = Helper.Utility.Execute_Recordset_Query(company, SQL_P);
                    if (RC_P2.RecordCount > 0)
                    {
                        double Premium_Ch = Convert.ToDouble(RC_P_Child.Fields.Item("U_ST_PREMIUM").Value.ToString());
                        double Discount_Value_Ch = Convert.ToDouble(RC_P_Child.Fields.Item("U_ST_DISCOUNT_VALUE").Value.ToString());
                        double Incoming_Payment_Ch = Convert.ToDouble(RC_P2.Fields.Item(0).Value.ToString());
                        double Member_Amount_Ch = Premium_Ch - Discount_Value_Ch - Incoming_Payment_Ch;
                        total += Member_Amount_Ch;
                    }
                    RC_P_Child.MoveNext();
                }
            }
            form.DataSources.UserDataSources.Item("220").Value = total.ToString();*/

            string Invoiced = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0);

            if (!string.IsNullOrEmpty(Invoiced))
            {
                string SQL_Invoice_Unpaid1 = $@"SELECT T0.""DocTotal"", T0.""PaidToDate"" FROM OINV T0 WHERE T0.""U_ST_MEMBERSHIP_CODE""='{Membership_Code}'  And  T0.""CANCELED"" ='N' AND  T0.""DocStatus"" ='O'";
                Recordset RC_Invoice_npaid = Helper.Utility.Execute_Recordset_Query(company, SQL_Invoice_Unpaid1);
                if (RC_Invoice_npaid.RecordCount > 0)
                {
                    double DocTotal = Convert.ToDouble(RC_Invoice_npaid.Fields.Item("DocTotal").Value.ToString());
                    double PaidToDate = Convert.ToDouble(RC_Invoice_npaid.Fields.Item("PaidToDate").Value.ToString());
                    Due_Amount = DocTotal - PaidToDate;
                    form.DataSources.UserDataSources.Item("253").Value = Due_Amount.ToString();

                }

            }
            string SQL_Child = $@"Select T0.""Code"" From ""@ST_INDIV_MEMBERSHIP"" T0 Where T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Membership_Code}' ";
            Recordset RC_Child = Helper.Utility.Execute_Recordset_Query(company, SQL_Child);
            double Total_Due = Due_Amount;
            //U_ST_END_DATE
            string End_Date_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0);
            DateTime EndDate = DateTime.ParseExact(End_Date_Text, "yyyyMMdd", null);
            string isActive = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACTIVE", 0);
            if (DateTime.Now < EndDate && isActive != "N")
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACTIVE", 0, "Y");
            else
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACTIVE", 0, "N");

            //string Code = RC_Child.Fields.Item("Code").Value.ToString();
            //string SQL_Invoice_Unpaid = $@"SELECT T0.""DocTotal"", T0.""PaidToDate"" FROM OINV T0 WHERE T0.""U_ST_MEMBERSHIP_CODE""='{Code}' And  T0.""CANCELED"" ='N' AND  T0.""DocStatus"" ='O'";
            string SQL_Invoice_Unpaid = $@"SELECT Sum(T0.""DocTotal"" - T0.""PaidToDate"") FROM OINV T0 WHERE T0.""CANCELED""='N'  AND  (T0.""U_ST_MEMBERSHIP_CODE"" in (Select T1.""Code"" From ""@ST_INDIV_MEMBERSHIP"" T1  where T1.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Membership_Code}' AND U_ST_PARENT_MEMBERSHIP_TYPE = 'I')  or T0.""U_ST_MEMBERSHIP_CODE""='{Membership_Code}')";
            Recordset RC_Invoice_Unpaid = Helper.Utility.Execute_Recordset_Query(company, SQL_Invoice_Unpaid);
            string SQL_DownPayment_Unpaid = $@"SELECT Sum(T0.""DocTotal"" - T0.""PaidToDate"") FROM ODPI T0 WHERE T0.""CANCELED""='N'  AND  (T0.""U_ST_MEMBERSHIP_CODE"" in (Select T1.""Code"" From ""@ST_INDIV_MEMBERSHIP"" T1  where T1.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Membership_Code}' AND U_ST_PARENT_MEMBERSHIP_TYPE = 'I')  or T0.""U_ST_MEMBERSHIP_CODE""='{Membership_Code}')";
            Recordset RC_DownPayment_Unpaid = Helper.Utility.Execute_Recordset_Query(company, SQL_DownPayment_Unpaid);

            Total_Due = (double) RC_Invoice_Unpaid.Fields.Item(0).Value + (double)RC_DownPayment_Unpaid.Fields.Item(0).Value;
           
                form.DataSources.UserDataSources.Item("220").Value = Total_Due.ToString();

            if (form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INSTALLMENT_TYPE", 0) == "-1")
            {
                form.DataSources.UserDataSources.Item("198").Value = form.DataSources.UserDataSources.Item("220").Value;
            }
            else
            {
                form.DataSources.UserDataSources.Item("198").Value = "0";
            }

            string U_ST_APPROVAL_STATUS1 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
            SBO_Application.SetStatusBarMessage("Loading", BoMessageTime.bmt_Short, false);
            isInvoiced = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0);
            if (!string.IsNullOrEmpty(isInvoiced))
            {
                foreach (Item item in form.Items)
                {
                    string id = item.UniqueID;
                    if (id == "203" || id == "205" || id == "19" || id == "159")
                    {
                        form.Items.Item("203").Click();
                    }
                    else
                    {
                        if (form.Items.Item(id).Type == BoFormItemTypes.it_EDIT)
                        {
                            form.Items.Item(id).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                            form.Items.Item(id).Enabled = false;

                        }
                    }
                }
                isInvoiced = "Y";
            }
            else
            {
                isInvoiced = "N";
                string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "181", SQL_Invoice, false);
                string code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
                string SQL_Card = $@"SELECT T1.""SlpName"" FROM ""@ST_CCI_INDIV_CARD""  T0 JOIN OSLP T1 ON T0.U_ST_ACCOUNT_MANAGER = T1.""SlpCode"" WHERE T0.""Code"" = '{code}'";
                Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, RC_Card.Fields.Item("SlpName").Value.ToString());
                string waitingPeriod = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_WAITING_PERIOD", 0);
                string startDateString = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_START_DATE", 0);
                if (!string.IsNullOrEmpty(waitingPeriod) && !string.IsNullOrEmpty(startDateString))
                {
                    startDateString = startDateString.Insert(4, "/");
                    startDateString = startDateString.Insert(7, "/");
                    DateTime startDate = Convert.ToDateTime(startDateString);
                    DateTime waitingPeriodDate = startDate.AddMonths(int.Parse(waitingPeriod));
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WAITING_PERIOD_DATE", 0, waitingPeriodDate.ToString("yyyyMMdd"));
                }
            }
            string Payment_Method = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PAYMENT_METHOD", 0);
            string Aramex_Code = Configurations.Get_Aramex_Payment_Method_Code(company);
            bool Visiblity = false;
            if (Payment_Method == Aramex_Code)
            {
                Visiblity = true;
            }
            foreach (string Item in Aramex_ITems)
            {
                if (Visiblity == true)
                {
                    form.Items.Item(Item).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                }
                else
                {
                    form.Items.Item(Item).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                }
            }
            if (U_ST_APPROVAL_STATUS == "A")
            {
                form.Items.Item("198").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                form.Items.Item("351").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
            }
            else
            {
                form.Items.Item("198").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                form.Items.Item("351").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            }

            // form.Items.Item("198").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);


        }

        private static void Set_Data_Load_Items_Enabled(SAPbouiCOM.Form form)
        {
            string[] Premium_Items = new string[] { "15", "143", "161", "167", "194" };
            bool Must_Enabled;
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                if (form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INVOICE_NUMBER", 0) =="")
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
                Must_Enabled= true;
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

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            string SQL = $@"Select T0.""U_ST_MEMBER_CARD"" From ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""Code""='{UDO_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string UDO_Member_Code = "";
            if (RC.RecordCount > 0)
            {
                UDO_Member_Code = RC.Fields.Item(0).Value.ToString();
            }
            
            if (businessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD )
            {
                string Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBERSHIP_STATUS", 0);
                if ((Status == "R" || Status == "P") && form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_RENEW_CHILDREN", 0) == "Y")
                {
                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                    string MemberCard_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBER_CARD", 0);
                    Membership.Renewal_Children(company, MemberCard_Code,UDO_Code, UDO_Info);
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
            
            if (pVal.MenuUID == "1282" || pVal.MenuUID == "1281")//Add || Find
            {
                Form form = SBO_Application.Forms.ActiveForm;

                if (pVal.MenuUID == "1282")
                {
                    Form_Obj.Set_Fields(form);

                    // string Booth_Employee = Utility.Get_Booth_Employee(company);
                    //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_EMPLOYEE_ID", 0, Booth_Employee);
                }
                else if (pVal.MenuUID == "1281")
                {
                    Form_Obj.Set_Fields(form);
                }
                    //DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    //DT_Members.Rows.Clear();
                    Helper.Utility.Clear_ComboBox(form, new string[] { "181"});
                Set_Data_Load_Items_Enabled(form);
                //if (pVal.MenuUID == "1281")
                //{
                //    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                //    form.Items.Item("9").Enabled = true;
                //}
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE || (isInvoiced == "N" && SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_OK_MODE))
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
                
            }

            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE && Form_Obj != null)
            {
                if (SBO_Application.Forms.ActiveForm.TypeEx == Form_Obj.Form_Type)
                {
                    //Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
                    Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);

                }
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

        private static void Set_Default_Value_Befoe_Adding(SAPbouiCOM.Form form)
        {
            //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATION_DATE", 0, DateTime.Today.ToString("yyyyMMdd"));
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_INCOMPLETE", 0, "Y");
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_EMPLOYEE_ID", 0, Utility.Get_Booth_Employee(company));
               
                // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "Ibtn_cmb_wa
            }

        }

        private static void Print_Invoice(ItemEvent pval, Company oCompany)
        {
            List<Helper.Utility.Crystal_Report_Parameter> Result = new List<Helper.Utility.Crystal_Report_Parameter>();
            string Rpt_File = Utility.Get_Configuration(oCompany, "Indi_Member_Print_Path", "Individual Membership Print Path", "");
            //System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //System.Configuration.AppSettingsSection appSettings = (System.Configuration.AppSettingsSection)config.GetSection("appSettings");
            //string Connection_String = appSettings.Settings["HANA_Connection_String"].Value;
            string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(oCompany, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Utility.Get_Configuration(oCompany, "Report_Output_Folder_Path", "Report Output Folder Path", ""));
            SBO_Application.StatusBar.SetText("Report has been created successfully at " + Pdf_File_Name, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Success);
        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            if (ItemUID == "167")
            {
                SAPbouiCOM.Form form1 = SBO_Application.Forms.Item(FormUID);
                string Payment_Method = form1.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PAYMENT_METHOD", 0);
                string Aramex_Code = Configurations.Get_Aramex_Payment_Method_Code(company);
                bool Visiblity = false;
                if (Payment_Method == Aramex_Code)
                {
                    Visiblity = true;
                }
                foreach (string Item in Aramex_ITems)
                {
                    if (Visiblity == true)
                    {
                        form1.Items.Item(Item).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                    }
                    else
                    {
                        form1.Items.Item(Item).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    }
                }
            }

            if (ItemUID == "15")// Start Date
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string StartDate_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0);
                string Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBERSHIP_STATUS", 0);
                if (StartDate_Text == "")
                {
                    return;
                }
                DateTime StartDate = DateTime.ParseExact(StartDate_Text, "yyyyMMdd", null);
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_END_DATE", 0, StartDate.AddYears(1).AddDays(-1).ToString("yyyyMMdd"));
                string Member_Card = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBER_CARD", 0);
                if (Member_Card != "")
                {
                    string SQL_Card = $@"SELECT T0.U_ST_DATE_OF_BIRTH FROM ""@ST_CCI_INDIV_CARD"" T0 WHERE T0.""Code"" = '{Member_Card}'";
                    Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);
                    DateTime birthDate = (DateTime)RC_Card.Fields.Item("U_ST_DATE_OF_BIRTH").Value;
                    if (birthDate.Year > DateTime.Now.Year)
                    {
                        birthDate = birthDate.AddYears(-1000);
                    }
                    int Age = (StartDate - (birthDate)).Days / 365;
                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_AGE", 0, Age.ToString());
                    if (Status == "R")
                    {
                        bool isPast = false;
                        string SQL_Status = $@"SELECT TOP 1 ""U_ST_MEMBERSHIP_STATUS"",""U_ST_START_DATE"",""U_ST_END_DATE"",
""U_ST_WAITING_PERIOD"",""U_ST_WAITING_PERIOD_DATE"",""U_ST_EMPLOYEE_ID"",
""U_ST_DEPARTMENT"" ,""U_ST_BRANCH"",""U_ST_CLASS"",""U_ST_SECTION"",""U_ST_DISCOUNT_PERCENTAGE"", ""U_ST_OTHER_PAYMENT_FORM"",""U_ST_STUDENT_FINANCIAL_NO"" ,
""U_ST_PAYMENT_METHOD"", ""U_ST_COVERAGE"", ""U_ST_INSTALLMENT_TYPE"", ""U_ST_DISCOUNT_VALUE"",""U_ST_APPROVAL_STATUS"",""U_ST_CHANNEL"",""U_ST_SUB_CHANNEL"" 
FROM ""@ST_INDIV_MEMBERSHIP"" 
WHERE ""U_ST_MEMBER_CARD""='{Member_Card}' AND ""U_ST_MEMBERSHIP_STATUS"" IN ('N','R','P') ORDER BY ""U_ST_START_DATE"" DESC ";
                        Recordset RC_Status = Helper.Utility.Execute_Recordset_Query(company, SQL_Status);

                        if (RC_Status.RecordCount > 0)
                        {
                            DateTime EndDate = Convert.ToDateTime(RC_Status.Fields.Item("U_ST_END_DATE").Value.ToString());
                            Get_New_Renewal_StartDate(company, EndDate, out isPast, StartDate);
                            if(isPast)
                            {
                                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_STATUS", 0, "P");
                            }
                        }
                    }
                }

            }
            //if (ItemUID == "169")
            //{
            //    Form form = SBO_Application.Forms.Item(FormUID);
            //    string Member_Card = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBER_CARD", 0);
            //    string Coverage_Rule = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_COVERAGE", 0);
            //    string Age = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_AGE", 0);
            //    string dsds = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0);
            //    string Channel = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_CHANNEL", 0);
            //    string Sub_Channel = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_SUB_CHANNEL", 0);
            //    //DateTime Startdate = Convert.ToDateTime(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0));
            //    string start_date= dsds;//= Startdate.ToString("yyyyMMdd"); 

            //    if (string.IsNullOrEmpty(Member_Card))
            //    {
            //        throw new Custom_Exception("please Choose Member Card");
            //    }
            //    string SQL_Card = $@"select * from ""@ST_CCI_INDIV_CARD"" T0  where T0.""Code""={Member_Card}";
            //    Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);
            //    string Gender = RC_Card.Fields.Item("U_ST_GENDER").Value.ToString();
            //    string Membership_Nationality = RC_Card.Fields.Item("U_ST_NATIONALITY").Value.ToString();
            //    string Membership_RESIDENCY = RC_Card.Fields.Item("U_ST_RESIDENCY").Value.ToString();
            //  //  string Channel = RC_Card.Fields.Item("U_ST_CHANNEL").Value.ToString();
            //    //string Sub_Channel = RC_Card.Fields.Item("U_ST_SUB_CHANNEL").Value.ToString();
            //    string Broker = RC_Card.Fields.Item("U_ST_BROKER1").Value.ToString();

            //    string sql_nationality = $@"Select ""Code"" From OCRY Where ""Name""='{Membership_Nationality}'";
            //    Recordset RC_nationalityt = Helper.Utility.Execute_Recordset_Query(company, sql_nationality);
            //    RC_nationalityt.DoQuery(sql_nationality);
            //   // Membership_Nationality = RC_nationalityt.Fields.Item("Code").Value.ToString();

            //    string sql_resan = $@"Select ""Code"" From OCRY Where ""Name""='{Membership_RESIDENCY}'";
            //    Recordset RC_resan = Helper.Utility.Execute_Recordset_Query(company, sql_resan);
            //    RC_resan.DoQuery(sql_resan);
            //    //Membership_RESIDENCY = RC_resan.Fields.Item("Code").Value.ToString();

            //    if (RC_nationalityt.RecordCount > 0)
            //        Membership_Nationality = RC_nationalityt.Fields.Item("Code").Value.ToString();

            //    if (RC_resan.RecordCount > 0)
            //        Membership_RESIDENCY = RC_resan.Fields.Item("Code").Value.ToString();
            //    string SQL_Coverage = "";

            //    if (Membership_Nationality != "JO")
            //    {

            //        SQL_Coverage = $@"SELECT T1.U_ST_DISCOUNT_PERCENTAGE, T1.U_ST_DISCOUNT_VALUE, T1.U_ST_WAITING_PERIOD
            //    FROM ""@ST_COVERAGE_RULES""  T0 INNER JOIN ""@ST_COVERAGE_RULES_L""  T1 ON T0.""Code"" = T1.""Code"" where T1.""U_ST_COVERAGE"" = '{Coverage_Rule}'
            //    And  T0.U_ST_CHANNEL = '{Channel}'  
            //    AND (T0.U_ST_SUB_CHANNEL = '{Sub_Channel}' OR IFNULL(T0.U_ST_SUB_CHANNEL,'') = '') 
            //    AND (T0.U_ST_BROKER = '{Broker}' OR IFNULL(T0.U_ST_BROKER,'') = '') 
            //    AND ({Age} BETWEEN  T1.U_ST_AGE_FROM AND  T1.U_ST_AGE_TO) AND  (T1.U_ST_GENDER = '{Gender}' OR T1.U_ST_GENDER = 'B') 
            //    AND  T1.U_ST_NATIONALITY = '{Membership_Nationality}' 
            //    AND  (T1.U_ST_RESIDENCY = '{Membership_RESIDENCY}' OR IFNULL(T1.U_ST_RESIDENCY,'') = '')
            //    AND ('{start_date}' BETWEEN  T1.U_ST_START_DATE AND  T1.U_ST_END_DATE)";
            //    }
            //    else
            //    {
            //        SQL_Coverage = $@"SELECT T1.U_ST_DISCOUNT_PERCENTAGE, T1.U_ST_DISCOUNT_VALUE, T1.U_ST_WAITING_PERIOD
            //    FROM ""@ST_COVERAGE_RULES""  T0 INNER JOIN ""@ST_COVERAGE_RULES_L""  T1 ON T0.""Code"" = T1.""Code"" where T1.""U_ST_COVERAGE"" = '{Coverage_Rule}'
            //      AND T0.U_ST_CHANNEL = '{Channel}'
            //    AND (T0.U_ST_SUB_CHANNEL = '{Sub_Channel}' OR IFNULL(T0.U_ST_SUB_CHANNEL,'') = '') 
            //    AND (T0.U_ST_BROKER = '{Broker}' OR IFNULL(T0.U_ST_BROKER,'') = '') 
            //    AND ({Age} BETWEEN  T1.U_ST_AGE_FROM AND  T1.U_ST_AGE_TO) AND  (T1.U_ST_GENDER = '{Gender}' OR T1.U_ST_GENDER = 'B') 
            //    AND  (T1.U_ST_NATIONALITY = '{Membership_Nationality}' OR  T1.U_ST_NATIONALITY = 'NJ') 
            //    AND  (T1.U_ST_RESIDENCY = '{Membership_RESIDENCY}' OR IFNULL(T1.U_ST_RESIDENCY,'') = '')
            //    AND ('{start_date}' BETWEEN  T1.U_ST_START_DATE AND  T1.U_ST_END_DATE)";
            //    }

            //    //here coverage
            //    if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0).ToString()) ||
            //        string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_END_DATE", 0).ToString()))
            //    {
            //        throw new Custom_Exception("you should fill start and end date first!");
            //    }
            //    DateTime Start_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0), "yyyyMMdd", null);//.ToString("MM/dd/yyyy");
            //    //DateTime End_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_END_DATE", 0), "yyyyMMdd", null);//.ToString("MM/dd/yyyy");
            //    //string SQL_Coverage = $@"SELECT T2.""U_ST_COVERAGE"", T2.""U_ST_WAITING_PERIOD"", T2.""U_ST_START_DATE"", T2.""U_ST_END_DATE"" ,T2.""U_ST_DISCOUNT_VALUE"" FROM  ""@ST_COVERAGE_RULES_L"" T2 where T2.""U_ST_COVERAGE"" ='{Coverage_Rule}'";
            //    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Coverage);
            //    bool v = false;
            //    if (RC.RecordCount > 0)
            //    {
            //        string waiting_period = RC.Fields.Item("U_ST_WAITING_PERIOD").Value.ToString();
            //        string U_ST_DISCOUNT_VALUE = RC.Fields.Item("U_ST_DISCOUNT_VALUE").Value.ToString();
            //        string waiting_date = Start_Date.AddMonths(Convert.ToInt32(waiting_period)).ToString("yyyyMMdd");
            //        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_WAITING_PERIOD", 0, waiting_period);
            //        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_WAITING_PERIOD_DATE", 0, waiting_date);
            //        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_DISCOUNT_VALUE", 0, U_ST_DISCOUNT_VALUE);
            //        v = true;
            //    }
            //   //while (!RC.EoF)
            //   // {
            //   //     DateTime startdate = (DateTime)RC.Fields.Item("U_ST_START_DATE").Value;
            //   //     DateTime enddate = (DateTime)RC.Fields.Item("U_ST_END_DATE").Value;
            //   //     //int Age_From = (int)RC.Fields.Item("U_ST_AGE_FROM").Value;
            //   //     //int Age_To = (int)RC.Fields.Item("U_ST_AGE_TO").Value;
            //   //     //int Gender = (int)RC.Fields.Item("U_ST_GENDER").Value;
            //   //     if (Start_Date <= startdate && enddate >= End_Date)
            //   //     {
            //   //         string waiting_period =RC.Fields.Item("U_ST_WAITING_PERIOD").Value.ToString();
            //   //         string U_ST_DISCOUNT_VALUE = RC.Fields.Item("U_ST_DISCOUNT_VALUE").Value.ToString();
            //   //         string waiting_date = Start_Date.AddMonths(Convert.ToInt32(waiting_period)).ToString("yyyyMMdd");
            //   //         form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_WAITING_PERIOD", 0, waiting_period);
            //   //         form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_WAITING_PERIOD_DATE", 0,waiting_date) ;
            //   //         form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_DISCOUNT_VALUE", 0, U_ST_DISCOUNT_VALUE) ;
            //   //         v = true;
            //   //         break;
            //   //     }
            //   //     RC.MoveNext();
            //   // }
            //    if (!v)
            //    {
            //        throw new Custom_Exception("please Ckeck Coverage Rules for The Start and end date");
            //    }

            //}


//            if (ItemUID == "17")
//            {
//                Form form = SBO_Application.Forms.Item(FormUID);
//                string Employee_ID = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_EMPLOYEE_ID", 0);
//                string sql_employee = $@"SELECT T1.""branch"",T1.""dept""
//FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
//WHERE T1.""salesPrson"" ='{Employee_ID}'";
//                Recordset RC_Employee = Helper.Utility.Execute_Recordset_Query(company, sql_employee);
//                if (RC_Employee.RecordCount > 0)
//                {
//                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_BRANCH", 0, RC_Employee.Fields.Item("branch").Value.ToString());
//                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_DEPARTMENT", 0, RC_Employee.Fields.Item("dept").Value.ToString());
//                }
                

//            }

            if (Form_Obj.Get_Depends_Parent_Item_IDs_List().Contains(ItemUID))
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                Form_Obj.Load_One_Depends_Parent_Item(form, ItemUID);
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            Matrix Mat_Add = (Matrix)form.Items.Item("500").Specific;
            //Mat_Add.LinkPressedBefore = 

            if (form.Mode == BoFormMode.fm_UPDATE_MODE)
            {
                string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                string SQL_Financial = $@"SELECT T0.""U_ST_INVOICE_NUMBER"", T0.""U_ST_PAYMENT_NUMBER"", T0.""U_ST_JE_NUMBER"" FROM ""@ST_INDIV_MEMBERSHIP"" T0 Where T0.""Code""='{Membership_Code}'";
                Recordset RC_financial = Helper.Utility.Execute_Recordset_Query(company, SQL_Financial);
                if (RC_financial.RecordCount > 0)
                {
                    string Invoice_Number = RC_financial.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString();
                    string Payment_Number = RC_financial.Fields.Item("U_ST_PAYMENT_NUMBER").Value.ToString();
                    string JE_Number = RC_financial.Fields.Item("U_ST_JE_NUMBER").Value.ToString();
                    if (!string.IsNullOrEmpty(Invoice_Number) || !string.IsNullOrEmpty(Payment_Number) || !string.IsNullOrEmpty(JE_Number))
                    {
                        //form.Mode = BoFormMode.fm_OK_MODE;

                        Logic.Classes.Field_Definition[] All_Membership_Fields = Logic.Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == KHCF_Objects.Individual_Membership).ToArray();
                        string[] Excluded_Fields = new string[] { "U_ST_PAYER" };
                        string SQL_DB_Data = $@"SELECT * FROM ""@ST_INDIV_MEMBERSHIP"" T0 Where T0.""Code""='{Membership_Code}'";
                        Recordset RC_DB_Data = Helper.Utility.Execute_Recordset_Query(company, SQL_DB_Data);
                        bool Need_to_Stop = false;
                        foreach (Field_Definition One_Field in All_Membership_Fields)
                        {
                            if (Excluded_Fields.Contains(One_Field.Column_Name_In_DB))
                            {
                                continue;
                            }
                            string DB_Data;
                            string Form_Data = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue(One_Field.Column_Name_In_DB, 0);
                            if (One_Field.Data_Type == BoFieldTypes.db_Date)
                            {
                                DateTime Temp = (DateTime)RC_DB_Data.Fields.Item(One_Field.Column_Name_In_DB).Value;
                                if (Temp.Year == 1899)
                                {
                                    DB_Data = "";
                                }
                                else
                                {
                                    DB_Data = Temp.ToString("yyyyMMdd");
                                }                              
                            }
                            else if (One_Field.Data_Type == BoFieldTypes.db_Float || One_Field.Data_Type == BoFieldTypes.db_Numeric)
                            {
                                double Temp;
                                double.TryParse(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue(One_Field.Column_Name_In_DB, 0), out Temp);
                                Form_Data = Temp.ToString();
                                if (string.IsNullOrWhiteSpace(Form_Data))
                                {
                                    Form_Data = "0";
                                }
                                DB_Data = RC_DB_Data.Fields.Item(One_Field.Column_Name_In_DB).Value.ToString();
                            }
                            else
                            {
                                DB_Data = RC_DB_Data.Fields.Item(One_Field.Column_Name_In_DB).Value.ToString();
                            }

                            if (DB_Data != Form_Data)
                            {
                                Need_to_Stop = true;
                                break;
                            }
                        }
                        if (Need_to_Stop == true)
                        {
                            throw new Custom_Exception($"Membership [{Membership_Code}] cannot be updated because of its financial impact.");
                        }
                    }
                    else
                    {

                        string SQL_Corp = $@"SELECT T0.""Code"",T0.""U_ST_CORPORATE_TYPE"" , T0.""U_ST_END_DATE"" FROM ""@ST_CORP_MEMBERSHIP""  T0 WHERE T0.""Code"" = (SELECT T0.""U_ST_PARENT_MEMBERSHIP_ID"" FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""Code""='{Membership_Code}')";
                        Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Corp);
                        if (RC.RecordCount > 0)
                        {
                            string Corp_Type = RC.Fields.Item("U_ST_CORPORATE_TYPE").Value.ToString();
                            DateTime EndDate = Convert.ToDateTime(RC.Fields.Item("U_ST_END_DATE").Value.ToString());
                            DateTime Membership_StartDate = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_START_DATE", 0), "yyyyMMdd", null);
                            DateTime Membership_EndDate = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0), "yyyyMMdd", null);

                            if (Corp_Type == "R")
                            {
                                if ((DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0), "yyyyMMdd", null)) > EndDate)
                                    throw new Custom_Exception("The End date of Membership shouldn`t exceed its Corporate Membership");
                            }
                            else if (Corp_Type == "C")
                            {
                                if (Membership_StartDate.AddYears(1) < Membership_EndDate)
                                {
                                    throw new Custom_Exception("The End date of Membership shouldn`t exceed one year from the start date");
                                }
                            }
                        }

                    }
                }
              

            }

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
               
                if (pVal.ItemUID == "191" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Invoice(pVal);
                    Create_Child_Invoice(pVal);
                    Check_Incomplete(pVal);
                    
                }
                if (pVal.ItemUID == "189" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Payment(pVal);
                    Check_Incomplete(pVal);
                } 

                if (pVal.ItemUID == "24" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
                    if (form.Mode != BoFormMode.fm_ADD_MODE)
                        Approve(pVal);
                    else
                        throw new Custom_Exception("Cannot Approve the membership in Add Mode.");
                }
                if (pVal.ItemUID == "25" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
                    if (form.Mode != BoFormMode.fm_ADD_MODE)
                        Reject(pVal);
                    else
                        throw new Custom_Exception("Cannot Reject the membership in Add Mode.");
                }
                
                if (pVal.ItemUID == "192" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Calculate_Premium(pVal);
                }
                if (pVal.ItemUID == "301" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Print_Invoice(pVal,company);
                }
                if (pVal.ItemUID == "300" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Member_Card_Choose_From_List(pVal);
                }
                if (pVal.ItemUID == "320" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Parent_Membership_Choose_From_List(pVal);
                }
                if (pVal.ItemUID == "19" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Actions(pVal);
                }
                if (pVal.ItemUID == "502" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                    //SBO_Application.Menus.Item("1304").Activate();
                }
                if (pVal.ItemUID == "503" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                    //SBO_Application.Menus.Item("1304").Activate();
                }
                if (pVal.ItemUID == "504" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }

                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }

                if (pVal.ItemUID =="26" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Document(pVal);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Open_Document(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;

            string Document_Type = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_DOCUMENT_TYPE", 0);
            string DocEntry = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INVOICE_NUMBER", 0);
            if (Document_Type == "")
            {
                throw new Logic.Custom_Exception("The Document type is missing.");
            }
            SBO_Application.OpenForm((BoFormObjectEnum)int.Parse(Document_Type), "", DocEntry);

        }

        internal static void Reject(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);

            Logic.Membership.Reject(company, UDO_Code, UDO_Info);

//            string SQL_Membership =
//$@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_MEMBERSHIP_STATUS"" 
//FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" = 'P'
//And T0.""U_ST_APPROVAL_STATUS"" ='P' AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{UDO_Code}'";

            string SQL_Membership = Membership.SQL_Parent_Memberships(UDO_Code, "I", "P", "P");

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

        internal static void Approve(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);

            Logic.Membership.Approve(company, UDO_Code, UDO_Info);

            //            string SQL_Membership =
            //$@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_MEMBERSHIP_STATUS"" 
            //FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" = 'P'
            //And T0.""U_ST_APPROVAL_STATUS"" ='P' AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{UDO_Code}'";

            string SQL_Membership = Membership.SQL_Parent_Memberships(UDO_Code, "I", "P", "P");

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

            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("The action can be run only in OK mode.");
            }
            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("19").Specific;
            string Title = "";
            string Action_ID = form.DataSources.UserDataSources.Item("19").Value;
            if (Action_ID == "-")
                throw new Logic.Custom_Exception("Please select an action.");
            else
            {
                Title = Utility.Get_Field_Configuration(company, form.TypeEx.Replace("ST","Frm") + "_" + Action_ID, "", "");
            }
            if (Action_ID == "SA")
            {
                Title = "Stop Children";
            }
            else if (Action_ID == "CA")
            {
                Title = "Cancel Children";
            }
            if (Title == "" || string.IsNullOrEmpty(Title))
                throw new Logic.Custom_Exception($"This Action [{Action_ID}] is not supported.");
            else if (Title.ToLower() == "Stop".ToLower())
            {
                string Stop_Date_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_DATE", 0);
                if (Stop_Date_Text == "" || form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_NOTE", 0) == "")
                {
                    form.Items.Item("203").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    form.Items.Item("205").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    if (Stop_Date_Text == "")
                    {
                        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_STOP_DATE", 0, (DateTime.Today).ToString("yyyyMMdd"));
                       // form.Mode = BoFormMode.fm_UPDATE_MODE;
                    }
                    SBO_Application.MessageBox("Please confirm the Stop Date and the Stop Reason and try again");
                    SBO_Application.StatusBar.SetText("Please confirm the Stop Date and the Stop Reason and try again", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);
                    return;
                }
            }
          
            if (SBO_Application.MessageBox($"Are you sure you want to {Title} the membership?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);

            switch (Action_ID)
            {
                case "C"://Cancel
                    Membership.Cancel_Individual_Membership(company, UDO_Code, UDO_Info);
                    if (Membership.Check_Membership_Children(company,UDO_Code, "I"))
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
                    if (Membership.Check_Membership_Children(company, UDO_Code, "I"))
                    {
                        if (SBO_Application.MessageBox($@"Do you want to close From Children Membership?", 1, "Yes", "No") == 1)
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
                case "P"://Close
                    Convert_Past_to_Renew(form);
                    break;
                case "R"://Remove
                    Membership.Remove(company, UDO_Code, UDO_Info);
                    if (Membership.Check_Membership_Children(company, UDO_Code, "I"))
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
                case "S"://Stop
                    DateTime StopDate = DateTime.ParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_DATE", 0), "yyyyMMdd", null);
                    string Stop_Note = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_NOTE", 0);
                    Membership.Stop_Individual_Membership(company, UDO_Code, UDO_Info, StopDate, Stop_Note);
                    if (Membership.Check_Membership_Children(company, UDO_Code, "I"))
                    {
                        if (SBO_Application.MessageBox($@"Are you sure you want to stop the related memberships?", 1, "Yes", "No") == 1)
                        {
                            Form Stop_Form = Frm_Stop_Cancel_Children.Create_Form();
                            Stop_Form.DataSources.UserDataSources.Item("Type").Value = "S";
                            Stop_Form.DataSources.UserDataSources.Item("UD_2").Value = UDO_Code;
                            Frm_Stop_Cancel_Children.FillData(Stop_Form);
                            Stop_Form.Visible = true;
                        }
                    }
                    break;
                default:
                    throw new Logic.Custom_Exception($"This Action [{Action_ID}] is not supported");
            }

            SBO_Application.StatusBar.SetText("Operation completed successfully.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
        }

        internal static void Convert_Past_to_Renew(Form form)
        {
            string Status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS", 0);
            if (Status != "P")
            {
                throw new Logic.Custom_Exception("The Membership status is not Past Membershipز");
            }

            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
            Membership.Convert_Past_to_Renew(company, Membership_Code, Form_Obj, UDO_Info);

            if (Membership.Check_Membership_Children(company, Membership_Code, "I"))
            {
                if (SBO_Application.MessageBox($@"Are you sure you want to renew the related past memberships?", 1, "Yes", "No") == 1)
                {
                    //                    string SQL_Membership = 
                    //$@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_MEMBERSHIP_STATUS"" 
                    //FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" = 'P'
                    //And T0.""U_ST_APPROVAL_STATUS"" ='A' AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Membership_Code}'";

                    string SQL_Membership = Membership.SQL_Parent_Memberships(Membership_Code, "I", "A", "P");

                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);

                    for (int i = 0; i< RC.RecordCount; i++)
                    {
                        string Code = RC.Fields.Item("Code").Value.ToString();
                        Membership.Convert_Past_to_Renew(company, Code, Form_Obj, UDO_Info);
                        RC.MoveNext();
                    }
                    
                }
            }

        }

        private static void Renewal_Form(SAPbouiCOM.Form form,bool isNew_Form)
        {
            string Original_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string MemberCard_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            string MemberCard_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_NAME", 0);
            DateTime Old_End_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0), "yyyyMMdd", null);
            bool Is_Past;
            DateTime New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Old_End_Date, out Is_Past);
            int Months = Utility.GetMonthDifference(Old_End_Date, DateTime.Now);
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS", 0) == "S")
            {
                DateTime Stop_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_STOP_DATE", 0), "yyyyMMdd", null);
                New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Stop_Date, out Is_Past);
                
            }
            UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == Form_Obj.KHCF_Object);
            Form Renewal_Form;
            if (isNew_Form)
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

                foreach (Field_Definition One_Field in Fields) // !One_Field.Column_Name_In_DB.Contains("CURRENCY") &&
                {
                    if (!One_Field.Column_Name_In_DB.Contains("NAME") && !One_Field.Column_Name_In_DB.Contains("PAYMENT_METHOD") &&
                        !One_Field.Column_Name_In_DB.Contains("COVERAGE") &&
                        !One_Field.Column_Name_In_DB.Contains("INSTALLMENT") &&
                        !One_Field.Column_Name_In_DB.Contains("DISCOUNT_PERCENT"))
                        Renewal_Form.DataSources.DBDataSources.Item(0).SetValue(One_Field.Column_Name_In_DB, 0, form.DataSources.DBDataSources.Item(0).GetValue(One_Field.Column_Name_In_DB, 0));
                }

                Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, "");
                Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_NAME", 0, MemberCard_Name);
                Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_INVOICE_NUMBER", 0, "");
                Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_START_DATE", 0, New_Renewal_StartDate.ToString("yyyyMMdd"));
                DateTime ddd = New_Renewal_StartDate.AddMonths(Configurations.Get_Renewal_Month_for_End_Date(company, true)).AddDays(-1);
                Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_END_DATE", 0, New_Renewal_StartDate.AddMonths(Configurations.Get_Renewal_Month_for_End_Date(company, true)).AddDays(-1).ToString("yyyyMMdd"));
                if (Is_Past)
                {
                    Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_STATUS", 0, "P");
                }
                else
                {
                    Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_STATUS", 0, "R");
                }

                Renewal_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PREVIOUS_MEMBERSHIP_CODE", 0, Original_Code);

                Form_Obj.Set_Fields(Renewal_Form);
                if (isNew_Form)
                    form.Close();
            }
            finally
            {
                Renewal_Form.Freeze(false);
            }

        }

        private static void Create_Payment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.ActiveForm;
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Exception("We can create a Payment in the OK mode only");
            }
            string Other_Payment = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_OTHER_PAYMENT_FORM", 0);
            if (Other_Payment != "N")
            {
                throw new Custom_Exception("You cannot create an incoming payment for these payment forms");
            }
            if ((new string[] { "S", "C" }).Contains(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS",0)))
            {
                throw new Logic.Custom_Exception($"According the Status, We can't create Payment,");
            }
            string Invoice_Number = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INVOICE_NUMBER", 0);
            if (Invoice_Number == "" || Invoice_Number == "0")
            {
                throw new Custom_Exception("You cannot create an incoming payment if the Membership is not related to an Invoice");
            }

            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Member_Card = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            string Currency = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CURRENCY", 0);
            Create_Payment_Validation(form, Membership_Code, Member_Card);

            double Exchange_Rate = Utility.Get_Exchange_Rate(company, Currency, DateTime.Today);
            double Payment_Amount = Math.Round(double.Parse(form.DataSources.UserDataSources.Item("198").Value) * (1 / Exchange_Rate), 3);
            if (!string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PREMIUM", 0)) && !string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DISCOUNT_VALUE", 0)))
            {
                double premium = Math.Round(Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PREMIUM", 0)) * (1 / Exchange_Rate), 3);
                double discount = Math.Round(Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DISCOUNT_VALUE", 0)) * (1 / Exchange_Rate), 3);
                //form.DataSources.UserDataSources.Item("253").Value = (premium - discount - Payment_Amount).ToString();
                double value = premium - discount;
            }

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

            //string CardCode = Utility.Get_CardCode_By_CardNumber(company, Card_Number); 2079f
            string StartDate_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_START_DATE", 0);
            DateTime start_date = DateTime.ParseExact(StartDate_Text, "yyyyMMdd", null);
            SBO_Application.ActivateMenuItem("2817");
            Form Frm_Payment = SBO_Application.Forms.ActiveForm;

            Form UDF_Form = SBO_Application.Forms.Item(Frm_Payment.UDFFormUID);
            System.Threading.Thread.Sleep(1000);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            string BP_Code = Utility.Get_BP_Code(company, Member_Card, UDO_Info);

            ((EditText)Frm_Payment.Items.Item("5").Specific).Value = BP_Code;
            //System.Threading.Thread.Sleep(2000);
            Utility.Set_UDF_Value_on_Form(Membership_Code, UDF_Form, "U_ST_MEMBERSHIP_CODE", true);
            Utility.Set_UDF_Value_on_Form("I", UDF_Form, "U_ST_MEMBERSHIP_TYPE", false);
            ((EditText)Frm_Payment.Items.Item("26").Specific).Value = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PAYER", 0); ;

            System.Threading.Thread.Sleep(1000);
           // Frm_Payment.Items.Item("14").Click();
            Matrix Mat_Payment_Lines = (Matrix)Frm_Payment.Items.Item("20").Specific;
            double Rest_Amount = Payment_Amount;
            bool Has_Selected_Invoice = false;
            for (int i = 0; i < Mat_Payment_Lines.RowCount; i++)
            {
                if (Rest_Amount <= 0)
                {
                    break;
                }
                EditText Txt_Doc_Type = (EditText) Mat_Payment_Lines.GetCellSpecific("45", i + 1);
                if (Txt_Doc_Type.Value != "13" && Txt_Doc_Type.Value != "203")
                {
                    continue;
                }

                EditText Txt_DueBalance = (EditText) Mat_Payment_Lines.GetCellSpecific("7", i + 1);
                string Due_Balance_Text = Txt_DueBalance.Value;
                if (Due_Balance_Text.IndexOf("-") >= 0)
                {
                    continue;
                }
                int Space_Index = Due_Balance_Text.IndexOf(" ");
                string Due_Currency = Due_Balance_Text.Substring(0, Space_Index);
                Due_Balance_Text = Due_Balance_Text.Replace(Due_Currency, "");

                double Due_Balance =double.Parse(Due_Balance_Text.Trim());
                SAPbouiCOM.CheckBox Ckb_Selected = (SAPbouiCOM.CheckBox)Mat_Payment_Lines.GetCellSpecific("10000127", i + 1);
                EditText Txt_Invoice_Payment = (EditText)Mat_Payment_Lines.GetCellSpecific("24", i + 1);

                Ckb_Selected.Checked=true;
                Txt_Invoice_Payment.Value = Math.Min(Rest_Amount, Due_Balance).ToString();

                Rest_Amount -= Due_Balance;
                Has_Selected_Invoice = true;
            }
            if (Has_Selected_Invoice == false)
            {
                Frm_Payment.Items.Item("37").Click();
                ((EditText)Frm_Payment.Items.Item("13").Specific).Value = Payment_Amount.ToString();
            }


            Frm_Payment.Items.Item("5").Enabled = false;
           // Frm_Payment.Items.Item("10").Enabled = false;
            Frm_Payment.Items.Item("37").Enabled = false;
            UDF_Form.Items.Item("U_ST_MEMBERSHIP_CODE").Enabled = false;

            string Aramex_Code = Configurations.Get_Aramex_Payment_Method_Code(company);
            string OnLine_Code = Configurations.Get_OnLine_Payment_Method_Code(company);
            string Smart_Line_Code = Configurations.Get_Smart_Link_Payment_Method_Code(company);

            string Current_Payment_Metho = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PAYMENT_METHOD", 0);
            if (!(new string[] { Aramex_Code, OnLine_Code, Smart_Line_Code}).Contains(Current_Payment_Metho))
            {
                Frm_Payment.Items.Item("10").Enabled = false;
            }
            //return;
            //Frm_Payment.Items.Item("5").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            //Frm_Payment.Items.Item("37").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            //UDF_Form.Items.Item("U_ST_MEMBERSHIP_CODE").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            ////System_Forms.Display_Payment_Form(SBO_Application, company, CardCode, Card_Number, Frm_Payment, Payment_Type, Client_Number);
            ///

        }

        private static void Check_Incomplete(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Member_Card = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);

            string SQL_Invoice = $@"Select T0.""DocStatus"" From OINV T0 where  T0.""CANCELED"" = 'N' And T0.""U_ST_MEMBERSHIP_CODE""='{Membership_Code}'";
            Recordset RC_Invoice = Helper.Utility.Execute_Recordset_Query(company, SQL_Invoice);
            if (RC_Invoice.RecordCount == 0)
            {
                SQL_Invoice = $@"Select T0.""DocStatus"" From ODPI T0 where  T0.""CANCELED"" = 'N' And T0.""U_ST_MEMBERSHIP_CODE""='{Membership_Code}'";
                RC_Invoice = Helper.Utility.Execute_Recordset_Query(company, SQL_Invoice);
            }
            if (RC_Invoice.RecordCount > 0)
            {
                for (int i = 0; i < RC_Invoice.RecordCount; i++)
                {
                    string Doc_Status = RC_Invoice.Fields.Item("DocStatus").Value.ToString();
                    if (Doc_Status == "O")
                    {
                        UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                        Field_Data Fld_Date = new Field_Data() { Field_Name = "U_ST_INCOMPLETE", Value = "Y" };
                        Utility.Update_UDO(company, UDO_Info, Membership_Code, new Field_Data[] { Fld_Date });
                        break;
                    }
                }
            }
           
        }

        private static void Create_Payment_Validation(Form form, string Membership_Code, string Member_Card)
        {
            //string SQL_MemberCard = $@"SELECT T0.U_ST_IS_ORPHANS, T0.U_ST_PARENT_ID, T0.U_ST_PARENT_TYPE FROM ""@ST_CCI_INDIV_CARD""  T0 WHERE T0.""Code"" = '{Member_Card}'";
            //Recordset RC_MemberCard = Helper.Utility.Execute_Recordset_Query(company, SQL_MemberCard);
            //if (RC_MemberCard.Fields.Item("U_ST_IS_ORPHANS").Value.ToString() == "Y")
            //{
            //    throw new Exception("We can't add a payment for Orphans Members");
            //}
            //string KHCF_MemberCard_Code = Configurations.Get_KHCF_MemberCard(company, false);
            //if (RC_MemberCard.Fields.Item("U_ST_PARENT_TYPE").Value.ToString() == "C" && RC_MemberCard.Fields.Item("U_ST_PARENT_ID").Value.ToString() == KHCF_MemberCard_Code)
            //{
            //    throw new Exception("We can't add a payment for KHCF Members");
            //}

            //string SQL_Membership = $@"SELECT T0.U_ST_PAYMENT_METHOD FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""Code"" = '{Membership_Code}'";
            //Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            //string[] Gifts_Codes = new string[] { "018", "019" };
            //if (Gifts_Codes.Contains(RC_Membership.Fields.Item("U_ST_PAYMENT_METHOD").Value.ToString()))
            //{
            //    throw new Exception("We can't add a payment for Gift Memberships");
            //}

            string Payment_Form = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_OTHER_PAYMENT_FORM", 0);
            string Payment_Text = "";
            switch (Payment_Form)
            {
                case "E":
                    Payment_Text = "Employees Salary Deduction";
                    break;
                case "G":
                    Payment_Text = "Gift";
                    break;
                case "O":
                    Payment_Text = "Orphans";
                    break;
                default:
                    break;
            }
            if (Payment_Text != "")
            {
                throw new Exception($"We can't add a payment for {Payment_Text} Memberships");
            }

        }

        internal static void Create_Invoice(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.ActiveForm;
            
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("Invoice can be created only in OK mode.");
            }

            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string SQL_Membership = $@"Select T0.""U_ST_INVOICE_NUMBER"" From ""@ST_INDIV_MEMBERSHIP"" T0 
 WHERE T0.""Code"" = '{Membership_Code}'  ";
            Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            if(RC_Membership.RecordCount > 0)
            {
                string invoiceNum = RC_Membership.Fields.Item(0).Value.ToString();
                if(string.IsNullOrEmpty(invoiceNum))
                {
                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                    bool Is_One_Installment = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INSTALLMENT_TYPE", 0) == "O";
                    int Payment_Terms = Convert.ToInt32(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INSTALLMENT_TYPE", 0));
                    string Currency = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CURRENCY", 0);
                    Inoice_Data Inv_Data = Update_Premium(company, Membership_Code, UDO_Info, Payment_Terms, Currency, Is_One_Installment);

                    string type = string.Empty;
                    int NewEntry = Membership.Create_Invoice(company, Inv_Data, UDO_Info, out type);
                    SBO_Application.StatusBar.SetText($"New {type}[{NewEntry}] has been created", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
            }

            SBO_Application.ActivateMenuItem("1304");
        }

        private static void Member_Card_Choose_From_List(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            if (Code == "")
            {
                return;
            }

            Form form =  SBO_Application.Forms.Item(pVal.FormUID);
            Select_MemberCard(Code, form);
        }

        private static void Parent_Membership_Choose_From_List(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            if (Code == "")
            {
                return;
            }
            Form form = SBO_Application.Forms.ActiveForm;
        }

        private static void Select_MemberCard(string MemberCard_Code, SAPbouiCOM.Form form)
        {
            try
            {
                form.Freeze(true);
                string SQL_Card = $@"SELECT T0.U_ST_DATE_OF_BIRTH, T0.U_ST_CHANNEL, T0.U_ST_SUB_CHANNEL ,T0.U_ST_FULL_NAME_AR
, U_ST_CURRENCY,U_ST_ACCOUNT_MANAGER, U_ST_PARENT_TYPE, U_ST_PARENT_ID
FROM ""@ST_CCI_INDIV_CARD""  T0 
WHERE T0.""Code"" = '{MemberCard_Code}'";
                Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);
                //var X = form.DataSources.DBDataSources.Item(0).TableName+ form.Title;
                string StartDate_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_START_DATE", 0);
                if (StartDate_Text != "")
                {
                    DateTime StartDate = DateTime.ParseExact(StartDate_Text, "yyyyMMdd", null);
                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_END_DATE", 0, StartDate.AddYears(1).ToString("yyyyMMdd"));

                    int Age = (StartDate - ((DateTime)RC_Card.Fields.Item("U_ST_DATE_OF_BIRTH").Value)).Days / 365;
                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_AGE", 0, Age.ToString());
                }

                string CCI_Department_ID = Configurations.Get_CCI_Department(company);
                string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" = {CCI_Department_ID} And T0.""SlpCode""='{RC_Card.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString()}'";
                Recordset RC_Manager = Helper.Utility.Execute_Recordset_Query(company, SQL_Account_Manager);

                string SQL_Status = $@"SELECT TOP 1 ""Code"", ""U_ST_MEMBERSHIP_STATUS"",""U_ST_START_DATE"",""U_ST_END_DATE"",""U_ST_WAITING_PERIOD"",""U_ST_WAITING_PERIOD_DATE"",""U_ST_EMPLOYEE_ID"",
""U_ST_DEPARTMENT"" ,""U_ST_BRANCH"",""U_ST_CLASS"",""U_ST_SECTION"",""U_ST_DISCOUNT_PERCENTAGE"", ""U_ST_OTHER_PAYMENT_FORM"",""U_ST_STUDENT_FINANCIAL_NO"" , ""U_ST_STOP_DATE"",
""U_ST_PAYMENT_METHOD"", ""U_ST_COVERAGE"", ""U_ST_INSTALLMENT_TYPE"", ""U_ST_DISCOUNT_VALUE"",""U_ST_APPROVAL_STATUS"",""U_ST_CHANNEL"",""U_ST_SUB_CHANNEL"" FROM ""@ST_INDIV_MEMBERSHIP"" 
WHERE ""U_ST_MEMBER_CARD""='{MemberCard_Code}' AND ""U_ST_MEMBERSHIP_STATUS"" IN ('N','R','P','S') ORDER BY ""U_ST_START_DATE"" DESC ";
                Recordset RC_Status = Helper.Utility.Execute_Recordset_Query(company, SQL_Status);

                if (RC_Status.RecordCount == 0)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBERSHIP_STATUS", 0, "N");
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CHANNEL", 0, RC_Card.Fields.Item("U_ST_CHANNEL").Value.ToString());
                    Form_Obj.Load_One_Depends_Parent_Item(form, "139");
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SUB_CHANNEL", 0, RC_Card.Fields.Item("U_ST_SUB_CHANNEL").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_NAME", 0, RC_Card.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, RC_Card.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString());

                    #region currency manipulation

                    string memberCurrency = RC_Card.Fields.Item("U_ST_CURRENCY").Value.ToString();
                    string currency = string.Empty;
                    if (memberCurrency == "##")
                        currency = "JOD";
                    else
                        currency = memberCurrency;
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CURRENCY", 0, currency);

                    #endregion

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
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_CARD", 0, MemberCard_Code);
                    
                    string StartDate_T = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_START_DATE", 0).ToString();
                    if (StartDate_T != "")
                    {
                        int Age = (StartDate - ((DateTime)RC_Card.Fields.Item("U_ST_DATE_OF_BIRTH").Value)).Days / 365;
                        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_AGE", 0, Age.ToString());
                    }
                    
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CHANNEL", 0, RC_Status.Fields.Item("U_ST_CHANNEL").Value.ToString());
                    Form_Obj.Load_One_Depends_Parent_Item(form, "139");
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SUB_CHANNEL", 0, RC_Status.Fields.Item("U_ST_SUB_CHANNEL").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_NAME", 0, RC_Card.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, RC_Card.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_EMPLOYEE_ID", 0, RC_Status.Fields.Item("U_ST_EMPLOYEE_ID").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DEPARTMENT", 0, RC_Status.Fields.Item("U_ST_DEPARTMENT").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BRANCH", 0, RC_Status.Fields.Item("U_ST_BRANCH").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CLASS", 0, RC_Status.Fields.Item("U_ST_CLASS").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SECTION", 0, RC_Status.Fields.Item("U_ST_SECTION").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DISCOUNT_PERCENTAGE", 0, RC_Status.Fields.Item("U_ST_DISCOUNT_PERCENTAGE").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_OTHER_PAYMENT_FORM", 0, RC_Status.Fields.Item("U_ST_OTHER_PAYMENT_FORM").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_STUDENT_FINANCIAL_NO", 0, RC_Status.Fields.Item("U_ST_STUDENT_FINANCIAL_NO").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PAYMENT_METHOD", 0, RC_Status.Fields.Item("U_ST_PAYMENT_METHOD").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COVERAGE", 0, RC_Status.Fields.Item("U_ST_COVERAGE").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_INSTALLMENT_TYPE", 0, RC_Status.Fields.Item("U_ST_INSTALLMENT_TYPE").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DISCOUNT_VALUE", 0, RC_Status.Fields.Item("U_ST_DISCOUNT_VALUE").Value.ToString());
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DISCOUNT_VALUE", 0, RC_Status.Fields.Item("U_ST_DISCOUNT_VALUE").Value.ToString());
                    //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_MEMBERSHIP_ID", 0, RC_Status.Fields.Item("Code").Value.ToString());
                    //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_MEMBERSHIP_TYPE", 0, "I");
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "A");
                }
                if (RC_Manager.RecordCount > 0)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, RC_Manager.Fields.Item("Name").Value.ToString());
                }

                if (RC_Card.Fields.Item("U_ST_CURRENCY").Value.ToString() == "##")
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CURRENCY", 0, "JOD");
                    form.Items.Item("161").Enabled = true;
                }
                else
                {
                    string SQL_Currency = $@"SELECT T0.""CurrCode"" AS ""Code"",T0.""CurrCode"" AS ""Name"" FROM OCRN T0 Where T0.""CurrCode""='{RC_Card.Fields.Item("U_ST_CURRENCY").Value.ToString()}'";
                    Recordset RC_Currency = Helper.Utility.Execute_Recordset_Query(company, SQL_Currency);
                    if (RC_Manager.RecordCount > 0)
                    {

                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CURRENCY", 0, RC_Currency.Fields.Item("Name").Value.ToString());
                        form.Items.Item("161").Enabled = false;
                    }
                }

                string Parent_Type = RC_Card.Fields.Item("U_ST_PARENT_TYPE").Value.ToString();
                string Parent_ID = RC_Card.Fields.Item("U_ST_PARENT_ID").Value.ToString();
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PARENT_MEMBERSHIP_TYPE", 0, Parent_Type);
                UDO_Definition UDO_Parent_Info;
                if (Parent_Type == "C")
                {
                    UDO_Parent_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);
                }
                else
                {
                    UDO_Parent_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                }

                string Parent_Membership_ID = Utility.Get_Last_Individual_Membership_Per_Card(company, Parent_ID, UDO_Parent_Info, false);
                if (Parent_Membership_ID == "")
                {
                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PARENT_MEMBERSHIP_TYPE", 0, "N");
                }
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PARENT_MEMBERSHIP_ID", 0, Parent_Membership_ID);

                var Customer_Group = Utility.Get_Member_Card_Customer_Group(company, MemberCard_Code);
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_CUSTOMER_GROUP", 0, Customer_Group.Code_Name);

            }
            finally
            {
                form.Freeze(false);
            }
        }

        private static void Calculate_Premium(ItemEvent businessObjectInfo)
        {
            Form form = Loader.SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Exception("We can calculate the Premium in the OK mode only");
            }
            string status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS", 0).ToString();
            if(status == "C" || status == "S")
                throw new Custom_Exception("Cannot Calculate Premium. Membership is in Cancel or Stop status.");
            if (form.Mode == BoFormMode.fm_ADD_MODE)
                throw new Custom_Exception("Cannot Calculate Premium in Add Mode.");

            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            Premium_Data Premium = Membership.Calculate_Premium(company, Membership_Code);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PREMIUM", 0, Premium.Premium_Amount.ToString("N03"));
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DISCOUNT_PERCENTAGE", 0, Premium.Discount_Percentage.ToString());
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DISCOUNT_VALUE", 0, Premium.Discount_Value.ToString());
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WAITING_PERIOD", 0, Premium.Waiting_Period.ToString());
            string startDateString = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_START_DATE", 0);
            if (!string.IsNullOrEmpty(startDateString))
            {
                if (Premium.Waiting_Period != 0)
                {
                    startDateString = startDateString.Insert(4, "/");
                    startDateString = startDateString.Insert(7, "/");
                    DateTime startDate = Convert.ToDateTime(startDateString);
                    DateTime waitingPeriodDate = startDate.AddMonths(Premium.Waiting_Period);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WAITING_PERIOD_DATE", 0, waitingPeriodDate.ToString("yyyyMMdd"));
                }
                else
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WAITING_PERIOD_DATE", 0, "");
                }
            }

            //Calculate Child Premium and total premuim of Parent
            double Total_Premium = Premium.Premium_Amount;
            double Total_Discount = Premium.Discount_Value;
            //string SQL_Child = $@"Select T0.""Code"" From ""@ST_INDIV_MEMBERSHIP"" T0 Where T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Membership_Code}'";
            string SQL_Child = Membership.SQL_Parent_Memberships(Membership_Code, "I", string.Empty, string.Empty);
            Recordset RC_Child = Helper.Utility.Execute_Recordset_Query(company, SQL_Child);
            if (RC_Child.RecordCount > 0)
            {
                for (int i = 0; i < RC_Child.RecordCount; i++)
                {
                    string Child_Code = RC_Child.Fields.Item(0).Value.ToString();
                    Premium_Data Premium_Child = Membership.Calculate_Premium(company, Child_Code);
                    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_PREMIUM", Value = Premium_Child.Premium_Amount.ToString("N03") };
                    Field_Data Fld1 = new Field_Data() { Field_Name = "U_ST_DISCOUNT_PERCENTAGE", Value = Premium_Child.Discount_Percentage.ToString() };
                    Field_Data Fld2 = new Field_Data() { Field_Name = "U_ST_DISCOUNT_VALUE", Value = Premium_Child.Discount_Value.ToString() };
                    Field_Data Fld3 = new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Premium_Child.Waiting_Period.ToString() };

                    Utility.Update_UDO(company, Form_Obj.UDO_Info, Child_Code, new Field_Data[] { Fld, Fld1, Fld2, Fld3 });

                    Total_Premium += Premium_Child.Premium_Amount;
                    Total_Discount += Premium_Child.Discount_Value;
                    RC_Child.MoveNext();
                }
            }
            form.DataSources.UserDataSources.Item("225").Value = Total_Premium.ToString("N03");
            form.DataSources.UserDataSources.Item("226").Value = Total_Discount.ToString("N03");
            string Payment_Method = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INSTALLMENT_TYPE", 0);
            string Cash_Code = Configurations.Get_Cash_Payment_Method_Code(company, true);
            //if (Payment_Method == Cash_Code)
            //{
            //    form.DataSources.UserDataSources.Item("198").Value = Total_Premium.ToString("N03");
            //}
            form.Mode = BoFormMode.fm_UPDATE_MODE;
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
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_CCI_IND_SHP_ATT");
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

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            //form.Freeze(false);

            //return;

            DS_Attachment.SetValue("U_ST_FILE_NAME", Count, BF.FileName);
            if (form.Mode == BoFormMode.fm_OK_MODE && Count !=0)
            {
                int Line_id = Convert.ToInt32(DS_Attachment.GetValue("LineId", Count - 1).ToString())+1;
                DS_Attachment.SetValue("LineId", Count, Line_id.ToString());
            }
            else
                DS_Attachment.SetValue("LineId", Count, (Count + 1).ToString());

            //DS_Address.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);
            //if (form.Mode == BoFormMode.fm_OK_MODE)
            //{
            //    string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //    Update_Attachment_Matrix(form, UDO_Code,"Update");
            //    Matrix Mat = (Matrix)form.Items.Item("500").Specific;

            //    Mat.FlushToDataSource();
            //    Mat.LoadFromDataSource();
            //    Mat_Add.AutoResizeColumns();
            //}

           


        }

        private static void Remove_Attachment(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
            Form_Obj.Remove_Matrix_Row(form, "500");
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

            //if (form.Mode == BoFormMode.fm_OK_MODE)
            //{
            //    string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //    Update_Attachment_Matrix(form, UDO_Code, "Remove");

            //    Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            //    Mat.FlushToDataSource();
            //    Mat.LoadFromDataSource();
            //    Mat.AutoResizeColumns();
            //}



        }

        private static void Open_Attachment(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;


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

        public static void Create_Child_Memberships(BusinessObjectInfo businessObjectInfo,string Parent_Type)
        {
            if (businessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
            {
                Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

                string UDO_Member_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
                string UDO_Parent_Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                string Status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS", 0); //U_ST_MEMBERSHIP_STATUS

                if (Status == "N")
                {
                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                    string SQL_Children = $@"select T0.""Code"" From ""@ST_CCI_INDIV_CARD"" T0 WHERE T0.""U_ST_PARENT_ID""='{UDO_Member_Code}' AND T0.""U_ST_PARENT_TYPE"" = '{Parent_Type}' ";
                    Recordset RC_Child = Helper.Utility.Execute_Recordset_Query(company, SQL_Children);
                    for (int i = 0; i < RC_Child.RecordCount; i++)
                    {
                        int x = RC_Child.RecordCount;
                        Membership.Create_Child_MemberShip(company, UDO_Member_Code, UDO_Info, UDO_Parent_Membership_Code, Parent_Type,RC_Child.Fields.Item(0).Value.ToString());
                        RC_Child.MoveNext();
                    }
                }
            }
        }

        private static void Create_Child_Invoice(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.ActiveForm;
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string SQL_Child = $@"Select T0.""Code"" From ""@ST_INDIV_MEMBERSHIP"" T0 
 WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{Code}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'I' 
 AND ( T0.""U_ST_INVOICE_NUMBER"" IS NULL OR T0.""U_ST_INVOICE_NUMBER"" = '' ) ";
            Recordset RC_Child = Helper.Utility.Execute_Recordset_Query(company,SQL_Child);
            if (RC_Child.RecordCount > 0)
            {
                for (int i = 0; i < RC_Child.RecordCount; i++)
                {
                    if (form.Mode != BoFormMode.fm_OK_MODE)
                    {
                        throw new Logic.Custom_Exception("Invoice can be created ONLY in OK mode.");
                    }
                    Inoice_Data Inv_Data = new Inoice_Data();
                    string Membership_Code = RC_Child.Fields.Item(0).Value.ToString();
                    Premium_Data Premium = Membership.Calculate_Premium(company, Membership_Code);
                    Inv_Data.Source_Code = Membership_Code;
                    Inv_Data.Premium_Amount = Premium.Premium_Amount;
                    Inv_Data.Discount_Value = Premium.Discount_Value;
                    Inv_Data.Discount_Percentage = Premium.Discount_Percentage;
                    Inv_Data.Waiting_Period = Premium.Waiting_Period;
                    Inv_Data.Is_One_Installment = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INSTALLMENT_TYPE", 0) == "O";
                    Inv_Data.Payment_Terms = Convert.ToInt32(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INSTALLMENT_TYPE", 0));
                    Inv_Data.Currency = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CURRENCY", 0);


                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                    List<Field_Data> Premium_Field_Data = new List<Field_Data>();
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_PREMIUM", Value = Premium.Premium_Amount });
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_PERCENTAGE", Value = Premium.Discount_Percentage });
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_VALUE", Value = Premium.Discount_Value });
                    Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Premium.Waiting_Period });
                    Utility.Update_UDO(company, UDO_Info, Membership_Code, Premium_Field_Data.ToArray());
                    string type = string.Empty;
                    int NewEntry = Membership.Create_Invoice(company, Inv_Data, UDO_Info, out type);

                    SBO_Application.StatusBar.SetText($"New {type}[{NewEntry}] has been created for Membership [{Code}]", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                    RC_Child.MoveNext();
                }
            }
            else
            {
                throw new Logic.Custom_Exception("No available memberships to generate invoices to.");
            }
        }

        internal static void Update_Attachment_Matrix(SAPbouiCOM.Form form,string UDO_Code,string Case)
        {
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_CCI_IND_SHP_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("500").Specific;
            int Count = DS_Attachment.Size;
            UserTable UDT_Rel = company.UserTables.Item("ST_CCI_IND_SHP_ATT");
            string SQL_Att = $@"SELECT *  FROM ""@ST_CCI_IND_SHP_ATT""  T0 INNER JOIN  ""@ST_INDIV_MEMBERSHIP""  T1 on T0.""Code""= '{UDO_Code}'";
            UDO_Definition UDO_Info_Att = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Individual_Membership_Attachment);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
            
            Field_Definition[] Address_Fields = Logic.Fields_Logic.All_Field_Definition.Where(U => U.KHCF_Object == KHCF_Objects.CCI_Individual_Membership_Attachment).ToArray();
           
            CompanyService oCmpSrv = company.GetCompanyService();
            GeneralService oGeneralService = oCmpSrv.GetGeneralService(Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
            GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
            oGeneralParams.SetProperty("Code", UDO_Code);
            GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
            SAPbobsCOM.GeneralDataCollection Attatchment_Children = oGeneralData.Child("ST_CCI_IND_SHP_ATT");

            for (int J = Count -1; J < form.DataSources.DBDataSources.Item("@ST_CCI_IND_SHP_ATT").Size; J++)
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

                        oChild.SetProperty(One_Add_Field.Column_Name_In_DB, form.DataSources.DBDataSources.Item("@ST_CCI_IND_SHP_ATT").GetValue(One_Add_Field.Column_Name_In_DB, J));
                    }
                }  
            }
            oGeneralService.Update(oGeneralData);
        }

        internal static void Fill_ComboButton_Values(Form form,string Membership_Type)
        {
            string Status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBERSHIP_STATUS", 0);
            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("19").Specific;

            int Count = Btn_Cmb_Warrning.ValidValues.Count;
            for (int i = 0; i < Count; i++)
            {
                Btn_Cmb_Warrning.ValidValues.Remove(Btn_Cmb_Warrning.ValidValues.Count - 1, SAPbouiCOM.BoSearchKey.psk_Index);
            }

            Btn_Cmb_Warrning.ValidValues.Add("-", "Can Also");
            //Btn_Cmb_Warrning.ValidValues.Add("RE", "Renew");
            if(Status == "P")
             Btn_Cmb_Warrning.ValidValues.Add("P", "Convert Past to Renew");
            Btn_Cmb_Warrning.ValidValues.Add("S", "Stop");
            //Btn_Cmb_Warrning.ValidValues.Add("L", "Close");
            Btn_Cmb_Warrning.ValidValues.Add("C", "Cancel");
            Btn_Cmb_Warrning.ValidValues.Add("R", "Remove");

            form.DataSources.UserDataSources.Item("19").Value = "-";
        }

        private static void Load_Memberships(SAPbouiCOM.Form form, string Card_ID)
        {
            DataTable DT_Membership = form.DataSources.DataTables.Item("MEMBERSHIP");
            DT_Membership.Rows.Clear();
            string SQL_Membership = $@"SELECT T0.""Code"", T0.""U_ST_MEMBER_CARD"", T1.""U_ST_FULL_NAME_AR"",T0.""U_ST_MEMBERSHIP_STATUS"",T0.""U_ST_CREATION_DATE""
, T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE,T0.U_ST_AGE,T0.U_ST_PREMIUM,T1.U_ST_CUSTOMER_GROUP, 
T0.""U_ST_PARENT_MEMBERSHIP_ID"" as ""ParentID"", (select T2.""U_ST_MEMBER_NAME"" from ""@ST_INDIV_MEMBERSHIP"" T2 WHERE T2.""Code""='{Card_ID}') as ""ParentName"" , 'I' as ""ParentType""
FROM ""@ST_INDIV_MEMBERSHIP""  T0
JOIN ""@ST_CCI_INDIV_CARD"" T1 ON T1.""Code"" = T0.""U_ST_MEMBER_CARD""
WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" ='{Card_ID}' And T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'I' ";
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

    }
}
