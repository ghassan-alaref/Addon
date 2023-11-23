using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Dreams_Come_True : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;


        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);

            ComboBox Combo_Donor_Type = (ComboBox)form.Items.Item("Item_7").Specific;
            Combo_Donor_Type.ValidValues.Add("I", "Individual");
            Combo_Donor_Type.ValidValues.Add("C", "Corporate");

            Matrix Mat_Att = (Matrix)form.Items.Item("63").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            //prev = Utility.Add_Time_Log("F", "Attachments", prev);
            SAPbouiCOM.ChooseFromList CFL_Doner = form.ChooseFromLists.Item("CFL_DONER");
            Conditions Doner_Cons = CFL_Doner.GetConditions();
            Condition Doner_Con = Doner_Cons.Add();
            Doner_Con.Alias = "U_ST_DONOR_ADD_UPDATE";
            Doner_Con.CondVal = "Y";
            Doner_Con.Operation = BoConditionOperation.co_EQUAL;
            CFL_Doner.SetConditions(Doner_Cons);

            SAPbouiCOM.ChooseFromList CFL_PO = form.ChooseFromLists.Item("CFL_PO");
            Conditions PO_Cons = CFL_PO.GetConditions();
            Condition PO_Con = PO_Cons.Add();
            PO_Con.Alias = "U_ST_DREAMS_TYPE";
            PO_Con.CondVal = "001";
            PO_Con.Operation = BoConditionOperation.co_EQUAL;
            CFL_PO.SetConditions(PO_Cons);

            SAPbouiCOM.ChooseFromList CFL_GI = form.ChooseFromLists.Item("CFL_GI");
            Conditions GI_Cons = CFL_GI.GetConditions();
            Condition GI_Con = GI_Cons.Add();
            GI_Con.Alias = "U_ST_DREAMS_TYPE";
            GI_Con.CondVal = "001";
            GI_Con.Operation = BoConditionOperation.co_EQUAL;
            CFL_GI.SetConditions(GI_Cons);

            KHCF_Logic_Utility.Set_Individua_Fund_Chosse_From_List_Basic_Condition(form, "CFL_DONER");
            KHCF_Logic_Utility.Set_Corporate_Fund_Chosse_From_List_Basic_Condition(form, "CFL_CORP");

            if (form.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(form);
            }
            form.Items.Item("19").Click();
           
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
                   Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);                    
                }
                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    bool Check_DB = Validate_DB(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    if (!Check_DB)
                    {
                        return Check_DB;
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
            //Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            //System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            //string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            //XML_Doc.LoadXml(XML_Text);




        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            Check_Approve(form);
            //string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);

            //form.Freeze(true);

            Form_Obj.Set_Fields(form);

            //form.Freeze(false);
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Set_Default_Value_Before_Adding(form);
            }
        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("Code", 0, New_UDO_Code);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

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
                if ((pVal.ItemUID == "10" || pVal.ItemUID == "11") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Frm_Expected_Donations.Choose_General_Item_ID(pVal);
                }
                if (pVal.ItemUID == "Item_7" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Card_Type(pVal);
                }
                if (pVal.ItemUID == "37" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_PO(pVal);
                }
                if (pVal.ItemUID == "38" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_GI(pVal);
                }
                if (pVal.ItemUID == "10" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Patient(pVal);
                }
                if (pVal.ItemUID == "11" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Donor(pVal);
                }
                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "62" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "16" && pVal.ItemChanged && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    string wishType = ((ComboBox)form.Items.Item("16").Specific).Selected.Value;
                    string SQL_Wish_Sub_Type = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_DREAMS_SUB_TYPE"" T0 
WHERE  T0.""U_ST_DREAM_TYPE""= '{wishType}'";
                    Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "17", SQL_Wish_Sub_Type, true);
                }
            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
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
               // Form form = SBO_Application.Forms.ActiveForm;
                //Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info);
                //form.DataSources.UserDataSources.Item("27").Value = "0";
                //form.DataSources.UserDataSources.Item("172").Value = "0";

            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_ADD_MODE)
            {
                Check_Approve(SBO_Application.Forms.ActiveForm);
            }

        }

        private static void Add_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            // BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_DREAMS_COME_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("63").Specific;
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
            DS_Attachment.SetValue("LineId", Count, "-1");

            //DS_Address.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);

        }

        private static void Remove_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Form_Obj.Remove_Matrix_Row(form, "63");

        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("63").Specific;
            for (int i = 0; i < Mat.RowCount; i++)
            {
                CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i + 1);
                if (Chk_Selected.Checked)
                {
                    EditText Txt_FileName = (EditText)Mat.GetCellSpecific("FileName", i + 1);
                    System.Diagnostics.Process.Start(Txt_FileName.Value);
                }
            }


        }
        private static void Choose_PO(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("DocNum", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PO_NUMBER", 0, Code);
            }
        }
        private static void Choose_GI(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("DocNum", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_GI_NUMBER", 0, Code);
            }
        }
        private static void Choose_Patient(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PATIENT_ID", 0, Code);
            }
        }
        private static void Choose_Donor(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_ID", 0, Code);
            }
        }

        internal static void Selected_Card_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("11").Specific).ChooseFromListUID = "CFL_DONER";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("11").Specific).ChooseFromListUID = "CFL_CORP";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";
            }
            else
            {
                //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
                
            }

            ((EditText)form.Items.Item("11").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_ID", 0, "");

        }

        private static bool Validate_DB(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            string patient_ID = form.DataSources.DBDataSources.Item("@ST_DREAMS_COME_TRUE").GetValue("U_ST_PATIENT_ID", 0);
            string Type = form.DataSources.DBDataSources.Item("@ST_DREAMS_COME_TRUE").GetValue("U_ST_TYPE", 0);
            string Sub_Type = form.DataSources.DBDataSources.Item("@ST_DREAMS_COME_TRUE").GetValue("U_ST_SUB_TYPE", 0);
            string SQL_Validation = $@"SELECT count(*)  FROM ""@ST_DREAMS_COME_TRUE""  T0 where T0.""U_ST_PATIENT_ID"" ='{patient_ID}' AND  T0.""U_ST_TYPE"" = '{Type}' And T0.""U_ST_SUB_TYPE"" ='{Sub_Type}' And T0.""U_ST_STATUS"" <>'P'";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Validation);

            if (RC.RecordCount > 0)
            {
                int Count = Convert.ToInt32(RC.Fields.Item(0).Value.ToString());
                if (Count > 0)
                {
                    Loader.New_Msg = "Same Data can not be added agian";
                    return false;
                }
            }
            return true;
        }

        private static void Check_Approve(Form form)
        {
            string SQL_Approve = $@"SELECT T0.""U_ST_CAN_APPROVE_DREAM"" FROM OUSR T0 where  T0.""USER_CODE"" = '{company.UserName}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Approve);
            if (RC.RecordCount > 0)
            {
                if (RC.Fields.Item(0).Value.ToString() == "Y")
                    form.Items.Item("40").Enabled = true;
                else
                    form.Items.Item("40").Enabled = false;
            }

        }

    }
}
