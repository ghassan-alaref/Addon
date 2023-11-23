using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ST.KHCF.Customization.Logic.Membership;
using System.IO;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_Coverage_Rules : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "7", "9", "13"});

        //    return Result.ToArray();
        //}
        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5"});

        //    return Result.ToArray();
        //}

        internal override Depends_List[] Get_Depends_List_List()
        {
            List<Depends_List> Result = new List<Depends_List>();
            Result.AddRange(base.Get_Depends_List_List());
            Result.Add(new Depends_List() { Item_ID = "15", Parent_Item_ID = "13", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '{{0}}'" });

            return Result.ToArray();
        }


        internal override void Initialize_Form(Form form)
        {


            //Desc_value = "Mandatary fields List For Coverage Rules";
            //Man_fields = "7,9,13";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "CCI Coverage Rules" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);

            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("C", "base.Initialize Form", startTime);


            Fill_Customer_Group(form);
            //prev = Utility.Add_Time_Log("C", "Customer Group", prev);

            //            string SQL_Broker = $@"SELECT T0.""CardCode"" AS ""Code"", T0.""CardName"" AS ""Name"" 
            //FROM OCRD T0 WHERE T0.""GroupCode"" = {Broker_Vendor_Group}";
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "17", SQL_Broker, true);
            string Broker_Vendor_Group = Configurations.Get_Broker_Vendor_Group(company);
            SAPbouiCOM.ChooseFromList CFL_Broker = form.ChooseFromLists.Item("CFL_Broker");
            Conditions Broker_Cons = CFL_Broker.GetConditions();
            Condition Broker_Con = Broker_Cons.Add();
            Broker_Con.Alias = "GroupCode";
            Broker_Con.Operation = BoConditionOperation.co_EQUAL;
            Broker_Con.CondVal = Broker_Vendor_Group;
            CFL_Broker.SetConditions(Broker_Cons);

            SAPbouiCOM.ChooseFromList CFL_Customer = form.ChooseFromLists.Item("CFL_Customer");
            Conditions Customer_Cons = CFL_Customer.GetConditions();
            Condition Customer_Con = Customer_Cons.Add();
            Customer_Con.Alias = "GroupCode";
            Customer_Con.Operation = BoConditionOperation.co_EQUAL;
            Customer_Con.CondVal = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_CUSTOMER_GROUP", 0);
            CFL_Customer.SetConditions(Customer_Cons);


            //prev = Utility.Add_Time_Log("C", "Broker", prev);

            //Matrix Mat_Coverage = (Matrix)form.Items.Item("19").Specific;
            //string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""
            //""";
            //Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_2", SQL_Cov);
            //prev = Utility.Add_Time_Log("C", "Coverage", prev);

            //string SQL_Nationality = $@"SELECT T0.""Code"", T0.""Name"" FROM OCRY T0";
            //Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_6", SQL_Nationality, true);
            //prev = Utility.Add_Time_Log("C", "Nationality 1", prev);
            //Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_7", SQL_Nationality, true);
            //prev = Utility.Add_Time_Log("C", "Nationality 2", prev);
            //Mat_Coverage.Columns.Item("SELECTED").AffectsFormMode = false;
            //Matrix Mat_Add = (Matrix)form.Items.Item("20").Specific;
            //base.Fill_Address_ComboBox(Mat_Add);

            //string SQL_Employee = $@"SELECT T0.""empID"" AS ""Code"", T0.""lastName"" AS ""Name"" FROM OHEM T0";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "135", SQL_Employee, true);

            //string SQL_Currancies = $@"SELECT T0.""CurrCode"" AS ""Code"",T0.""CurrCode"" AS ""Name"" FROM OCRN T0";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "161", SQL_Currancies, true);

            //Grid Grd_Membership = (Grid)form.Items.Item("154").Specific;
            //string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_COVERAGE"" T0";
            //Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_COVERAGE", SQL);
            //prev = Utility.Add_Time_Log("C", "", startTime, true);

            Matrix Mat_Coverage = (Matrix)form.Items.Item("19").Specific;
            string SQL_Nationality = $@"SELECT T0.""Code"", T0.""Name"" FROM OCRY T0 ORDER BY  T0.""Name""";
            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE"" Order By ""Code""";
            Column coverageColumn = Mat_Coverage.Columns.Item("C_0_2");
            if (coverageColumn.ValidValues.Count == 0)
            {
                Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_2", SQL_Cov);
            }
            Column nationalityColumn = Mat_Coverage.Columns.Item("C_0_6");
            if (nationalityColumn.ValidValues.Count == 0)
            {
                Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_6", SQL_Nationality);
            }
            Column nationalityColumn2 = Mat_Coverage.Columns.Item("C_0_7");
            if (nationalityColumn2.ValidValues.Count == 0)
            {
                Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_7", SQL_Nationality);
            }
            Mat_Coverage.Columns.Item("SELECTED").AffectsFormMode = false;


            form.Items.Item("20").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            form.Items.Item("21").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            form.Items.Item("19").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

        }

        private static void Fill_Customer_Group(Form form)
        {
            string Type = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_CUSTOMER_GROUP_TYPE", 0);

            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND U_ST_TYPE = '{Type}' AND U_ST_CUSTOMER_TYPE = 'C'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "9", SQL_Customer_Group, true);
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
                    SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                    Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    Fill_Customer_Group(form);
                    Select_Customer_Group(form);
                    Form_Obj.Load_One_Depends_Parent_Item(form, "13", false);

                }

            }
            catch (Exception ex)
            {
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
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
            // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item("@ST_COVERAGE_RULES").SetValue("Code", 0, New_UDO_Code);
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
                if (pVal.ItemUID == "20" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    //Matrix Mat_Coverage = (Matrix)form.Items.Item("19").Specific;
                    //string SQL_Nationality = $@"SELECT T0.""Code"", T0.""Name"" FROM OCRY T0";
                    //string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE"" Order By ""Code""";
                    //Column coverageColumn = Mat_Coverage.Columns.Item("C_0_2");
                    //if (coverageColumn.ValidValues.Count == 0)
                    //{
                    //    Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_2", SQL_Cov);
                    //}
                    //Column nationalityColumn = Mat_Coverage.Columns.Item("C_0_6");
                    //if (nationalityColumn.ValidValues.Count == 0)
                    //{
                    //    Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_6", SQL_Nationality);
                    //}
                    //Column nationalityColumn2 = Mat_Coverage.Columns.Item("C_0_7");
                    //if (nationalityColumn2.ValidValues.Count == 0)
                    //{
                    //    Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Coverage, "C_0_7", SQL_Nationality);
                    //}
                    //Mat_Coverage.Columns.Item("SELECTED").AffectsFormMode = false;
                    Add_Line(pVal);
                }
                if (pVal.ItemUID == "21" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal);
                }

                if (pVal.ItemUID == "19" && (pVal.ColUID == "C_0_8" || pVal.ColUID == "C_0_9") && pVal.ItemChanged && !pVal.BeforeAction)
                {
                    Calculate_Discount_Value(pVal);
                }

                //if (pVal.ItemUID == "192" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Calculate_Premium(pVal);
                //}

                if (pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_CUSTOMER_GROUP", 0, "");
                    Fill_Customer_Group(form);
                }
                if (pVal.ItemUID == "17" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Broker(pVal);
                }
                if (pVal.ItemUID == "11" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Customer(pVal);
                }
                if (pVal.ItemUID == "9" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Select_Customer_Group(form);
                }
                if (pVal.ItemUID == "13" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Form_Obj.Load_One_Depends_Parent_Item(form, pVal.ItemUID);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }



        private static void Select_Customer_Group(Form form)
        {
            string Cust_Group = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_CUSTOMER_GROUP", 0);
            SAPbouiCOM.ChooseFromList CFL_Customer = form.ChooseFromLists.Item("CFL_Customer");
            Conditions Customer_Cons = CFL_Customer.GetConditions();
            Condition Customer_Con = Customer_Cons.Item(0);
            //Customer_Con.Alias = "GroupCode";
            //Customer_Con.Operation = BoConditionOperation.co_EQUAL;
            Customer_Con.CondVal = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_CUSTOMER_GROUP", 0);
            CFL_Customer.SetConditions(Customer_Cons);

        }

        private static void Choose_From_List_Broker(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string C = Chos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_BROKER", 0, C);
            }
        }
        private static void Choose_From_List_Customer(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string C = Chos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_CUSTOMER", 0, C);
            }
        }

        private static void Calculate_Discount_Value(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            double Premium_Amount, Discount_Percentage;

            Matrix Mat_Lines = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
            double.TryParse(((EditText)Mat_Lines.GetCellSpecific("C_0_8", pVal.Row)).Value, out Premium_Amount);
            double.TryParse(((EditText)Mat_Lines.GetCellSpecific("C_0_9", pVal.Row)).Value, out Discount_Percentage);

            double Discount_Amount = Premium_Amount * Discount_Percentage /100;
            ((EditText)Mat_Lines.GetCellSpecific("C_0_10", pVal.Row)).Value = Discount_Amount.ToString();
        }

        private static void Add_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
           // form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_COVERAGE_RULES_L");
            Matrix Mat_Lines = (Matrix)form.Items.Item("19").Specific;
            //form.Items.Item("19").Click();
            //SBO_Application.Menus.Item("1292").Enabled = true;
            //SBO_Application.Menus.Item("1292").Activate();
            Mat_Lines.AddRow();
            Mat_Lines.ClearRowData(Mat_Lines.RowCount);
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            return;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_COVERAGE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }

            Mat_Lines.LoadFromDataSource();
            form.Freeze(false);

        }

        private static void Remove_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("19").Specific;
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
            
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE && Form_Obj !=null)
            {
                if (SBO_Application.Forms.ActiveForm.TypeEx == Form_Obj.Form_Type)
                 Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
        }
    }
}
