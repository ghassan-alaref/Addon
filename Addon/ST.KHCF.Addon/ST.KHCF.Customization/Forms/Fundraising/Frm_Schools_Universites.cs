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
    internal class Frm_Schools_Universites : Parent_Form
    {

        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

     

        internal override void Initialize_Form(Form form)
        {
           
            base.Initialize_Form(form);

           // Form_Obj.Set_Fields(form);
            string SQL_Type = $@"Select T0.""Code"",T0.""Name"" From ""@ST_ACTIVITY_TYPE"" T0";
            Matrix Mat_Act = (Matrix)form.Items.Item("20").Specific;
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Act, "Activity", SQL_Type);
            Mat_Act.Columns.Item("SELECTED").AffectsFormMode = false;

            string SQL_Init = $@"Select T0.""Code"",T0.""Name"" From ""@ST_INITIATIVE"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "12", SQL_Init);

            SAPbouiCOM.ChooseFromList CFL_BP = form.ChooseFromLists.Item("CFL_BP");
            Conditions BP_Cons = CFL_BP.GetConditions();
            Condition BP_Con = BP_Cons.Add();
            BP_Con.Alias = "CardType";
            BP_Con.Operation = BoConditionOperation.co_EQUAL;
            BP_Con.CondVal = "S";
            BP_Con.Relationship = BoConditionRelationship.cr_AND;
            Condition BP_Con2 = BP_Cons.Add();
            BP_Con2.Alias = "GroupCode";
            BP_Con2.Operation = BoConditionOperation.co_NOT_EQUAL;
            BP_Con2.CondVal = "123";
            BP_Con2.Relationship = BoConditionRelationship.cr_AND;
            Condition BP_Con3 = BP_Cons.Add();
            BP_Con3.Alias = "GroupCode";
            BP_Con3.Operation = BoConditionOperation.co_NOT_EQUAL;
            BP_Con3.CondVal = "122";
            BP_Con3.Relationship = BoConditionRelationship.cr_AND;
            Condition BP_Con4 = BP_Cons.Add();
            BP_Con4.Alias = "GroupCode";
            BP_Con4.Operation = BoConditionOperation.co_NOT_EQUAL;
            BP_Con4.CondVal = "121";
            BP_Con4.Relationship = BoConditionRelationship.cr_AND;
            Condition BP_Con5 = BP_Cons.Add();
            BP_Con5.Alias = "GroupCode";
            BP_Con5.Operation = BoConditionOperation.co_NOT_EQUAL;
            BP_Con5.CondVal = "120";
            CFL_BP.SetConditions(BP_Cons);

            KHCF_Logic_Utility.Set_Corporate_Fund_Chosse_From_List_Basic_Condition(form, "CFL_Card");



            Matrix Mat_Grade = (Matrix)form.Items.Item("35").Specific;
            string SQL_Grade = $@"Select T0.""Code"",T0.""Name"" From ""@ST_GRADE"" T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Grade, "Grade", SQL_Grade);

            Matrix Mat_Item = (Matrix)form.Items.Item("33").Specific;
            Matrix Mat_Det = (Matrix)form.Items.Item("62").Specific;

            Mat_Item.AutoResizeColumns();
            Mat_Act.AutoResizeColumns();
            Mat_Grade.AutoResizeColumns();
            Mat_Det.AutoResizeColumns();

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
                    form.Items.Item("60").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("61").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    DisableTabButtons(form);
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
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

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
                if (pVal.ItemUID == "Item_8" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Donor_Card(pVal);
                }
                if (pVal.ItemUID == "20" && pVal.ColUID == "Intprov" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Int_Matrix(pVal);
                }
                if (pVal.ItemUID == "33" && pVal.ColUID == "ICode" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Item_Matrix(pVal);
                }
                if (pVal.ItemUID == "33" && pVal.ColUID == "Good" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Good_Matrix(pVal);
                }
                if (pVal.ItemUID == "20" && pVal.ColUID == "Extprov" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Ext_Matrix(pVal);
                }
                if ((pVal.ItemUID == "Item_3") && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Matrix_Row(pVal, "20", "@ST_SCHOOL_UNI_ACT", "U_ST_ACTIVITY");
                }
                if (pVal.ItemUID == "Item_2"  && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Matrix_Row(pVal, "20");
                }
                if ((pVal.ItemUID == "Item_14") && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Matrix_Row(pVal, "35", "@ST_SCHOOL_GRADE", "U_ST_GRADE");
                }
                if (pVal.ItemUID == "Item_13" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Matrix_Row(pVal, "35");
                }

                if ((pVal.ItemUID == "Item_7") && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Matrix_Row(pVal, "33", "@ST_SCHOOL_UNI_ITM", "U_ST_CODE");
                }
                if (pVal.ItemUID == "Item_5" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Matrix_Row(pVal, "33");
                }

                if (pVal.ItemUID == "62" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Patient_Choos_From_List(pVal, "@ST_SCHOOL_UNI_DET");
                }
                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_School_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_School_Line(pVal, "@ST_SCHOOL_UNI_DET");
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

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_BP_CODE", Index, Code);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
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

            }
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

        internal static void Add_School_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("62").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_BP_CODE", Count - 1) != "")
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
            form.Freeze(false);

        }

        internal static void Remove_School_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("62").Specific;
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

        private static void Choose_From_List_Int_Matrix(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Matrix Mat_Line = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("firstName", 0).ToString();
                string C2 = Chos_Event.SelectedObjects.GetValue("lastName", 0).ToString();
                EditText Txt_Emp = (EditText)Mat_Line.GetCellSpecific(pVal.ColUID, pVal.Row);
                //Txt_Emp.Active = true;
                Txt_Emp.Value = C;
                //try
                //{
                //    Txt_Emp.Value = C + " " + C2;
                //}
                //catch (Exception ex) { }
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
        }

        private static void Choose_From_List_Ext_Matrix(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Matrix Mat_Line = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("CardName", 0).ToString();
                EditText Txt_Emp = (EditText)Mat_Line.GetCellSpecific(pVal.ColUID, pVal.Row);
                Txt_Emp.Value = C;
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
        }

        private static void Choose_From_List_Item_Matrix(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Matrix Mat_Line = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("ItemCode", 0).ToString();
                string C2 = Chos_Event.SelectedObjects.GetValue("ItemName", 0).ToString();
                EditText Txt = (EditText)Mat_Line.GetCellSpecific(pVal.ColUID, pVal.Row);
                EditText Txt2 = (EditText)Mat_Line.GetCellSpecific("IName", pVal.Row);
                Txt.Value = C;
                Txt2.Value = C2;
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
        }

        private static void Choose_From_List_Good_Matrix(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Matrix Mat_Line = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("DocEntry", 0).ToString();
                
                //Txt.LinkedObject = BoLinkedObject.;
                try
                {
                    EditText Txt = (EditText)Mat_Line.GetCellSpecific(pVal.ColUID, pVal.Row);
                    Txt.Value = C;
                }
                catch (Exception ex) { }
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
        }

        private static void Choose_Donor_Card(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string Name = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
               form.DataSources.DBDataSources.Item("@ST_SCHOOL_UNI").SetValue("U_ST_SC_UN", 0, Name);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void DisableTabButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "60", "61", "Item_3", "Item_2", "Item_5", "Item_7", "Item_13", "Item_14" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }

        }

        private static void Add_Matrix_Row(ItemEvent pVal, string Matrix_ID, string DatatableSource,string COL_Name)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Table = form.DataSources.DBDataSources.Item(DatatableSource);
            Matrix Mat_Add = (Matrix)form.Items.Item(Matrix_ID).Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Table.Size;
            if (Count == 1)
            {
                if (DS_Table.GetValue(COL_Name, Count - 1) != "")
                {
                    DS_Table.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Table.InsertRecord(Count);
                    Mat_Add.LoadFromDataSource();
                    Mat_Add.DeleteRow(1);
                    Mat_Add.FlushToDataSource();
                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Table.InsertRecord(Count);
            }

            Mat_Add.LoadFromDataSource();

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);
        }

        private static void Remove_Matrix_Row(ItemEvent pVal, string Matrix_ID)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Form_Obj.Remove_Matrix_Row(form, Matrix_ID);
        }
    }
}
