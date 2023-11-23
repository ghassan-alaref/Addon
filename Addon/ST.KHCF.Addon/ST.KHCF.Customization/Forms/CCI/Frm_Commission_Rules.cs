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
    internal class Frm_Commission_Rules : Parent_Form
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
            
            //Desc_value = "Mandatary fields List For CCI Commission Rules";
            //Man_fields = "300";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "CCI Commission Rules" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);

            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("C", "base.Initialize Form", startTime);
            //prev = Utility.Add_Time_Log("C", "", startTime, true);
            //            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
            //WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "9", SQL_Customer_Group, true);

            Matrix Mat_Line = (Matrix)form.Items.Item("19").Specific;
//            string Broker_Vendor_Group = Configurations.Get_Broker_Vendor_Group(company);
//            string SQL_Broker = $@"SELECT T0.""CardCode"" AS ""Code"", T0.""CardName"" AS ""Name"" 
//FROM OCRD T0 WHERE T0.""GroupCode"" = {Broker_Vendor_Group}";
//            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "BROKER", SQL_Broker, true);
         
//            string SQL_Channel = $@"SELECT ""Code"",""Name"" FROM ""@ST_CHANNEL"" ";
//            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "CHANNEL", SQL_Channel, true);
//            string SQL_Sub_Channel = $@"SELECT ""Code"",""Name"" FROM ""@ST_SUB_CHANNEL"" ";
//            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "SUB_CHANN", SQL_Sub_Channel, true);
//            string SQL_Employee = $@"SELECT T0.""empID"" AS ""Code"", T0.""lastName"" AS ""Name"" FROM OHEM T0";
//            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Line, "SALES_EMP", SQL_Employee, true);
            Mat_Line.Columns.Item("SELECTED").AffectsFormMode = false;
            Mat_Line.AutoResizeColumns();

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
               
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {
                    Code_value = BusinessObjectInfo.FormTypeEx.Replace("ST", "Frm");
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
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
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_COMMISS_RULES_L");
            Matrix Mat_Lines = (Matrix)form.Items.Item("19").Specific;
            int count = DS_Lines.Size;
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
                form.DataSources.DBDataSources.Item("@ST_COMMISSION_RULES").SetValue("Code", 0, New_UDO_Code);
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
                //Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                if (pVal.ItemUID == "20" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal);
                }
                if (pVal.ItemUID == "21" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal);
                }

                //if (pVal.ItemUID == "192" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Calculate_Premium(pVal);
                //}

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Add_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            //form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_COMMISS_RULES_L");
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
                if (DS_Lines.GetValue("U_ST_COMMISSION", Count - 1) != "")
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
            else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE && Form_Obj != null)
            {
                if (SBO_Application.Forms.ActiveForm.TypeEx == Form_Obj.Form_Type)
                    Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
        }

    }
}
