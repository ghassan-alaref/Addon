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
    internal class Frm_Patient_Activity : Parent_Form
    {

        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Relations_Grid_IDs = new string[] { "70", "71"};

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "10","11","12","13" });  "10,11,12,13"

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
            //Code_value = "Frm_Patient_Activity";
            //Desc_value = "Mandatary fields List For Patient Activity ";
            //Man_fields = "10,11,12,13";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "FundRaising Patient Activity" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("F", "base.Initialize Form", startTime);
            Matrix Mat_Att = (Matrix)form.Items.Item("68").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
          //  Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "12", $@"Select T0.""Code"",T0.""Name"" From ""@ST_PAT_ACT_SUPPORT"" T0");
            //prev = Utility.Add_Time_Log("F", "Attachments", prev);
            //SAPbouiCOM.ChooseFromList CFL_Doner = form.ChooseFromLists.Item("CFL_DONER");
            //Conditions Doner_Cons = CFL_Doner.GetConditions();
            //Condition Doner_Con = Doner_Cons.Add();
            //Doner_Con.Alias = "U_ST_DONOR_ADD_UPDATE";
            //Doner_Con.CondVal = "Y";
            //Doner_Con.Operation = BoConditionOperation.co_EQUAL;

            //string SQL_Supp = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUPPORT_TYPE""  T0";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "12", SQL_Supp);

            //CFL_Doner.SetConditions(Doner_Cons);
            Matrix Mat_P = (Matrix)form.Items.Item("62").Specific;
            

            Mat_Att.AutoResizeColumns();
            Mat_P.AutoResizeColumns();
            
            form.Items.Item("19").Click();
            //prev = Utility.Add_Time_Log("F", "", startTime);

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
                    form.Items.Item("60").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("61").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
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
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);
            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;

           // Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);

        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);

            form.Freeze(true);

            if (form.DataSources.DataTables.Item("ST_SCHOOLS_UNIVERS").Rows.Count == 0)
            {
                Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);
            }

            Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            Form_Obj.Set_Fields(form);

            form.Freeze(false);
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


                if (pVal.ItemUID == "65" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "66" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "67" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }

                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Patient_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Patient_Line(pVal, "@ST_PATIENT_ACTV_PAT");
                }
                if (pVal.ItemUID == "62" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Patient_Choos_From_List(pVal, "@ST_PATIENT_ACTV_PAT");
                }


            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        internal static void Patient_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("62").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Patients_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            bool isAdded = Check_Code(form, Code);
            if (!isAdded)
            {
                throw new Logic.Custom_Exception("Same patient can not be added more than once");
            }
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PATIENTS_CODE", Index, Code);
            Set_Patient_Data(form, DT_Patient_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Set_Patient_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PATIENTS_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Code}'";
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


        internal static void Add_Patient_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Patients_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("62").Specific;
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
            if (DS_Lines.Size > DT_Patients_Details.Rows.Count)
            {
                DT_Patients_Details.Rows.Add();
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
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Patients_Details");
            Matrix Mat = (Matrix)form.Items.Item("62").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Patient_Details.Rows.Remove(i);
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
                 Form form = SBO_Application.Forms.ActiveForm;
                //Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info);
                //form.DataSources.UserDataSources.Item("27").Value = "0";
                //form.DataSources.UserDataSources.Item("172").Value = "0";
                string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

                Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);
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
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_PATIENT_ACTV_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("68").Specific;
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

            Form_Obj.Remove_Matrix_Row(form, "68");

        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("68").Specific;
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

        private static bool Check_Code(Form form ,string Code)
        {
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_PATIENT_ACTV_PAT");
            Matrix Mat_Lines = (Matrix)form.Items.Item("62").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            for (int i = 0; i < Count; i++)
            {
                if (DS_Lines.GetValue("U_ST_PATIENTS_CODE", i) != "")
                {
                    if (DS_Lines.GetValue("U_ST_PATIENTS_CODE", i).ToString() == Code)
                        return false;
                }
            }
            return true;
        }


    }
}
