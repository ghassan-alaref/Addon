using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.CCI
{
    class Frm_Revenue_Realization
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;

        internal static Form Create_Form()
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_CCI_Revenue_Realization;
            var form = SBO_Application.Forms.AddEx(form_params);
            //form.Visible = false;
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form_params.Modality = BoFormModality.fm_Modal;

          //  Inv_Form_Data = Inv_Data;

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
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "CCI Revenue Realization" + Environment.NewLine);
            string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            DateTime Previous_Date = DateTime.Today.AddMonths(-1);
            ComboBox Cmb = (ComboBox)form.Items.Item("6").Specific;
            for (int i = 1; i <= 12; i++)
            {
                Cmb.ValidValues.Add(i.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));
            }
            form.DataSources.UserDataSources.Item("4").Value = Previous_Date.Year.ToString();
            form.DataSources.UserDataSources.Item("6").Value = Previous_Date.Month.ToString();
            //DateTime prev = Utility.Add_Time_Log("C", "Combo box", startTime);

            Grid Grd_Result = (Grid)form.Items.Item("8").Specific;
            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE""";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Result, "Coverage", SQL_Cov);
            //prev = Utility.Add_Time_Log("C", "Coverage", prev);
            //prev = Utility.Add_Time_Log("C", "", startTime, true);

        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Revenue_Realization")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Filter(form);
                }
                if (pVal.ItemUID == "9" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Run(pVal);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Run(ItemEvent pVal)
        {
            if(SBO_Application.MessageBox("Are you sure you want to run the process?" , 1, "Yes", "No") != 1)
            {
                return ;
            }

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Run_Thread));
            t.Start(pVal.FormUID);
            //Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ////((Grid)form.Items.Item("8").Specific).DataTable.Clear();
            //DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            //DT_Result.Clear();


        }

        private static void Run_Thread(object FormUID)
        {
            Form form = SBO_Application.Forms.Item(FormUID);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");

            int Year = int.Parse(form.DataSources.UserDataSources.Item("4").Value);
            int Month = int.Parse(form.DataSources.UserDataSources.Item("6").Value);

            DateTime Start_Date = new DateTime(Year, Month, 1);
            DateTime End_Date = Start_Date.AddMonths(1).AddDays(-1);
            string Allowance_Account = Configurations.Get_Allowance_Account(company);
            SBO_Application.StatusBar.SetText("Begin!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            form.Freeze(true);
            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }
                    int Inv_Num = (int)DT_Result.GetValue("DOCNUM", i);
                string SQL_Valid = $@"SELECT T0.""TransId"" FROM OJDT T0 
WHERE T0.U_ST_TYPE = 'R' AND  T0.U_ST_REVENUE_REALIZATION_YEAR = {Year} AND  T0.U_ST_REVENUE_REALIZATION_MONTH = {Month} AND  T0.""U_ST_REVENUE_REALIZATION_INV_NUM"" ={Inv_Num}";
                Recordset RC_Valid = Helper.Utility.Execute_Recordset_Query(company, SQL_Valid);
                if (RC_Valid.RecordCount != 0)
                {
                    SBO_Application.StatusBar.SetText($"There is already Revenue Realization JE[{RC_Valid.Fields.Item("TransId").Value}] created for the Invoice[{Inv_Num}].", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Error);
                    continue;
                }
                try
                {
                    company.StartTransaction();
                    JournalEntries JE = (JournalEntries)company.GetBusinessObject(BoObjectTypes.oJournalEntries);

                    JE.ReferenceDate = End_Date;
                    JE.TaxDate = JE.TaxDate = DateTime.Today;
                    string Remarks = $@"Created for Revenue Realization for Invoice[{Inv_Num}]";
                    JE.Memo = Remarks;
                    JE.Series = Configurations.GEt_Revenue_Realization_JE_Series(company);
                    JE.UserFields.Fields.Item("U_ST_TYPE").Value = "R";
                    JE.UserFields.Fields.Item("U_ST_REVENUE_REALIZATION_YEAR").Value = Year;
                    JE.UserFields.Fields.Item("U_ST_REVENUE_REALIZATION_MONTH").Value = Month;
                    JE.UserFields.Fields.Item("U_ST_REVENUE_REALIZATION_INV_NUM").Value = Inv_Num;

                    JE.Lines.ShortName = DT_Result.GetValue("Unearned_Revenue_Account", i).ToString();
                    JE.Lines.Debit = (double)DT_Result.GetValue("Premium", i);
                    JE.Lines.LineMemo = Remarks;
                    JE.Lines.Add();

                    JE.Lines.ShortName = DT_Result.GetValue("Revenue_Account", i).ToString();
                    JE.Lines.Credit = (double)DT_Result.GetValue("Revenue", i);
                    JE.Lines.LineMemo = Remarks;
                    JE.Lines.Add();

                    JE.Lines.ShortName = Allowance_Account;
                    JE.Lines.Credit = (double)DT_Result.GetValue("Allowance", i);
                    JE.Lines.LineMemo = Remarks;
                    JE.Lines.Add();

                    if (JE.Add() != 0)
                    {
                        throw new Logic.Custom_Exception($"Error during add the Journal Entry for the Invoice[{Inv_Num}][{company.GetLastErrorDescription()}]");
                    }
                    string New_JE;
                    company.GetNewObjectCode(out New_JE);
                    //Documents Inv_Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oInvoices);
                    //Inv_Doc.GetByKey((int)DT_Result.GetValue("DOCENTRY", i));

                    //for (int j = 0; j < Inv_Doc.Lines.Count; j++)
                    //{
                    //    Inv_Doc.Lines.SetCurrentLine(i);

                    //    if (Inv_Doc.Lines.AccountCode == DT_Result.GetValue("Unearned_Revenue_Account", i).ToString())
                    //    {
                    //        Inv_Doc.Lines.UserFields.Fields.Item("U_ST_REVENUE_REALIZATION_JE").Value = int.Parse(New_JE);
                    //    }
                    //}

                    //if (Inv_Doc.Update() != 0)
                    //{
                    //    throw new Exception($"Error during update the Invoice[{Inv_Num}][{company.GetLastErrorDescription()}]");
                    //} 
                    //                    string SQL_Update = $@"Update INV1 set U_ST_REVENUE_REALIZATION_JE = {New_JE} 
                    //Where ""DocEntry"" = {DT_Result.GetValue("DOCENTRY", i)} AND ""AcctCode"" = '{DT_Result.GetValue("Unearned_Revenue_Account", i)}'";

                    //Helper.Utility.Execute_Recordset_Query(company, SQL_Update);

                    company.EndTransaction(BoWfTransOpt.wf_Commit);
                    SBO_Application.StatusBar.SetText($"The Journal Entry[{New_JE}] has been created successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
                catch (Exception ex)
                {
                    try
                    {
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);
                        SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    }
                    catch (Exception)
                    {
                    }
                }
                //finally
                //{
                //}
            }

            //Grid Grd_Result = (Grid)form.Items.Item("8").Specific;
            //DT_Result.Rows.Clear();
            Filter(form);
            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            form.Freeze(false);

        }



        private static void Filter(Form form)
        {
            form.Freeze(true);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");

            int Year = int.Parse(form.DataSources.UserDataSources.Item("4").Value);
            int Month = int.Parse(form.DataSources.UserDataSources.Item("6").Value);

            DateTime Start_Date = new DateTime(Year, Month, 1);
            DateTime End_Date = Start_Date.AddMonths(1).AddDays(-1);

            double Allowance_Rate = Configurations.Get_Allowance_Rate(company);

            string SQL = $@"SELECT T0.U_ST_INVOICE_NUMBER, T1.""DocNum"", T3.""CardCode"", T3.""CardName"", T2.""Code"" AS ""Member_Card_Code"" 
, T0.""Code"" AS ""Memebership_Code"", T1.""DocDate"", T4.""LineTotal"", T6.U_ST_REVENUE_ACCOUNT, T0.U_ST_START_DATE, T0.U_ST_END_DATE, T0.U_ST_COVERAGE
, T6.U_ST_UNEARNED_REVENUE_ACCOUNT, T6.U_ST_REVENUE_ACCOUNT
FROM ""@ST_INDIV_MEMBERSHIP""  T0 
INNER JOIN OINV T1 ON T0.U_ST_INVOICE_NUMBER = CAST(T1.""DocEntry"" AS VARCHAR)
INNER JOIN ""@ST_CCI_INDIV_CARD""  T2 ON T0.U_ST_MEMBER_CARD = T2.""Code"" 
INNER JOIN OCRD T3 ON T1.""CardCode"" = T3.""CardCode"" 
INNER JOIN INV1 T4 ON T1.""DocEntry"" = T4.""DocEntry"" 
INNER JOIN ""@ST_COVERAGE"" T5 ON T0.U_ST_COVERAGE = T5.""Code"" 
INNER JOIN ""@ST_INV_ACCOUNT_MAPP"" T6 ON T0.U_ST_COVERAGE = T6.""U_ST_COVERAGE""  AND T2.U_ST_CUSTOMER_GROUP = T6.U_ST_CUSTOMER_GROUP_CODE

WHERE T4.""AcctCode"" =  T6.U_ST_UNEARNED_REVENUE_ACCOUNT 
AND  T1.""DocDate"" BETWEEN '{Start_Date:yyyyMMdd}' AND '{End_Date:yyyyMMdd}'
AND (T1.""DocNum"" NOT IN (SELECT IFNULL(T0.U_ST_REVENUE_REALIZATION_INV_NUM,0) FROM OJDT T0 
WHERE T0.U_ST_TYPE = 'R' AND  T0.U_ST_REVENUE_REALIZATION_YEAR = {Year} AND  T0.U_ST_REVENUE_REALIZATION_MONTH = {Month}))";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            DT_Result.Rows.Clear();
            DT_Result.Rows.Add(RC.RecordCount);

            for (int i = 0; i < RC.RecordCount; i++)
            {
                DT_Result.SetValue("SELECTED", i, "Y");
                DT_Result.SetValue("DOCENTRY", i, RC.Fields.Item("U_ST_INVOICE_NUMBER").Value);
                DT_Result.SetValue("DOCNUM", i, RC.Fields.Item("DocNum").Value);
                DT_Result.SetValue("CARDCODE", i, RC.Fields.Item("CardCode").Value);
                DT_Result.SetValue("CARDNAME", i, RC.Fields.Item("CardName").Value);
                DT_Result.SetValue("MEMEBER_CARD_CODE", i, RC.Fields.Item("Member_Card_Code").Value);
                DT_Result.SetValue("MEMBERSHIP_CODE", i, RC.Fields.Item("Memebership_Code").Value);
                DT_Result.SetValue("DOCDATE", i, RC.Fields.Item("DocDate").Value);
                double Premium = (double)RC.Fields.Item("LineTotal").Value;
                double Allowance = Premium * Allowance_Rate / 100;
                double Revenue = Premium - Allowance;
                DT_Result.SetValue("Premium", i, Premium);
                DT_Result.SetValue("Allowance", i, Allowance);
                DT_Result.SetValue("Revenue", i, Revenue);

                DateTime Mem_StartDate = (DateTime)RC.Fields.Item("U_ST_START_DATE").Value;
                DateTime Mem_EndDate = (DateTime)RC.Fields.Item("U_ST_END_DATE").Value;
                DateTime Calc_Start_Date = (new DateTime[] { Start_Date, Mem_StartDate }).Max();
                DateTime Calc_End_Date = (new DateTime[] { End_Date, Mem_EndDate }).Min();
                int Days = (Calc_End_Date - Calc_Start_Date).Days;
                DT_Result.SetValue("DAYS", i, Days);
                DT_Result.SetValue("Coverage", i, RC.Fields.Item("U_ST_COVERAGE").Value);
                DT_Result.SetValue("Unearned_Revenue_Account", i, RC.Fields.Item("U_ST_UNEARNED_REVENUE_ACCOUNT").Value);
                DT_Result.SetValue("Revenue_Account", i, RC.Fields.Item("U_ST_REVENUE_ACCOUNT").Value);
               // DT_Result.SetValue("", i, RC.Fields.Item("").Value);

                RC.MoveNext();
            }
            ((Grid)form.Items.Item("8").Specific).AutoResizeColumns();
            form.Freeze(false);

        }
 
    
    }
}
