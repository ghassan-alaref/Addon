using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;
using SAPbouiCOM;

namespace ST.KHCF.Customization.Forms
{
    internal class Frm_System_Time_Sheet
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static string[] Relations_Grid_IDs = new string[] { "ST_102", "ST_112", "ST_122" };

        internal static void SBO_Application_RightClickEvent(ref ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {

            }
            catch (Exception ex)
            {
                SBO_Application.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, true);
            }
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (pVal.FormTypeEx != "234000044")
            {
                return;
            }

            try
            {

                if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);

                    Initialize_Form(form);

                }

                if (pVal.ItemUID == "ST_10" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.PaneLevel = 10;
                }
                if (pVal.ItemUID == "ST_11" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.PaneLevel = 11;
                }
                if (pVal.ItemUID == "ST_12" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.PaneLevel = 12;
                }

                if (pVal.ItemUID == "ST_101" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Add_Grid_Row(form, "DT_PATIENT");
                }

                if (pVal.ItemUID == "ST_100" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Remove_Grid_Row(form, "DT_PATIENT");
                }
                if (pVal.ItemUID == "ST_111" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Add_Grid_Row(form, "DT_SUBJECT");
                }

                if (pVal.ItemUID == "ST_110" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Remove_Grid_Row(form, "DT_SUBJECT");
                }
                if (pVal.ItemUID == "ST_121" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Add_Grid_Row(form, "DT_GRADES");
                }

                if (pVal.ItemUID == "ST_120" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Remove_Grid_Row(form, "DT_GRADES");
                }

                if (pVal.ItemUID == "ST_102" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Patient(pVal);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }

        }

        private static void Choose_Patient(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Grid Grd = (Grid)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();

                int Datatable_Index = Grd.GetDataTableRowIndex(pVal.Row);
                DataTable DT_Patient = form.DataSources.DataTables.Item("DT_PATIENT");

                DT_Patient.SetValue("Code", Datatable_Index, C);
                DT_Patient.SetValue("Name", Datatable_Index, Chos_Event.SelectedObjects.GetValue("U_ST_FULL_ARABIC_NAME", 0).ToString());
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }

                Grd.AutoResizeColumns();
            }


        }

        private static void Initialize_Form(Form form)
        {
            Helper.Utility.Load_Form_Changes_as_XML(SBO_Application, form, Properties.Resources.Frm_System_Time_Sheet);

            form.Freeze(true);
            Folder Fd_Patient = (Folder)form.Items.Item("ST_10").Specific;
            Folder Fd_Subject = (Folder)form.Items.Item("ST_11").Specific;
            Folder Fd_Grades = (Folder)form.Items.Item("ST_12").Specific;

            Fd_Patient.GroupWith("234000005");
            Fd_Patient.AutoPaneSelection = true;
            Fd_Subject.GroupWith("234000005");
            Fd_Subject.AutoPaneSelection = true;
            Fd_Grades.GroupWith("234000005");
            Fd_Grades.AutoPaneSelection = true;

            string SQL_Program = $@"SELECT ""Code"", ""Name"" from ""@ST_PROGRAM_LEVEL1""";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "ST_2", SQL_Program);

            string SQL_Subject = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUBJECT""  T0";
            Grid Grd_Subject = (Grid)form.Items.Item("ST_112").Specific;
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Subject, "Code", SQL_Subject);
            string SQL_Grade = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_GRADE""  T0";
            Grid Grd_Grades = (Grid)form.Items.Item("ST_122").Specific;
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Grades, "Code", SQL_Grade);

            ((ComboBox)form.Items.Item("ST_2").Specific).DataBind.SetBound(true, "OTSH", "U_ST_PROGRAM");


            form.Freeze(false);
        }

        internal static bool SBO_Application_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo)
        {
            bool BubbleEvent = true;
            if (BusinessObjectInfo.FormTypeEx != "234000044")
            {
                return BubbleEvent;
            }
            try
            {
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    ADD_Update_UDO(BusinessObjectInfo);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }
            return BubbleEvent;

        }

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item("OTSH").GetValue("AbsEntry", 0);
            Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, form.TypeEx, false);


        }


        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {

            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item("OTSH").GetValue("AbsEntry", 0);

            string SQL_Subject = $@"SELECT T0.""U_ST_KHCF_TABLE_CODE"" as ""Code"" FROM ""@ST_REL_OBJECTS""  T0 
WHERE T0.""U_ST_KHCF_TABLE_NAME"" = 'DT_SUBJECT' AND T0.""U_ST_KHCF_OBJECT_TYPE"" = '{form.TypeEx}' AND  T0.""U_ST_KHCF_OBJECT_CODE"" = '{UDO_Code}'";
            Recordset RC_Subject = Helper.Utility.Execute_Recordset_Query(company, SQL_Subject);
            DataTable DT_Subject = form.DataSources.DataTables.Item("DT_SUBJECT");
            Helper.Utility.Fill_DataTable_By_Recordsert(RC_Subject, DT_Subject);

            string SQL_Grades = $@"SELECT T0.""U_ST_KHCF_TABLE_CODE"" as ""Code"" FROM ""@ST_REL_OBJECTS""  T0 
WHERE T0.""U_ST_KHCF_TABLE_NAME"" = 'DT_GRADES' AND T0.""U_ST_KHCF_OBJECT_TYPE"" = '{form.TypeEx}' AND  T0.""U_ST_KHCF_OBJECT_CODE"" = '{UDO_Code}'";
            Recordset RC_Grades = Helper.Utility.Execute_Recordset_Query(company, SQL_Grades);
            DataTable DT_Grades = form.DataSources.DataTables.Item("DT_GRADES");
            Helper.Utility.Fill_DataTable_By_Recordsert(RC_Grades, DT_Grades);

            string SQL_Patient = $@"SELECT T0.""U_ST_KHCF_TABLE_CODE"" as ""Code"", T1.U_ST_FULL_ARABIC_NAME as ""Name""
FROM ""@ST_REL_OBJECTS""  T0 INNER JOIN ""@ST_PATIENTS_CARD"" T1 ON T1.""Code"" = T0.U_ST_KHCF_TABLE_CODE
WHERE T0.""U_ST_KHCF_TABLE_NAME"" = 'DT_PATIENT' AND T0.""U_ST_KHCF_OBJECT_TYPE"" = '{form.TypeEx}' AND  T0.""U_ST_KHCF_OBJECT_CODE"" = '{UDO_Code}'";
            Recordset RC_Patient = Helper.Utility.Execute_Recordset_Query(company, SQL_Patient);
            DataTable DT_Patient = form.DataSources.DataTables.Item("DT_PATIENT");
            Helper.Utility.Fill_DataTable_By_Recordsert(RC_Patient, DT_Patient);

            foreach (string OneGrid in Relations_Grid_IDs)
            {
                ((Grid)form.Items.Item(OneGrid).Specific).AutoResizeColumns();
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            //if (form.DataSources.DBDataSources.Item(0).GetValue("CardType", 0) == "C")
            //{
            //    throw new Exception("We can Update Lead BP only in System Form.");
            //}
            return true;
        }



    }
}
