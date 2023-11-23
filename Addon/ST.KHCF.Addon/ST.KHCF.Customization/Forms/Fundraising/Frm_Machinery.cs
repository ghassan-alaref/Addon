using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Machinery : Parent_Form
    {

        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "10","12","11","14","15","26","27","24","25","28","29","16" });  "10,12,11,14,15,26,27,24,25,28,29,16" 

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
            base.Initialize_Form(form);

            SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)form.Items.Item("Item_4").Specific;
            comboBox.ValidValues.Add("N", "No");
            comboBox.ValidValues.Add("Y", "Yes");

            Matrix Mat_Att = (Matrix)form.Items.Item("63").Specific;
            Mat_Att.AutoResizeColumns();
            
            form.Items.Item("Item_0").Click();
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
                    form.Items.Item("37").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("38").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
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

        internal static void Add_Line(ItemEvent pVal, string Line_DataSource_Table, string Matrix_Id, string Code_Col, bool isDBTable)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
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

        internal static void Remove_Selected_Lines(ItemEvent pVal, string Matrix_Id)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item(Matrix_Id).Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
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
                form.DataSources.DBDataSources.Item("@ST_MACHINERY").SetValue("Code", 0, New_UDO_Code);
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
                if ((pVal.ItemUID == "30") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Frm_Expected_Donations.Choose_General_Item_ID(pVal);
                }


                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "62" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }

                if (pVal.ItemUID == "37" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_MACHIN_INST_INFO","33", "U_ST_NEEDED_QUANTITY",true);
                }
                if (pVal.ItemUID == "38" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal,"33");
                }

                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
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
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_MACHIN_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("63").Specific;
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

            Form_Obj.Remove_Matrix_Row(form, "63");

        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);


            Matrix Mat = (Matrix)form.Items.Item("63").Specific;
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
            if (Form_Obj == null || SBO_Application.Forms.ActiveForm.TypeEx != Form_Obj.Form_Type)
            {
                return;
            }
            Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form form = SBO_Application.Forms.ActiveForm;
                DisableMatrixButtons(form);
            }
        }

        private static void DisableMatrixButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "60", "61", "62" };// "116", "115", "119", "118", "129", "128", "601", "600" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);



            }
        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields
            string machineTableName = "@ST_MACHINERY";

            string[] Arabic_Names_Items = new string[] { "25", "12", "27", "15","Item_3" };
            if (Arabic_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                
                string MachineName = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_MACHINE_NAME_AR", 0);
                string Impact = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_MACHINE_IMPACT_AR", 0);
                string Description = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_MACHINE_DESCRIPTION_AR", 0);
                string Dep = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_KHCC_DEPARTMENT_AR", 0);
                string Sign = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_SIGNAGE_AR", 0);

                if (!Utility.Check_Text(MachineName) && !string.IsNullOrEmpty(MachineName))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_MACHINE_NAME_AR", 0, string.Empty);
                    form.Items.Item("12").Click();
                }
                else if (!Utility.Check_Text(Impact) && !string.IsNullOrEmpty(Impact))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_MACHINE_IMPACT_AR", 0, string.Empty);
                    form.Items.Item("25").Click();

                }
                else if (!Utility.Check_Text(Description) && !string.IsNullOrEmpty(Description))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_MACHINE_DESCRIPTION_AR", 0, string.Empty);
                    form.Items.Item("27").Click();

                }
                else if (!Utility.Check_Text(Dep) && !string.IsNullOrEmpty(Dep))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_KHCC_DEPARTMENT_AR", 0, string.Empty);
                    form.Items.Item("15").Click();

                }
                else if (!Utility.Check_Text(Sign) && !string.IsNullOrEmpty(Sign))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_SIGNAGE_AR", 0, string.Empty);
                    form.Items.Item("Item_3").Click();

                }

            }

            string[] English_Names_Items = new string[] { "11", "14", "24", "26", "Item_7" };

            if (English_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string MachineName = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_MACHINE_NAME_EN", 0);
                string Impact = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_MACHINE_IMPACT_EN", 0);
                string Dep = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_KHCC_DEPARTMENT_EN", 0);
                string Des = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_MACHINE_DESCRIPTION_EN", 0);
                string Sign = form.DataSources.DBDataSources.Item(machineTableName).GetValue("U_ST_SIGNAGE_EN", 0);

                if (Utility.Check_Text(MachineName) && !string.IsNullOrEmpty(MachineName))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_MACHINE_NAME_EN", 0, string.Empty);
                    form.Items.Item("11").Click();
                }
                else if (Utility.Check_Text(Impact) && !string.IsNullOrEmpty(Impact))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_MACHINE_IMPACT_EN", 0, string.Empty);
                    form.Items.Item("24").Click();
                }
                else if (Utility.Check_Text(Dep) && !string.IsNullOrEmpty(Dep))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_KHCC_DEPARTMENT_EN", 0, string.Empty);
                    form.Items.Item("14").Click();
                }
                else if (Utility.Check_Text(Des) && !string.IsNullOrEmpty(Des))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_MACHINE_DESCRIPTION_EN", 0, string.Empty);
                    form.Items.Item("26").Click();
                }
                else if (Utility.Check_Text(Sign) && !string.IsNullOrEmpty(Sign))
                {
                    form.DataSources.DBDataSources.Item(machineTableName).SetValue("U_ST_SIGNAGE_EN", 0, string.Empty);
                    form.Items.Item("Item_7").Click();
                }

            }
            
       
            #endregion



        }

    }
}
