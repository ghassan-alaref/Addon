using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Potential_Grants : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "10","9","20" }); "10,9,20"
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5" });
        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            //Code_value = "Frm_Potential_Grants";
            //Desc_value = "Mandatary fields List For Potential Grants ";
            //Man_fields = "10,9,20";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "FundRaising Potential Grants" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("F", "base.Initialize Form", startTime);
            ComboBox Cmb_Department = (ComboBox)form.Items.Item("13").Specific;
            Cmb_Department.ValidValues.Add("LID","LID");
            Cmb_Department.ValidValues.Add("LCD", "LCD");
            Cmb_Department.ValidValues.Add("IDD", "IDD");
            //prev = Utility.Add_Time_Log("F", "Department", prev);

          
            

            ComboBox Cmb_Submitted = (ComboBox)form.Items.Item("18").Specific;
            Cmb_Submitted.ValidValues.Add("Y", "Yes");
            Cmb_Submitted.ValidValues.Add("N", "No");
            //prev = Utility.Add_Time_Log("F", "Submitted", prev);
            ComboBox Cmb_Status = (ComboBox)form.Items.Item("20").Specific;
            Cmb_Status.ValidValues.Add("O", "Open");
            Cmb_Status.ValidValues.Add("W", "Won");
            Cmb_Status.ValidValues.Add("L", "Lost");
            //prev = Utility.Add_Time_Log("F", "status", prev);
            string FundRaising_Department_ID = Configurations.Get_Fundraising_Department(company);
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" in ({FundRaising_Department_ID})";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "66", SQL_Account_Manager, true);

            string SQL_CO_Department = $@"SELECT T0.""Code"",T0.""Name"" FROM OUDP T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "14", SQL_CO_Department, true);
            //prev = Utility.Add_Time_Log("F", "Account Manager", prev);
            //prev = Utility.Add_Time_Log("F", "", startTime, true);
            form.Items.Item("21").Click();

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
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
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
            Form_Obj.Set_Fields(form);
            //string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);


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

            Form_Obj.Remove_Matrix_Row(form, "26");

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
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
        }

    }
}
