using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms
{
    internal class Frm_Set_Installment
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        private static Inoice_Data Inv_Form_Data;

        internal static Form Create_Form(Logic.Classes.Inoice_Data Inv_Data)
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Set_Installment;
            var form = SBO_Application.Forms.AddEx(form_params);
            //form.Visible = false;
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form_params.Modality = BoFormModality.fm_Modal;

            Inv_Form_Data = Inv_Data;

            try
            {
                Initialize_Form(form);
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            form.Visible = true;
            return form;
        }

        private static void Initialize_Form(Form form)
        {
            form.DataSources.UserDataSources.Item("4").Value =Inv_Form_Data.Source_Code;
            form.DataSources.UserDataSources.Item("6").Value = (Inv_Form_Data.Premium_Amount - Inv_Form_Data.Discount_Value).ToString();
            form.DataSources.UserDataSources.Item("8").Value = "0";
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Installment")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "9" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Row(pVal);
                }
                if (pVal.ItemUID == "10" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Rows(pVal);
                }
                if (pVal.ItemUID == "1" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Invoice(pVal);
                }
                if (pVal.ItemUID == "11" && pVal.ColUID == "AMOUNT" && pVal.ItemChanged && !pVal.BeforeAction)
                {
                    Calculate_Sum_Amount(pVal);
                }

                //if (pVal.FormTypeEx == "720" && pVal.ItemUID == "ST_ITM_STK" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Run_Item_Stock(pVal);
                //}


            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Calculate_Sum_Amount(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            double Sum_ST_Amount = 0;
            DataTable DT_Rows = form.DataSources.DataTables.Item("DT_INSTALLMENT");
            for (int i = 0; i < DT_Rows.Rows.Count; i++)
            {
                Sum_ST_Amount += (double)DT_Rows.GetValue("AMOUNT", i);
            }

            form.DataSources.UserDataSources.Item("8").Value = Sum_ST_Amount.ToString();
        }

        private static void Add_Invoice(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Validate_Data(form);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
            string type = string.Empty;
            int NewEntry = Membership.Create_Invoice(company, Inv_Form_Data, UDO_Info, out type);
            SBO_Application.StatusBar.SetText($"New {type}[{NewEntry}] has been created", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

            form.Close();
        }

        private static Inoice_Data Validate_Data(Form form)
        {
            DataTable DT_Rows = form.DataSources.DataTables.Item("DT_INSTALLMENT");
            List<Installment_Line> Lines = new List<Installment_Line>();
            double Sum_ST_Amount = 0;
            for (int i = 0; i < DT_Rows.Rows.Count; i++)
            {
                Installment_Line OneLine = new Installment_Line();
                if (DT_Rows.GetValue("AMOUNT", i) == null || (double)DT_Rows.GetValue("AMOUNT", i)== 0)
                {
                    throw new Logic.Custom_Exception($"Please set the amount value");
                }
                if (DT_Rows.GetValue("DUE_DATE", i) == null || (double)DT_Rows.GetValue("AMOUNT", i)== 0)
                {
                    throw new Logic.Custom_Exception($"Please set the Due Date value");
                }
                OneLine.Amount = (double)DT_Rows.GetValue("AMOUNT", i);
                OneLine.DueDate = (DateTime)DT_Rows.GetValue("DUE_DATE", i);
                Sum_ST_Amount += OneLine.Amount;

                Lines.Add(OneLine);
            }
            double Invoice_Amount = double.Parse(form.DataSources.UserDataSources.Item("6").ValueEx);

            if (Invoice_Amount != Sum_ST_Amount )
            {
                throw new Logic.Custom_Exception("The sum Amount is not equal than the Invoice amount");
            }
            Inv_Form_Data.Installment_Lines = Lines.ToArray();

            return Inv_Form_Data;
        }

        private static void Remove_Selected_Rows(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Rows = form.DataSources.DataTables.Item("DT_INSTALLMENT");
            for (int i = 0; i < DT_Rows.Rows.Count; i++)
            {
                if (DT_Rows.GetValue("SELECTED", i).ToString() == "Y")
                {
                    DT_Rows.Rows.Remove(i);
                }
            }
            Calculate_Sum_Amount(pVal);
        }

        private static void Add_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Rows = form.DataSources.DataTables.Item("DT_INSTALLMENT");
            DT_Rows.Rows.Add();
        }
    }
}
