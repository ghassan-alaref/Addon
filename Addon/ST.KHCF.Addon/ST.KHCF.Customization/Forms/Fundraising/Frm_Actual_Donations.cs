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

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Actual_Donations : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Relations_Grid_IDs = new string[] { "72" };

        internal override bool Get_Is_Approval_Status(Form form)
        {
            return form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FINANCE_CONFIRMATION", 0) == "Y";
        }
        
        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);
            Initializ_Actual_Form(form);          
        }

        internal static void Initializ_Actual_Form(Form form)
        {
            string fundDepartments = Utility.Get_Configuration(company, "Fundraising_Department");
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" WHERE T1.""dept"" in ({fundDepartments})";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "36", SQL_Account_Manager, true);

            string SQL_Cost_Center = $@"SELECT T0.""PrcCode"" AS ""Code"", T0.""PrcName"" AS ""Name"" FROM OPRC T0 ";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "32", SQL_Cost_Center, true);

            //string SQL_PaymentTerms = $@"SELECT T0.""GroupNum"" AS ""Code"",T0.""PymntGroup"" AS ""Name"" FROM OCTG T0";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "95", SQL_PaymentTerms, true);

            string SQL_Booth = $@"SELECT T0.""empID"" AS ""Code"", CONCAT(T0.""firstName"",T0.""lastName"") AS ""Name"" FROM OHEM T0 WHERE T0.""dept"" = '3'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "77", SQL_Booth, true);

            //SAPbouiCOM.ChooseFromList CFL_Orphans = form.ChooseFromLists.Item("CFL_Orphans");
            //Conditions Orphans_Cons = CFL_Orphans.GetConditions();
            //Condition Orphans_Con = Orphans_Cons.Add();
            //Orphans_Con.Alias = "CardType";
            //Orphans_Con.CondVal = "L";
            //Orphans_Con.Operation = BoConditionOperation.co_EQUAL;
            //Orphans_Con.Relationship = BoConditionRelationship.cr_AND;
            //Orphans_Con = Orphans_Cons.Add();f
            //Orphans_Con.Alias = "U_ST_CUSTOMER_TYPE";
            //Orphans_Con.CondVal = "C";
            //Orphans_Con.Operation = BoConditionOperation.co_EQUAL;

            // CFL_Orphans.SetConditions(Orphans_Cons);

            SAPbouiCOM.ChooseFromList CFL_Card = form.ChooseFromLists.Item("CFL_Corp");
            Conditions Card_Cons = CFL_Card.GetConditions();
            Condition Card_Con = Card_Cons.Add();
            Card_Con.Alias = "U_ST_IS_DONOR";
            Card_Con.CondVal = "Y";
            Card_Con.Operation = BoConditionOperation.co_EQUAL;
            Card_Con.Relationship = BoConditionRelationship.cr_AND;
            Card_Con = Card_Cons.Add();
            Card_Con.Alias = "CreateDate";
            Card_Con.Operation = BoConditionOperation.co_NOT_NULL;
            CFL_Card.SetConditions(Card_Cons);

            //SAPbouiCOM.ChooseFromList CFL_INDIV_Card = form.ChooseFromLists.Item("CFL_Corp");
            //Conditions Card_I_Cons = CFL_INDIV_Card.GetConditions();
            //Condition Card_I_Con = Card_I_Cons.Add();
            //Card_I_Con.Alias = "U_ST_CREATION_DATE";
            //Card_I_Con.Operation = BoConditionOperation.co_NOT_NULL;

            //CFL_INDIV_Card.SetConditions(Card_I_Cons);


            SAPbouiCOM.ChooseFromList CFL_Ambassador = form.ChooseFromLists.Item("CFL_Ambassador");
            Conditions Ambassador_Cons = CFL_Ambassador.GetConditions();
            Condition Ambassador_Con = Ambassador_Cons.Add();
            Ambassador_Con.Alias = "U_ST_DONOR_ADD_UPDATE";
            Ambassador_Con.CondVal = "Y";
            Ambassador_Con.Operation = BoConditionOperation.co_EQUAL;
            Ambassador_Con.Relationship = BoConditionRelationship.cr_AND;
            Ambassador_Con = Ambassador_Cons.Add();
            Ambassador_Con.Alias = "U_ST_CREATION_DATE";
            Ambassador_Con.Operation = BoConditionOperation.co_NOT_NULL;
            //Condition Ambassador_Con2 = Ambassador_Cons.Add();
            //Ambassador_Con2.Alias = "U_ST_AMBASSADOR_DATE";
            //Ambassador_Con2.Operation = BoConditionOperation.co_NOT_NULL;

            CFL_Ambassador.SetConditions(Ambassador_Cons);

            SAPbouiCOM.ChooseFromList CFL_Donor = form.ChooseFromLists.Item("CFL_DONOR");
            Conditions Donor_Cons = CFL_Donor.GetConditions();
            Condition Donor_Con = Donor_Cons.Add();
            Donor_Con.Alias = "CardCode";
            Donor_Con.CondVal = "DON";
            Donor_Con.Operation = BoConditionOperation.co_START;
            CFL_Donor.SetConditions(Donor_Cons);

            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);
            ((Grid)form.Items.Item("72").Specific).Columns.Item("Code").Visible = false;

            string canApproveSQL = $@"SELECT T0.""U_ST_CAN_CONFIRM_EXPECTED_DONATION"" FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
            Recordset approveRC = Helper.Utility.Execute_Recordset_Query(company, canApproveSQL);
            if (approveRC.Fields.Item("U_ST_CAN_CONFIRM_EXPECTED_DONATION").Value.ToString() == "Y")
            {
                form.Items.Item("42").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
            }
            else
            {
                form.Items.Item("42").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

            }

            form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            //form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);

            form.Items.Item("49").Click();
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
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    //form.Items.Item("50").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("51").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("60").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("61").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("101").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("102").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("201").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("202").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                    form.Items.Item("71").Enabled = false;
                    //form.Items.Item("Item_23").Enabled = false;
                }
                else
                {
                    form.Items.Item("71").Enabled = true;
                    //form.Items.Item("Item_23").Enabled = true;
                }
                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
                }

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    ADD_Update_UDO(BusinessObjectInfo);
                }


                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }

                if (!BusinessObjectInfo.BeforeAction)
                {
                    //Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                    UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                    if (Form_Obj.Set_ReadOnly(form, KHCF_Object))
                    {
                        // return;
                    }

                }

                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
            }
            catch (Exception ex)
            {
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;

            Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            string Target_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TARGET", 0);
            DateTime Don_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_DATE", 0).ToString(), "yyyyMMdd", null);
            if (!string.IsNullOrEmpty(Target_ID))
            {
                Update_Machine_Status(Target_ID,Don_Date);
                Update_Area_Status(Target_ID,form);
            }
            string Exp_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_EXP_ID", 0);
            if (!string.IsNullOrEmpty(Exp_Code))
            {
                string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                Check_Expected_Lines(Exp_Code, Code);
            }
        }

        internal static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            //Load_Orphans(form, "@ST_FUND_EXP_DON_ORP");
            //Load_Patients(form, "@ST_FUND_EXP_DON_PAT");
            //Load_Naming(form, "@ST_FUND_EXP_DON_NAM");
            //Load_Machinery(form, "@ST_FUND_EXP_DON_MAC");

            //Load_Activities(form, Card_ID, Form_Obj);

            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            string invoiceSQL = $@"SELECT T0.""DocEntry"" FROM OINV T0 WHERE T0.""U_ST_ACTUAL_DONATION_CODE"" = '{UDO_Code}'";
            Recordset RC_Invoice = Helper.Utility.Execute_Recordset_Query(company, invoiceSQL);
            if (RC_Invoice.RecordCount > 0)
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_INVOICE_NUMBER", 0, RC_Invoice.Fields.Item("DocEntry").Value.ToString());
            }

            string Invoice_Entry = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0);
            if (!string.IsNullOrEmpty(Invoice_Entry))
            {
                //form.Items.Item("71").Enabled = false;
                //form.Items.Item("Item_23").Enabled = false;
            }
            else
            {
                //form.Items.Item("71").Enabled = true;
                //form.Items.Item("Item_23").Enabled = true;
            }
            if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_TARGET", 0)))
            {
                form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);
            }
            else
            {
                form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_False);
            }

            Form_Obj.Set_Fields(form);

        }

        internal static void Load_Activities(Form form, string Card_ID, Parent_Form Form_Obj_Data)
        {
            DataTable DT_Activities = form.DataSources.DataTables.Item("DT_ACTIVITIES");
            DT_Activities.Rows.Clear();
            string SQL_Activities = $@"SELECT T0.""ClgCode"", T0.""Recontact"", T0.""Details"", T0.""Duration"", T0.""Closed"", T0.""Notes"" 
FROM OCLG T0 WHERE T0.U_ST_KHCF_OBJECT_CODE = '{Card_ID}' AND  T0.U_ST_KHCF_OBJECT_TYPE = '{((int)Form_Obj_Data.KHCF_Object).ToString()}'";
            Recordset RC_Activitiesp = Helper.Utility.Execute_Recordset_Query(company, SQL_Activities);
            DT_Activities.Rows.Add(RC_Activitiesp.RecordCount);

            for (int i = 0; i < RC_Activitiesp.RecordCount; i++)
            {
                for (int J = 0; J < DT_Activities.Columns.Count; J++)
                {
                    string Col_Name = DT_Activities.Columns.Item(J).Name;
                    string UDF_Name;

                    UDF_Name = Col_Name;

                    DT_Activities.SetValue(Col_Name, i, RC_Activitiesp.Fields.Item(UDF_Name).Value);
                }
                RC_Activitiesp.MoveNext();
            }
            Grid Grd_Activities = (Grid)form.Items.Item("56").Specific;

            Grd_Activities.AutoResizeColumns();
        }

        internal static void Load_Orphans(Form form, string Line_DataSource_Table)
        {
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");
            DT_Orphans_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
            DT_Orphans_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Orphan_Data(form, DT_Orphans_Details, i, Line_DataSource_Table);
            }

            ((Matrix)form.Items.Item("52").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("52").Specific).AutoResizeColumns();
        }

        internal static void Load_Patients(Form form, string Line_DataSource_Table)
        {
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Patients_Details");
            DT_Patients_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
            DT_Patients_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Patient_Data(form, DT_Patients_Details, i, Line_DataSource_Table);
            }

            ((Matrix)form.Items.Item("62").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("62").Specific).AutoResizeColumns();
        }

        internal static void Set_Orphan_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_ORPHANS_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""CardName"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID 
FROM OCRD T0 WHERE T0.""CardCode"" = '{Code}'

UNION ALL

SELECT T0.U_ST_FULL_NAME_AR, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
FROM ""@ST_CCI_INDIV_CARD""  T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Orphans_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Orphans_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_ORPHANS_CODE")
                {
                    DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Set_Default_Value_Before_Adding(form);
            }
        }

        internal static void Set_Default_Value_Before_Adding(Form form)
        {
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }

            SAPbouiCOM.DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            bool isEmpty = true;
            int count = 0;
            if (DT_Departments.Rows.Count > 0)
            {
                for (int i = 0; i < DT_Departments.Rows.Count; i++)
                {
                    if (DT_Departments.GetValue("SELECTED", i).ToString() == "Y")
                    {
                        isEmpty = false;
                        count++;
                    }
                }
            }

            if (isEmpty)
            {
                Loader.New_Msg = "please select department";
                return false;
            }
            if (count>1)
            {
                Loader.New_Msg = "Please select one department only";
                return false;
            }
            return true;
        }

        private static bool Validate_Amount(out string errorMsg)
        {
            errorMsg = string.Empty;
            Form form = SBO_Application.Forms.ActiveForm;
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;
            if (Mat.RowCount == 0)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0)))
            {
                double matrixAmount = 0;
                double donationAmount = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
                for (int i = 0; i < Mat.RowCount; i++)
                {
                    EditText amountET = (SAPbouiCOM.EditText)Mat.Columns.Item("");
                    if (!string.IsNullOrEmpty(amountET.Value))
                    {
                        matrixAmount += Convert.ToDouble(amountET.Value);
                    }
                }
                if (matrixAmount == donationAmount)
                {
                    return true;
                }
                else
                {
                    errorMsg = "Payment Schedule Amount does not Equal Donation Amount";
                    return false;
                }
            }
            else
            {
                errorMsg = "Donation Amount is not Filled";
                return false;
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
                //Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info);
                //form.DataSources.UserDataSources.Item("27").Value = "0";
                //form.DataSources.UserDataSources.Item("172").Value = "0";
                //string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

                //Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);
                DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
                Dictionary<string, string> Departments = new Dictionary<string, string>();
                Departments.Add("LID", "LID");
                Departments.Add("LCD", "LCD");
                Departments.Add("IDD", "IDD");
                Helper.Utility.FillGridForDictionary(DT_Departments, Departments);
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
                SBO_Application.Forms.ActiveForm.Items.Item("71").Enabled = true;
                Form form = SBO_Application.Forms.ActiveForm;
                if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_TYPE", 0) == "C")
                {
                    form.Items.Item("96").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);

                }
                //SBO_Application.Forms.ActiveForm.Items.Item("Item_23").Enabled = true;
            }
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                SBO_Application.Forms.ActiveForm.Items.Item("71").Enabled = false;
                //SBO_Application.Forms.ActiveForm.Items.Item("Item_23").Enabled = false;
            }

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
                if (pVal.ItemUID == "52" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Orphans_Choos_From_List(pVal, "@ST_FUND_EXP_DON_ORP");
                }
                if (pVal.ItemUID == "50" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Orphans_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "51" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Orphans_Line(pVal, "@ST_FUND_EXP_DON_ORP");
                }
                if (pVal.ItemUID == "57" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Activity(pVal);
                }
                if (pVal.ItemUID == "44" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Select_Donor(pVal);
                }
                if (pVal.ItemUID == "502" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Target_ID(pVal);
                }
                if (pVal.ItemUID == "505" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Activity_ID(pVal);
                }
                if (pVal.ItemUID == "85" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Trip(pVal);
                }
                if (pVal.ItemUID == "Item_29" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Check_Donor_CFL(pVal);
                    Choose_Exp_Donor(pVal);
                }
                if (pVal.ItemUID == "20" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Donation_Type(pVal);
                }
                if (pVal.ItemUID == "71" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    //Convert_To_Actual(pVal);
                    //Create_Invoice(pVal);
                    //
                     Create_Invoice_Payment(pVal);// Create_Payment(pVal); // 
                }
                if (pVal.ItemUID == "75" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Print(pVal);
                }

                if (pVal.ItemUID == "54" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Card_Type(pVal);
                }
                if (pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Card_ID(pVal);
                }
                if (pVal.ItemUID == "43" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Ambassador(pVal);
                }
                //if (pVal.ItemUID == "Item_23" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                //{
                //    Choose_Giving_Monthly(pVal);
                //}
                if (pVal.ItemUID == "45" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_General_Item_ID(pVal);
                }

                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Patient_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Patient_Line(pVal, "@ST_FUND_EXP_DON_PAT");
                }
                if (pVal.ItemUID == "62" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Patient_Choos_From_List(pVal, "@ST_FUND_EXP_DON_PAT");
                }

                if (pVal.ItemUID == "101" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Naming_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "102" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Naming_Line(pVal, "@ST_FUND_EXP_DON_NAM");
                }
                if (pVal.ItemUID == "103" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Naming_Choos_From_List(pVal, "@ST_FUND_EXP_DON_NAM");
                }

                if (pVal.ItemUID == "201" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Payment_Dets", "203");
                }
                if (pVal.ItemUID == "202" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_FUND_EXP_DON_PAY", "Payment_Dets", "203", "U_ST_DATE", true);
                }
                if (pVal.ItemUID == "203" && pVal.ColUID == "Activity" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Activity_choose_From_List(pVal, "@ST_FUND_EXP_DON_PAY");
                }
                if (pVal.ItemUID == "132" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Ambassador_Type(pVal);
                }
                if (pVal.ItemUID == "136" && (pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST || pVal.EventType == BoEventTypes.et_ITEM_PRESSED) && pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    string ambassador = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_AMBASSADOR", 0);
                    SAPbouiCOM.ChooseFromList CFL_Orphans = form.ChooseFromLists.Item("CFL_AmbAct");
                    Conditions Orphans_Cons = CFL_Orphans.GetConditions();
                    if (Orphans_Cons.Count > 0)
                    {
                        Orphans_Cons.Item(0).CondVal = ambassador;
                    }
                    else
                    {
                        Condition Orphans_Con = Orphans_Cons.Add();
                        Orphans_Con.Alias = "U_ST_CONTACT_CARD";
                        Orphans_Con.CondVal = ambassador;
                        Orphans_Con.Operation = BoConditionOperation.co_EQUAL;
                    }
                    CFL_Orphans.SetConditions(Orphans_Cons);
                }
                if (pVal.ItemUID == "136" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Ambassador_ID(pVal);
                }
                if (pVal.ItemUID == "95" && (pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST || pVal.EventType == BoEventTypes.et_ITEM_PRESSED) && pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    ComboBox typeCmb = (ComboBox)form.Items.Item("54").Specific;
                    EditText donorCodeET = (EditText)form.Items.Item("7").Specific;
                    string cardType = typeCmb.Selected.Value;
                    string card = donorCodeET.Value;
                    if (!string.IsNullOrEmpty(card))
                    {
                        SAPbouiCOM.ChooseFromList CFL_Recurring = form.ChooseFromLists.Item("CFL_Rec");
                        Conditions recurring_Cons = CFL_Recurring.GetConditions();
                        if (recurring_Cons.Count == 0)
                        {
                            Condition recurring_Con = recurring_Cons.Add();
                            recurring_Con.Alias = "U_ST_CONTACT_CARD";
                            recurring_Con.CondVal = card;
                            recurring_Con.Operation = BoConditionOperation.co_EQUAL;
                            recurring_Con.Relationship = BoConditionRelationship.cr_AND;
                            recurring_Con = recurring_Cons.Add();
                            recurring_Con.Alias = "U_ST_CONTACT_TYPE";
                            recurring_Con.CondVal = cardType;
                            recurring_Con.Operation = BoConditionOperation.co_EQUAL;
                        }
                        else
                        {
                            recurring_Cons.Item(0).CondVal = card;
                            recurring_Cons.Item(1).CondVal = cardType;
                        }

                        CFL_Recurring.SetConditions(recurring_Cons);
                    }
                }
                if (pVal.ItemUID == "95" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Recurring_Donation(pVal);

                }

                if (pVal.ItemUID == "1000" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    KHCF_Logic_Utility.Select_Allocation(pVal, Form_Obj, "502");
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        private static void Choose_Recurring_Donation(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RECURRING", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        private static void Choose_Ambassador_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AMBASSADOR_ACT", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        internal static void Selected_Donation_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_TYPE", 0) == "C")
            {
                form.Items.Item("96").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
                form.Items.Item("96").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);

            }
            else
            {
                ComboBox donationType = (ComboBox)form.Items.Item("96").Specific;
                donationType.Select("");
                form.Items.Item("96").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                form.Items.Item("96").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

            }

        }

        internal static void Print(ItemEvent pVal)
        {
            List<Helper.Utility.Crystal_Report_Parameter> Result = new List<Helper.Utility.Crystal_Report_Parameter>();
            string Rpt_File = Utility.Get_Configuration(company, "Expected_Donation_Print", "Expected Donation Print Path", "");
            string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(company, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Utility.Get_Configuration(company, "Report_Output_Folder_Path", "Report Output Folder Path", ""));
            SBO_Application.StatusBar.SetText("Report has been Created Successfully at " + Pdf_File_Name, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Success);
        }

        internal static void Patient_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("62").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Patients_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PATIENTS_CODE", Index, Code);
            Set_Patient_Data(form, DT_Patient_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Set_Patient_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PATIENTS_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_PATIENTS_CODE")
                {
                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }

                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

        internal static void Add_Patient_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Patients_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("62").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_PATIENTS_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Patients_Details.Rows.Count)
            {
                DT_Patients_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Patient_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Patients_Details");
            Matrix Mat = (Matrix)form.Items.Item("62").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Patient_Details.Rows.Remove(i);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Set_Naming_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_NAMING_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_AREA_NAME, T0.U_ST_AREA_DESCRIPTION, T0.U_ST_AREA_NAMING_AMOUNT, T0.U_ST_NAMED_DONOR
FROM ""@ST_NAMING""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_NAMING_CODE")
                {
                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

        internal static void Load_Naming(Form form, string Line_DataSource_Table)
        {
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Naming_Details");
            DT_Patients_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
            DT_Patients_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Naming_Data(form, DT_Patients_Details, i, Line_DataSource_Table);
            }

    ((Matrix)form.Items.Item("103").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("103").Specific).AutoResizeColumns();
        }

        internal static void Add_Naming_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Details = form.DataSources.DataTables.Item("Naming_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("103").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_NAMING_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Details.Rows.Count)
            {
                DT_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Naming_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Naming_Details");
            Matrix Mat = (Matrix)form.Items.Item("103").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Patient_Details.Rows.Remove(i - 1);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Naming_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("103").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Naming_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAMING_CODE", Index, Code);
            Set_Naming_Data(form, DT_Patient_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Set_Machinery_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_MACHINERY_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_MACHINE_NAME_AR, T0.U_ST_MACHINE_IMPACT_AR, T0.U_ST_MACHINE_DESCRIPTION_AR
FROM ""@ST_MACHINERY""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_MACHINERY_CODE")
                {
                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

        internal static void Load_Machinery(Form form, string Line_DataSource_Table)
        {
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Machinery_Details");
            DT_Patients_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
            DT_Patients_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Machinery_Data(form, DT_Patients_Details, i, Line_DataSource_Table);
            }

    ((Matrix)form.Items.Item("203").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("203").Specific).AutoResizeColumns();
        }

        internal static void Add_Machinery_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Details = form.DataSources.DataTables.Item("Machinery_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("203").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_MACHINERY_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Details.Rows.Count)
            {
                DT_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Machinery_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Machinery_Details");
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Patient_Details.Rows.Remove(i - 1);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Machinery_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Machinery_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_MACHINERY_CODE", Index, Code);
            Set_Machinery_Data(form, DT_Patient_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        private static void Convert_To_Actual(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FINANCE_CONFIRMATION", 0) != "Y")
            {
                throw new Logic.Custom_Exception("We can't convert to Actual Donation if we don't have Finance Confirmation");
            }

            //string MemberCard_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            //string X = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0);
            //DateTime Old_End_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0), "yyyyMMdd", null);
            //bool Is_Past;
            //DateTime New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Old_End_Date, out Is_Past);


            UDO_Definition KHCF_Actual_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Objects.Actual_Donations);
            Form Actual_Form = Loader.Open_UDO_Form(KHCF_Actual_Object.KHCF_Object);
            Actual_Form.Mode = BoFormMode.fm_ADD_MODE;
            Field_Definition[] Fields = Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == Form_Obj.KHCF_Object).ToArray();
            Actual_Form.Freeze(true);

            foreach (Field_Definition One_Field in Fields)
            {
                Actual_Form.DataSources.DBDataSources.Item(0).SetValue(One_Field.Column_Name_In_DB, 0, form.DataSources.DBDataSources.Item(0).GetValue(One_Field.Column_Name_In_DB, 0));
            }

            int Index = 0;
            int Rows_Count = form.DataSources.DBDataSources.Item("@ST_FUND_EXP_DON_ORP").Size;

            DataTable DT_Orphans_Details = Actual_Form.DataSources.DataTables.Item("Orphans_Details");
            DT_Orphans_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                string Orphans_Code = form.DataSources.DBDataSources.Item("@ST_FUND_EXP_DON_ORP").GetValue("U_ST_ORPHANS_CODE", 0);
                if (Orphans_Code == "")
                {
                    return;
                }

                Actual_Form.DataSources.DBDataSources.Item("@ST_FUND_ACT_DON_ORP").InsertRecord(Index + 1);
                Actual_Form.DataSources.DBDataSources.Item("@ST_FUND_ACT_DON_ORP").SetValue("U_ST_ORPHANS_CODE", Index, Orphans_Code);

                //DT_Orphans_Details.
                Set_Orphan_Data(Actual_Form, DT_Orphans_Details, Index, "@ST_FUND_ACT_DON_ORP");

                Index++;
            }

            Matrix Mat = (Matrix)Actual_Form.Items.Item("52").Specific;
            Mat.LoadFromDataSource();


            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            DataTable DT_Actual_Departments = Actual_Form.DataSources.DataTables.Item("DT_Departments");
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Actual_Departments, Departments, true);
            for (int i = 0; i < DT_Departments.Rows.Count; i++)
            {
                string X = DT_Departments.GetValue("SELECTED", i).ToString();
                DT_Actual_Departments.SetValue("SELECTED", i, X);
            }

            Actual_Form.Freeze(false);
        }

        private static void Approve(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Field_Data Fld = new Field_Data() { Field_Name = "U_ST_FINANCE_CONFIRMATION", Value = "Y" };
            Utility.Update_UDO(company, Form_Obj.UDO_Info, UDO_Code, new Field_Data[] { Fld });

            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

        }

        internal static void Add_Activity(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can add Activity if the form in OK Mode only");
            }
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0);
            if (BP_Code == "")
            {
                throw new Logic.Custom_Exception("There is no BP Code(Donor Master Record )");
            }

            SBO_Application.ActivateMenuItem("2563");
            Form Frm_Activity = SBO_Application.Forms.ActiveForm;

            Form UDF_Form = SBO_Application.Forms.Item(Frm_Activity.UDFFormUID);
            System.Threading.Thread.Sleep(1000);
            //UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            ((EditText)Frm_Activity.Items.Item("9").Specific).Value = BP_Code;
            Utility.Set_UDF_Value_on_Form(Code, UDF_Form, "U_ST_KHCF_OBJECT_CODE", true);
            Utility.Set_UDF_Value_on_Form(((int)Form_Obj.KHCF_Object).ToString(), UDF_Form, "U_ST_KHCF_OBJECT_TYPE", false);
            Frm_Activity.Items.Item("37").Click();
            // Frm_Payment.Items.Item("37").Click();
            //((EditText)Frm_Activity.Items.Item("13").Specific).Value = Payment_Amount.ToString();
            //System.Threading.Thread.Sleep(1000);
            //Frm_Activity.Items.Item("14").Click();
            //Frm_Activity.Items.Item("5").Enabled = false;
            //Frm_Activity.Items.Item("10").Enabled = false;
            //Frm_Activity.Items.Item("37").Enabled = false;
            //UDF_Form.Items.Item("U_ST_MEMBERSHIP_CODE").Enabled = false;

        }

        internal static void Choose_General_Item_ID(ItemEvent pVal)
        {
            //string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ON_BEHALF_OF", 0, Code);
            }
        }

        internal static void Choose_Card_ID(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            string SQL = "";


            UDO_Definition UDO_Info;
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "I")
            {
                UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
                SQL = $@"SELECT T0.""U_ST_BP_CODE"",T0.U_ST_FULL_NAME_AR FROM ""@{UDO_Info.Table_Name}""  T0 WHERE T0.""Code"" = '{Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_MASTER_RECORD", 0, RC.Fields.Item("U_ST_BP_CODE").Value.ToString());
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONATION_NAME", 0, RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
            }
            else
            {
                UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Objects.Fundraising_Corporate_Card);
                SQL = $@"SELECT T0.""U_ST_BP_CODE"",T0.U_ST_COMPANY_ARABIC_NAME FROM ""@{UDO_Info.Table_Name}""  T0 WHERE T0.""Code"" = '{Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_MASTER_RECORD", 0, RC.Fields.Item("U_ST_BP_CODE").Value.ToString());
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONATION_NAME", 0, RC.Fields.Item("U_ST_COMPANY_ARABIC_NAME").Value.ToString());
            }

        }

        internal static void Select_Donor(ItemEvent pVal)
        {
            string Code = Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            if (Code == "")
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string SQL = $@"SELECT T0.""CardName"" FROM OCRD T0 WHERE T0.""CardCode"" = '{Code}'";
            Recordset rs = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONATION_NAME", 0, rs.Fields.Item("CardName").Value.ToString());
        }

        internal static string Chosse_From_List_For_Code_And_DBDataSource(SAPbouiCOM.ItemEvent pVal, string ItemUID, bool Is_User_DataSource = false, string DataSource_Tablename = "")
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return "";
            }

            string UDF_Name = Helper.Utility.Get_Item_DB_Datasource(form.Items.Item(ItemUID));
            string Code = Choos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
            if (Is_User_DataSource)
            {
                form.DataSources.UserDataSources.Item(UDF_Name).Value = Code;
            }
            else
            {
                string X;
                if (DataSource_Tablename == "")
                {
                    X = form.DataSources.DBDataSources.Item(0).TableName;
                }
                else
                {
                    X = DataSource_Tablename;
                }

                form.DataSources.DBDataSources.Item(X).SetValue(UDF_Name, 0, Code);
                //string Y = pVal.ItemUID;
                //((SAPbouiCOM.EditText)form.Items.Item(pVal.ItemUID).Specific).Value = Code;
            }


            return Code;
        }

        internal static void Selected_Card_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (!string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0)))
            {
                SAPbouiCOM.ChooseFromList CFL_EXP = form.ChooseFromLists.Item("CFL_EXP");
                Conditions Exp_Cons = CFL_EXP.GetConditions();
                Condition Exp_Con;
                if (Exp_Cons.Count == 0)
                    Exp_Con = Exp_Cons.Add();
                else
                    Exp_Con = Exp_Cons.Item(0);
                Exp_Con.Alias = "U_ST_CARD_TYPE";
                Exp_Con.CondVal = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0).ToString();
                Exp_Con.Operation = BoConditionOperation.co_EQUAL;

                CFL_EXP.SetConditions(Exp_Cons);
            }
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("7").Specific).ChooseFromListUID = "CFL_INDIV_CARD";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";

            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("7").Specific).ChooseFromListUID = "CFL_CORP_CARD";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";

            }
            else
            {
                //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
            }
            ((EditText)form.Items.Item("7").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CONTACT_CARD", 0, "");
        }

        internal static void Orphans_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            //SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            //if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            //{
            //    return;
            //}
            Matrix Mat = (Matrix)form.Items.Item("52").Specific;
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");

            //string Code = Choos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
            int Index = pVal.Row - 1;
            //form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_ORPHANS_CODE", Index, Code);

            //string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_ORPHANS_CODE", Index);
            Set_Orphan_Data(form, DT_Orphans_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }

        internal static void Add_Orphans_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("52").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_ORPHANS_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Orphans_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");
            Matrix Mat = (Matrix)form.Items.Item("52").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        private static void Choose_Ambassador(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AMBASSADOR", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void Choose_Activity_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACTIVITY_NO", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void Choose_Exp_Donor(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_EXP_ID", 0, Code);
                Logic.Classes.Field_Definition[] All_Fields = Logic.Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == KHCF_Objects.Expected_Donations).ToArray();
                string[] Excluded_Fields = new string[] { "Code","Name", "DocEntry", "Canceled", "Object", "LogInst", "UserSign", "Transfered", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime","DataSource", "U_ST_INVOICE_NUMBER" };
                string SQL_DB_Data = $@"SELECT * FROM ""@ST_EXPEC_DONATION"" T0 Where T0.""Code""='{Code}'";
                Recordset RC_DB_Data = Helper.Utility.Execute_Recordset_Query(company, SQL_DB_Data);
                foreach (Field_Definition One_Field in All_Fields)
                {
                    if (Excluded_Fields.Contains(One_Field.Column_Name_In_DB))
                    {
                        continue;
                    }
                    try
                    {
                        string DB_Data = RC_DB_Data.Fields.Item(One_Field.Column_Name_In_DB).Value.ToString();
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
                        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue(One_Field.Column_Name_In_DB, 0, DB_Data);
                    }
                    catch (Exception ex) { }
                }

                }
        }

        private static void Check_Donor_CFL(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0);
            SAPbouiCOM.ChooseFromList CFL_EXP = form.ChooseFromLists.Item("CFL_EXP");
            Conditions Exp_Cons = CFL_EXP.GetConditions();

            if (!string.IsNullOrEmpty(BP_Code))
            {                 
                Condition Exp_Con = Exp_Cons.Add();
                Exp_Con.Alias = "U_ST_DONOR_MASTER_RECORD";
                Exp_Con.CondVal = BP_Code;
                Exp_Con.Operation = BoConditionOperation.co_EQUAL;

                CFL_EXP.SetConditions(Exp_Cons);
            }
            else
            {
                Exp_Cons = new Conditions();
            }
        }

        private static void Choose_Target_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_TARGET", 0, Code);
                if (form.Mode == BoFormMode.fm_OK_MODE)
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        internal static void Add_Line(ItemEvent pVal, string Line_DataSource_Table, string Datatable_Id, string Matrix_Id, string Code_Col, bool isDBTable)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT = form.DataSources.DataTables.Item(Datatable_Id);
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item(Matrix_Id).Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (isDBTable)
            {
                if (Count == 1)
                {
                    if (DS_Lines.GetValue(Code_Col, Count - 1) != "")
                    {
                        DS_Lines.InsertRecord(Count);
                    }
                    else
                    {
                        Count = 0;

                        DS_Lines.InsertRecord(Count);
                        Mat_Lines.LoadFromDataSource();
                        Mat_Lines.DeleteRow(1);
                        Mat_Lines.FlushToDataSource();
                        Mat_Lines.LoadFromDataSource();
                    }
                }
                else
                {
                    DS_Lines.InsertRecord(Count);
                }
                if (DS_Lines.Size > DT.Rows.Count)
                {
                    DT.Rows.Add();
                }
            }
            else
            {
                if (Count == 1)
                {
                    if (DT.GetValue(Code_Col, Count - 1) != "")
                    {
                        DS_Lines.InsertRecord(Count);
                    }
                    else
                    {
                        Count = 0;

                        DS_Lines.InsertRecord(Count);
                        Mat_Lines.LoadFromDataSource();
                        Mat_Lines.DeleteRow(1);
                        Mat_Lines.FlushToDataSource();
                        Mat_Lines.LoadFromDataSource();
                    }
                }
                else
                {
                    DS_Lines.InsertRecord(Count);
                }
                if (DS_Lines.Size > DT.Rows.Count)
                {
                    DT.Rows.Add();
                }

            }

            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            Mat_Lines.AutoResizeColumns();
            form.Freeze(false);

        }

        internal static void Remove_Selected_Lines(ItemEvent pVal, string Datatabe_Id, string Matrix_Id)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item(Datatabe_Id);
            Matrix Mat = (Matrix)form.Items.Item(Matrix_Id).Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void Activity_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("ClgCode", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_ACTIVITY_ID", Index, Code);
            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void Update_Machine_Status(string Target_ID,DateTime Don_Date)
        {
            string SQL_Target = $@"SELECT T1.""U_ST_MACHINE_ID""  FROM ""@ST_FUND_TARGET"" T0 inner join ""@ST_TARGET_MACHINES"" T1 ON T0.""Code"" = T1.""Code"" where T0.""Code"" = '{Target_ID}'";
            Recordset RC_Target = Helper.Utility.Execute_Recordset_Query(company, SQL_Target);
            if (RC_Target.RecordCount > 0)
            {
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Machinery_Installation_Det);
                for (int i = 0; i < RC_Target.RecordCount; i++)
                {
                    string Machine_Code = RC_Target.Fields.Item("U_ST_MACHINE_ID").Value.ToString();
                    
                    string Code = string.Empty;
                    int year = 0;
                    Recordset RC_Machine = Helper.Utility.Execute_Recordset_Query(company, $@"Select T0.""Code""  ,T0.""U_ST_YEAR"" From ""@ST_MACHIN_DET"" T0 where T0.""U_ST_MACHINE_ID""= '{Machine_Code}'");
                    if (RC_Machine.RecordCount > 0)
                    {
                        Code = RC_Machine.Fields.Item("Code").Value.ToString();
                        year = Convert.ToInt32(RC_Machine.Fields.Item("U_ST_YEAR").Value.ToString());
                    }
                    if (!string.IsNullOrEmpty(Code) && Don_Date.Year == year)
                    {
                        Field_Data Fld_Date = new Field_Data() { Field_Name = "U_ST_MACHINE_STATUS", Value = "A" };
                        Utility.Update_UDO(company, UDO_Info, Code, new Field_Data[] { Fld_Date });
                    }

                    RC_Target.MoveNext();
                }
            }
        }

        private static void Update_Area_Status(string Target_ID, Form form)
        {
            string SQL_Target = $@"SELECT T1.""U_ST_AREA_ID""  FROM ""@ST_FUND_TARGET"" T0 inner join ""@ST_TARGET_AREAS"" T1 ON T0.""Code"" = T1.""Code"" where T0.""Code"" = '{Target_ID}'";
            Recordset RC_Target = Helper.Utility.Execute_Recordset_Query(company, SQL_Target);
            if (RC_Target.RecordCount > 0)
            {
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Naming);
                for (int i = 0; i < RC_Target.RecordCount; i++)
                {
                    string Code = RC_Target.Fields.Item("U_ST_AREA_ID").Value.ToString();
                    if (!string.IsNullOrEmpty(Code))
                    {
                        string code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0).ToString();
                        string name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_NAME", 0).ToString();
                        string SQL_Name_En = $@"SELECT T0.""U_ST_FULL_NAME_EN"" FROM ""@ST_FUND_INDIV_CARD""  T0 WHERE T0.""Code"" ='{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0).ToString()}'";
                        Recordset RC_Name = Helper.Utility.Execute_Recordset_Query(company, SQL_Name_En);
                        if (RC_Name.RecordCount > 0)
                        {
                            name = RC_Name.Fields.Item("U_ST_FULL_NAME_EN").Value.ToString();
                        }

                        Field_Data Fld_Date = new Field_Data() { Field_Name = "U_ST_STATUS", Value = "N" };
                        Field_Data Fld_Date1 = new Field_Data() { Field_Name = "U_ST_DONOR_CARD", Value = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0).ToString() };
                        Field_Data Fld_Date2 = new Field_Data() { Field_Name = "U_ST_NAMED_DONOR", Value = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_NAME", 0).ToString() };
                        Field_Data Fld_Date3 = new Field_Data() { Field_Name = "U_ST_SIGN_AR", Value = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_NAME", 0).ToString() };
                        Field_Data Fld_Date4 = new Field_Data() { Field_Name = "U_ST_SIGN_EN", Value = name };
                        Field_Data Fld_Date5 = new Field_Data() { Field_Name = "U_ST_ACT_DONATION", Value = code };

                        Utility.Update_UDO(company, UDO_Info, Code, new Field_Data[] { Fld_Date, Fld_Date1, Fld_Date2, Fld_Date3, Fld_Date4, Fld_Date5 });
                    }
                    RC_Target.MoveNext();
                }
            }
        }

        private static void Create_Invoice(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.ActiveForm;

            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("Invoice can be created only in OK mode.");
            }

            string Actual_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string SQL_Membership = $@"Select T0.""U_ST_INVOICE_NUMBER"" From ""@ST_ACTUAL_DONATIONS"" T0 
 WHERE T0.""Code"" = '{Actual_Code}'  ";
            Recordset RC_Invoice = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            if (RC_Invoice.RecordCount > 0)
            {
                string invoiceNum = RC_Invoice.Fields.Item(0).Value.ToString();
                if (string.IsNullOrEmpty(invoiceNum))
                {
                    Documents Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oInvoices);
                    string Level_1 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PROGRAM_LEVEL1", 0);
                    string SQL_Program = $@"SELECT T0.""U_ST_GL_ACCOUNT"" FROM ""@ST_PROGRAM_LEVEL1""  T0 WHERE T0.""Code"" ='{Level_1}'";
                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Program);
                    string GL_Account = "_SYS00000003546";
                    if (RC.RecordCount > 0)
                    {
                        if (!string.IsNullOrEmpty(RC.Fields.Item(0).Value.ToString()))
                        {
                            GL_Account = RC.Fields.Item(0).Value.ToString();
                            GL_Account = GL_Account.Replace("-", "");
                        }
                    }
                    else
                    {
                        throw new Logic.Custom_Exception("GL Account For");
                    }
                    Doc.DocType = BoDocumentTypes.dDocument_Service;
                    Doc.TaxDate = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("CreateDate", 0).ToString(), "yyyyMMdd", null);
                    string Pay_Terms_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INSTALLMENT_TYPE", 0); //RC_Mem_Ship.Fields.Item("U_ST_INSTALLMENT_TYPE").Value.ToString();
                    if (Pay_Terms_Text != "" && Pay_Terms_Text != "0")
                    {
                        Doc.PaymentGroupCode = int.Parse(Pay_Terms_Text);
                    }
                    if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0)))
                    {
                        throw new Logic.Custom_Exception("There is no Busniuss Partner for This Donation");
                    }
                    Doc.DocDueDate = DateTime.Today;
                    Doc.CardCode = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0);
                    Doc.JournalMemo = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_NAME", 0);
                    Doc.Comments = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_NAME", 0);
                    Doc.Lines.AccountCode = GL_Account;
                    Doc.Lines.LineTotal = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
                    double d = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
                    Doc.Lines.ItemDescription = $@"Actual Donation From {Actual_Code}";
                    Doc.Lines.Add();
                    
                    if (Doc.Add() != 0)
                    {
                        string X = company.GetLastErrorDescription();
                        throw new Logic.Custom_Exception($" cannot be created for the Donation Invoice there is an Eroor : ,[{company.GetLastErrorDescription()}]");
                    }

                    string NewEntry = "";

                    try
                    {
                        company.GetNewObjectCode(out NewEntry);

                        UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Actual_Donations);
                        Field_Data Fld_Invoice = new Field_Data() { Field_Name = "U_ST_INVOICE_NUMBER", Value = NewEntry };
                        Utility.Update_UDO(company, UDO_Info, Actual_Code, new Field_Data[] { Fld_Invoice });
                        
                    }
                    catch (Exception ex)
                    {
                        throw new Logic.Custom_Exception($"Error during updating the Invoice Number on Actual Donation [{ex.Message}]");
                    }

                    SBO_Application.StatusBar.SetText($@"Invioce has been created Successfully", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);

                }
            }
        }

        private static void Create_Payment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.ActiveForm;
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0);
            string Date_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_DATE", 0);
            double Payment_Amount = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
            DateTime Don_date = DateTime.ParseExact(Date_Text, "yyyyMMdd", null);

            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Exception("We can create a Payment in the OK mode only");
            }


            SBO_Application.ActivateMenuItem("2817");
            Form Frm_Payment = SBO_Application.Forms.ActiveForm;

            Form UDF_Form = SBO_Application.Forms.Item(Frm_Payment.UDFFormUID);
            System.Threading.Thread.Sleep(1000);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Actual_Donations);
            

            ((EditText)Frm_Payment.Items.Item("5").Specific).Value = BP_Code;
            //System.Threading.Thread.Sleep(2000);
            //((EditText)Frm_Payment.Items.Item("26").Specific).Value = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PAYER", 0); ;

            System.Threading.Thread.Sleep(1000);
            // Frm_Payment.Items.Item("14").Click();
            Matrix Mat_Payment_Lines = (Matrix)Frm_Payment.Items.Item("20").Specific;
            double Rest_Amount = Payment_Amount;// = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
            bool Has_Selected_Invoice = false;
            for (int i = 0; i < Mat_Payment_Lines.RowCount; i++)
            {
                if (Rest_Amount <= 0)
                {
                    break;
                }
                EditText Txt_Doc_Type = (EditText)Mat_Payment_Lines.GetCellSpecific("45", i + 1);
                if (Txt_Doc_Type.Value != "13" && Txt_Doc_Type.Value != "203")
                {
                    continue;
                }

                EditText Txt_DueBalance = (EditText)Mat_Payment_Lines.GetCellSpecific("7", i + 1);
                string Due_Balance_Text = Txt_DueBalance.Value;
                if (Due_Balance_Text.IndexOf("-") >= 0)
                {
                    continue;
                }
                int Space_Index = Due_Balance_Text.IndexOf(" ");
                string Due_Currency = Due_Balance_Text.Substring(0, Space_Index);
                Due_Balance_Text = Due_Balance_Text.Replace(Due_Currency, "");

                double Due_Balance = double.Parse(Due_Balance_Text.Trim());
                SAPbouiCOM.CheckBox Ckb_Selected = (SAPbouiCOM.CheckBox)Mat_Payment_Lines.GetCellSpecific("10000127", i + 1);
                EditText Txt_Invoice_Payment = (EditText)Mat_Payment_Lines.GetCellSpecific("24", i + 1);

                Ckb_Selected.Checked = true;
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

        }

        private static void DisableInvoiceButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "71" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }
            
        }

        private static void Create_Invoice_Payment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.ActiveForm;
            string Invoice_Entry = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0);
            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0);
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Date_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_DATE", 0);
            double Payment_Amount = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
            string Level_1 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PROGRAM_LEVEL1", 0);

            DateTime Don_date = DateTime.ParseExact(Date_Text, "yyyyMMdd", null);
            if (!string.IsNullOrEmpty(Invoice_Entry))
            {
                throw new Logic.Custom_Exception($@"An invoice Already exists. Doc entry {Invoice_Entry}");
            }
            if (string.IsNullOrEmpty(BP_Code))
            {
                throw new Logic.Custom_Exception($@"can not create Invoice there is no businuss partner");
            }

            string SQL_Program = $@"SELECT T0.""U_ST_GL_ACCOUNT"" FROM ""@ST_PROGRAM_LEVEL1""  T0 WHERE T0.""Code"" ='{Level_1}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Program);
            string GL_Account = "216000000-01-03-050";
            if (RC.RecordCount > 0)
            {
                if (!string.IsNullOrEmpty(RC.Fields.Item(0).Value.ToString()))
                {
                    GL_Account = RC.Fields.Item(0).Value.ToString();
                    GL_Account = GL_Account.Replace("-", "");
                }
            }
            else
            {
                throw new Logic.Custom_Exception("GL Account For");
            }
            
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Exception("We can create a Payment in the OK mode only");
            }
            bool isDonor = CheckisDonor(form);
            if (!isDonor)
            {
                throw new Logic.Custom_Exception("Cannot create invoice, please flag the contact as donor");
            }

            SBO_Application.ActivateMenuItem("2054");
            Form Frm_Payment = SBO_Application.Forms.ActiveForm;

            Form UDF_Form = SBO_Application.Forms.Item(Frm_Payment.UDFFormUID);
            System.Threading.Thread.Sleep(1000);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Actual_Donations);
            Utility.Set_UDF_Value_on_Form(Code, UDF_Form, "U_ST_ACTUAL_DONATION_CODE", true);

            ((EditText)Frm_Payment.Items.Item("4").Specific).Value = BP_Code;
            ComboBox combo = (ComboBox)Frm_Payment.Items.Item("3").Specific;
            combo.Select("S", SAPbouiCOM.BoSearchKey.psk_ByValue);

            System.Threading.Thread.Sleep(1000);

            Matrix Mat_Payment_Lines = (Matrix)Frm_Payment.Items.Item("39").Specific;
            
            double Rest_Amount = Payment_Amount;
            
            for (int i = Mat_Payment_Lines.RowCount-1; i < Mat_Payment_Lines.RowCount; i++)
            {
                if (Rest_Amount <= 0)
                {
                    break;
                }

                EditText Txt_DueBalance = (EditText)Mat_Payment_Lines.GetCellSpecific("7", i + 1);
                string Due_Balance_Text = Txt_DueBalance.Value;
                if (Due_Balance_Text.IndexOf("-") >= 0)
                {
                    continue;
                }

                EditText Txt_Doc_Type = (EditText)Mat_Payment_Lines.GetCellSpecific("94", i + 1);
                EditText Txt_Invoice_Payment = (EditText)Mat_Payment_Lines.GetCellSpecific("12", i + 1);
                try
                {
                    SAPbouiCOM.Column col = (SAPbouiCOM.Column)Mat_Payment_Lines.Columns.Item("94");
                    string CFL_UID = col.ChooseFromListUID;
                    ChooseFromListCollection ch = Frm_Payment.ChooseFromLists;
                    SAPbobsCOM.ChooseFromList c = (SAPbobsCOM.ChooseFromList)form.Items.Item(CFL_UID).Specific;
                    
                    Txt_Doc_Type.Value = GL_Account;
                }
                catch (Exception ex)
                {
                    string SQL_Account = $@"SELECT T0.""FormatCode""  FROM OACT T0 WHERE T0.""AcctCode"" = '{GL_Account}'";
                    Recordset RC_Account = Helper.Utility.Execute_Recordset_Query(company, SQL_Account);
                    if (RC_Account.RecordCount > 0)
                    {
                        Txt_Doc_Type.Value = RC_Account.Fields.Item(0).Value.ToString();
                        Frm_Payment.Items.Item("12").Click();
                    }
                }

                double Due_Balance = double.Parse(Due_Balance_Text.Trim());

                Txt_Invoice_Payment.Value = Rest_Amount.ToString();// Math.Min(Rest_Amount, Due_Balance).ToString();

                i = Mat_Payment_Lines.RowCount;
            }

            Frm_Payment.Items.Item("4").Enabled = false;
            Frm_Payment.Items.Item("37").Enabled = false;

        }

        private static bool CheckisDonor(Form form)
        {
            string cardType = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0);
            string donorCode = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CONTACT_CARD", 0);
            if (cardType == "I")
            {
                string donorSQL = $@"SELECT T0.""U_ST_DONOR_ADD_UPDATE"" FROM ""@ST_FUND_INDIV_CARD"" T0 WHERE T0.""Code"" = '{donorCode}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, donorSQL);
                if (RC.Fields.Item("U_ST_DONOR_ADD_UPDATE").Value.ToString() == "Y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                string donorSQL = $@"SELECT T0.""U_ST_IS_DONOR"" FROM ""@ST_FUND_CORP_CARD"" T0 WHERE T0.""Code"" = '{donorCode}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, donorSQL);
                if (RC.Fields.Item("U_ST_IS_DONOR").Value.ToString() == "Y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal static void Selected_Ambassador_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_AMBASSADOR_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("43").Specific).ChooseFromListUID = "CFL_Ambassador";
                ((LinkedButton)form.Items.Item("234").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";

            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_AMBASSADOR_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("43").Specific).ChooseFromListUID = "CFL_Corp";
                ((LinkedButton)form.Items.Item("234").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";

            }
            else
            {
                //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
            }
            ((EditText)form.Items.Item("43").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AMBASSADOR", 0, "");
        }

        private static void Choose_Trip(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("PrjName", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TRIP_NAME", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void Choose_Giving_Monthly(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RECURRING", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void Check_Expected_Lines(string Exp_Code, string Actual_Code)
        {
            string SQL = $@"Select T0.""U_ST_FINANCE_CON"" From ""@ST_FUND_EXP_DON_PAY"" T0 Where T0.""Code""='{Exp_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int i = 0; i < RC.RecordCount; i++)
            {
                string Fininace_Confirm = RC.Fields.Item(0).Value.ToString();
                if (Fininace_Confirm.ToLower() == "y")
                {
                    string SQL_Update = $@"update ""@ST_FUND_EXP_DON_PAY"" T0 Set T0.""U_ST_ACTUAL_DON_ID"" = '{Actual_Code}' where T0.""Code"" = '{Exp_Code}'";
                    Recordset RC_Update = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
                    RC_Update.DoQuery(SQL_Update);
                }
                RC.MoveNext();
            }
        }

    }
}
