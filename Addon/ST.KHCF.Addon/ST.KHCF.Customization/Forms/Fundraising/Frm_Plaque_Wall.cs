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
    internal class Frm_Plaque_Wall : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;


        internal override void Initialize_Form(Form form)
        {
            
            base.Initialize_Form(form);
           

            Matrix Mat_Lines = (Matrix)form.Items.Item("10").Specific;
            Mat_Lines.AutoResizeColumns();

            //SAPbouiCOM.ChooseFromList CFL_Doner = form.ChooseFromLists.Item("CFL_DONER");
            //Conditions Doner_Cons = CFL_Doner.GetConditions();
            //Condition Doner_Con = Doner_Cons.Add();
            //Doner_Con.Alias = "U_ST_DONOR_ADD_UPDATE";
            //Doner_Con.CondVal = "Y";
            //Doner_Con.Operation = BoConditionOperation.co_EQUAL;

            //CFL_Doner.SetConditions(Doner_Cons);

            //            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
            //WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'F'";
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "33", SQL_Customer_Group, true);
            //prev = Utility.Add_Time_Log("F", "Customer Group", prev);
            //prev = Utility.Add_Time_Log("F", "", startTime, true);

            form.Items.Item("9").Click();
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

                //if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                //{
                //    ADD_Update_UDO(BusinessObjectInfo);
                //}
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                    DisableTabButtons(form);
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

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            //Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            //string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);
            //Load_Patient(form);
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_FIND_MODE)
            {
                form.Items.Item("11").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                form.Items.Item("12").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);   
            }

            Form_Obj.Set_Fields(form);
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
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("10").Specific;
            int Count = Mat.RowCount;
            if (Count == 0)
            {
                throw new Custom_Exception("Cannot Add with no lines");
                return false;
            }
            int checkMatrix = CheckMatrix(businessObjectInfo);
            int checkZero = CheckZeroMatrix(businessObjectInfo);
            if (checkMatrix != -5)
            {
                throw new Custom_Exception($@"Line {checkMatrix + 1} contains from value larger than to value");
                return false;
            }
            if (checkZero != -5)
            {
                throw new Custom_Exception($@"Line {checkZero} contains from and to both have zero value");
                return false;
            }

            Validate_Lines(businessObjectInfo);
            
            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }

            return true;
        }

        private static int CheckMatrix(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("10").Specific;
            int Count = Mat.RowCount;
            int result = -5;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                string from = ((EditText)Mat.Columns.Item("Col_0").Cells.Item(i).Specific).Value;
                string To = ((EditText)Mat.Columns.Item("Col_1").Cells.Item(i).Specific).Value;
                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(To))
                {
                    double fromValue = Convert.ToDouble(from);
                    double toValue = Convert.ToDouble(To);
                    if (fromValue > toValue)
                    {
                        result = i;
                        return result;
                    }
                }

            }
            return result;
        }

        private static int CheckZeroMatrix(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("10").Specific;
            int Count = Mat.RowCount;
            int result = -5;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                string from = ((EditText)Mat.Columns.Item("Col_0").Cells.Item(i).Specific).Value;
                string To = ((EditText)Mat.Columns.Item("Col_1").Cells.Item(i).Specific).Value;
                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(To))
                {
                    double fromValue = Convert.ToDouble(from);
                    double toValue = Convert.ToDouble(To);
                    if (fromValue == 0 && toValue == 0)
                    {
                        result = i;
                        return result;
                    }
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
                    Add_Line(pVal, "@ST_PLAQUE_LINES", "details","10", "U_ST_FROM", true);
                }
                if (pVal.ItemUID == "12" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "details", "10");
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
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
                DisableTabButtons(SBO_Application.Forms.ActiveForm);
            }
        }

        private static void DisableTabButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "11","12" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }

        }

        private static void Validate_Lines(BusinessObjectInfo businessObjectInfo)
        {
            
                SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(businessObjectInfo.FormUID);
                SAPbouiCOM.DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_PLAQUE_LINES");

                int Count = DS_Lines.Size;
                SAPbouiCOM.Matrix Mat_Lines = (SAPbouiCOM.Matrix)form.Items.Item("10").Specific;
               
                for (int i = 0; i < Count; i++)
                {
                    SAPbouiCOM.EditText From = (SAPbouiCOM.EditText)Mat_Lines.Columns.Item("Col_0").Cells.Item(i + 1).Specific;
                    SAPbouiCOM.EditText To = (SAPbouiCOM.EditText)Mat_Lines.Columns.Item("Col_1").Cells.Item(i + 1).Specific;
                    SAPbouiCOM.EditText Grade = (SAPbouiCOM.EditText)Mat_Lines.Columns.Item("Col_2").Cells.Item(i + 1).Specific;
                    string From_Value = From.Value;
                    string To_Value = To.Value;
                    string Grade_Value = Grade.Value;
                    
                    if (string.IsNullOrEmpty(From_Value) || string.IsNullOrEmpty(To_Value) || string.IsNullOrEmpty(Grade_Value))
                    {
                        throw new Custom_Exception($"Please Fill all fields in Plaque Lines at row [{i + 1}]");
                    }
                }

        }

    }
}
