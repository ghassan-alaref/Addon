using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Grants : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "9" });
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5", "10" });  "5,10"

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            
            //Desc_value = "Mandatary fields List For Grants ";
            //Man_fields = "9";


            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "FundRaising Grants" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("F", "base.Initialize Form", startTime);

            Matrix Mat_Att = (Matrix)form.Items.Item("13").Specific;
            string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_GRANTS_PORTALS""  T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Att, "REC_TYPE", SQL, true);
            //prev = Utility.Add_Time_Log("F", "Grants Portal", prev);

            SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_EXPEC_POST_DATES""  T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Att, "Col_0", SQL, true);
            //prev = Utility.Add_Time_Log("F", "Post Dates", prev);
            SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_AREA_OF_INTEREST""  T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Att, "Col_1", SQL, true);
            //prev = Utility.Add_Time_Log("F", "Area of Interest", prev);
            SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_ALERT_MODE""  T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Att, "Col_2", SQL, true);
            //prev = Utility.Add_Time_Log("F", "Alert Mode", prev);
            SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SEARCH_FREQUENCY""  T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Att, "Col_3", SQL, true);
            //prev = Utility.Add_Time_Log("F", "Search Frequency", prev);

            SAPbouiCOM.ChooseFromList CFL_Doner = form.ChooseFromLists.Item("CFL_DONER");
            Conditions Doner_Cons = CFL_Doner.GetConditions();
            Condition Doner_Con = Doner_Cons.Add();
            Doner_Con.Alias = "U_ST_DONOR_ADD_UPDATE";
            Doner_Con.CondVal = "Y";
            Doner_Con.Operation = BoConditionOperation.co_EQUAL;
            CFL_Doner.SetConditions(Doner_Cons);
            //prev = Utility.Add_Time_Log("F", "CFL Donor", prev);
            //prev = Utility.Add_Time_Log("F", "", startTime, true);
            // form.Items.Item("7").Click();


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
                    form.Items.Item("11").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("12").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
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
                form.DataSources.DBDataSources.Item("@ST_GRANTS").SetValue("Code", 0, New_UDO_Code);
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
                if ((pVal.ItemUID == "9") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Doner_From_List(pVal);
                }


                if (pVal.ItemUID == "11" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal);
                }
                if (pVal.ItemUID == "12" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal);
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Choose_Doner_From_List(ItemEvent pVal)
        {
           string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID, false, "@ST_GRANTS");

            string SQL = $@"SELECT T1.U_ST_ENTITY_TYPE 
FROM ""@ST_FUND_INDIV_CARD""  T0 INNER JOIN OCRG T1 ON T0. U_ST_CUSTOMER_GROUP = T1.""GroupCode"" 
WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.DataSources.DBDataSources.Item("@ST_GRANTS").SetValue("U_ST_ENTITY_TYPE", 0, RC.Fields.Item("U_ST_ENTITY_TYPE").Value.ToString());

        }

        internal static void Add_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            //string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
            //form.DataSources.DBDataSources.Item("@ST_RECOMMEND_REC").SetValue("Code", 0, New_UDO_Code);
            //return;

            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_GRANTS_LINES");
            Matrix Mat_Lines = (Matrix)form.Items.Item("13").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_GRANT_PORTALS", Count - 1) != "")
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

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            Mat_Lines.AutoResizeColumns();
            form.Freeze(false);

        }

        internal static void Remove_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("13").Specific;
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
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
        }
    }
}
