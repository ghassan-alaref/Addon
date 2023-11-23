using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Ambassador : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "10","12","7" });  "10,12,7"
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
            
            //Desc_value = "Mandatary fields List For Ambassador ";
            //Man_fields = "10,12,7";

            base.Initialize_Form(form);
            string fundDepartments = Utility.Get_Configuration(company, "Fundraising_Department");
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" WHERE T1.""dept"" in ({fundDepartments})";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "101", SQL_Account_Manager, true);
            string SQL_Activity_type = $@"SELECT ""Code"", ""Name"" FROM ""@ST_ACTIVITY_TYPE"" ";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "9", SQL_Activity_type, true);

            form.Height = 244;
            form.Width = 768;
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
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
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
            string activityId = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string actualSummary = $@"SELECT SUM(T0.""U_ST_DONATION_AMOUNT"") AS ""SUM"" FROM ""@ST_ACTUAL_DONATIONS"" T0 where T0.""U_ST_AMBASSADOR_ACT"" = '{activityId}' ";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, actualSummary);
            if (RC.RecordCount > 0)
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RECEIVED_DONATION", 0, RC.Fields.Item("SUM").Value.ToString());
            }
            string expectedSummary = $@"SELECT SUM(T0.""U_ST_DONATION_AMOUNT"") AS ""SUM"" FROM ""@ST_EXPEC_DONATION"" T0 where T0.""U_ST_AMBASSADOR_ACT"" = '{activityId}' ";
            Recordset expectedRC = Helper.Utility.Execute_Recordset_Query(company, expectedSummary);
            if (expectedRC.RecordCount > 0)
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_EXPECTED_DONATION", 0, expectedRC.Fields.Item("SUM").Value.ToString());
            }
            Form_Obj.Set_Fields(form);
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
                if (pVal.ItemUID == "9" && pVal.ItemChanged && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    string activityType = ((ComboBox)form.Items.Item("9").Specific).Selected.Value;
                    string SQL_Activity_Sub_Type = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_ACT_SUB_TYPE"" T0 
WHERE  T0.""U_ST_ACTIVITY_TYPE""= '{activityType}'";
                    Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "10", SQL_Activity_Sub_Type, true);
                }
                if (pVal.ItemUID == "712" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Card_Type(pVal);
                }
                if (pVal.ItemUID == "6" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Card(pVal);
                }
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        internal static void Patient_Choos_From_List(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("18").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Patients_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item("@ST_PLEDGES_PATIENTS").SetValue("U_ST_PATIENTS_CODE", Index, Code);
            Set_Patient_Data(form, DT_Patient_Details, Index);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Add_Patient_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_PLEDGES_PATIENTS");
            Matrix Mat_Lines = (Matrix)form.Items.Item("18").Specific;
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

        internal static void Remove_Patient_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
            Matrix Mat = (Matrix)form.Items.Item("18").Specific;
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

        internal static void Load_Patient(Form form)
        {
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
            DT_Orphans_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item("@ST_PLEDGES_PATIENTS").Size;
            DT_Orphans_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Patient_Data(form, DT_Orphans_Details, i);
            }

((Matrix)form.Items.Item("18").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("18").Specific).AutoResizeColumns();
        }

        internal static void Set_Patient_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index)
        {
            string Code = form.DataSources.DBDataSources.Item("@ST_PLEDGES_PATIENTS").GetValue("U_ST_PATIENTS_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID FROM ""@ST_PATIENTS_CARD"" T0 
WHERE T0.""Code"" = '{Code}'";
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
                string X = RC.Fields.Item(Col_Name).Value.ToString();
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }


        internal static void Participants_Choos_From_List(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("33").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("DT_Participants_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item("@ST_PLEDGES_PART").SetValue("U_ST_PARTICIPANT_CODE", Index, Code);
            Set_Participants_Data(form, DT_Patient_Details, Index);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Add_Participants_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("DT_Participants_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_PLEDGES_PART");
            Matrix Mat_Lines = (Matrix)form.Items.Item("33").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_PARTICIPANT_CODE", Count - 1) != "")
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

        internal static void Remove_Participants_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("DT_Participants_Details");
            Matrix Mat = (Matrix)form.Items.Item("33").Specific;
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
             
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }

        }

        internal static void Set_Participants_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index)
        {
            string Code = form.DataSources.DBDataSources.Item("@ST_PLEDGES_PART").GetValue("U_ST_PARTICIPANT_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""U_ST_FULL_NAME_AR"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH 
FROM ""@ST_FUND_INDIV_CARD"" T0 
WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_PARTICIPANT_CODE")
                {
                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                string X = RC.Fields.Item(Col_Name).Value.ToString();
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

        internal static void Machinery_Choos_From_List(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("43").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Machinery_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item("@ST_PLEDGES_MAC").SetValue("U_ST_MACHINERY_CODE", Index, Code);
            Set_Machinery_Data(form, DT_Patient_Details, Index);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Add_Machinery_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Machinery_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_PLEDGES_MAC");
            Matrix Mat_Lines = (Matrix)form.Items.Item("43").Specific;
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

        internal static void Remove_Machinery_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Machinery_Details");
            Matrix Mat = (Matrix)form.Items.Item("43").Specific;
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
        internal static void Selected_Card_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("6").Specific).ChooseFromListUID = "CFL_Indiv";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("6").Specific).ChooseFromListUID = "CFL_Corp";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";
            }
            else
            {
                //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
            }
            ((EditText)form.Items.Item("6").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CONTACT_CARD", 0, "");
        }
        private static void Choose_Card(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                if (Chos_Event.ChooseFromListUID == "CFL_Indiv")
                {
                    Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                    
                }
                else
                {
                    Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                }
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CONTACT_CARD", 0, Code);
            }
        }
        internal static void Load_Machinery(Form form)
        {
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Machinery_Details");
            DT_Orphans_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item("@ST_PLEDGES_MAC").Size;
            DT_Orphans_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Machinery_Data(form, DT_Orphans_Details, i);
            }

((Matrix)form.Items.Item("43").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("43").Specific).AutoResizeColumns();
        }

        internal static void Set_Machinery_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index)
        {
            string Code = form.DataSources.DBDataSources.Item("@ST_PLEDGES_MAC").GetValue("U_ST_MACHINERY_CODE", DT_Row_Index);
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
                string X = RC.Fields.Item(Col_Name).Value.ToString();
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

    }
}
