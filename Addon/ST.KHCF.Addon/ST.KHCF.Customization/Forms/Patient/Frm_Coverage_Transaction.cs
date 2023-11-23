using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Forms.Fundraising;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Patient
{
    internal class Frm_Coverage_Transaction : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static Frm_Expected_Donations Form_Exp_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "6","7","15","16","17","14","9" }); "6,7,15,16,17,14,9"
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5" });

        //   return Result.ToArray();
        //}
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
                if (Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info) == true)
                {
                    // return;
                }

            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
        }

        internal override void Initialize_Form(Form form)
        {
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", "Patient Coverage Transaction" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            //Code_value = "Frm_Coverage_Transaction";
            //Desc_value = "Mandatary fields List For Coverage Transaction ";
            //Man_fields = "6,7,15,16,17,14,9";

            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("P", "base.Initialize Form", startTime);
            form.Items.Item("20").AffectsFormMode = false;
            form.Items.Item("22").AffectsFormMode = false;

            string SQL_Support_Type = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUPPORT_TYPE"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "17", SQL_Support_Type, true);
            //prev = Utility.Add_Time_Log("P", "Support Type", prev);
            string SQL_Fund_Box = $@"SELECT T0.""Code"", T0.U_ST_NAME AS ""Name"" FROM ""@ST_GOOD_WILL_FUNDS"" T0 WHERE IFNULL(T0.""U_ST_AGREEMENT_END_DATE"", '20990101') > CURRENT_DATE";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "18", SQL_Fund_Box, true);
            //prev = Utility.Add_Time_Log("P", "Fund Box", prev);
            ComboBox Cmb_Patient_Type = (ComboBox)form.Items.Item("16").Specific;
            Cmb_Patient_Type.ValidValues.Add("-", "");
            Cmb_Patient_Type.ValidValues.Add("C", "CCI");
            Cmb_Patient_Type.ValidValues.Add("G", "Goodwill");
            Cmb_Patient_Type.ValidValues.Add("O", "Other CCI Companies");
            //prev = Utility.Add_Time_Log("P", "Combo Box", prev);
            //prev = Utility.Add_Time_Log("P", "", startTime, true);

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
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !BusinessObjectInfo.BeforeAction)
                {
                    Create_AP_Invoice(company, BusinessObjectInfo);
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
        private static void Create_AP_Invoice(SAPbobsCOM.Company company, BusinessObjectInfo businessObjectInfo)
        {
            Form form = Loader.SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Coverage_Date = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COVERAGE_DATE", 0).ToString();
            Coverage_Date = Coverage_Date.Insert(4, "/");
            Coverage_Date = Coverage_Date.Insert(7, "/");
            string itemDesc = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SUPPORT_TYPE", 0).ToString();
            string Amount = form.DataSources.UserDataSources.Item("12").Value.ToString();
            string SQL_Support_Type = $@"SELECT T0.""Name"",T0.""U_ST_EXPENSE_ACCOUNT_CODE"" FROM ""@ST_SUPPORT_TYPE"" T0 WHERE T0.""Code"" = '{itemDesc}'";
            Recordset RC_Support = Helper.Utility.Execute_Recordset_Query(company, SQL_Support_Type);
            Documents AP_Invoice = (Documents)company.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
            AP_Invoice.UserFields.Fields.Item("U_ST_COVERAGE_TRANSACTION_CODE").Value = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            AP_Invoice.DocType = BoDocumentTypes.dDocument_Service;
            AP_Invoice.DocDate = Convert.ToDateTime(Coverage_Date);
            AP_Invoice.TaxDate = Convert.ToDateTime(Coverage_Date);
            AP_Invoice.CardCode = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PATIENT_VENDOR_CODE", 0).ToString();
            if (!string.IsNullOrEmpty(RC_Support.Fields.Item("U_ST_EXPENSE_ACCOUNT_CODE").Value.ToString()))
            {
                AP_Invoice.Lines.AccountCode = RC_Support.Fields.Item("U_ST_EXPENSE_ACCOUNT_CODE").Value.ToString();
            }
            else
            {
                throw new Logic.Custom_Exception($"Please check the expense code linked to the support type");
            }
            AP_Invoice.Lines.ItemDescription = RC_Support.Fields.Item("Name").Value.ToString();
            AP_Invoice.Lines.LineTotal = Convert.ToDouble(Amount);
            AP_Invoice.Lines.Add();
            AP_Invoice.DocTotal = Convert.ToDouble(Amount);
            if (AP_Invoice.Add() != 0)
            {
                string X = company.GetLastErrorDescription();
                throw new Logic.Custom_Exception($"Error during create the Invoice],[{company.GetLastErrorDescription()}]");
            }

        }

        private static void Stop_Transaction(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can Stop the Coverage Transaction if the form in OK mode only");
            }

            if (SBO_Application.MessageBox("Are you sure you want to Stop the Coverage Transaction?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            string Stop_Reason = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_REASON", 0);
            if (Stop_Reason == "-")
            {
                throw new Logic.Custom_Exception("Please select the Stop Reason");
            }
            string Stop_Date_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_DATE", 0);
            if (Stop_Date_Text == "")
            {
                throw new Logic.Custom_Exception("Please select the Stop Date");
            }

            KHCF_Logic_Utility.Stop_Coverage_Transaction(company, UDO_Code, Stop_Reason, DateTime.ParseExact(Stop_Date_Text, "yyyyMMdd", null), SBO_Application);

            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Success);
        }
        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
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
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
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
                if (pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Patient_Card_Choose_From_List(pVal);
                }
                if (pVal.ItemUID == "15" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Coverage_Request_Choose_From_List(pVal);
                }
                if (pVal.ItemUID == "23" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Stop_Transaction(pVal);
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        private static void Coverage_Request_Choose_From_List(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            if (Code == "")
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Set_Coverage_Request_Data(form, Code);
        }

        internal static void Set_Coverage_Request_Data(Form form, string Coverage_Request_Code)
        {

            string SQL = $@"SELECT T0.U_ST_PATIENT_TYPE, T0.U_ST_PATIENT_CARD, T0.U_ST_PATIENT_VENDOR_CODE, T0.U_ST_SUPPORT_TYPE,T0.U_ST_SUPPORT_AMOUNT 
FROM ""@ST_COVERAGE_REQUEST""  T0 WHERE T0.""Code"" = '{Coverage_Request_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.RecordCount == 0)
            {
                return;
            }
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_TYPE", 0, RC.Fields.Item("U_ST_PATIENT_TYPE").Value.ToString());
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_CARD", 0, RC.Fields.Item("U_ST_PATIENT_CARD").Value.ToString());
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_CODE", 0, RC.Fields.Item("U_ST_PATIENT_VENDOR_CODE").Value.ToString());
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_SUPPORT_TYPE", 0, RC.Fields.Item("U_ST_SUPPORT_TYPE").Value.ToString());
            form.DataSources.UserDataSources.Item("12").Value = RC.Fields.Item("U_ST_SUPPORT_AMOUNT").Value.ToString();

            //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("", 0, RC.Fields.Item("").Value.ToString());

        }

        private static void Patient_Card_Choose_From_List(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            if (Code == "")
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Select_Patient_Card(Code, form);
        }
        private static void Select_Patient_Card(string Patient_Card_code, Form form)
        {
            string SQL_Card = $@"SELECT T0.U_ST_BP_CODE,T0.U_ST_COVERAGE_MEMBERSHIP,T0.U_ST_COVERAGE_CCI
                                 FROM ""@ST_PATIENTS_CARD""  T0 
                                 WHERE T0.""Code"" = '{Patient_Card_code}'";
            Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);
            string BP_Code = RC_Card.Fields.Item("U_ST_BP_CODE").Value.ToString();
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PATIENT_VENDOR_CODE", 0, BP_Code);
            string type = BP_Code.Split('-')[0];
            if (type.Contains("GW"))
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PATIENT_TYPE", 0, "G");
            }
            else if (type.Contains("CCI"))
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PATIENT_TYPE", 0, "C");
                form.DataSources.UserDataSources.Item("12").Value = RC_Card.Fields.Item("U_ST_COVERAGE_MEMBERSHIP").Value.ToString();
            }
            else
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PATIENT_TYPE", 0, "O");
                form.DataSources.UserDataSources.Item("12").Value = RC_Card.Fields.Item("U_ST_COVERAGE_CCI").Value.ToString();
            }
            string SQL_Fund_Box = $@"SELECT T0.""Code"" FROM ""@ST_GOOD_WILL_FUNDS"" T0 
                                     JOIN ""@ST_PATIENTS_CARD"" T1 ON T1.U_ST_GENDER = T0.U_ST_GENDER
                                     AND T0.U_ST_NATIONALITY = T1.U_ST_NATIONALITY WHERE T1.""Code"" = '{Patient_Card_code}'";
            Recordset RC_Fund = Helper.Utility.Execute_Recordset_Query(company, SQL_Fund_Box);
            if (RC_Fund.RecordCount > 0)
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FUND_BOX", 0, RC_Fund.Fields.Item("Code").Value.ToString());
            }
        }
    }
}


