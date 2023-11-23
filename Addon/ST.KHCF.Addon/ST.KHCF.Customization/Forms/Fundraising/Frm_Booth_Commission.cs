using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Booth_Commission : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "8","9","11","12","10","13" });  "8,9,11,12,10,13"
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
            //Code_value = "Frm_Booth_Commission";
            //Desc_value = "Mandatary fields List For Booth Commission ";
            //Man_fields = "8,9,11,12,10,13";
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "FundRaising Booth Commissions" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("F", "base.Initialize Form", startTime);

            //Matrix Mat_Att = (Matrix)form.Items.Item("26").Specific;
            //base.Fill_Attachment_ComboBox(Mat_Att);

            //SAPbouiCOM.ChooseFromList CFL_Doner = form.ChooseFromLists.Item("CFL_DONER");
            //Conditions Doner_Cons = CFL_Doner.GetConditions();
            //Condition Doner_Con = Doner_Cons.Add();
            //Doner_Con.Alias = "U_ST_DONOR_ADD_UPDATE";
            //Doner_Con.CondVal = "Y";
            //Doner_Con.Operation = BoConditionOperation.co_EQUAL;

            //CFL_Doner.SetConditions(Doner_Cons);

            //form.Items.Item("9").Click();
            //prev = Utility.Add_Time_Log("F", "", startTime, true);


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
            //string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);
            //Load_Patient(form);

            Fill_Frequency_Repetition(form, false, "13");
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
                //if ((pVal.ItemUID == "11") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                //{
                //    Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
                //}
                if ((pVal.ItemUID == "10") && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Fill_Frequency_Repetition(form, true, "13");
                }


            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        internal static void Fill_Frequency_Repetition(Form form, bool Set_Empty_Value, string Repetition_Item_ID)
        {
            string Frequency = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FREQUENCY", 0);

            string SQL = $@"SELECT T1.""Code"", T1.""Name"" 
FROM ""@ST_FREQ_REPET_MAPP""  T0 INNER JOIN ""@ST_FREQ_REPETITION""  T1 ON T0.""U_ST_FREQ_REPETITION"" = T1.""Code"" 
WHERE T0.""U_ST_FREQUENCY"" = '{Frequency}'";

            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, Repetition_Item_ID, SQL);
            if (Set_Empty_Value)
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FREQUENCY_REPETITION", 0, "");

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
