using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using SAPbobsCOM;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Monthly_Giving : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
       

        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);
            form.Height = 181;
            form.Width = 699;

            KHCF_Logic_Utility.Set_Corporate_Fund_Chosse_From_List_Basic_Condition(form, "CFL_Corp");

            string SQL_Frequency = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_FREQUENCIES"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "156", SQL_Frequency, true);

            string Fundraising_Department_ID = Configurations.Get_Fundraising_Department(company);
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" in ({Fundraising_Department_ID})";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "40", SQL_Account_Manager, true);

            //ComboBox combo_type = (ComboBox)form.Items.Item("Item_11").Specific;
            //combo_type.ValidValues.Add("I", "Individual");
            //combo_type.ValidValues.Add("C", "Corporate");

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
                    form.Items.Item("77").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("78").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
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

                //if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    ADD_Update_UDO(BusinessObjectInfo);
                }

                //if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                //{
                //    ADD_Update_UDO(BusinessObjectInfo);
                //}


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

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            //string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);

            //Frm_Booth_Commission.Fill_Frequency_Repetition(form, true, "157");

            Form_Obj.Set_Fields(form);

        }

        private static void Add_Installation_Info_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            // form.Freeze(true);
            DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_MONTH_GIVING_ACC");
            Matrix Mat_Add = (Matrix)form.Items.Item("46").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Address.Size;
            if (Count == 1)
            {
                if (DS_Address.GetValue("U_ST_DONATION_AMOUNT", Count - 1) != "")
                {
                    DS_Address.InsertRecord(Count);
                }
                else
                {
                    Count = 0;
                    DS_Address.InsertRecord(Count);
                    Mat_Add.LoadFromDataSource();
                    Mat_Add.DeleteRow(1);
                    Mat_Add.FlushToDataSource();
                }
            }
            else
            {
                DS_Address.InsertRecord(Count);
            }

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            // form.Freeze(false);

        }

        private static void Remove_Installation_Info_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("46").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
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
         private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Code = form.DataSources.DBDataSources.Item("@ST_MONTHLY_GIVING").GetValue("Code", 0).ToString();
            string Card = form.DataSources.DBDataSources.Item("@ST_MONTHLY_GIVING").GetValue("U_ST_CONTACT_CARD", 0).ToString();
            //if (!string.IsNullOrEmpty(Code))
            //{
            //    UDO_Definition UDO_Inf = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Corporate_Card);
            //    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_RECURRING", Value = Code };
            //    Utility.Update_UDO(company, UDO_Inf, Card, new Field_Data[] { Fld });

            //}
        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
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

                //if (pVal.ItemUID == "23" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Add_Attachment(pVal);
                //}
                //if (pVal.ItemUID == "24" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Remove_Attachment(pVal);
                //}
                //if (pVal.ItemUID == "25" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Open_Attachment(pVal);
                //}

                if ((pVal.ItemUID == "156") && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    //Frm_Booth_Commission.Fill_Frequency_Repetition(form, true, "157");
                }

                if (pVal.ItemUID == "Item_11" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Card_Type(pVal);

                }
                if (pVal.ItemUID == "17" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    string donorCode = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
                    if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CONTACT_TYPE", 0) == "I")
                    {
                        string SQL = $@"SELECT T0.""U_ST_FULL_NAME_AR"" FROM ""@ST_FUND_INDIV_CARD"" T0 WHERE T0.""Code"" = '{donorCode}'";
                        Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                        if (RC.RecordCount > 0)
                        {
                            string donorName = RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString();
                            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_NAME", 0, donorName);
                        }

                    }
                    else
                    {
                        string SQL = $@"SELECT T0.""U_ST_COMPANY_ARABIC_NAME"" FROM ""@ST_FUND_CORP_CARD"" T0 WHERE T0.""Code"" = '{donorCode}'";
                        Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                        if (RC.RecordCount > 0)
                        {
                            string donorName = RC.Fields.Item("U_ST_COMPANY_ARABIC_NAME").Value.ToString();
                            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_NAME", 0, donorName);
                        }
                    }

                }
                if (pVal.ItemUID == "77" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Installation_Info_Row(pVal);

                }
                if (pVal.ItemUID == "78" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Installation_Info_Row(pVal);

                }
             

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
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
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_NAMING_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("26").Specific;
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

            Form_Obj.Remove_Matrix_Row(form, "26");

        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);


            Matrix Mat = (Matrix)form.Items.Item("26").Specific;
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
        internal static void SBO_Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.BeforeAction)
                return;
            //Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
            if (Form_Obj == null || SBO_Application.Forms.ActiveForm.TypeEx != Form_Obj.Form_Type)
            {
                return;
            }
           
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
        }

        internal static void Selected_Card_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item("@ST_MONTHLY_GIVING").GetValue("U_ST_CONTACT_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("17").Specific).ChooseFromListUID = "CFL_Indiv";
                ((LinkedButton)form.Items.Item("Item_9").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";
            }
            else if (form.DataSources.DBDataSources.Item("@ST_MONTHLY_GIVING").GetValue("U_ST_CONTACT_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("17").Specific).ChooseFromListUID = "CFL_Corp";
                ((LinkedButton)form.Items.Item("Item_9").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";
            }
            else
            {
                //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
                string s = form.DataSources.DBDataSources.Item("@ST_MONTHLY_GIVING").GetValue("U_ST_CONTACT_TYPE", 0);

            }

            ((EditText)form.Items.Item("17").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CONTACT_CARD", 0, "");

        }
    }
}
