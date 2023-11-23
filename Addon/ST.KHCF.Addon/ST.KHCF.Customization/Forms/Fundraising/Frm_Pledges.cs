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
    internal class Frm_Pledges : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "10" });
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] {"9", "5" });

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);
            string SQL_Purpose = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_DON_PURPOSE_LVL2"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "13", SQL_Purpose, true);

            string SQL_Sub_Purpose = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_DON_SUB_PRP_LVL3"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "14", SQL_Sub_Purpose, true);
            KHCF_Logic_Utility.Set_Individua_Fund_Chosse_From_List_Basic_Condition(form, "CFL_Indiv");
            KHCF_Logic_Utility.Set_Corporate_Fund_Chosse_From_List_Basic_Condition(form, "CFL_Corp");

            form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            //form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);


            form.Items.Item("15").Click();
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
                    form.Items.Item("21").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("22").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
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
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID); //
            string Amount_str = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PLEDGE_TARGET_AMOUNT_CREATOR", 0);
            string StartDate_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_START_DATE", 0);
            string EndDate_Text = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0);
            if (!string.IsNullOrEmpty(StartDate_Text) && !string.IsNullOrEmpty(EndDate_Text))
            {
                DateTime StartDate = DateTime.ParseExact(StartDate_Text, "yyyyMMdd", null);
                DateTime EndDate = DateTime.ParseExact(EndDate_Text, "yyyyMMdd", null);
                if (StartDate > EndDate)
                {
                    Loader.New_Msg = "Start Date should be less than end date";
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(Amount_str))
            {
               // int Amount = Convert.ToInt32(Amount_str);
                if (Amount_str == "0.0")
                {
                    Loader.New_Msg = "Pledge Target Amount must not be zero";
                    return false;
                }
            }
            else
            {
                Loader.New_Msg = "Pledge Target Amount must not beempty";
                return false;
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
                if ((pVal.ItemUID == "11") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
                }
                if (pVal.ItemUID == "40" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_GOOD_WILL_PARTI", "Participants_Details", "42", "U_ST_PARTICIPANT_CODE", true);
                }
                if (pVal.ItemUID == "41" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Participants_Details", "42");
                }
                if (pVal.ItemUID == "42" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Participants_Choos_From_List(pVal, "@ST_GOOD_WILL_PARTI");
                }
                if (pVal.ItemUID == "55" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Rule_ID(pVal);
                }
                if (pVal.ItemUID == "51" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Target_ID(pVal);
                }
                if (pVal.ItemUID == "200" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Creator_Type(pVal);
                }
                if (pVal.ItemUID == "9" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Creator_ID(pVal);
                }
                if (pVal.ItemUID == "1000" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    KHCF_Logic_Utility.Select_Allocation(pVal, Form_Obj, "51");
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        internal static void Participants_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("42").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PARTICIPANT_CODE", Index, Code);
            Set_Participants_Data(form, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }
        internal static void Set_Participants_Data(Form form,int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PARTICIPANT_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""U_ST_FULL_NAME_AR"" FROM ""@ST_FUND_INDIV_CARD"" T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
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

        private static void DisableMatrixButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() {"40","41" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

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
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form form = SBO_Application.Forms.ActiveForm;
                DisableMatrixButtons(form);
            }
        }
        private static void Choose_Rule_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("U_ST_NAME", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FUND_RULE_ID", 0, Code);
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
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FUND_TARGET", 0, Code);
            }
        }
        private static void Choose_Creator_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            string name = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                name = Chos_Event.SelectedObjects.GetValue("U_ST_FULL_NAME_AR", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR_NAME", 0, name);

            }
        }
        internal static void Selected_Creator_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CREATOR_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("9").Specific).ChooseFromListUID = "CFL_Indiv";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CREATOR_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("9").Specific).ChooseFromListUID = "CFL_Corp";
            }
        }

    }
}
