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
    internal class Frm_Recommending_Recognitions : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;


        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);
            
            Matrix Mat_Att = (Matrix)form.Items.Item("8").Specific;
            string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_GIVEN_RECOGNIT""  T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Att, "REC_TYPE", SQL, true);

            Mat_Att.AutoResizeColumns();

            form.Items.Item("7").Click();
            

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
                    form.Items.Item("11").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("12").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
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
                    bool Check_DB = Validate_DB(BusinessObjectInfo);
                    if (!Check_DB)
                    {
                        return Check_DB;
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
            //string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);
            //Load_Patient(form);


        }

        //   internal static void Load_Patient(Form form)
        //   {
        //       DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("DT_Patients_Details");
        //       DT_Orphans_Details.Rows.Clear();
        //       int Rows_Count = form.DataSources.DBDataSources.Item("@ST_PLEDGES_PATIENTS").Size;
        //       DT_Orphans_Details.Rows.Add(Rows_Count);
        //       for (int i = 0; i < Rows_Count; i++)
        //       {
        //           Set_Patient_Data(form, DT_Orphans_Details, i);
        //       }

        //((Matrix)form.Items.Item("18").Specific).LoadFromDataSource();
        //       ((Matrix)form.Items.Item("18").Specific).AutoResizeColumns();
        //   }

        //internal static void Set_Patient_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index)
        //{
        //    return;
        //    string Code = form.DataSources.DBDataSources.Item("@ST_PLEDGES_PATIENTS").GetValue("U_ST_PATIENTS_CODE", DT_Row_Index);
        //    string SQL = $@"SELECT T0.""CardName"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID FROM OCRD T0 WHERE T0.""CardCode"" = '{Code}'";
        //    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
        //    for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
        //    {
        //        string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
        //        if (Col_Name == "SELECTED")
        //        {
        //            continue;
        //        }
        //        if (Col_Name == "U_ST_PATIENTS_CODE")
        //        {
        //            DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
        //            continue;
        //        }
        //        string X = RC.Fields.Item(Col_Name).Value.ToString();
        //        DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
        //    }
        //}

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
                form.DataSources.DBDataSources.Item("@ST_RECOMMEND_REC").SetValue("Code", 0, New_UDO_Code);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            bool checkValues = CheckValues(businessObjectInfo);
            if (!checkValues)
            {
                throw new Custom_Exception("From value is larger than to value");
                return false;
            }
            Matrix Mat = (Matrix)form.Items.Item("8").Specific;
            int Count = Mat.RowCount;
            if (Count == 0)
            {
                throw new Custom_Exception("Cannot Add with no lines");
                return false;
            }
            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }


            return true;
        }

        private static bool CheckValues(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string from = ((EditText)form.Items.Item("9").Specific).Value;
            string to = ((EditText)form.Items.Item("6").Specific).Value;
            bool result = true;
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                double fromValue = Convert.ToDouble(from);
                double toValue = Convert.ToDouble(to);
                if (fromValue > toValue)
                {
                    result = false;
                }
            }
            return result;
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
                //if ((pVal.ItemUID == "11") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                //{
                //    Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
                //}


                if (pVal.ItemUID == "11" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Types_Line(pVal);
                }
                if (pVal.ItemUID == "12" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Types_Selected_Lines(pVal);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        internal static void Add_Types_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            //string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
            //form.DataSources.DBDataSources.Item("@ST_RECOMMEND_REC").SetValue("Code", 0, New_UDO_Code);
            //return;

            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_RECOM_REC_TYPE");
            Matrix Mat_Lines = (Matrix)form.Items.Item("8").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_RECOGNITION_TYPE", Count - 1) != "")
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

            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);
            Mat_Lines.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Types_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("8").Specific;
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
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form form = SBO_Application.Forms.ActiveForm;
                Form_Obj.UnSet_Mondatory_Fields_Color(form);
                DisableMatrixButtons(form);
            }
        }

        private static void DisableMatrixButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() {"11","12"};
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }
        }

        private static bool Validate_DB(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0).ToString();
            string From = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FROM", 0).ToString();
            string To = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TO", 0).ToString();
            string SQL = $@"Select count(*) from ""@ST_RECOMMEND_REC"" T0 Where T0.""U_ST_FROM""= '{From}' And T0.""U_ST_TO"" = '{To}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.RecordCount > 0)
            {
                int count = Convert.ToInt32(RC.Fields.Item(0).Value.ToString());
                if (count > 0)
                {
                    Loader.New_Msg = "Same Data can not be added again";
                    return false;
                }
            }
            return true;
        }
    }
}
