using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_Sales_Target : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    //Result.Add("300");

        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //     Result.AddRange(new string[] { "5" });

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            
            //Desc_value = "Mandatary fields List For Sales Target";
            //Man_fields = "300";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "CCI Sales Target" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("C", "base.Initialize Form", startTime);


            //            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
            //WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "9", SQL_Customer_Group, true);

            Matrix Mat_Line = (Matrix)form.Items.Item("19").Specific;
            //            string Broker_Vendor_Group = Configurations.Get_Broker_Vendor_Group(company);
            //            string SQL_Broker = $@"SELECT T0.""CardCode"" AS ""Code"", T0.""CardName"" AS ""Name"" 
            //FROM OCRD T0 WHERE T0.""GroupCode"" = {Broker_Vendor_Group}";
            //            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "BROKER", SQL_Broker, true);
            //prev = Utility.Add_Time_Log("C", "Broker", prev);
            string Broker_Vendor_Group = Configurations.Get_Broker_Vendor_Group(company);
            SAPbouiCOM.ChooseFromList CFL_Broker = form.ChooseFromLists.Item("CFL_Broker");
            Conditions Broker_Cons = CFL_Broker.GetConditions();
            Condition Broker_Con = Broker_Cons.Add();
            Broker_Con.Alias = "GroupCode";
            Broker_Con.Operation = BoConditionOperation.co_EQUAL;
            Broker_Con.CondVal = Broker_Vendor_Group;
            CFL_Broker.SetConditions(Broker_Cons);


            string SQL_Channel = $@"SELECT ""Code"",""Name"" FROM ""@ST_CHANNEL"" ";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "CHANNEL", SQL_Channel, true);
            //prev = Utility.Add_Time_Log("C", "Channel", prev);
            string SQL_Sub_Channel = $@"SELECT ""Code"",""Name"" FROM ""@ST_SUB_CHANNEL"" ";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "SUB_CHANN", SQL_Sub_Channel, true);
            //prev = Utility.Add_Time_Log("C", "Sub Channel", prev);
            //string SQL_Employee = $@"SELECT T0.""empID"" AS ""Code"", (T0.""firstName"" || ' ' || T0.""lastName"") AS ""Name"" FROM OHEM T0";
            //Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "SALES_EMP", SQL_Employee, true);
            string CCI_Department_ID = Configurations.Get_CCI_Department(company);
            string SQL_Employee = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" = {CCI_Department_ID}";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "SALES_EMP", SQL_Employee, true);
            //prev = Utility.Add_Time_Log("C", "Sales Employee", prev);
            Mat_Line.Columns.Item("SELECTED").AffectsFormMode = false;

            form.Items.Item("20").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            form.Items.Item("21").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            form.Items.Item("19").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

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
            Mat_Line.AutoResizeColumns();
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
                    Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    Matrix Mat_Line = (Matrix)form.Items.Item("19").Specific;
                    Mat_Line.AutoResizeColumns();
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
                form.DataSources.DBDataSources.Item("@ST_SALES_TARGET").SetValue("Code", 0, New_UDO_Code);
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
                    Add_Line(pVal);
                }
                if (pVal.ItemUID == "21" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal);
                }

                if (pVal.ItemUID == "19" && pVal.ColUID == "BROKER" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Broker(pVal);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        private static void Choose_From_List_Broker(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Matrix Mat_Line = (Matrix)form.Items.Item("19").Specific;
                string C = Chos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
                EditText Txt_Broker = (EditText)Mat_Line.GetCellSpecific(pVal.ColUID, pVal.Row);
                Txt_Broker.Value = C;
            }
        }
        private static void Add_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            //form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_SALES_TARGET_L");
            Matrix Mat_Lines = (Matrix)form.Items.Item("19").Specific;
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
                if (DS_Lines.GetValue("U_ST_SALES_EMPLOYEE", Count - 1) != "")
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
        }


    }
}
